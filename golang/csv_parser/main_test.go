package main  
  
import "testing"  
  
func TestProcessCSVData(t *testing.T) {  
    // Test with a valid URL and database connection string  
    err := processCSVData("https://www.ardeshir.io/file.csv", "postgres://postgres:postgres@localhost:5432/data?sslmode=disable")  
    if err != nil {  
        t.Errorf("Expected no error, but got %v", err)  
    }  
  
    // Test with an invalid URL  
    err = processCSVData("https://www.ardeshir.io/does_not_exist.csv", "postgres://postgres:postgres@localhost:5432/data?sslmode=disable")  
    if err == nil {  
        t.Errorf("Expected an error, but got none")  
    }  
  
    // Test with an invalid database connection string  
    err = processCSVData("https://www.ardeshir.io/file.csv", "invalid_connection_string")  
    if err == nil {  
        t.Errorf("Expected an error, but got none")  
    }  
}  
