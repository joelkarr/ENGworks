
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, and Azure
-- --------------------------------------------------
-- Date Created: 08/23/2012 22:13:01
-- Generated from EDMX file: C:\Dev\ENGworks\Model\CDWKS.Model.EF.Content\BXCContentEntityModel.edmx
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

IF OBJECT_ID(N'[dbo].[FK_AutodeskFileImage]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Images] DROP CONSTRAINT [FK_AutodeskFileImage];
GO
IF OBJECT_ID(N'[dbo].[FK_AutodeskFileTreeNode_AutodeskFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AutodeskFileTreeNodes] DROP CONSTRAINT [FK_AutodeskFileTreeNode_AutodeskFile];
GO
IF OBJECT_ID(N'[dbo].[FK_AutodeskFileTreeNode_TreeNode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AutodeskFileTreeNodes] DROP CONSTRAINT [FK_AutodeskFileTreeNode_TreeNode];
GO
IF OBJECT_ID(N'[dbo].[FK_AutodeskFileTreeNodeAutodeskFileIESFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AutodeskFileIESFiles] DROP CONSTRAINT [FK_AutodeskFileTreeNodeAutodeskFileIESFile];
GO
IF OBJECT_ID(N'[dbo].[FK_ContentLibraryTreeNode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TreeNodes] DROP CONSTRAINT [FK_ContentLibraryTreeNode];
GO
IF OBJECT_ID(N'[dbo].[FK_IESFileAutodeskFileIESFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AutodeskFileIESFiles] DROP CONSTRAINT [FK_IESFileAutodeskFileIESFile];
GO
IF OBJECT_ID(N'[dbo].[FK_ItemAutodeskFile]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Items] DROP CONSTRAINT [FK_ItemAutodeskFile];
GO
IF OBJECT_ID(N'[dbo].[FK_ItemParameterSearchNames]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameters] DROP CONSTRAINT [FK_ItemParameterSearchNames];
GO
IF OBJECT_ID(N'[dbo].[FK_ItemParameterSearchValues]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameters] DROP CONSTRAINT [FK_ItemParameterSearchValues];
GO
IF OBJECT_ID(N'[dbo].[FK_ParameterItem]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Parameters] DROP CONSTRAINT [FK_ParameterItem];
GO
IF OBJECT_ID(N'[dbo].[FK_TreeNodeTreeNode]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[TreeNodes] DROP CONSTRAINT [FK_TreeNodeTreeNode];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AutodeskFileIESFiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AutodeskFileIESFiles];
GO
IF OBJECT_ID(N'[dbo].[AutodeskFiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AutodeskFiles];
GO
IF OBJECT_ID(N'[dbo].[AutodeskFileTreeNodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AutodeskFileTreeNodes];
GO
IF OBJECT_ID(N'[dbo].[CDSLinks]', 'U') IS NOT NULL
    DROP TABLE [dbo].[CDSLinks];
GO
IF OBJECT_ID(N'[dbo].[ContentLibraries]', 'U') IS NOT NULL
    DROP TABLE [dbo].[ContentLibraries];
GO
IF OBJECT_ID(N'[dbo].[IESFiles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[IESFiles];
GO
IF OBJECT_ID(N'[dbo].[Images]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Images];
GO
IF OBJECT_ID(N'[dbo].[Items]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Items];
GO
IF OBJECT_ID(N'[dbo].[Parameters]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Parameters];
GO
IF OBJECT_ID(N'[dbo].[SearchNames]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SearchNames];
GO
IF OBJECT_ID(N'[dbo].[SearchValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[SearchValues];
GO
IF OBJECT_ID(N'[dbo].[TreeNodes]', 'U') IS NOT NULL
    DROP TABLE [dbo].[TreeNodes];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'AutodeskFileIESFiles'
CREATE TABLE [dbo].[AutodeskFileIESFiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AutodeskFileTreeNode_Id] int  NOT NULL,
    [IESFile_Id] int  NOT NULL
);
GO

-- Creating table 'AutodeskFiles'
CREATE TABLE [dbo].[AutodeskFiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [MC_OwnerId] nvarchar(max)  NOT NULL,
    [Version] int  NOT NULL,
    [TypeCatalogHeader] nvarchar(max)  NULL
);
GO

-- Creating table 'AutodeskFileTreeNodes'
CREATE TABLE [dbo].[AutodeskFileTreeNodes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [AutodeskFile_Id] int  NOT NULL,
    [TreeNodes_Id] int  NOT NULL
);
GO

-- Creating table 'CDSLinks'
CREATE TABLE [dbo].[CDSLinks] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Family] nvarchar(max)  NOT NULL,
    [ItemType] nvarchar(max)  NOT NULL,
    [CDSManufacturer] nvarchar(max)  NOT NULL,
    [CDSProdNum] nvarchar(max)  NOT NULL,
    [Url] nvarchar(max)  NULL
);
GO

-- Creating table 'ContentLibraries'
CREATE TABLE [dbo].[ContentLibraries] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'IESFiles'
CREATE TABLE [dbo].[IESFiles] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'Images'
CREATE TABLE [dbo].[Images] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Location] nvarchar(max)  NOT NULL,
    [FileType] nvarchar(max)  NOT NULL,
    [AutodeskFile_Id] int  NULL
);
GO

-- Creating table 'Items'
CREATE TABLE [dbo].[Items] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [AutodeskFile_Id] int  NOT NULL,
    [TypeCatalogEntry] nvarchar(max)  NULL
);
GO

-- Creating table 'Parameters'
CREATE TABLE [dbo].[Parameters] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [SearchValues_Id] int  NOT NULL,
    [SearchNames_Id] int  NOT NULL,
    [Featured] bit  NOT NULL,
    [Hidden] bit  NOT NULL,
    [Item_Id] int  NOT NULL
);
GO

-- Creating table 'SearchNames'
CREATE TABLE [dbo].[SearchNames] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'SearchValues'
CREATE TABLE [dbo].[SearchValues] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Value] nvarchar(max)  NOT NULL
);
GO

-- Creating table 'TreeNodes'
CREATE TABLE [dbo].[TreeNodes] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Parent_Id] int  NULL,
    [ContentLibrary_Id] int  NOT NULL
);
GO

-- Creating table 'RevitVersions'
CREATE TABLE [dbo].[RevitVersions] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Version] nvarchar(max)  NOT NULL,
    [AutodeskFile_Id] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'AutodeskFileIESFiles'
ALTER TABLE [dbo].[AutodeskFileIESFiles]
ADD CONSTRAINT [PK_AutodeskFileIESFiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AutodeskFiles'
ALTER TABLE [dbo].[AutodeskFiles]
ADD CONSTRAINT [PK_AutodeskFiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AutodeskFileTreeNodes'
ALTER TABLE [dbo].[AutodeskFileTreeNodes]
ADD CONSTRAINT [PK_AutodeskFileTreeNodes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'CDSLinks'
ALTER TABLE [dbo].[CDSLinks]
ADD CONSTRAINT [PK_CDSLinks]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'ContentLibraries'
ALTER TABLE [dbo].[ContentLibraries]
ADD CONSTRAINT [PK_ContentLibraries]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'IESFiles'
ALTER TABLE [dbo].[IESFiles]
ADD CONSTRAINT [PK_IESFiles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Images'
ALTER TABLE [dbo].[Images]
ADD CONSTRAINT [PK_Images]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Items'
ALTER TABLE [dbo].[Items]
ADD CONSTRAINT [PK_Items]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Parameters'
ALTER TABLE [dbo].[Parameters]
ADD CONSTRAINT [PK_Parameters]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SearchNames'
ALTER TABLE [dbo].[SearchNames]
ADD CONSTRAINT [PK_SearchNames]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'SearchValues'
ALTER TABLE [dbo].[SearchValues]
ADD CONSTRAINT [PK_SearchValues]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'TreeNodes'
ALTER TABLE [dbo].[TreeNodes]
ADD CONSTRAINT [PK_TreeNodes]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'RevitVersions'
ALTER TABLE [dbo].[RevitVersions]
ADD CONSTRAINT [PK_RevitVersions]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [AutodeskFileTreeNode_Id] in table 'AutodeskFileIESFiles'
ALTER TABLE [dbo].[AutodeskFileIESFiles]
ADD CONSTRAINT [FK_AutodeskFileTreeNodeAutodeskFileIESFile]
    FOREIGN KEY ([AutodeskFileTreeNode_Id])
    REFERENCES [dbo].[AutodeskFileTreeNodes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AutodeskFileTreeNodeAutodeskFileIESFile'
CREATE INDEX [IX_FK_AutodeskFileTreeNodeAutodeskFileIESFile]
ON [dbo].[AutodeskFileIESFiles]
    ([AutodeskFileTreeNode_Id]);
GO

-- Creating foreign key on [IESFile_Id] in table 'AutodeskFileIESFiles'
ALTER TABLE [dbo].[AutodeskFileIESFiles]
ADD CONSTRAINT [FK_IESFileAutodeskFileIESFile]
    FOREIGN KEY ([IESFile_Id])
    REFERENCES [dbo].[IESFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_IESFileAutodeskFileIESFile'
CREATE INDEX [IX_FK_IESFileAutodeskFileIESFile]
ON [dbo].[AutodeskFileIESFiles]
    ([IESFile_Id]);
GO

-- Creating foreign key on [AutodeskFile_Id] in table 'Images'
ALTER TABLE [dbo].[Images]
ADD CONSTRAINT [FK_AutodeskFileImage]
    FOREIGN KEY ([AutodeskFile_Id])
    REFERENCES [dbo].[AutodeskFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AutodeskFileImage'
CREATE INDEX [IX_FK_AutodeskFileImage]
ON [dbo].[Images]
    ([AutodeskFile_Id]);
GO

-- Creating foreign key on [AutodeskFile_Id] in table 'AutodeskFileTreeNodes'
ALTER TABLE [dbo].[AutodeskFileTreeNodes]
ADD CONSTRAINT [FK_AutodeskFileTreeNode_AutodeskFile]
    FOREIGN KEY ([AutodeskFile_Id])
    REFERENCES [dbo].[AutodeskFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AutodeskFileTreeNode_AutodeskFile'
CREATE INDEX [IX_FK_AutodeskFileTreeNode_AutodeskFile]
ON [dbo].[AutodeskFileTreeNodes]
    ([AutodeskFile_Id]);
GO

-- Creating foreign key on [AutodeskFile_Id] in table 'Items'
ALTER TABLE [dbo].[Items]
ADD CONSTRAINT [FK_ItemAutodeskFile]
    FOREIGN KEY ([AutodeskFile_Id])
    REFERENCES [dbo].[AutodeskFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ItemAutodeskFile'
CREATE INDEX [IX_FK_ItemAutodeskFile]
ON [dbo].[Items]
    ([AutodeskFile_Id]);
GO

-- Creating foreign key on [TreeNodes_Id] in table 'AutodeskFileTreeNodes'
ALTER TABLE [dbo].[AutodeskFileTreeNodes]
ADD CONSTRAINT [FK_AutodeskFileTreeNode_TreeNode]
    FOREIGN KEY ([TreeNodes_Id])
    REFERENCES [dbo].[TreeNodes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AutodeskFileTreeNode_TreeNode'
CREATE INDEX [IX_FK_AutodeskFileTreeNode_TreeNode]
ON [dbo].[AutodeskFileTreeNodes]
    ([TreeNodes_Id]);
GO

-- Creating foreign key on [ContentLibrary_Id] in table 'TreeNodes'
ALTER TABLE [dbo].[TreeNodes]
ADD CONSTRAINT [FK_ContentLibraryTreeNode]
    FOREIGN KEY ([ContentLibrary_Id])
    REFERENCES [dbo].[ContentLibraries]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ContentLibraryTreeNode'
CREATE INDEX [IX_FK_ContentLibraryTreeNode]
ON [dbo].[TreeNodes]
    ([ContentLibrary_Id]);
GO

-- Creating foreign key on [Item_Id] in table 'Parameters'
ALTER TABLE [dbo].[Parameters]
ADD CONSTRAINT [FK_ParameterItem]
    FOREIGN KEY ([Item_Id])
    REFERENCES [dbo].[Items]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ParameterItem'
CREATE INDEX [IX_FK_ParameterItem]
ON [dbo].[Parameters]
    ([Item_Id]);
GO

-- Creating foreign key on [SearchNames_Id] in table 'Parameters'
ALTER TABLE [dbo].[Parameters]
ADD CONSTRAINT [FK_ItemParameterSearchNames]
    FOREIGN KEY ([SearchNames_Id])
    REFERENCES [dbo].[SearchNames]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ItemParameterSearchNames'
CREATE INDEX [IX_FK_ItemParameterSearchNames]
ON [dbo].[Parameters]
    ([SearchNames_Id]);
GO

-- Creating foreign key on [SearchValues_Id] in table 'Parameters'
ALTER TABLE [dbo].[Parameters]
ADD CONSTRAINT [FK_ItemParameterSearchValues]
    FOREIGN KEY ([SearchValues_Id])
    REFERENCES [dbo].[SearchValues]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_ItemParameterSearchValues'
CREATE INDEX [IX_FK_ItemParameterSearchValues]
ON [dbo].[Parameters]
    ([SearchValues_Id]);
GO

-- Creating foreign key on [Parent_Id] in table 'TreeNodes'
ALTER TABLE [dbo].[TreeNodes]
ADD CONSTRAINT [FK_TreeNodeTreeNode]
    FOREIGN KEY ([Parent_Id])
    REFERENCES [dbo].[TreeNodes]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_TreeNodeTreeNode'
CREATE INDEX [IX_FK_TreeNodeTreeNode]
ON [dbo].[TreeNodes]
    ([Parent_Id]);
GO

-- Creating foreign key on [AutodeskFile_Id] in table 'RevitVersions'
ALTER TABLE [dbo].[RevitVersions]
ADD CONSTRAINT [FK_AutodeskFileRevitVersion]
    FOREIGN KEY ([AutodeskFile_Id])
    REFERENCES [dbo].[AutodeskFiles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;

-- Creating non-clustered index for FOREIGN KEY 'FK_AutodeskFileRevitVersion'
CREATE INDEX [IX_FK_AutodeskFileRevitVersion]
ON [dbo].[RevitVersions]
    ([AutodeskFile_Id]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------