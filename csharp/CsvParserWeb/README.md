# CsvParserWeb
Evolving the CLI to also run a Web Service with --web (or basic cli --url)
The webservice --web listense for param url /CsvParser?url=https://ardeshir.io.file.csv 

## Rep 
- dotnet dev-certs https --clean
- dotnet dev-certs https --trust
## Requirements 
- dotnet add package Microsoft.Extensions.Logging
- dotnet add package Microsoft.Extensions.Logging.Console
- dotnet add package CsvHelper
- dotnet add package Npgsql
