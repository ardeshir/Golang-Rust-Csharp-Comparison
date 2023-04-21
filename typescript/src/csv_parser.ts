import * as csv from 'csv-parser';  
import * as fs from 'fs';  
import * as jsonfile from 'jsonfile';  
import * as pg from 'pg';  
import * as request from 'request';  
import express from 'express';  
  
interface ApiRow {  
  id: number;  
  url: string;  
  name: string;  
  created: number;  
}  
  
interface Api {  
  id: number;  
  url: string;  
  name: string;  
  created: number;  
}  
  
async function createTableIfNotExists(client: pg.Client) {  
  await client.query(`  
    CREATE TABLE IF NOT EXISTS api (  
      id SERIAL PRIMARY KEY,  
      url TEXT,  
      name TEXT,  
      created INTEGER  
    );  
  `);  
}  
  
async function printTableApi(dbConnString: string) {  
  const client = new pg.Client(dbConnString);  
  await client.connect();  
  
  try {  
    await createTableIfNotExists(client);  
  
    const res = await client.query('SELECT * FROM api;');  
    const rows: any[] = res.rows;  
    const columns = res.fields.map((field) => field.name);  
  
    console.log(`Table: api`);  
    rows.forEach((row) => {  
      columns.forEach((column, index) => {  
        console.log(`${column}: ${row[column]} `);  
      });  
      console.log('');  
    });  
  } catch (err) {  
    console.error(err);  
  } finally {  
    await client.end();  
  }  
}  
  
async function processCsvData(url: string, dbConnString: string) {  
  const client = new pg.Client(dbConnString);  
  await client.connect();  
  
  try {  
    await createTableIfNotExists(client);  
  
    request  
      .get(url)  
      .on('error', (err) => {  
        throw err;  
      })  
      .pipe(csv())  
      .on('data', async (row: ApiRow) => {  
        try {  
          await client.query(  
            `  
              INSERT INTO api (id, url, name, created)  
              VALUES (DEFAULT, $1, $2, $3);  
            `,  
            [row.url, row.name, row.created]  
          );  
        } catch (err) {  
          console.error(err);  
        }  
      })  
      .on('end', async () => {  
        console.log('Data inserted successfully!');  
        await printTableApi(dbConnString);  
        await client.end();  
      });  
  } catch (err) {  
    console.error(err);  
    await client.end();  
  }  
}  
  
async function apiHandler(req: any, res: any): Promise<void> {  
  const dbConnString = 'postgres://postgres:postgres@localhost:5432/data';  
  const client = new pg.Client(dbConnString);  
  await client.connect();  
  
  try {  
    const result = await client.query('SELECT * FROM api;');  
    const rows: any[] = result.rows;  
  
    res.json(rows);  
  } catch (err) {  
    console.error(err);  
    res.status(500).send('Internal Server Error');  
  } finally {  
    await client.end();  
  }  
}  
  
if (require.main === module) {  
  const args = require('yargs')  
    .option('url', {  
      type: 'string',  
      default: 'https://ardeshir.io/file.csv',  
      description: 'The URL of the CSV file to fetch',  
    })  
    .option('web', {  
      type: 'boolean',  
      description: 'Start the application as a web service',  
    }).argv;  
  
  const dbConnString = 'postgres://postgres:postgres@localhost:5432/data';  
  
  processCsvData(args.url, dbConnString);  
  
  if (args.web) {  
    const express = require('express');  
    const app = express();  
  
    app.get('/api', apiHandler);  
  
    app.listen(3000, () => {  
      console.log('Server started on port 3000');  
    });  
  }  
}  
