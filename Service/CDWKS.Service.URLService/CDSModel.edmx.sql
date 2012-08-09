
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 07/03/2011 15:17:04
-- Generated from EDMX file: C:\Users\jkarr\Desktop\CDWKS.CMS\CDWKS.Service.URLService\CDSModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BXC_Content];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------


-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'CDSLinks'
CREATE TABLE [dbo].[CDSLinks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Family] nvarchar(max)  NOT NULL,
    [Type] nvarchar(max)  NOT NULL,
    [CDSManfucaturer] nvarchar(max)  NOT NULL,
    [CDSProdNum] nvarchar(max)  NOT NULL,
    [Url] nvarchar(max)  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'CDSLinks'
ALTER TABLE [dbo].[CDSLinks]
ADD CONSTRAINT [PK_CDSLinks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------