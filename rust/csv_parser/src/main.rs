use std::error::Error;
use std::io::Read;
use std::str::FromStr;

use csv::ReaderBuilder;
use postgres::{Client, NoTls};

#[derive(Debug)]
struct Record {
    id: i32,
    url: String,
    name: String,
    created: i32,
}

impl Record {
    fn from_csv_record(record: csv::StringRecord) -> Result<Self, Box<dyn Error>> {
        let mut writer = csv::Writer::from_writer(vec![]);
        writer.write_record(&record)?;
        let s = String::from_utf8(writer.into_inner()?)?;
        let s = s.trim_end_matches('\n').to_owned();
        Self::from_str(&s)
    }
}

impl FromStr for Record {
    type Err = Box<dyn Error>;

    fn from_str(s: &str) -> Result<Self, Self::Err> {
        let mut fields = s.split(',');
        let id_str = fields.next().ok_or("missing id")?.trim();

        let url = fields
            .next()
            .ok_or("missing url")?
            .trim_matches('"')
            .to_owned();

        let name = fields
            .next()
            .ok_or("missing name")?
            .trim_matches('"')
            .to_owned();
        let created_str = fields.next().ok_or("missing created")?.trim();

        println!("id_str: {}", id_str); // Print id_str
        println!("record: {}", s); // Print the entire CSV record

        let id = id_str
            .chars()
            .all(|c| c.is_digit(10))
            .then(|| id_str.parse::<i32>().ok())
            .flatten()
            .ok_or("invalid id")?;

        let created = created_str
            .chars()
            .all(|c| c.is_digit(10))
            .then(|| created_str.parse::<i32>().ok())
            .flatten()
            .ok_or("invalid created")?;

        Ok(Record {
            id,
            url,
            name,
            created,
        })
    }
}

fn main() -> Result<(), Box<dyn Error>> {
    let url = "https://ardeshir.io/file.csv";
    let mut response = reqwest::blocking::get(url)?;
    let mut csv_data = String::new();
    response.read_to_string(&mut csv_data)?;

    let mut reader = ReaderBuilder::new()
        .has_headers(true)
        .from_reader(csv_data.as_bytes());

       // Skip the first row  
       // reader.records().next(); 

    let mut records = Vec::new();
    for result in reader.records() {
        let record = Record::from_csv_record(result?)?;
        records.push(record);
    }

    let mut client = Client::connect("postgresql://postgres:postgres@localhost:5432/data", NoTls)?;
    client.batch_execute(
        "CREATE TABLE IF NOT EXISTS api (  
             id SERIAL PRIMARY KEY,  
             url VARCHAR(256) NOT NULL,  
             name VARCHAR(256) NOT NULL,  
             created INTEGER NOT NULL  
         )",
    )?;

    for record in records.iter() {
        client.execute(
            "INSERT INTO api (url, name, created) VALUES ($1, $2, $3)",
            &[&record.url, &record.name, &record.created],
        )?;
    }

    let rows = client.query("SELECT id, url, name, created FROM api", &[])?;
    for row in rows.iter() {
        let id: i32 = row.get(0);
        let url: String = row.get(1);
        let name: String = row.get(2);
        let created: i32 = row.get(3);
        println!("{:<3} | {:<25} | {:<8} | {}", id, url, name, created);
    }

    Ok(())
}
// error: linking with `x86_64-w64-mingw32-gcc` failed: exit code: 1
//  note: ld: cannot find -lntdll