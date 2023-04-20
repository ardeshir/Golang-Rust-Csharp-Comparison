package main      
      
import (      
    "database/sql"      
    "encoding/csv"      
    "flag"      
    "fmt"      
    "net/http" 
    "encoding/json"    
      
    _ "github.com/lib/pq"      
)      

func printTableApi( dbConnString string) error {

	db, err := sql.Open("postgres", dbConnString)  
    if err != nil {  
        return err  
    }  
    defer db.Close()  

      // Select all rows from the api table  
	  rows, err := db.Query("SELECT * FROM api")  
	  if err != nil {  
		  fmt.Println(err)  
	  }  
	  defer rows.Close()  
	
	  // Get column names  
	  columns, err := rows.Columns()  
	  if err != nil {  
		  fmt.Println(err)  
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
		  }  
	
		  // Print the values  
		  for i, col := range values {  
			  fmt.Printf("%s: %s ", columns[i], string(col))  
		  }  
		  fmt.Println("")  
	  }  
	 
	  if err = rows.Err(); err != nil {  
		  fmt.Println(err)  
	  } 
  
  
	  return nil  
} 


func processCSVData(url string, dbConnString string) error {  

    data := make([][]string, 0)  
  
    res, err := http.Get(url)  
    if err != nil {  
        return err  
    }  
    defer res.Body.Close()  
  
    reader := csv.NewReader(res.Body)  
  
    records, err := reader.ReadAll()  
    if err != nil {  
        return err  
    }  
   
    for _, record := range records {  
        data = append(data, record)  
    }  
  
    db, err := sql.Open("postgres", dbConnString)  
    if err != nil {  
        return err  
    }  
    defer db.Close()  
  
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
        return err  
    }  
  
    tx, err := db.Begin()  
    if err != nil {  
        return err  
    }  
    defer tx.Rollback()  
  
    insertQuery := `  
        INSERT INTO api (url, name, created) VALUES ($1, $2, $3)  
    `  
    stmt, err := tx.Prepare(insertQuery)  
    if err != nil {  
        return err  
    }  
    defer stmt.Close()  
  
    for i := 1; i < len(data); i++ {  
        url := data[i][1]  
        name := data[i][2]  
        created := data[i][3]  
  
        _, err = stmt.Exec(url, name, created)  
        if err != nil {  
            return err  
        }  
    }  
  
    err = tx.Commit()  
    if err != nil {  
        return err  
    }  
    fmt.Println("Data inserted successfully!")  

    return nil  
}  

func apiHandler(w http.ResponseWriter, r *http.Request) {  
    dbConnString := "postgres://postgres:postgres@localhost:5432/data?sslmode=disable"  

    db, err := sql.Open("postgres", dbConnString)  

    if err != nil {  
        http.Error(w, err.Error(), http.StatusInternalServerError)  
        return  
    }  
    defer db.Close()  
  
    rows, err := db.Query("SELECT * FROM api")  
    if err != nil {  
        http.Error(w, err.Error(), http.StatusInternalServerError)  
        return  
    }  
    defer rows.Close()  
  
    type API struct {  
        ID      int    `json:"id"`  
        URL     string `json:"url"`  
        Name    string `json:"name"`  
        Created string `json:"created"`  
    }  
  
    apis := make([]API, 0)  
    for rows.Next() {  
        var api API  
        err := rows.Scan(&api.ID, &api.URL, &api.Name, &api.Created)  
        if err != nil {  
            http.Error(w, err.Error(), http.StatusInternalServerError)  
            return  
        }  
        apis = append(apis, api)  
    }  
  
    if err := rows.Err(); err != nil {  
        http.Error(w, err.Error(), http.StatusInternalServerError)  
        return  
    }  
  
    w.Header().Set("Content-Type", "application/json")  
    json.NewEncoder(w).Encode(apis)  
}  

func main() {      
	   
    url := flag.String("url", "https://www.ardeshir.io/file.csv", "The URL of the CSV file to fetch")   
    web := flag.Bool("web", false, "Start the application as a web service")  

    flag.Parse()    
    
	dbConnString := "postgres://postgres:postgres@localhost:5432/data?sslmode=disable" 

    err := processCSVData(*url , dbConnString)
    if err != nil {
        fmt.Println(err)
        return 
    }

    err = printTableApi(dbConnString)
    if err != nil {
        fmt.Println(err)
        return 
    }
    // If you want a web service ...
    if *web {  
        // Start the web service 
        http.HandleFunc("/api", apiHandler)  
        
        err := http.ListenAndServe(":8080", nil)  
        if err != nil {  
            fmt.Println(err) 
        } 
    
    }  

} 