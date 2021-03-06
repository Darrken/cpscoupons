USE [Toolkit]
GO
/****** Object:  Table [dbo].[Program]    Script Date: 07/25/2014 23:36:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Program](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](max) NULL,
	[Disclaimer] [varchar](max) NULL,
	[CouponWordCount] [int] NULL,
 CONSTRAINT [PK_Program] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Program_Retailers]    Script Date: 07/25/2014 23:36:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Program_Retailers](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[UrlGuid] [uniqueidentifier] NOT NULL,
	[ProgramId] [int] NOT NULL,
 CONSTRAINT [PK_Program_Retailers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Program_Fields]    Script Date: 07/25/2014 23:36:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Program_Fields](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ProgramId] [int] NOT NULL,
 CONSTRAINT [PK_Program_Fields] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Program_Field_Values]    Script Date: 07/25/2014 23:36:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Program_Field_Values](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProgramFieldId] [int] NOT NULL,
	[ProgramRetailerId] [int] NOT NULL,
	[Value] [varchar](max) NOT NULL,
 CONSTRAINT [PK_Program_Field_Values] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Program_Retailer_Selected_Malls]    Script Date: 07/25/2014 23:36:19 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Program_Retailer_Selected_Malls](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ProgramRetailerId] [int] NOT NULL,
	[MallId] [int] NOT NULL,
 CONSTRAINT [PK_Program_Retailer_Selected_Malls] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  ForeignKey [FK_Program_Field_Values_Program_Retailers]    Script Date: 07/25/2014 23:36:19 ******/
ALTER TABLE [dbo].[Program_Field_Values]  WITH CHECK ADD  CONSTRAINT [FK_Program_Field_Values_Program_Retailers] FOREIGN KEY([ProgramRetailerId])
REFERENCES [dbo].[Program_Retailers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Program_Field_Values] CHECK CONSTRAINT [FK_Program_Field_Values_Program_Retailers]
GO
/****** Object:  ForeignKey [FK_Program_Fields_Program]    Script Date: 07/25/2014 23:36:19 ******/
ALTER TABLE [dbo].[Program_Fields]  WITH CHECK ADD  CONSTRAINT [FK_Program_Fields_Program] FOREIGN KEY([ProgramId])
REFERENCES [dbo].[Program] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Program_Fields] CHECK CONSTRAINT [FK_Program_Fields_Program]
GO
/****** Object:  ForeignKey [FK_Program_Retailer_Selected_Malls_Mall]    Script Date: 07/25/2014 23:36:19 ******/
ALTER TABLE [dbo].[Program_Retailer_Selected_Malls]  WITH CHECK ADD  CONSTRAINT [FK_Program_Retailer_Selected_Malls_Mall] FOREIGN KEY([MallId])
REFERENCES [dbo].[Mall] ([ID])
GO
ALTER TABLE [dbo].[Program_Retailer_Selected_Malls] CHECK CONSTRAINT [FK_Program_Retailer_Selected_Malls_Mall]
GO
/****** Object:  ForeignKey [FK_Program_Retailer_Selected_Malls_Program_Retailers]    Script Date: 07/25/2014 23:36:19 ******/
ALTER TABLE [dbo].[Program_Retailer_Selected_Malls]  WITH CHECK ADD  CONSTRAINT [FK_Program_Retailer_Selected_Malls_Program_Retailers] FOREIGN KEY([ProgramRetailerId])
REFERENCES [dbo].[Program_Retailers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Program_Retailer_Selected_Malls] CHECK CONSTRAINT [FK_Program_Retailer_Selected_Malls_Program_Retailers]
GO
/****** Object:  ForeignKey [FK_Program_Retailers_Program]    Script Date: 07/25/2014 23:36:19 ******/
ALTER TABLE [dbo].[Program_Retailers]  WITH CHECK ADD  CONSTRAINT [FK_Program_Retailers_Program] FOREIGN KEY([ProgramId])
REFERENCES [dbo].[Program] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Program_Retailers] CHECK CONSTRAINT [FK_Program_Retailers_Program]
GO
