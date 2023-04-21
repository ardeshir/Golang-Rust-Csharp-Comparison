
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

function printTableApi(dbConnString: string): void {

        const client = new pg.Client(dbConnString);

        client.connect();

        client.query('SELECT * FROM api;', (err, res) => {
            if (err) throw err;

            const rows: ApiRow[] = res.rows;    
            const columns = res.fields.map(field => field.name);    

            console.log(`Table: api`);    
            rows.forEach(row => {    
                columns.forEach((column, index) => {   
                console.log(`${column}: ${row[column]} `);     
                });    
            console.log('');    
        });    

        client.end(); 

        });
}

function processCsvData(url: string, dbConnString: string): void {

        request.get(url)
        .on('error', err => { throw err; })
        .pipe(csv())
        .on('data', (row: any) => {
            const client = new pg.Client(dbConnString);
            client.connect();

        client.query(`    
            CREATE TABLE IF NOT EXISTS api (    
            id SERIAL PRIMARY KEY,    
            url TEXT,    
            name TEXT,    
            created INTEGER    
         );    
        `,
        (err, res) => {    
                 if (err) throw err;    

            client.query(`    
                INSERT INTO api (url, name, created)    
                 VALUES ($1, $2, $3);    
             `, [row.url, row.name, row.created], (err, res) => {    
                 if (err) throw err;    

                    client.end();    
             });    
        });    
    })    
    .on('end', () => {    
        console.log('Data inserted successfully!');    
    });  
      
}

async function apiHandler(): Promise<string> {
    const dbConnString = 'postgres://postgres:postgres@localhost:5432/data';
    const client = new pg.Client(dbConnString);
    await client.connect();

    try {
     const res = await client.query('SELECT * FROM api;')
     const rows: ApiRow[] = res.rows;    
     const columns = res.fields.map(field => field.name);    

    const apis = rows.map(row => {    
          const api = {};    
          columns.forEach((column, index) => {    
          api[column] = row[column];    
     });  

      return api;  

    });    
        await  client.end(); 
        await json.writeFile('apis.json', apis); 
        return 'Data written to apis.json' ;

    } catch(err) {
       throw err;
    } 
}


if (require.main === module) {
    const args = require('yargs')
    .option('url', {
      type: 'string',
      default: 'https://www.ardeshir.io/file.csv',
      description: 'The URL of the CSV file to fetch'
      })
      .option('web', {
      type: 'boolean',
       description: 'Start the application as a web service'
    })
    .argv;


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
