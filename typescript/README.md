# TypeScript 
## TS Docs
(nvm is my choice of node manager) 
- nvm install v18
- npm init
- npm install -g typescript ts-node
- npm install --save request express 
- npm install --save-dev @types/request   
- npm install --save pg csv-parser jsonfile yargs 
## To Prettier or not to prettier
- npm install --save-dev prettier
- prettier --write src/*.ts.
## Eslint
- npm install --save-dev eslint @typescript-eslint/parser @typescript-eslint/eslint-plugin
- eslint --fix src/*.ts | eslint src/*.ts 

## Run 
- npm install 
- npx ts-node src/csv_parser.ts --web

## When in doubt clean house 
-- rm -rf node_modules  
-- npm install  

# Improvments
- Use a tool like Knex.js or TypeORM to define and manage your database schema programmatically. This can help ensure that your database schema is always in sync with your code.


