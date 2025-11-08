CREATE DATABASE LoanAppDb;
GO
USE LoanAppDb;
GO
CREATE TABLE LoanApplication (
	Id INT IDENTITY(1,1) PRIMARY KEY,
	CustomerName VARCHAR(200) NOT NULL,
	NicPassport VARCHAR(50) NOT NULL,
	LoanTypeId INT NOT NULL,
	InterestRate decimal(5, 2) NOT NULL,
	LoanAmount DECIMAL(18,2) NOT NULL,
	DurationMonths INT NOT NULL,
	Status VARCHAR(20) NOT NULL,
	IsDeleted bit NOT NULL,
	CreatedAt DATETIME NOT NULL,
	CreatedBy varchar(250) NOT NULL,
	UpdatedAt DATETIME NOT NULL,
	UpdatedBy varchar(250) NOT NULL,
	CONSTRAINT FK_LoanApplication_LoanType 
	FOREIGN KEY (LoanTypeId) REFERENCES LoanType(Id)
);
GO

CREATE TABLE [dbo].[LoanType](
	Id INT IDENTITY(1,1) PRIMARY KEY,
	Name varchar(200) NOT NULL,
	InterestRate decimal(5, 2) NOT NULL
	);
GO

INSERT INTO [dbo].[LoanType]
           ([Name]
           ,[InterestRate])
     VALUES
           ('Personal' ,8),
           ('Housing' ,12),
		   ('Vehicle', 10)
GO


