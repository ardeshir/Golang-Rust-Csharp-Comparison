using CsvHelper;  
using CsvHelper.Configuration;  
using Microsoft.AspNetCore.Mvc;  
using Microsoft.Extensions.Logging;  
using Npgsql;  
using System.Collections.Generic;  
using System.Globalization;  
using System.IO;  
using System.Linq;  
using System.Net.Http;  
using System.Threading.Tasks;  
  
namespace CsvParserWeb.Controllers  
{  
    [ApiController]  
    [Route("[controller]")]  
    public class CsvParserController : ControllerBase  
    {  
        private readonly ILogger<CsvParserController> _logger;  
        private readonly string _connectionString;  
  
        public CsvParserController(ILogger<CsvParserController> logger)  
        {  
            _logger = logger;  
            //_connectionString = "Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword";  
            _connectionString = "Server=localhost;Port=5432;Database=data;User Id=postgres;Password=postgres"; 
        }  
  
        [HttpGet]  
        public async Task<IActionResult> Get(string url = "https://ardeshir.io/file.csv")  
        {  
            using (var client = new HttpClient())  
            using (var response = await client.GetAsync(url))  
            using (var streamReader = new StreamReader(await response.Content.ReadAsStreamAsync()))  
            using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))  
            {  
                csv.Context.RegisterClassMap<CsvRecordMap>();  
                var records = csv.GetRecords<CsvRecord>().ToList();  
  
                // Save records to database  
                using (var connection = new NpgsqlConnection(_connectionString))  
                {  
                    await connection.OpenAsync();  
                    using (var transaction = connection.BeginTransaction())  
                    {  
                        using (var command = new NpgsqlCommand("INSERT INTO api (url, name, created) VALUES (@url, @name, @created)", connection))  
                        {  
                            //command.Parameters.AddWithValue("@id", NpgsqlTypes.NpgsqlDbType.Integer, null);  
                            command.Parameters.AddWithValue("@url", NpgsqlTypes.NpgsqlDbType.Varchar, ""); 
                            command.Parameters.AddWithValue("@name", NpgsqlTypes.NpgsqlDbType.Varchar, "");   
                            command.Parameters.AddWithValue("@created", NpgsqlTypes.NpgsqlDbType.Integer, null);  
  
                            foreach (var record in records)  
                            {  
                                // command.Parameters[0].Value = record.Id;  
                                command.Parameters[0].Value = record.Url;  
                                command.Parameters[1].Value = record.Name;  
                                command.Parameters[2].Value = record.Created;  
                                await command.ExecuteNonQueryAsync();  
                            }  
                        }  
                        transaction.Commit();  
                    }  
                }  
  
                // Return records as JSON  
                return Ok(records);  
            }  
        }  
    }  
  
    public class CsvRecord  
    {  
        public int Id { get; set; }  
        public string? Url { get; set;}  
        public string? Name { get; set;}  
        public int Created { get; set; }  
    }  
  
    public class CsvRecordMap : ClassMap<CsvRecord>  
    {  
        public CsvRecordMap()  
        {  
            Map(m => m.Id).Index(0).Name("Id"); 
            Map(m => m.Url).Index(1).Name("Url");  
            Map(m => m.Name).Index(2).Name("Name");   
            Map(m => m.Created).Index(3).Name("Created");  
        }  
    }  
}  




/* using CsvHelper;  
using CsvHelper.Configuration;  
using Microsoft.AspNetCore.Mvc;  
using Microsoft.Extensions.Logging;  
using System.Globalization;  
using System.IO;  
using System.Net.Http;
using System.Threading.Tasks;
  
namespace CsvParserWeb.Controllers  
{  
    [ApiController]  
    [Route("[controller]")]  
    public class CsvParserController : ControllerBase  
    {  
        private readonly ILogger<CsvParserController> _logger;  
  
        public CsvParserController(ILogger<CsvParserController> logger)  
        {  
            _logger = logger;  
        }  
  
        [HttpGet]  
        public async Task<IActionResult> Get( string url = "https://ardeshir.io/file.csv")  
        {  
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var streamReader = new StreamReader( await response.Content.ReadAsStreamAsync()))
            using (var csv = new CsvReader(streamReader, CultureInfo.InvariantCulture))  
            {  
                csv.Context.RegisterClassMap<CsvRecordMap>();  
                var records = csv.GetRecords<CsvRecord>();  
                return Ok(records);  
            }  
        }  
    }  
  
    public class CsvRecord  
    {  
        public int Id { get; set; }
        public string Name { get; set; }  
        public string Url { get; set; }  

        public int Created { get; set;}
    }  
  
    public class CsvRecordMap : ClassMap<CsvRecord>  
    {  
        public CsvRecordMap()  
        {  
            Map(m => m.Id).Name("Id");
            Map(m => m.Name).Name("Name");  
            Map(m => m.Url).Name("Url");  
            Map(m => m.Created).Name("Created");  
        }  
    }  
}  

*/