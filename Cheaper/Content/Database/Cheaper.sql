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
"FullName" TEXT NOT NULL,
"Multiplier" REAL NOT NULL); -- Multiplier to convert everything to ounces (fluid ounce for volume)
COMMIT;

BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Comparison";
CREATE TABLE "Comparison" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"UnitTypeId" INTEGER REFERENCES UnitType(Id) ON DELETE NO ACTION,
"UnitId" INTEGER REFERENCES UnitType(Id) ON DELETE NO ACTION,
"CategoryId" INTEGER REFERENCES Category(Id) ON DELETE NO ACTION DEFAULT NULL,
"Name" TEXT DEFAULT NULL,
"CheapestComparableId" DEFAULT NULL);
COMMIT;

BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Comparable";
CREATE TABLE "Comparable" (
"Id" INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
"ComparisonId" INTEGER REFERENCES Comparison(id) ON DELETE CASCADE NULL,
"UnitId" INTEGER REFERENCES Unit(id) ON DELETE NO ACTION NOT NULL,
"Store" TEXT DEFAULT NULL,
"Product" TEXT DEFAULT NULL,
"Price" REAL NOT NULL DEFAULT 0,
"Quantity" REAL NOT NULL DEFAULT 0,
"ModifiedOn" TEXT NOT NULL);
COMMIT;


BEGIN TRANSACTION;
DROP TABLE IF EXISTS "Meta";
CREATE TABLE "Meta" (
"DbVersion" REAL DEFAULT NULL);
COMMIT;

insert into Meta (DbVersion) values (1);

insert into UnitType (Id, Name) values (1, "Weight");
insert into UnitType (Id, Name) values (2, "Volume");
insert into UnitType (Id, Name) values (3, "Count");

insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('each', 'each', 3, 1);

insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('oz', 'ounce', 1, 1);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('lb', 'pound', 1, 16);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('g', 'gram', 1, 0.0352739619);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('kg', 'kilogram', 1, 35.2739619);

insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('fl oz', 'fluid ounce', 2, 1);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('l', 'liter', 2, 33.8140227);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('ml', 'milliliter', 2, 0.0338140227);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('pt', 'pint', 2, 16);
insert into Unit (Name, FullName, UnitTypeId, Multiplier) values ('gal', 'gallon', 2, 128);

