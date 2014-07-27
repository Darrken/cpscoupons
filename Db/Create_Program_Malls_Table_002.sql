USE [Toolkit]
GO

/****** Object:  Table [dbo].[Program_Malls]    Script Date: 07/26/2014 22:16:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Program_Malls](
	[ProgramId] [int] NOT NULL,
	[MallId] [int] NOT NULL,
 CONSTRAINT [PK_Program_Malls] PRIMARY KEY CLUSTERED 
(
	[ProgramId] ASC,
	[MallId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Program_Malls]  WITH CHECK ADD  CONSTRAINT [FK_Program_Malls_Mall] FOREIGN KEY([MallId])
REFERENCES [dbo].[Mall] ([ID])
GO

ALTER TABLE [dbo].[Program_Malls] CHECK CONSTRAINT [FK_Program_Malls_Mall]
GO

ALTER TABLE [dbo].[Program_Malls]  WITH CHECK ADD  CONSTRAINT [FK_Program_Malls_Program] FOREIGN KEY([ProgramId])
REFERENCES [dbo].[Program] ([Id])
GO

ALTER TABLE [dbo].[Program_Malls] CHECK CONSTRAINT [FK_Program_Malls_Program]
GO

