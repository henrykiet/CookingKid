create database CookingKid
go
use CookingKid
go
create table Customer(
    CustomerId VARCHAR(50) PRIMARY KEY DEFAULT CONVERT(VARCHAR(50), NEWID()),
    FullName NVARCHAR(255),
    EstablishmentDate DATE,
    BirthDate DATE,
    RegisteredAddress NVARCHAR(255),
    DeliveryAddress NVARCHAR(255),
    PhoneNumber VARCHAR(20),
    Email VARCHAR(100),
    Note NVARCHAR(MAX)
);

delete from Customer