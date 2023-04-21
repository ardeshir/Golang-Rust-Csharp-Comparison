# TypeScript 
## TS Docs
(nvm is my choice of node manager) 
- nvm install v18
- npm init
- npm install -g ts-node
- npm install --save express 
- npm install -g request 
- npm install --save-dev @types/request   
- npm install -g typescript
- npm install --save yargs  
- npm install --save pg csv-parser jsonfile
## To Prettier or not to prettier
- npm install --save-dev prettier
- prettier --write src/*.ts.
## Eslint
- npm install --save-dev eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
- eslint --fix src/*.ts | eslint src/*.ts 

## Run 
- npx ts-node csv_parser.ts

## When in doubt clean house 
-- rm -rf node_modules  
-- npm install  

# Improvments
- Use a tool like Knex.js or TypeORM to define and manage your database schema programmatically. This can help ensure that your database schema is always in sync with your code.
## TypeORM csv_parser_typeorm
- npm install --save typeorm pg reflect-metadata  
- npx ts-node csv_parser_typeorm.ts

