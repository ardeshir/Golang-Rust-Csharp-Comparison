# Golang, Rust, CSharp Comparison
 A golang, rust, csharp fetch csv file parser with postgres db

 # Golang Test
## These tests cover a few different scenarios: 
1. Testing with a valid URL and database connection string
2. Testing with an invalid URL
3. Testing with an invalid database connection string 
 # Test Docs:
 - go test 
```go test
Data inserted successfully!
Data inserted successfully!
--- FAIL: TestProcessCSVData (0.62s)
    main_test.go:15: Expected an error, but got none
FAIL
exit status 1
FAIL    csv_parser      0.995s 
```
# Golang Web Service
- Use go run main.go --web to expose :8080/api service 

# Rust Test
In these test functions, we create sample data for each test case and call the function being tested with the sample data. We then use assertions to check ## that the function behaves correctly for each test case. 

# CSharp  Test

## Test Docs: 
- Nuget [ https://www.nuget.org/packages/NUnit]
- dotnet add package NUnit --version 3.13.3
- NUnit testing [https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit]

```Microsoft (R) Test Execution Command Line Tool Version 17.3.1 (arm64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 814 ms - /Users/ardeshir/Golang-Rust-Csharp-Comparison/csharp/CsvParser.Tests/bin/Debug/net6.0/CsvParserTests.dll (net6.0)
```

## Csharp : CsvParserWeb a web service option

- Evolving the CLI to also run a Web Service with --web (or basic cli --url)
- The webservice --web listense for param url /CsvParser?url=https://ardeshir.io.file.csv 


# Scripting Languages: (for extra points, no guarantee of support ;P) 
## Python3 venv

- python3 -m venv env  
- source env/bin/activate 
- pip install psycopg2-binary requests Flask  
- python3 csv_parser.py --url https://www.ardeshir.io/file.csv --web  
- navigate to http://127.0.0.1:5000/api 


