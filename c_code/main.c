#include <stdio.h>  
#include <stdlib.h>  
#include <string.h>  
#include <curl/curl.h>  
#include <libpq-fe.h>  
#include <jansson.h>  
  
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

static size_t curl_callback(void *contents, size_t size, size_t nmemb, void *userp) {  
    return size * nmemb;  
} 

void process_csv_data(char* url, PGconn* conn) {  
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
        curl_easy_setopt(curl, CURLOPT_URL, url);  
        curl_easy_setopt(curl, CURLOPT_FOLLOWLOCATION, 1L);  
        curl_easy_setopt(curl, CURLOPT_WRITEFUNCTION, curl_callback);  
        curl_easy_setopt(curl, CURLOPT_WRITEDATA, &data);  
        res = curl_easy_perform(curl);  
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
            char* cols[4];  
            j = 0;  
            token = strtok(rows[i], ",");  
            while (token != NULL) {  
                cols[j++] = token;  
                token = strtok(NULL, ",");  
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
  
    printf("Data inserted successfully!\n");  
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

int main(int argc, char** argv) {  
    char* url = "https://www.ardeshir.io/file.csv";  
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
  
    if (is_web) {  
        // Start web service  
        json_str = api_handler(conn);  
        printf("API response:\n%s\n", json_str);  
        free(json_str);  
    }  
  
    PQfinish(conn);  
  
    return 0;  
}  
