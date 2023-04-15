using System;  
using System.Collections.Generic;  
using System.IO;  
using System.Net.Http;  
using Npgsql;  
  
namespace CsvParser  
{  
    class db  
    {  
        static void Main(string[] args)  
        {  
            // Check if the user has provided a CSV file URL  
            string csvFileUrl = "https://www.ardeshir.io/file.csv"; // Default URL  
            if (args.Length > 0)  
            {  
                csvFileUrl = args[0];  
            }  
              
            List<string[]> data = new List<string[]>();  
  
            // Fetch the CSV file from the internet  
            using (var httpClient = new HttpClient())  
            {  
                using (var stream = httpClient.GetStreamAsync(csvFileUrl).Result)  
                {  
                    using (var reader = new StreamReader(stream))  
                    {  
                        while (!reader.EndOfStream)  
                        {  
                            var line = reader.ReadLine();  
                            var values = line.Split(',');  
                            data.Add(values);  
                        }  
                    }  
                }  
            }  
  
            // Store the data in PostgreSQL database  
            var connectionString = "Server=localhost;Port=5432;Database=data;User Id=postgres;Password=postgres";  
            using (var connection = new NpgsqlConnection(connectionString))  
            {  
                connection.Open();  
                using (var transaction = connection.BeginTransaction())  
                {  
                    try  
                    {  
                        var createTableCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS api (id SERIAL PRIMARY KEY, url TEXT, name TEXT, created INTEGER)", connection);  
                        createTableCommand.ExecuteNonQuery();  
  
                        var insertCommand = new NpgsqlCommand("INSERT INTO api (id, url, name, created) VALUES (@id, @url, @name, @created)", connection);  
                        insertCommand.Parameters.AddWithValue("@id", 0);  
                        insertCommand.Parameters.AddWithValue("@url", 1);  
                        insertCommand.Parameters.AddWithValue("@name", 2); 
                        insertCommand.Parameters.AddWithValue("@created",3); 
  
                        for (int i = 1; i < data.Count; i++)  
                        {  
                            insertCommand.Parameters[0].Value = Convert.ToInt32(data[i][0]);  
                            insertCommand.Parameters[1].Value = data[i][1];  
                            insertCommand.Parameters[2].Value = data[i][2]; 
                            insertCommand.Parameters[3].Value = Convert.ToInt32(data[i][3]);   
                            // insertCommand.Parameters[3].Value = data[i][3];   
                            //insertCommand.Parameters[4].Value = data[i][4];   
                            insertCommand.ExecuteNonQuery();  
                        }  
  
                        transaction.Commit();  
                        Console.WriteLine("Data inserted successfully!");  
                    }  
                    catch (Exception ex)  
                    {  
                        Console.WriteLine("Error inserting data: " + ex.Message);  
                        transaction.Rollback();  
                    }  
                }  
            }  
        }  
    }  
}  