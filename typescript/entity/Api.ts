import { Entity, PrimaryGeneratedColumn, Column } from 'typeorm';  
  
@Entity()  
export class Api {  
  @PrimaryGeneratedColumn()  
  id: number;  
  
  @Column()  
  url: string;  
  
  @Column()  
  name: string;  
  
  @Column()  
  created: number;  
}  

