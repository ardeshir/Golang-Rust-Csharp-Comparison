#[cfg(test)]
mod tests {
   use clap::{Command, Arg};  
   use csv::ReaderBuilder;  
   use postgres::{Client, NoTls};  
  
#[test]  
fn test_record_from_csv_record() {  
    let record = csv::StringRecord::from(vec!["1", "\"https://example.com\"", "\"Example\"", "1626796800"]);  
    let result = Record::from_csv_record(record).unwrap();  
    assert_eq!(result.id, 1);  
    assert_eq!(result.url, "https://example.com");  
    assert_eq!(result.name, "Example");  
    assert_eq!(result.created, 1626796800);  
}  
  
#[test]  
fn test_record_from_str() {  
    let s = "1,\"https://example.com\",\"Example\",1626796800";  
    let result = Record::from_str(s).unwrap();  
    assert_eq!(result.id, 1);  
    assert_eq!(result.url, "https://example.com");  
    assert_eq!(result.name, "Example");  
    assert_eq!(result.created, 1626796800);  
}  
  
#[test]  
fn test_main() {  
    // Set up a test database  
    let (mut client, conn) = Client::connect("postgresql://postgres:postgres@localhost:5432/test", NoTls).unwrap();  
    conn.execute("CREATE TABLE IF NOT EXISTS api (id SERIAL PRIMARY KEY, url VARCHAR(256) NOT NULL, name VARCHAR(256) NOT NULL, created INTEGER NOT NULL)", &[]).unwrap();  
  
    // Create a test CSV file  
    let csv_data = "1,\"https://example.com\",\"Example\",1626796800\n2,\"https://example.org\",\"Example 2\",1626796801";  
  
    // Run the main function  
    let mut reader = ReaderBuilder::new().has_headers(true).from_reader(csv_data.as_bytes());  
    let mut records = Vec::new();  
    for result in reader.records() {  
        let record = Record::from_csv_record(result.unwrap()).unwrap();  
        records.push(record);  
    }  
    main_with_client(&mut client, &records).unwrap();  
  
    // Check that the records were inserted into the database  
    let rows = client.query("SELECT id, url, name, created FROM api", &[]).unwrap();  
    assert_eq!(rows.len(), 2);  
    assert_eq!(rows[0].get::<_, i32>(0), 1);  
    assert_eq!(rows[0].get::<_, String>(1), "https://example.com");  
    assert_eq!(rows[0].get::<_, String>(2), "Example");  
    assert_eq!(rows[0].get::<_, i32>(3), 1626796800);  
    assert_eq!(rows[1].get::<_, i32>(0), 2);  
    assert_eq!(rows[1].get::<_, String>(1), "https://example.org");  
    assert_eq!(rows[1].get::<_, String>(2), "Example 2");  
    assert_eq!(rows[1].get::<_, i32>(3), 1626796801);  
  
    // Clean up the test database  
    conn.execute("DROP TABLE api", &[]).unwrap();  
}  

}