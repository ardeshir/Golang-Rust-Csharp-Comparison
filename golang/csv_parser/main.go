package main    
    
import (    
    "database/sql"    
    "encoding/csv"    
    "flag"    
    "fmt"    
    "net/http"    
    
    _ "github.com/lib/pq"    
)    
    
func main() {    
    // Define command line parameters  
    url := flag.String("url", "https://www.ardeshir.io/file.csv", "The URL of the CSV file to fetch")  
    flag.Parse()  
  
    data := make([][]string, 0)    
    
    // Fetch the CSV file from the internet    
    res, err := http.Get(*url)    
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
			name TEXT,
            created INTEGER    
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
        INSERT INTO api (url, name, created) VALUES ($1, $2, $3)    
    `    
    stmt, err := tx.Prepare(insertQuery)    
    if err != nil {    
        fmt.Println(err)    
        return    
    }    
    defer stmt.Close()    
    
    for i := 1; i < len(data); i++ {    
        url := data[i][1]    
        name := data[i][2]  
        created := data[i][3]    
    
        _, err = stmt.Exec(url, name, created)    
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
    // end of insert

    // Select all rows from the api table  
    rows, err := db.Query("SELECT * FROM api")  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
    defer rows.Close()  
  
    // Get column names  
    columns, err := rows.Columns()  
    if err != nil {  
        fmt.Println(err)  
        return  
    }  
  
    // Create a slice for storing the values  
    values := make([]sql.RawBytes, len(columns))  
  
    // Create a slice for storing the pointers to the values  
    scanArgs := make([]interface{}, len(values))  
    for i := range values {  
        scanArgs[i] = &values[i]  
    }  
  
    // Print the table  
    fmt.Printf("Table: %s\n", "api")  
    for rows.Next() {  
        // Scan the values into the pointers  
        err = rows.Scan(scanArgs...)  
        if err != nil {  
            fmt.Println(err)  
            return  
        }  
  
        // Print the values  
        for i, col := range values {  
            fmt.Printf("%s: %s ", columns[i], string(col))  
        }  
        fmt.Println("")  
    }  
  
    // Check for errors during row iteration  
    if err = rows.Err(); err != nil {  
        fmt.Println(err)  
        return  
    }  

    // end of print table
}   