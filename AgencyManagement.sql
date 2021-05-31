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
select * from Product
insert into Product
values (5,'Bánh', 'Gói',null,9000,10000,1000,0)
insert into Product
values (6,'Sữa', 'Hộp',null,9000,10000,1000,0)
select * from Product
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

insert into InvoiceInfo values(3)

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
--Quang test
--last month
insert into Invoice values (3,1,GETDATE() - 31,2000000,4000000)
insert into InvoiceInfo 
values (3,1,200,1000000),
(3,2,100,1500000),
(3,3,100,1500000)
insert into Invoice values (4,2,GETDATE() - 31,1000000,4000000)
insert into InvoiceInfo 
values (4,1,200,1000000),
(4,2,100,1500000),
(4,3,100,1500000)
insert into Invoice values (5,3,GETDATE() - 31,2000000,5000000)
insert into InvoiceInfo 
values (5,1,100,500000),
(5,2,100,1500000),
(5,3,100,1500000),
(5,4,100,1500000)
delete from InvoiceInfo
where InvoiceID = 5
--yesterday
insert into Invoice values (6,1,GETDATE() - 1,1000000,5000000)
insert into InvoiceInfo 
values (6,1,200,1000000),
(6,2,100,1500000),
(6,3,100,1500000),
(6,5,100,1000000)
insert into Invoice values (7,2,GETDATE() - 1,1000000,5000000)
insert into InvoiceInfo 
values (7,1,200,1000000),
(7,2,100,1500000),
(7,3,100,1500000),
(7,6,100,1000000)
insert into Invoice values (8,3,GETDATE() - 1,1000000,5000000)
insert into InvoiceInfo 
values (8,5,200,2000000),
(8,6,300,3000000)
insert into Invoice values (9,4,GETDATE() - 1,1000000,50000000)
insert into InvoiceInfo 
values (9,5,2000,20000000),
(9,6,3000,30000000)
--today
insert into Invoice values (10,4,GETDATE(),1000000,8000000)
insert into InvoiceInfo 
values (10,1,100,500000),
(10,2,100,1500000),
(10,3,100,1500000),
(10,4,100,1500000),
(10,5,100,1000000),
(10,6,200,2000000)
insert into Invoice values (11,5,GETDATE(),1000000,8000000)
insert into InvoiceInfo 
values (11,1,100,500000),
(11,2,100,1500000),
(11,3,100,1500000),
(11,4,100,1500000),
(11,5,100,1000000),
(11,6,200,2000000)
insert into Invoice values (13,3,GETDATE(),1000000,8000000)
insert into InvoiceInfo 
values (13,1,100,500000),
(13,2,100,1500000),
(13,3,100,1500000),
(13,4,100,1500000),
(13,5,100,1000000),
(13,6,200,2000000)
insert into Invoice values (14,4,GETDATE(),1000000,2000000)
insert into InvoiceInfo 
values (14,5,100,1000000),
(14,6,100,1000000)
insert into Invoice values (15,5,GETDATE(),0,1000000)
insert into InvoiceInfo 
values (15,5,100,1000000)
insert into Invoice values (16,6,GETDATE(),0,1000000)
insert into InvoiceInfo 
values (16,6,100,1000000)
insert into Invoice values (17,5,GETDATE(),0,1000000)
insert into InvoiceInfo 
values (17,5,100,1000000)
insert into Invoice values (18,6,GETDATE(),2000000,5000000)
insert into InvoiceInfo 
values (18,1,200,1000000),
(18,2,100,1500000),
(18,3,100,1500000),
(18,6,100,1000000)
insert into Invoice values (19,6,GETDATE(),2000000,5000000)
insert into InvoiceInfo 
values (19,1,200,1000000),
(19,2,100,1500000),
(19,3,100,1500000),
(19,6,100,1000000)
insert into Invoice values (20,5,GETDATE(),0,1000000)
insert into InvoiceInfo 
values (20,6,100,1000000)

select * from Product

select * from InvoiceInfo

delete from Invoice
where ID > 2

select count(ID) from Invoice
where Checkout = (select CAST (GETDATE() as date))

select GETDATE() - 1

select sum(total) as total
from Invoice
where Checkout = (select CAST (GETDATE() as date))


select * from Invoice
where Checkout = (select CAST (GETDATE() - 1 as date))

select sum(Total) from Invoice
where Checkout = (select CAST (GETDATE() - 1 as date))

select sum(Total) from Invoice
where ( select month(Checkout) as month) = (select month(GETDATE()) as month)


select  sum(Total) as total from Invoice
where ( select month(Checkout) as month) = (select month(GETDATE()) as month)
group by AgencyID
order by Total DESC

SELECT TOP 5 Agency.ID FROM Agency  
                JOIN Invoice ON Agency.ID = Invoice.AgencyID 
                WHERE MONTH(CHECKOUT) = 05
                GROUP BY Agency.ID 
                ORDER BY SUM(INVOICE.TOTAL) DESC

select sum(Total) as total from InvoiceInfo
group by ProductID
order by Total DESC

SELECT TOP 5 Product.ID FROM Product
                JOIN InvoiceInfo ON Product.ID = InvoiceInfo.ProductID
                GROUP BY Product.ID
                ORDER BY SUM(InvoiceInfo.TOTAL) DESC

select Total from Invoice
where ( select month(Checkout) as month) = (select month(GETDATE()) - 1 as month) and
		Invoice.AgencyID = 3

select CAST (GETDATE() - 1 as date)

SELECT MONTH(GETDATE()) - 1 AS Month;

SELECT SUM(Debt) AS TOTAL FROM Invoice  
WHERE YEAR(CHECKOUT) = 2021
GROUP BY DATEPART(QUARTER, CHECKOUT)

-- đống này kh insert được do thiếu product
--INSERT INTO InvoiceInfo VALUES (1, 1,3, 600000)
--INSERT INTO InvoiceInfo VALUES (1, 2,3, 450000)
--INSERT INTO InvoiceInfo VALUES (1, 3,3, 600000)
--INSERT INTO InvoiceInfo VALUES (1, 4,3, 240000)

--INSERT INTO InvoiceInfo VALUES (2, 1,3, 600000)
--INSERT INTO InvoiceInfo VALUES (2, 2,3, 450000)
--INSERT INTO InvoiceInfo VALUES (2, 3,3, 600000)
--INSERT INTO InvoiceInfo VALUES (2, 4,3, 1000000)
