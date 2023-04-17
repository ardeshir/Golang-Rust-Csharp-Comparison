
using NUnit.Framework;  


namespace CsvParser {

[TestFixture] 
 public class TestFetchFile {

    [Test]  
    public void TestFetchCSVFile() {  
            // Arrange  
            var httpClient = new HttpClient();  
            var csvFileUrl = "https://www.ardeshir.io/file.csv";  
  
            // Act  
            using (var stream = httpClient.GetStreamAsync(csvFileUrl).Result) {  
            using (var reader = new StreamReader(stream)) {  
            // Assert  
                Assert.IsNotNull(reader.ReadToEnd());  
             }  
        }  
    } 
 }
}