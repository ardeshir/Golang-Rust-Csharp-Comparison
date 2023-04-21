import * as csv from 'csv-parser';  
import * as fs from 'fs';  
import * as json from 'jsonfile';  
import * as pg from 'pg';  
import * as request from 'request';  
  
interface ApiRow {  
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
    const rows: ApiRow[] = res.rows;  
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
      .on('data', async (row: any) => {  
        try {  
          await client.query(  
            `  
              INSERT INTO api (url, name, created)  
              VALUES ($1, $2, $3);  
            `,  
            [row.url, row.name, row.created]  
          );  
        } catch (err) {  
          console.error(err);  
        }  
      })  
      .on('end', () => {  
        console.log('Data inserted successfully!');  
        client.end();  
      });  
  } catch (err) {  
    console.error(err);  
    await client.end();  
  }  
}  
  
async function apiHandler(): Promise<string> {  
  const dbConnString = 'postgres://postgres:postgres@localhost:5432/data';  
  const client = new pg.Client(dbConnString);  
  await client.connect();  
  
  try {  
    const res = await client.query('SELECT * FROM api;');  
    const rows: ApiRow[] = res.rows;  
    const columns = res.fields.map((field) => field.name);  
  
    const apis = rows.map((row) => {  
      const api = {};  
      columns.forEach((column, index) => {  
        api[column] = row[column];  
      });  
      return api;  
    });  
  
    await json.writeFile('apis.json', apis);  
    return 'Data written to apis.json';  
  } catch (err) {  
    console.error(err);  
    throw err;  
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
  printTableApi(dbConnString);  
  
  if (args.web) {  
    const express = require('express');  
    const app = express();  
  
    app.get('/api', async (req, res) => {  
      const data = await apiHandler(); // Await the results  
      res.send(data);  
    });  
  
    app.listen(3000, () => {  
      console.log('Server started on port 3000');  
    });  
  }  
}  