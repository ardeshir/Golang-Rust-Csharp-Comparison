main.c:22:20: error: call to undeclared function 'api_handler'; ISO C99 and later do not support implicit function declarations [-Wimplicit-function-declaration]
        response = api_handler(conn); 

main.c:153:7: error: conflicting types for 'api_handler'
char* api_handler(PGconn* conn) {  
      ^
main.c:22:20: note: previous implicit declaration is here