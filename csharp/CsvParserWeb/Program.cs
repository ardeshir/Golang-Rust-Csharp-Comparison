using Microsoft.AspNetCore.Hosting;  
using Microsoft.Extensions.DependencyInjection;  
using Microsoft.Extensions.Hosting;  
using Microsoft.Extensions.Logging;  
using System;  
using System.IO;  
using System.Threading.Tasks; 
using Npgsql;   
  
namespace CsvParserWeb  
{  
    public class Program  
    {  
        public static async Task Main(string[] args)  
        {  
            var builder = CreateHostBuilder(args).Build();  
                          
  
            if (args.Length > 0 && args[0] == "--web")  {  

                await builder.RunAsync();  

            }  else if (args.Length > 0 && args[0] == "--url") {  
                // CLI application logic goes here  
                // Check if the user has provided a CSV file URL  
                string csvFileUrl = "https://www.ardeshir.io/file.csv"; // Default URL  
                if (args.Length > 1 )  {  
                    csvFileUrl = args[1];  
                }  
  
                // Setup DI container  
                var serviceProvider = new ServiceCollection()  
                 .AddLogging(configure => configure.AddConsole())  
                 .AddSingleton<HttpClient>()  
                 .AddSingleton<NpgsqlConnection>(provider =>  
                    {  
                    var connectionString = "Server=localhost;Port=5432;Database=data;User Id=postgres;Password=postgres";  
                    return new NpgsqlConnection(connectionString);  
                    })  
                 .BuildServiceProvider();  
  
                var logger = serviceProvider.GetService<ILogger<Program>>();  
                var httpClient = serviceProvider.GetService<HttpClient>();  
                var connection = serviceProvider.GetService<NpgsqlConnection>(); 
  
                // Fetch the CSV file from the internet  
                var data = new List<string[]>();  

                try  {  
                     using var stream = await httpClient.GetStreamAsync(csvFileUrl);  
                     using var reader = new StreamReader(stream);  
                    while (!reader.EndOfStream)  {  
                         var line = await reader.ReadLineAsync();  
                         var values = line.Split(',');  
                        data.Add(values);  
                     }  
                }  catch (Exception ex)  {  
                     logger.LogError(ex, "Error fetching CSV file");  
                     return;  
                }  
  
            // Store the data in PostgreSQL database  
                 try  {  
                     await connection.OpenAsync();  
                     using var transaction = connection.BeginTransaction();  
                     var createTableCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS api (id SERIAL PRIMARY KEY, url TEXT, name TEXT, created INTEGER)", connection);  
                     await createTableCommand.ExecuteNonQueryAsync();  
  
                     var insertCommand = new NpgsqlCommand("INSERT INTO api (url, name, created) VALUES (@url, @name, @created)", connection);  
                     insertCommand.Parameters.AddWithValue("@url", "");  
                     insertCommand.Parameters.AddWithValue("@name", "");  
                     insertCommand.Parameters.AddWithValue("@created", 0);  
  
                     foreach (var row in data.Skip(1))  {  
                         insertCommand.Parameters[0].Value = row[1];  
                         insertCommand.Parameters[1].Value = row[2];  
                         insertCommand.Parameters[2].Value = Convert.ToInt32(row[3]);  
                         await insertCommand.ExecuteNonQueryAsync();  
                     }  
  
                    await transaction.CommitAsync();  
                    logger.LogInformation("Data inserted successfully!");  
  
                    // Select all rows from the table and print them  
                     var selectCommand = new NpgsqlCommand("SELECT * FROM api", connection);  
                     using var reader = await selectCommand.ExecuteReaderAsync();  
                        while (await reader.ReadAsync())  {  
                             logger.LogInformation("{0}\t{1}\t{2}\t{3}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));  
                        }  

                 }  catch (Exception ex)  {  
                    logger.LogError(ex, "Error inserting data");  
                }  

            }  else {
                Console.WriteLine("Required --web or --url 'https://ardeshir.io/file.csv'");
            } 
        }  
  
        public static IHostBuilder CreateHostBuilder(string[] args) =>  
            Host.CreateDefaultBuilder(args)  
                .ConfigureWebHostDefaults(webBuilder =>  
                {  
                    webBuilder.UseStartup<Startup>();  
                })  
                .ConfigureLogging(logging =>  
                {  
                    logging.ClearProviders();  
                    logging.AddConsole();  
                });  
    }  
}  

