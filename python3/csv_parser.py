import csv  
import json  
import psycopg2  
import requests  
from typing import List  
  
def print_table_api(db_conn_string: str) -> None:  
    conn = psycopg2.connect(db_conn_string)  
    cursor = conn.cursor()  
  
    cursor.execute("SELECT * FROM api;")  
    rows = cursor.fetchall()  
    columns = [column[0] for column in cursor.description]  
  
    print(f"Table: {'api'}")  
    for row in rows:  
        for i, col in enumerate(row):  
            print(f"{columns[i]}: {col}", end=" ")  
        print("")  
  
    cursor.close()  
    conn.close()  
  
def process_csv_data(url: str, db_conn_string: str) -> None:  
    response = requests.get(url)  
    response.raise_for_status()  
    data = list(csv.reader(response.content.decode().splitlines()[1:]))  
  
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
  
    for row in data[1:]:  
        cursor.execute("""  
            INSERT INTO api (url, name, created)  
            VALUES (%s, %s, %s);  
        """, (row[1], row[2], row[3]))  
  
    conn.commit()  
    cursor.close()  
    conn.close()  
  
    print("Data inserted successfully!")  
  
def api_handler() -> str:  
    db_conn_string = "dbname=data user=postgres password=postgres host=localhost port=5432 sslmode=disable"  
  
    conn = psycopg2.connect(db_conn_string)  
    cursor = conn.cursor()  
  
    cursor.execute("SELECT * FROM api;")  
    rows = cursor.fetchall()  
  
    cursor.close()  
    conn.close()  
  
    columns = ["id", "url", "name", "created"]  
    apis = [dict(zip(columns, row)) for row in rows]  
  
    return json.dumps(apis)  
  
if __name__ == "__main__":  
    import argparse  
  
    parser = argparse.ArgumentParser()  
    parser.add_argument("--url", default="https://www.ardeshir.io/file.csv", help="The URL of the CSV file to fetch")  
    parser.add_argument("--web", action="store_true", help="Start the application as a web service")  
    args = parser.parse_args()  
  
    db_conn_string = "dbname=data user=postgres password=postgres host=localhost port=5432 sslmode=disable"  
  
    process_csv_data(args.url, db_conn_string)  
    print_table_api(db_conn_string)  
  
    if args.web:  
        from flask import Flask  
  
        app = Flask(__name__)  
  
        @app.route("/api")  
        def api():  
            return api_handler()  
  
        app.run()  
