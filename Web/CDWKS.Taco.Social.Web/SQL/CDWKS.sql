USE [master]
GO
/****** Object:  Database [CDWKS_SocialFeedback]    Script Date: 01/15/2012 18:40:01 ******/
CREATE DATABASE [CDWKS_SocialFeedback] ON  PRIMARY 
( NAME = N'CDWKS_SocialFeedback', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\CDWKS_SocialFeedback.mdf' , SIZE = 2048KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'CDWKS_SocialFeedback_log', FILENAME = N'c:\Program Files\Microsoft SQL Server\MSSQL10_50.MSSQLSERVER\MSSQL\DATA\CDWKS_SocialFeedback_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET COMPATIBILITY_LEVEL = 100
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [CDWKS_SocialFeedback].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET ANSI_NULL_DEFAULT OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET ANSI_NULLS OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET ANSI_PADDING OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET ARITHABORT OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET AUTO_CLOSE OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET AUTO_CREATE_STATISTICS ON
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET CURSOR_CLOSE_ON_COMMIT OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET CURSOR_DEFAULT  GLOBAL
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET CONCAT_NULL_YIELDS_NULL OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET NUMERIC_ROUNDABORT OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET QUOTED_IDENTIFIER OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET RECURSIVE_TRIGGERS OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET  DISABLE_BROKER
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET AUTO_UPDATE_STATISTICS_ASYNC OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET DATE_CORRELATION_OPTIMIZATION OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET TRUSTWORTHY OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET ALLOW_SNAPSHOT_ISOLATION OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET PARAMETERIZATION SIMPLE
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET READ_COMMITTED_SNAPSHOT OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET HONOR_BROKER_PRIORITY OFF
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET  READ_WRITE
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET RECOVERY SIMPLE
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET  MULTI_USER
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET PAGE_VERIFY CHECKSUM
GO
ALTER DATABASE [CDWKS_SocialFeedback] SET DB_CHAINING OFF
GO
USE [CDWKS_SocialFeedback]
GO
/****** Object:  Table [dbo].[SocialFeedbackForm]    Script Date: 01/15/2012 18:40:02 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SocialFeedbackForm](
	[SocialFeebackFormId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[Email] [nvarchar](150) NOT NULL,
	[FirstName] [nvarchar](50) NULL,
	[LastName] [nvarchar](100) NULL,
	[Company] [nvarchar](100) NULL,
	[Product] [nvarchar](100) NOT NULL,
	[Family] [nvarchar](100) NOT NULL,
	[Like] [bit] NOT NULL,
	[Comments] [nvarchar](120) NOT NULL,
	[Timestamp] [datetime] NOT NULL,
 CONSTRAINT [PK_SocialFeedbackForm] PRIMARY KEY CLUSTERED 
(
	[SocialFeebackFormId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Default [DF_SocialFeedbackForm_Timestamp]    Script Date: 01/15/2012 18:40:02 ******/
ALTER TABLE [dbo].[SocialFeedbackForm] ADD  CONSTRAINT [DF_SocialFeedbackForm_Timestamp]  DEFAULT (getdate()) FOR [Timestamp]
GO
