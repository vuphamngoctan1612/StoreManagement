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
select * from Product;
insert into Product 
values (1,'Mì gói','Gói',null,3000,5000,1000,0)