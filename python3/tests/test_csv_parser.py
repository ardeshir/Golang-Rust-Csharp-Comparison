import sys  
sys.path.append('/Users/ardeshir/Golang-Rust-Csharp-Comparison/python3/') 

import pytest  
import psycopg2  
from csv_parser import print_table_api, process_csv_data, api_handler  
  
# Define test fixtures  
@pytest.fixture  
def db_conn_string():  
    return "dbname=data user=postgres password=postgres host=localhost port=5432 sslmode=disable"  
  
@pytest.fixture  
def create_test_data(db_conn_string):  
    conn = psycopg2.connect(db_conn_string)  
    cursor = conn.cursor()  
  
    cursor.execute("""    
        CREATE TABLE IF NOT EXISTS api (    
            id SERIAL PRIMARY KEY,    
            url TEXT,    
            name TEXT,    
            created INTEGER    
        );    
    """)  
  
    cursor.execute("""    
        INSERT INTO api (url, name, created)    
        VALUES ('http://example.com', 'Example API', 1234567890);    
    """)  
  
    conn.commit()  
    cursor.close()  
    conn.close()  
  
# Define test functions  
def test_print_table_api(capsys, db_conn_string, create_test_data):  
    print_table_api(db_conn_string)  
    captured = capsys.readouterr()  
    assert "Table: api" in captured.out  
    assert "id: 1" in captured.out  
    assert "url: http://example.com" in captured.out  
    assert "name: Example API" in captured.out  
    assert "created: 1234567890" in captured.out  
  
def test_process_csv_data(db_conn_string, create_test_data):  
    url = "https://www.ardeshir.io/file.csv"  
    process_csv_data(url, db_conn_string)  
  
    conn = psycopg2.connect(db_conn_string)  
    cursor = conn.cursor()  
  
    cursor.execute("SELECT * FROM api;")  
    rows = cursor.fetchall()  
  
    cursor.close()  
    conn.close()  
  
    assert len(rows) == 3  
    assert rows[1][1] == "http://example.org"  
    assert rows[1][2] == "Example API 2"  
    assert rows[1][3] == 1234567891  
  
def test_api_handler(db_conn_string, create_test_data):  
    result = api_handler()  
    assert len(result) > 0  
    assert "id" in result  
    assert "url" in result  
    assert "name" in result  
    assert "created" in result  
