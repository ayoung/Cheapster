BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Category";
CREATE TABLE "Category" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Name" TEXT NOT NULL DEFAULT NULL);
COMMIT;

BEGIN TRANSACTION;
DROP TABLE IF EXISTS "UnitType";
CREATE TABLE "UnitType" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"Name" TEXT NOT NULL);
COMMIT;

BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Unit";
CREATE TABLE "Unit" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"UnitTypeId" INTEGER REFERENCES UnitType(Id) ON DELETE NO ACTION,
"Name" TEXT NOT NULL,
"FullName" TEXT NOT NULL);
COMMIT;

BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Comparison";
CREATE TABLE "Comparison" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"UnitTypeId" INTEGER REFERENCES UnitType(Id) ON DELETE NO ACTION,
"UnitId" INTEGER REFERENCES UnitType(Id) ON DELETE NO ACTION,
"CategoryId" INTEGER REFERENCES Category(Id) ON DELETE NO ACTION DEFAULT NULL,
"Name" TEXT DEFAULT NULL);
COMMIT;

BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Comparable";
CREATE TABLE "Comparable" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"ComparisonId" INTEGER REFERENCES Comparison(id) ON DELETE CASCADE NULL,
"UnitId" INTEGER REFERENCES Unit(id) ON DELETE NO ACTION NOT NULL,
"Store" TEXT DEFAULT NULL,
"Product" TEXT DEFAULT NULL,
"Price" REAL NOT NULL DEFAULT NULL,
"Quantity" REAL NOT NULL DEFAULT NULL);
COMMIT;

insert into UnitType (Id, Name) values (1, "Weight");
insert into UnitType (Id, Name) values (2, "Volume");
insert into UnitType (Id, Name) values (3, "Count");

insert into Unit (Name, FullName, UnitTypeId) values ('each', 'each', 3);
insert into Unit (Name, FullName, UnitTypeId) values ('oz', 'ounce', 1);
insert into Unit (Name, FullName, UnitTypeId) values ('lb', 'pound', 1);
insert into Unit (Name, FullName, UnitTypeId) values ('g', 'gram', 1);
insert into Unit (Name, FullName, UnitTypeId) values ('mg', 'milligram', 1);
insert into Unit (Name, FullName, UnitTypeId) values ('kg', 'kilogram', 1);

insert into Unit (Name, FullName, UnitTypeId) values ('floz', 'fluid ounce', 2);
insert into Unit (Name, FullName, UnitTypeId) values ('l', 'liter', 2);
insert into Unit (Name, FullName, UnitTypeId) values ('ml', 'milliliter', 2);
insert into Unit (Name, FullName, UnitTypeId) values ('pt', 'pint', 2);
insert into Unit (Name, FullName, UnitTypeId) values ('gal', 'gallon', 2);