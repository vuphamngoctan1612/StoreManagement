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
	Location nvarchar(100),
	PhoneNumber nvarchar(10),

	constraint PK_Account primary key(Username)
)
go
insert into Account
values ('dung', '625d45c587033e8970af8b4e3fdb575c', 'dung' , null)
insert into Account
values ('quang', 'c4ca4238a0b923820dcc509a6f75849b', 'quang' , null)
select * from Account

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
	Total bigint,

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

create table StockReceipt
(
	ID int,
	CheckIn date,
	Total bigint,

	constraint PK_StockReceipt primary key(ID)
)
go

create table StockReceiptInfo
(
	StockReceiptID int,
	ProductID int,
	Amount bigint,
	Price bigint,

	constraint PK_StockReceiptInfo primary key(StockReceiptID, ProductID)
)
go
update StockReceipt
set Total = 50000
where ID = 1


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

alter table StockReceiptInfo add constraint FK_StockReceipt foreign key(StockReceiptID) references StockReceipt(ID)
go

alter table StockReceiptInfo add constraint FK_Product foreign key(ProductID) references Product(ID)
go

-- insert
insert into TypeOfAgency values (1, 'Type 1', 1000000)
insert into TypeOfAgency values (2, 'Type 2', 2000000)

insert into Agency values (3, 'Agency 3', 1, '0987654321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 500000, 0)
insert into Agency values (4, 'Agency 4', 2, '0987234321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 600000, 0)
insert into Agency values (5, 'Agency 5', 2, '0985314321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 800000, 0)
insert into Agency values (6, 'Agency 6', 1, '0987844321', 'TP HCM', 'Quan 1', GETDATE(), 'abcdxyz@gmail.com', 900000, 0)

insert into Invoice values (1,1,'2021-2-19', 200000, 20000000)
insert into Invoice values (2,1,'2021-2-28', 340000, 2900000)
insert into Invoice values (3,1,'2021-2-3', 6100000, 5200000)

insert into Invoice values (4,2,'2021-3-20', 860000, 5600000)
insert into Invoice values (5,2,'2021-3-21', 900000, 6000000)
insert into Invoice values (6,2,'2021-3-24', 640000, 5000000)
insert into Invoice values (7,2,'2021-3-25', 2500000, 2500000)

insert into Invoice values (8,3,'2021-4-9', 700000, 9000000)
insert into Invoice values (9,3,'2021-4-10', 900000, 2500000)
insert into Invoice values (10,3,'2021-4-3', 550000, 200000)
insert into Invoice values (11,3,'2021-4-25', 1000000, 1000000)

insert into Invoice values (12,4,'2021-1-19', 2000000, 20000000)
insert into Invoice values (13,4,GETDATE(), 210000, 2900000)
insert into Invoice values (14,4,GETDATE(), 500000, 5200000)
insert into Invoice values (15,4,GETDATE(), 200000, 4000000)
insert into Invoice values (16,4,'2021-1-25', 290000, 2900000)
insert into Invoice values (3,1,GETDATE(),2000000,4000000)
insert into Invoice values (4,2,GETDATE(),1000000,4000000)
insert into Invoice values (5,1,GETDATE(),2000000,5000000)
insert into Invoice values (6,1,'2021-05-27',1000000,5000000)
insert into Invoice values (7,1,dateadd(day,datediff(day,1,GETDATE()),0),1000000,5000000)
insert into Invoice values (8,2,'2021-05-26',1000000,5000000)
insert into Invoice values (9,2,'2021-04-26',1000000,50000000)
insert into Invoice values (10,2,'2021-05-27',1000000,8000000)
insert into Invoice values (11,2,'2021-05-27',1000000,8000000)
insert into Invoice values (13,2,'2021-05-27',1000000,8000000)
insert into Invoice values (14,2,'2021-05-27',1000000,2000000)
insert into Invoice values (15,2,'2021-05-27',0,1000000)
insert into Invoice values (16,2,'2021-05-27',0,1000000)
insert into Invoice values (17,2,'2021-05-27',0,1000000)
select count(ID) from Invoice
where Checkout = (select CAST (GETDATE() as date))

select sum(Total) from Invoice
where Checkout = (select CAST (GETDATE() as date))


select * from Invoice
where Checkout = (select CAST (GETDATE() - 1 as date))

select sum(Total) from Invoice
where Checkout = (select CAST (GETDATE() - 1 as date))

select sum(Total) from Invoice
where ( select month(Checkout) as month) = (select month(GETDATE()) as month)

select sum(Total) from Invoice
where ( select month(Checkout) as month) = (select month(GETDATE()) - 1 as month)

select CAST (GETDATE() - 1 as date)

SELECT MONTH(GETDATE()) - 1 AS Month;

-- đống này kh insert được do thiếu product
--INSERT INTO InvoiceInfo VALUES (1, 1,3, 600000)
--INSERT INTO InvoiceInfo VALUES (1, 2,3, 450000)
--INSERT INTO InvoiceInfo VALUES (1, 3,3, 600000)
--INSERT INTO InvoiceInfo VALUES (1, 4,3, 240000)

--INSERT INTO InvoiceInfo VALUES (2, 1,3, 600000)
--INSERT INTO InvoiceInfo VALUES (2, 2,3, 450000)
--INSERT INTO InvoiceInfo VALUES (2, 3,3, 600000)
--INSERT INTO InvoiceInfo VALUES (2, 4,3, 1000000)
