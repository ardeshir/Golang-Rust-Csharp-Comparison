using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Net.Http;  
using Microsoft.Extensions.DependencyInjection;  
using Microsoft.Extensions.Logging;  
using Npgsql;  
  
namespace CsvParser  {  
    class Program  {  
        static async Task Main(string[] args)  {  
            // Check if the user has provided a CSV file URL  
            string csvFileUrl = "https://www.ardeshir.io/file.csv"; // Default URL  
            if (args.Length > 0)  {  
                csvFileUrl = args[0];  
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
  
            // Get dependencies  
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
        }  
    }  
}  

//  THIS IS OLDER SIMPLER SMALLER VERSION
/* using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Net.Http;  
using Npgsql;  
  
namespace CsvParser  {  
    class Program  {  
        static void Main(string[] args)  {  
            // Check if the user has provided a CSV file URL  
            string csvFileUrl = "https://www.ardeshir.io/file.csv"; // Default URL  
            if (args.Length > 0)   {  
                csvFileUrl = args[0];  
            }  
              
            List<string[]> data = new List<string[]>();  
  
            // Fetch the CSV file from the internet  
            using (var httpClient = new HttpClient())  {  
                using (var stream = httpClient.GetStreamAsync(csvFileUrl).Result)  {  
                    using (var reader = new StreamReader(stream)) {  
                        while (!reader.EndOfStream)  {  
                            var line = reader.ReadLine();  
                            var values = line.Split(',');  
                            data.Add(values);  
                        }  
                    }  
                }  
            }  
  
            // Store the data in PostgreSQL database  
            var connectionString = "Server=localhost;Port=5432;Database=data;User Id=postgres;Password=postgres";  
            using (var connection = new NpgsqlConnection(connectionString)) {  
                connection.Open();  
                using (var transaction = connection.BeginTransaction()) {  
                    try   {  
                        var createTableCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS api (id SERIAL PRIMARY KEY, url TEXT, name TEXT, created INTEGER)", connection);  
                        createTableCommand.ExecuteNonQuery();  

                        var insertCommand = new NpgsqlCommand("INSERT INTO api (url, name, created) VALUES (@url, @name, @created)", connection);    
                        insertCommand.Parameters.AddWithValue("@url", "");    
                        insertCommand.Parameters.AddWithValue("@name", "");   
                        insertCommand.Parameters.AddWithValue("@created", 0); 
  
                        for (int i = 1; i < data.Count; i++)  {  
                            insertCommand.Parameters[0].Value = data[i][1];    
                            insertCommand.Parameters[1].Value = data[i][2];   
                            insertCommand.Parameters[2].Value = Convert.ToInt32(data[i][3]);     
                            insertCommand.ExecuteNonQuery();  
                        }  
  
                        transaction.Commit();  
                        Console.WriteLine("Data inserted successfully!");  

                                                // create a SQL command object with a SELECT query  
                        using (var selectCommand = new NpgsqlCommand("SELECT * FROM api", connection))  
                        {  
                            // execute the query and get the resulting rows  
                            using (var reader = selectCommand.ExecuteReader())  
                            {  
                                // iterate over the rows and print the data  
                                while (reader.Read())  
                                {  
                                    Console.WriteLine("{0}\t{1}\t{2}\t{3}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2), reader.GetInt32(3));  
                                }  
                            }  
                        } 

                    }  catch (Exception ex) {  
                        Console.WriteLine("Error inserting data: " + ex.Message);  
                        transaction.Rollback();  
                    }  
                   // end of base 
                }  
            }  
        }  
    }  */