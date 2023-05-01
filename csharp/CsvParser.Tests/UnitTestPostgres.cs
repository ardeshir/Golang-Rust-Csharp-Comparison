using Npgsql;
using NUnit.Framework;  


namespace CsvParser {

[TestFixture] 
public class TestPorstgres {

[Test]  
public void TestStoreDataInPostgreSQL() {  
    // Arrange  
    var connectionString = "Server=localhost;Port=5432;Database=data;User Id=postgres;Password=postgres";  
    var connection = new NpgsqlConnection(connectionString);  
    connection.Open();  
  
    // Act  
    var createTableCommand = new NpgsqlCommand("CREATE TABLE IF NOT EXISTS api_test (id SERIAL PRIMARY KEY, url TEXT, name TEXT, created INTEGER)", connection);  
    createTableCommand.ExecuteNonQuery();  
  
    var insertCommand = new NpgsqlCommand("INSERT INTO api_test (url, name, created) VALUES (@url, @name, @created)", connection);  
    insertCommand.Parameters.AddWithValue("@url", "");  
    insertCommand.Parameters.AddWithValue("@name", "");  
    insertCommand.Parameters.AddWithValue("@created", 0);  
  
    var data = new List<string[]> {  
        new string[] {"1", "https://www.google.com", "Google", "1630863600"},  
        new string[] {"2", "https://www.amazon.com", "Amazon", "1630863660"},  
        new string[] {"3", "https://www.microsoft.com", "Microsoft", "1630863720"}  
    };  
  
    for (int i = 0; i < data.Count; i++) {  
        insertCommand.Parameters[0].Value = data[i][1];  
        insertCommand.Parameters[1].Value = data[i][2];  
        insertCommand.Parameters[2].Value = Convert.ToInt32(data[i][3]);  
        insertCommand.ExecuteNonQuery();  
    }  
  
    // Assert  
    using (var selectCommand = new NpgsqlCommand("SELECT COUNT(*) FROM api_test", connection)) {  
        var count = Convert.ToInt32(selectCommand.ExecuteScalar());  
        Assert.AreEqual(data.Count, count);  
    }  
}  

}
}