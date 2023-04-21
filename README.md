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


# Scripting Languages: 
- (for extra points, no guarantee of support ;P) 
## Python3 venv

- python3 -m venv env  
- source env/bin/activate 
- pip install psycopg2-binary requests Flask  
- python3 csv_parser.py --url https://www.ardeshir.io/file.csv --web  
- navigate to http://127.0.0.1:5000/api 

## Python Tests config
- pip install pytest pytest-cov  
- pytest --cov=csv_parser tests/ 

```
============ test session starts  =======================
Data inserted successfully!

---------- coverage: platform darwin, python 3.11.1-final-0 ----------
Name            Stmts   Miss  Cover
-----------------------------------
csv_parser.py      58     15    74%
-----------------------------------
TOTAL              58     15    74%

===============short test summary info =======================
FAILED tests/test_csv_parser.py::test_print_table_api - AssertionError: assert 'id: 1' in 'Table: api\nid: 32 url: http://example.com name: Example API created: 1234567890 \n'
FAILED tests/test_csv_parser.py::test_process_csv_data - AssertionError: assert 7 == 3
```
# TypeScript 
## TS Docs
(nvm is my choice of node manager) 
- nvm install v18
- npm init
- npm install -g ts-node
- npm install --save express 
- npm install -g request 
- npm install --save-dev @types/request   
- npm install -g typescript
- npm install --save yargs  
- npm install --save pg csv-parser jsonfile
## To Prettier or not to prettier
- npm install --save-dev prettier
- prettier --write src/*.ts.
## Eslint
- npm install --save-dev eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
- eslint --fix src/*.ts | eslint src/*.ts 

## Run 
- npx ts-node csv_parser.ts --web
```Server started on port 3000
Table: api
Data inserted successfully!
```

