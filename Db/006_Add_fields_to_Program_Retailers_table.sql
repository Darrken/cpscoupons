/*
   Saturday, August 02, 20142:38:35 PM
   User: 
   Server: .
   Database: Toolkit
   Application: 
*/

/* To prevent any potential data loss issues, you should review this script in detail before running it outside the context of the database designer.*/
BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION
GO
ALTER TABLE dbo.Program_Retailers ADD
	SubmittedTs datetime NOT NULL CONSTRAINT DF_Program_Retailers_SubmittedTs DEFAULT getdate(),
	RepName varchar(50) NULL,
	Address varchar(200) NULL,
	City varchar(50) NULL,
	State varchar(50) NULL,
	Zip varchar(50) NULL,
	Phone varchar(50) NULL,
	ContactName varchar(100) NULL
GO
ALTER TABLE dbo.Program_Retailers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
