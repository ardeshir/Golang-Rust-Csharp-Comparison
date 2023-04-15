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
            List<string[]> data = new List<string[]>();  
  
            // Fetch the CSV file from the internet  
            using (var httpClient = new HttpClient())  
            {  
                using (var stream = httpClient.GetStreamAsync("https://www.ardeshir.io/file.csv").Result)  
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
                        var createTableCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS api (id SERIAL PRIMARY KEY, url TEXT, name TEXT)", connection);  
                        createTableCommand.ExecuteNonQuery();  
  
                        var insertCommand = new NpgsqlCommand("INSERT INTO api (id, url, name) VALUES (@id, @url, @name)", connection);  
                        insertCommand.Parameters.AddWithValue("@id", 0);  
                        insertCommand.Parameters.AddWithValue("@url", 1);  
                        insertCommand.Parameters.AddWithValue("@name", 2);  
  
                        for (int i = 1; i < data.Count; i++)  
                        {  
                            insertCommand.Parameters[0].Value = Convert.ToInt32(data[i][0]);  
                            insertCommand.Parameters[1].Value = data[i][1];  
                            insertCommand.Parameters[2].Value = data[i][2];  
                            //insertCommand.Parameters[3].Value = data[i][3]; 
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