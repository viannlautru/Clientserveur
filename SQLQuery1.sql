Create database Personne
create table Users (id INT Primary key not null identity(1,1),Nom varchar(50),Age int, IPadresse varchar(200))
select * from Users