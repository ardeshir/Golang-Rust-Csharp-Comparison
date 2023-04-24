#include <stdio.h>  
#include <stdlib.h>  
#include <string.h>  
#include <curl/curl.h>  
#include <libpq-fe.h>  
#include <jansson.h>  
#include <microhttpd.h> 
  
#define DB_CONN_STRING "dbname=data user=postgres password=postgres host=localhost port=5432 sslmode=disable"  

  
void print_table_api(PGconn* conn) {  
   
    PGresult* res = PQexec(conn, "SELECT * FROM api;");  
    int rows = PQntuples(res);  
    int cols = PQnfields(res);  
  
    printf("Table: api\n");  
    for (int i = 0; i < rows; i++) {  
        for (int j = 0; j < cols; j++) {  
            printf("%s: %s ", PQfname(res, j), PQgetvalue(res, i, j));  
        }  
        printf("\n");  
    }  
  
    PQclear(res);  
    
}  

/* static size_t curl_callback(void *contents, size_t size, size_t nmemb, void *userp) {  
    return size * nmemb;  
} */


static size_t curl_callback(void *ptr, size_t size, size_t nmemb, void *userdata) {  
    size_t realsize = size * nmemb;  
    char* data = (char*) ptr;  

    // Write the received data to stdout  
    fwrite(data, 1, realsize, stdout);  
    return realsize;  
} 

void process_csv_data(char* url, PGconn* conn) { 
    printf("<<--||||||  Start of process_csv_data ||||||\n");
    CURL* curl;  
    CURLcode res;  
    char* data = NULL;  
    int data_len = 0;  
    char* line = NULL;  
    size_t line_len = 0;  
    ssize_t read_len;  
    int i = 0, j = 0;  
    PGresult* res2;  
  
    curl = curl_easy_init();  
    if (curl) {  
        
        // set to 1 for fun
        curl_easy_setopt(curl, CURLOPT_VERBOSE, 0); 
        curl_easy_setopt(curl, CURLOPT_URL, url);  
        curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);  
        // fix for curl
        curl_easy_setopt(curl, CURLOPT_SSL_VERIFYPEER, 0);
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, curl_callback); 

  
        // Perform the request  
        CURLcode res = curl_easy_perform(curl);  
 
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &data); 
        curl_easy_setopt(curl, CURLOPT_CAINFO, "./curl-ca-bundle.crt"); 
        res = curl_easy_perform(curl); 
                // Cleanup   
        curl_easy_cleanup(curl);  
  
        if (res != CURLE_OK) {  
            fprintf(stderr, "Error: %s\n", curl_easy_strerror(res));  
            return;  
        }  
  
        // Parse CSV data  
        char** rows = NULL;  
        int rows_len = 0;  
        char* token;  
     
        token = strtok(data, "\n");  
        while (token != NULL) {  
            rows_len++;  
            rows = (char**)realloc(rows, rows_len * sizeof(char*));  
            rows[rows_len - 1] = token;  
            token = strtok(NULL, "\n");  
            
        }  
  
        res2 = PQexec(conn, "CREATE TABLE IF NOT EXISTS api (id SERIAL PRIMARY KEY, url TEXT, name TEXT, created INTEGER);");  
  
        for (i = 1; i < rows_len; i++) {  
            printf("rows_len: %d\n", rows_len);
            char* cols[4];  
            j = 0;  
            token = strtok(rows[i], ",");  
            while (token != NULL) {  
                cols[j++] = token;  
                token = strtok(NULL, ",");  
                printf("Token %d : %s ", j, token); 
            }  
            char query[200];  
            sprintf(query, "INSERT INTO api (url, name, created) VALUES ('%s', '%s', %s);", cols[1], cols[2], cols[3]);   
            res2 = PQexec(conn, query);  
        }  
        PQclear(res2);  

        free(rows);  
        free(data); 

        
    }  
    else {  
        fprintf(stderr, "Error: failed to initialize curl\n");  
        return;  
    }  
  
    
}  
  
char* api_handler(PGconn* conn) {  
    
    PGresult* res = PQexec(conn, "SELECT * FROM api;");  
    int rows = PQntuples(res);  
    int cols = PQnfields(res);  
    char* json_str;  
    json_t* root;  
    json_t* apis;  
    json_t* row;  
    int i, j;  
  
    apis = json_array();  
    for (i = 0; i < rows; i++) {  
        row = json_object();  
        for (j = 0; j < cols; j++) {  
            json_object_set_new(row, PQfname(res, j), json_string(PQgetvalue(res, i, j)));  
        }  
        json_array_append_new(apis, row);  
    }  
  
    root = json_object();  
    json_object_set_new(root, "api", apis);  
  
    json_str = json_dumps(root, JSON_INDENT(2));  
    json_decref(root);  
  
    PQclear(res);  

    return json_str;  
}  


// adding http_hander

int http_handler(void* cls, struct MHD_Connection* connection,  
                 const char* url, const char* method, const char* version,  
                 const char* upload_data, size_t* upload_data_size, void** con_cls) {  
    PGconn* conn = (PGconn*)cls;  
    char* response;  
    struct MHD_Response* mhd_response;  
    int ret;  
  
    if (!strcmp(method, "GET") && !strcmp(url, "/api")) {  
        response = api_handler(conn);  
        mhd_response = MHD_create_response_from_buffer(strlen(response),  
                                                       response, MHD_RESPMEM_MUST_FREE);  
        MHD_add_response_header(mhd_response, "Content-Type", "application/json");  
        ret = MHD_queue_response(connection, MHD_HTTP_OK, mhd_response);  
        MHD_destroy_response(mhd_response);  
        return ret;  
    } else {  
        mhd_response = MHD_create_response_from_buffer(strlen("Not found"), "Not found",  
                                                       MHD_RESPMEM_PERSISTENT);  
        ret = MHD_queue_response(connection, MHD_HTTP_NOT_FOUND, mhd_response);  
        MHD_destroy_response(mhd_response);  
        return ret;  
    }  
} // end of http_handler



/**************/
/*  the main  */
/**************/
int main(int argc, char** argv) {  
    char* url = "http://www.ardeshir.io/file.csv";  
    int is_web = 0;  
    PGconn* conn;  
    char* json_str;  
  
    if (argc > 1) {  
        url = argv[1];  
    }  
    if (argc > 2 && !strcmp(argv[2], "--web")) {  
        is_web = 1;  
    }  
  
    conn = PQconnectdb(DB_CONN_STRING);  
    if (PQstatus(conn) != CONNECTION_OK) {  
        fprintf(stderr, "Connection to database failed: %s\n", PQerrorMessage(conn));  
        PQfinish(conn);  
        exit(1);  
    }  
  
    process_csv_data(url, conn);  
    print_table_api(conn);  
    
    // declare before 
    char* api_handler(PGconn* conn);  


    // adding http_handler 
    struct MHD_Daemon* daemon;  
  
    daemon = MHD_start_daemon(MHD_USE_SELECT_INTERNALLY, 8000, NULL, NULL,  
                              &http_handler, conn, MHD_OPTION_END);  
    if (NULL == daemon) {  
        fprintf(stderr, "Failed to start web server\n");  
        PQfinish(conn);  
        exit(1);  
    }  
  
    printf("Web server running on port 8000...\n");  /// with http_handler
    
    if (is_web) {  
        // Start web service  
        json_str = api_handler(conn);  
        printf("API response:\n%s\n", json_str);  
        free(json_str);  
    } 

    MHD_stop_daemon(daemon);   /// with http_hander
    PQfinish(conn);  
  
    return 0;  
}  
