
## Improving the code with modern programming best practices: 
1. Use async/await instead of .Result to avoid blocking the thread. This will help to improve the scalability of the application. 
2. Use dependency injection to inject the HttpClient and NpgsqlConnection objects into the program. This will help to make the code more testable and easier to maintain. 
3. Use a logger to log errors and other important information. This will help to debug the application and track down issues. 
4. Use LINQ to simplify the CSV parsing code. This will make the code more concise and easier to read. 
5. Use parameterized queries instead of string concatenation to avoid SQL injection attacks. This will help to make the application more secure. 

## Adding Assembly
- dotnet add package Microsoft.Extensions.DependencyInjection  
- dotnet add package Microsoft.Extensions.Logging  
- dotnet add package Microsoft.Extensions.Logging.Console  


