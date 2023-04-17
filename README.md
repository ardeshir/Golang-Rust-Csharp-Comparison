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


# CSharp  Test

## Test Docs: 
- Nuget [ https://www.nuget.org/packages/NUnit]
- dotnet add package NUnit --version 3.13.3
- NUnit testing [https://learn.microsoft.com/en-us/dotnet/core/testing/unit-testing-with-nunit]
```Microsoft (R) Test Execution Command Line Tool Version 17.3.1 (arm64)
Copyright (c) Microsoft Corporation.  All rights reserved.

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:     2, Skipped:     0, Total:     2, Duration: 814 ms - /Users/ardeshir/Golang-Rust-Csharp-Comparison/csharp/CsvParser.Tests/bin/Debug/net6.0/CsvParserTests.dll (net6.0)```
