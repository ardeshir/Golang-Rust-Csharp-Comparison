package main

import (
    //"bufio"  
    "database/sql"  
    "encoding/csv"  
    "fmt"  
    "net/http"  
    //"os"  
    //"strconv"  
    //"strings"  
  
    _ "github.com/lib/pq"  
)  
  
func main(){  
    data := make([][]string, 0)  
  
    // Fetch the CSV file from the internet  
    res, err := http.Get("https://www.ardeshir.io/file.csv")  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
    defer res.Body.Close()  
  
    // Create a new CSV reader  
    reader := csv.NewReader(res.Body)  
  
    // Read all the CSV records  
    records, err := reader.ReadAll()  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
  
    // Store the CSV records in a collection  
    for _, record := range records {  
        data = append(data, record)  
    }  
  
    // Connect to the PostgreSQL database  
    // connectionString := "postgres://postgres:postgres@localhost:5432/postgres?sslmode=require" OFF
	connectionString := "postgres://postgres:postgres@localhost:5432/data?sslmode=disable"  
    db, err := sql.Open("postgres", connectionString)  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
    defer db.Close()  
  
    // Create the data table if it doesn't exist  
    createTableQuery := `  
        CREATE TABLE IF NOT EXISTS api (  
            id SERIAL PRIMARY KEY, 
            url TEXT,
			name TEXT  
        )  
    `  
    _, err = db.Exec(createTableQuery)  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
  
    // Insert the data into the data table  
    tx, err := db.Begin()  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
    defer tx.Rollback()  
  
    insertQuery := `  
        INSERT INTO api (url, name) VALUES ($1, $2)  
    `  
    stmt, err := tx.Prepare(insertQuery)  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
    defer stmt.Close()  
    fmt.Println(len(data))
	
    for i := 1; i < len(data); i++ {  
        url := data[i][1]  
		fmt.Println(url)
        name := data[i][2] 
		fmt.Println(name)
        if err != nil {  
            fmt.Println(err)  
            return  
        }  
  
        _, err = stmt.Exec(url, name)  
        if err != nil {  
            fmt.Println(err)  
            return  
        }  
    }  
  
    err = tx.Commit()  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
  
    fmt.Println("Data inserted successfully!")  
}  
