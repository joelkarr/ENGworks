
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/27/2012 18:42:59
-- Generated from EDMX file: C:\Dev\ENGworks\Model\CDWKS.Model.EF.MasterControl\BXCMasterControlModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [BXC_MasterControl];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_LibraryDownload]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Downloads] DROP CONSTRAINT [FK_LibraryDownload];
GO
IF OBJECT_ID(N'[dbo].[FK_UserDownload]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Downloads] DROP CONSTRAINT [FK_UserDownload];
GO
IF OBJECT_ID(N'[dbo].[FK_ExtendedPropertyPropertyName]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExtendedProperties] DROP CONSTRAINT [FK_ExtendedPropertyPropertyName];
GO
IF OBJECT_ID(N'[dbo].[FK_ExtendedPropertyPropertyValue]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExtendedProperties] DROP CONSTRAINT [FK_ExtendedPropertyPropertyValue];
GO
IF OBJECT_ID(N'[dbo].[FK_UserExtendedProperty]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[ExtendedProperties] DROP CONSTRAINT [FK_UserExtendedProperty];
GO
IF OBJECT_ID(N'[dbo].[FK_LicenseOwner]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Licenses] DROP CONSTRAINT [FK_LicenseOwner];
GO
IF OBJECT_ID(N'[dbo].[FK_LicenseLibrary_Libraries]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LicenseLibrary] DROP CONSTRAINT [FK_LicenseLibrary_Libraries];
GO
IF OBJECT_ID(N'[dbo].[FK_LicenseLibrary_Licenses]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LicenseLibrary] DROP CONSTRAINT [FK_LicenseLibrary_Licenses];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Downloads]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Downloads];
GO
IF OBJECT_ID(N'[dbo].[ExtendedProperties]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ExtendedProperties];
GO
IF OBJECT_ID(N'[dbo].[Libraries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Libraries];
GO
IF OBJECT_ID(N'[dbo].[Licenses]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Licenses];
GO
IF OBJECT_ID(N'[dbo].[Owners]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Owners];
GO
IF OBJECT_ID(N'[dbo].[PropertyNames]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PropertyNames];
GO
IF OBJECT_ID(N'[dbo].[PropertyValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[PropertyValues];
GO
IF OBJECT_ID(N'[dbo].[UserLibraries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[UserLibraries];
GO
IF OBJECT_ID(N'[dbo].[Users]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Users];
GO
IF OBJECT_ID(N'[dbo].[LicenseLibrary]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LicenseLibrary];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Downloads'
CREATE TABLE [dbo].[Downloads] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Content_Id] nvarchar(max)  NOT NULL,
    [DateTime] datetime  NOT NULL,
    [Library_Id] int  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'ExtendedProperties'
CREATE TABLE [dbo].[ExtendedProperties] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [PropertyName_Id] int  NOT NULL,
    [PropertyValue_Id] int  NOT NULL,
    [User_Id] int  NOT NULL
);
GO

-- Creating table 'Libraries'
CREATE TABLE [dbo].[Libraries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Licenses'
CREATE TABLE [dbo].[Licenses] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AuthCode] nvarchar(max)  NOT NULL,
    [MaxUsers] bigint  NOT NULL,
    [LicenseType_Id] int  NOT NULL,
    [Owner_Id] int  NOT NULL
);
GO

-- Creating table 'Owners'
CREATE TABLE [dbo].[Owners] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [CreatedDate] datetime  NOT NULL
);
GO

-- Creating table 'PropertyNames'
CREATE TABLE [dbo].[PropertyNames] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'PropertyValues'
CREATE TABLE [dbo].[PropertyValues] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Value] nvarchar(max)  NOT NULL,
    [ExtendedProperty_Id] int  NOT NULL
);
GO

-- Creating table 'UserLibraries'
CREATE TABLE [dbo].[UserLibraries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] int  NOT NULL,
    [LibraryId] int  NOT NULL
);
GO

-- Creating table 'Users'
CREATE TABLE [dbo].[Users] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserName] nvarchar(max)  NOT NULL,
    [Password] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'LicenseLibrary'
CREATE TABLE [dbo].[LicenseLibrary] (
    [Libraries_Id] int  NOT NULL,
    [Licenses_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Downloads'
ALTER TABLE [dbo].[Downloads]
ADD CONSTRAINT [PK_Downloads]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ExtendedProperties'
ALTER TABLE [dbo].[ExtendedProperties]
ADD CONSTRAINT [PK_ExtendedProperties]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Libraries'
ALTER TABLE [dbo].[Libraries]
ADD CONSTRAINT [PK_Libraries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Licenses'
ALTER TABLE [dbo].[Licenses]
ADD CONSTRAINT [PK_Licenses]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Owners'
ALTER TABLE [dbo].[Owners]
ADD CONSTRAINT [PK_Owners]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PropertyNames'
ALTER TABLE [dbo].[PropertyNames]
ADD CONSTRAINT [PK_PropertyNames]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PropertyValues'
ALTER TABLE [dbo].[PropertyValues]
ADD CONSTRAINT [PK_PropertyValues]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'UserLibraries'
ALTER TABLE [dbo].[UserLibraries]
ADD CONSTRAINT [PK_UserLibraries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Users'
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [PK_Users]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Libraries_Id], [Licenses_Id] in table 'LicenseLibrary'
ALTER TABLE [dbo].[LicenseLibrary]
ADD CONSTRAINT [PK_LicenseLibrary]
    PRIMARY KEY NONCLUSTERED ([Libraries_Id], [Licenses_Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [Library_Id] in table 'Downloads'
ALTER TABLE [dbo].[Downloads]
ADD CONSTRAINT [FK_LibraryDownload]
    FOREIGN KEY ([Library_Id])
    REFERENCES [dbo].[Libraries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LibraryDownload'
CREATE INDEX [IX_FK_LibraryDownload]
ON [dbo].[Downloads]
    ([Library_Id]);
GO

-- Creating foreign key on [User_Id] in table 'Downloads'
ALTER TABLE [dbo].[Downloads]
ADD CONSTRAINT [FK_UserDownload]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserDownload'
CREATE INDEX [IX_FK_UserDownload]
ON [dbo].[Downloads]
    ([User_Id]);
GO

-- Creating foreign key on [PropertyName_Id] in table 'ExtendedProperties'
ALTER TABLE [dbo].[ExtendedProperties]
ADD CONSTRAINT [FK_ExtendedPropertyPropertyName]
    FOREIGN KEY ([PropertyName_Id])
    REFERENCES [dbo].[PropertyNames]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExtendedPropertyPropertyName'
CREATE INDEX [IX_FK_ExtendedPropertyPropertyName]
ON [dbo].[ExtendedProperties]
    ([PropertyName_Id]);
GO

-- Creating foreign key on [PropertyValue_Id] in table 'ExtendedProperties'
ALTER TABLE [dbo].[ExtendedProperties]
ADD CONSTRAINT [FK_ExtendedPropertyPropertyValue]
    FOREIGN KEY ([PropertyValue_Id])
    REFERENCES [dbo].[PropertyValues]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ExtendedPropertyPropertyValue'
CREATE INDEX [IX_FK_ExtendedPropertyPropertyValue]
ON [dbo].[ExtendedProperties]
    ([PropertyValue_Id]);
GO

-- Creating foreign key on [User_Id] in table 'ExtendedProperties'
ALTER TABLE [dbo].[ExtendedProperties]
ADD CONSTRAINT [FK_UserExtendedProperty]
    FOREIGN KEY ([User_Id])
    REFERENCES [dbo].[Users]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_UserExtendedProperty'
CREATE INDEX [IX_FK_UserExtendedProperty]
ON [dbo].[ExtendedProperties]
    ([User_Id]);
GO

-- Creating foreign key on [Owner_Id] in table 'Licenses'
ALTER TABLE [dbo].[Licenses]
ADD CONSTRAINT [FK_LicenseOwner]
    FOREIGN KEY ([Owner_Id])
    REFERENCES [dbo].[Owners]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LicenseOwner'
CREATE INDEX [IX_FK_LicenseOwner]
ON [dbo].[Licenses]
    ([Owner_Id]);
GO

-- Creating foreign key on [Libraries_Id] in table 'LicenseLibrary'
ALTER TABLE [dbo].[LicenseLibrary]
ADD CONSTRAINT [FK_LicenseLibrary_Libraries]
    FOREIGN KEY ([Libraries_Id])
    REFERENCES [dbo].[Libraries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Licenses_Id] in table 'LicenseLibrary'
ALTER TABLE [dbo].[LicenseLibrary]
ADD CONSTRAINT [FK_LicenseLibrary_Licenses]
    FOREIGN KEY ([Licenses_Id])
    REFERENCES [dbo].[Licenses]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_LicenseLibrary_Licenses'
CREATE INDEX [IX_FK_LicenseLibrary_Licenses]
ON [dbo].[LicenseLibrary]
    ([Licenses_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------