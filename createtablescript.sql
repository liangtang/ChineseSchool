Create table Semester
(
	SemesterID int primary key identity(1,1) not null,
	SemesterYear int not null,
	SemesterSeason nvarchar(8) not null,
	RegisterStartDate date not null,
	RegisterEndDate date not null,
	Price1 decimal(8,2) not null,
	Price1Description nvarchar(512) null,
	Price2 decimal(8,2) not null,
	Price2Description nvarchar(512) null,
	Price3 decimal(8,2) not null,
	Price3Description nvarchar(512) null,
	Price4 decimal(8,2) null,
	Price4Description nvarchar(512) null,
	Price5 decimal(8,2) null,
	Price5Description nvarchar(512) null,
	ActiveFlg bit not null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null,
	
) ON [PRIMARY]

Create table EnrichmentClass
(
	ClassID int primary key identity(1,1) not null,
	ClassName nvarchar(64) not null,
	Price1 decimal (8,2) not null,
	Price1Description nvarchar(512) null,
	Price2 decimal(8,2) not null,
	Price2Descrioption nvarchar(512) null,
	ActiveFlg bit not null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null
)
GO


Create table Parent
(
	ParentsId int primary key identity(1,1) not null,
	Parent1Firstname nvarchar(128) not null,
	Parent1LastName nvarchar(128) not null,
	Parent2Firstname nvarchar(128) not null,
	Parent2Lastname nvarchar(128) not null,
	AddressLine1 nvarchar(256) not null,
	AddressLine2 nvarchar(256) null,
	City nvarchar(128) not null,
	State nvarchar(32) not null,
	ZipCode nvarchar(32) not null,
	PrimaryPhone nvarchar(32) not null,
	SecondaryPhone nvarchar(32) null,
	PrimaryEmail nvarchar(256) not null,
	SecondaryEmail nvarchar(256) null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null
)
GO
Create table Student
(
	StudentId int primary key identity(1,1) not null,
	ParentsId int not null,
	Firstname nvarchar(128) not null,
	Lastname nvarchar(128) not null,
	MiddleInitial nvarchar(1) null,
	Chinesename nvarchar(128) null,
	Gender nchar(1) not null,
	Birthday date not null,
	Grade tinyint null,
	Class nvarchar(8) null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null,
	constraint FK_Student_ParentsID foreign key (ParentsId) references Parent (ParentsId)
)
GO
Create table Transactions
(
	TransactionID  int primary key identity(1,1) not null,
	UserID nvarchar(128) not null,
	ParentsId int not null,
	TransactionType nvarchar(128) not null,
	TransactionDate date not null,
	TransactionDescription nvarchar(512) not null,
	TransactionAmount decimal(8,2) not null,
	ActiveFlg bit not null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null,
	constraint FK_Transaction_ParentsID foreign key (ParentsId) references Parent (ParentsId)
)
GO

Create table Class
(
	ClassId int primary key identity(1,1) not null,
	Classname nvarchar(8) not null,
	ClassDescription nvarchar(512) null,
	ActiveFlg nvarchar(1) not null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null,
	
)

Create table ClassRegistration
(
	ClassRegistrationId int primary key identity(1,1) not null,
	StudentId int not null,
	SemesterId int not null,
	ClassId int null,
	Amount decimal(8,2) not null,
	Description nvarchar(512) null,
	ActiveFlag bit not null,
	CreateTimeStemp datetime not null,
	CreateUserId nvarchar(128) not null,
	UpdateTimeStemp datetime not null,
	UpdateUserId nvarchar(128) not null,
	constraint FK_Student_ClassID foreign key (StudentId) references Parent (ParentsId)
)