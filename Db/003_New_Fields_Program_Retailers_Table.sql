/*
   Saturday, July 26, 201411:17:20 PM
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
	StoreName varchar(50) NULL,
	Offer varchar(MAX) NULL,
	Restrictions varchar(MAX) NULL
GO
ALTER TABLE dbo.Program_Retailers SET (LOCK_ESCALATION = TABLE)
GO
COMMIT
