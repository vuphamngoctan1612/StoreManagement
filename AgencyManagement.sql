create database AgencyManagement
go
use AgencyManagement
go

create table Account
(
	Username varchar(100),
	Password varchar(1000),
	DisplayName nvarchar(100),
	Image image,

	constraint PK_Account primary key(Username)
)
go

create table Agency
(
	ID int,
	Name nvarchar(100),
	TypeOfAgency int,
	PhoneNumber varchar(100),
	Address nvarchar(100),
	District nvarchar(100),
	CheckIn date,
	Email varchar(100),
	Debt bigint,
	IsDelete bit,

	constraint PK_Agency primary key(ID)
)
go

create table TypeOfAgency
(
	ID int,
	Name nvarchar(100),
	MaxOfDebt bigint,

	CONSTRAINT PK_TypeOfAgency PRIMARY KEY (ID)
)
go

create table Product
(
	ID int,
	Name nvarchar(100),
	Unit nvarchar(100),
	Image image,
	ImportPrice bigint,
	ExportPrice bigint,
	Count bigint,
	IsDelete bit,

	constraint PK_Product primary key (ID)
)
go


create table Invoice
(
	ID int,
	AgencyID int,
	Checkout date,
	Debt bigint, 

	constraint PK_Invoice primary key(ID)
)
go

create table InvoiceInfo
(
	InvoiceID int,
	ProductID int,
	Amount int,
	Total bigint,

	constraint PK_InvoiceInfo primary key(InvoiceID, ProductID)
)
go

create table Receipt
(
	ID int,
	AgencyID int,
	Date date,
	Amount bigint,
	Message nvarchar(1000), --text để ghi nội dung thanh toán

	constraint PK_Receipt primary key(ID)
)
go


alter table InvoiceInfo add constraint FK_InvoiceID foreign key (InvoiceID) references Invoice(ID)
go
alter table InvoiceInfo add constraint FK_ProductID foreign key (ProductID) references Product(ID)
go

alter table Invoice add constraint FK_Invoice_AgencyID foreign key (AgencyID) references Agency(ID)
go

alter table Receipt add constraint FK_Receipt_AgencyID foreign key (AgencyID) references Agency(ID)
go

alter table Agency add constraint FK_Type foreign key (TypeOfAgency) references TypeOfAgency(ID)
go



-- insert
insert into TypeOfAgency values (1, 'Type 1', 1000000)
insert into TypeOfAgency values (2, 'Type 2', 2000000)

insert into Agency values (1, 'Agency 1', 1, '0987654321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 500000, 0)
insert into Agency values (2, 'Agency 2', 2, '0987234321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 600000, 0)
insert into Agency values (3, 'Agency 3', 2, '0985314321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 800000, 0)
insert into Agency values (4, 'Agency 4', 1, '0987844321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 900000, 0)

select * from InvoiceInfo
select * from Invoice
select * from Product


insert into Invoice values (1,1,GETDATE(), 200000)
insert into Invoice values (2,1,GETDATE(), 340000)
insert into Invoice values (3,1,GETDATE(), 6100000)

insert into Invoice values (4,2,GETDATE(), 860000)
insert into Invoice values (5,2,GETDATE(), 900000)
insert into Invoice values (6,2,GETDATE(), 640000)
insert into Invoice values (7,2,GETDATE(), 2500000)

insert into Invoice values (8,3,GETDATE(), 700000)
insert into Invoice values (9,3,GETDATE(), 900000)
insert into Invoice values (10,3,GETDATE(), 550000)
insert into Invoice values (11,3,GETDATE(), 1000000)

insert into Invoice values (12,4,GETDATE(), 2000000)
insert into Invoice values (13,4,GETDATE(), 210000)
insert into Invoice values (14,4,GETDATE(), 500000)
insert into Invoice values (15,4,GETDATE(), 200000)
insert into Invoice values (16,4,'2021-2-19', 290000)

--GET DAYS IN MONTH
SELECT DAY(CHECKOUT) AS DAY FROM Invoice
WHERE MONTH(CHECKOUT) = 4 AND YEAR(CHECKOUT) = 2021
GROUP BY DAY(CHECKOUT)
-- GET MONTH IN YEAR
SELECT MONTH(CHECKOUT) AS MONTH FROM Invoice
WHERE YEAR(CHECKOUT) = 2021
GROUP BY MONTH(CHECKOUT)
--GET QUARTER IN YEAR
SELECT DATEPART(QUARTER, CHECKOUT) AS QUARTER FROM Invoice
WHERE YEAR(CHECKOUT) = 2021
GROUP BY DATEPART(QUARTER, CHECKOUT)


-- GET DEBT BY YEAR
SELECT SUM(DEBT) AS TOTAL FROM Invoice
WHERE YEAR(CHECKOUT) = 2021
GROUP BY MONTH(CHECKOUT)
-- GET DEBT BY QUARTER
SELECT SUM(Debt) AS TOTAL FROM Invoice 
WHERE YEAR(CHECKOUT) = 2021
GROUP BY DATEPART(QUARTER, CHECKOUT)
-- GET DEBT BY DAYS
SELECT DAY(CHECKOUT) AS DAY, SUM(Debt) AS TOTAL FROM Invoice
WHERE MONTH(CHECKOUT) = 2 AND YEAR(CHECKOUT) = 2021
GROUP BY DAY(CHECKOUT)

SELECT * FROM Invoice
SELECT * FROM InvoiceInfo
SELECT * FROM Product

SELECT SUM(Total) FROM InvoiceInfo
JOIN Invoice ON InvoiceInfo.InvoiceID = Invoice.ID
WHERE MONTH(Checkout) = 4 AND YEAR(CHECKOUT) = 2021
GROUP BY DAY(CHECKOUT)

SELECT SUM(Total) FROM InvoiceInfo
JOIN Invoice ON InvoiceInfo.InvoiceID = Invoice.ID
WHERE YEAR(CHECKOUT) = 2021
GROUP BY MONTH(Checkout)

SELECT SUM(Total) FROM InvoiceInfo
JOIN Invoice ON InvoiceInfo.InvoiceID = Invoice.ID
WHERE YEAR(CHECKOUT) = 2021
GROUP BY DATEPART(QUARTER, CHECKOUT)

INSERT INTO InvoiceInfo VALUES (1, 1,3, 600000)
INSERT INTO InvoiceInfo VALUES (1, 2,3, 450000)
INSERT INTO InvoiceInfo VALUES (1, 3,3, 600000)
INSERT INTO InvoiceInfo VALUES (1, 4,3, 240000)

INSERT INTO InvoiceInfo VALUES (2, 1,3, 600000)
INSERT INTO InvoiceInfo VALUES (2, 2,3, 450000)
INSERT INTO InvoiceInfo VALUES (2, 3,3, 600000)
INSERT INTO InvoiceInfo VALUES (2, 4,3, 1000000)