USE [master]
GO

/****** Object:  Database [TTM]    Script Date: 11/13/2017 11:39:16 AM ******/
CREATE DATABASE [TTM]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'TTM', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\TTM.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'TTM_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL13.MSSQLSERVER\MSSQL\DATA\TTM_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO

ALTER DATABASE [TTM] SET COMPATIBILITY_LEVEL = 130
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [TTM].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [TTM] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [TTM] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [TTM] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [TTM] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [TTM] SET ARITHABORT OFF 
GO

ALTER DATABASE [TTM] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [TTM] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [TTM] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [TTM] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [TTM] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [TTM] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [TTM] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [TTM] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [TTM] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [TTM] SET  DISABLE_BROKER 
GO

ALTER DATABASE [TTM] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [TTM] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [TTM] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [TTM] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [TTM] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [TTM] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [TTM] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [TTM] SET RECOVERY SIMPLE 
GO

ALTER DATABASE [TTM] SET  MULTI_USER 
GO

ALTER DATABASE [TTM] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [TTM] SET DB_CHAINING OFF 
GO

ALTER DATABASE [TTM] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [TTM] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [TTM] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [TTM] SET QUERY_STORE = OFF
GO

USE [TTM]
GO

ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 0;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET MAXDOP = PRIMARY;
GO

ALTER DATABASE SCOPED CONFIGURATION SET LEGACY_CARDINALITY_ESTIMATION = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET LEGACY_CARDINALITY_ESTIMATION = PRIMARY;
GO

ALTER DATABASE SCOPED CONFIGURATION SET PARAMETER_SNIFFING = ON;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET PARAMETER_SNIFFING = PRIMARY;
GO

ALTER DATABASE SCOPED CONFIGURATION SET QUERY_OPTIMIZER_HOTFIXES = OFF;
GO

ALTER DATABASE SCOPED CONFIGURATION FOR SECONDARY SET QUERY_OPTIMIZER_HOTFIXES = PRIMARY;
GO

ALTER DATABASE [TTM] SET  READ_WRITE 
GO

USE [TTM]
GO
/****** Object:  Table [dbo].[ClientRegion]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ClientRegion](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ClientRegion] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CoreService]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CoreService](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_CoreService] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MarketOffering]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MarketOffering](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_MarketOffering] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OperationalRisk]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OperationalRisk](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[RiskNo] [int] NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_OperationalRisk] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Practice]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Practice](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Practice] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[QGPassed]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[QGPassed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_QGPassed] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RelevantRepository]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RelevantRepository](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_RelevantRepository] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ServiceDeliveryChain]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ServiceDeliveryChain](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[DisplayOrder] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_ServiceDeliveryChain] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_ServiceDeliveryChain] UNIQUE NONCLUSTERED 
(
	[DisplayOrder] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SolutionCentre]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SolutionCentre](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_SolutionCentre] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSO]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSO](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TSRId] [int] NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[TeamLeadId] [int] NOT NULL,
	[CoreServiceId] [int] NOT NULL,
	[RelevantRepositoryId] [int] NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[TargetCompletionDate] [datetime] NOT NULL,
	[EstimatedEffort] [float] NOT NULL,
	[PlannedEffort] [float] NOT NULL,
	[OperationalRiskId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSO] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSOServiceDeliveryChain]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSOServiceDeliveryChain](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TSOId] [int] NOT NULL,
	[ServiceDeliveryChainId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSOServiceDeliveryChain] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSOServiceDeliveryChainTask]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSOServiceDeliveryChainTask](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[TSOServiceDeliveryChainId] [int] NOT NULL,
	[PercentageComplete] [int] NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[StartDate] [datetime] NOT NULL,
	[TargetCompletionDate] [datetime] NOT NULL,
	[PlannedEffort] [float] NOT NULL,
	[ActualStartDate] [datetime] NOT NULL,
	[ActualCompletionDate] [datetime] NULL,
	[ActualEffort] [float] NOT NULL,
	[EffortsEnteredUntil] [datetime] NOT NULL,
	[PlannedProductivity] [float] NOT NULL,
	[ActualProductivity] [float] NOT NULL,
	[PlannedOutcome] [int] NULL,
	[ActualOutcome] [int] NULL,
	[PlannedServiceQuality] [int] NULL,
	[ActualServiceQuality] [int] NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSOServiceDeliveryChainTask] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSOStatus]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSOStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSOStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSR]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSR](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](150) NOT NULL,
	[DeliveryManagerId] [int] NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[TestManagerId] [int] NOT NULL,
	[VerticalId] [int] NOT NULL,
	[PracticeId] [int] NOT NULL,
	[SolutionCentreId] [int] NOT NULL,
	[ClientRegionId] [int] NOT NULL,
	[Client] [nvarchar](200) NOT NULL,
	[Account] [nvarchar](100) NOT NULL,
	[Engagement] [nvarchar](100) NOT NULL,
	[AccountManagerId] [int] NOT NULL,
	[ERPordernumber] [nvarchar](50) NOT NULL,
	[MarketOfferingId] [int] NULL,
	[StartDate] [datetime] NOT NULL,
	[TargetCompletionDate] [datetime] NOT NULL,
	[Estimatedeffort] [float] NOT NULL,
	[Plannedeffort] [float] NOT NULL,
	[OperationalRiskId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSR] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSRCoreServices]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSRCoreServices](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TSRId] [int] NOT NULL,
	[CoreServiceId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSRCoreServices] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSRRelevantRepositories]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSRRelevantRepositories](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[TSRId] [int] NOT NULL,
	[RelevantRepositoryId] [int] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSTReleventRepositories] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TSRStatus]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TSRStatus](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_TSRStatus] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[User]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[UserTypeID] [int] NULL,
	[EmailID] [nvarchar](200) NOT NULL,
	[UserId] [nvarchar](50) NOT NULL,
	[Password] [nvarchar](500) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[ActivationKey] [nvarchar](150) NOT NULL,
	[ProfilePicLocation] [nvarchar](255) NULL,
	[Activated] [bit] NOT NULL,
	[Locked] [bit] NOT NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserType]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserType](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](2000) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_UserType] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Vertical]    Script Date: 11/13/2017 11:35:00 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Vertical](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[Description] [nvarchar](500) NULL,
	[CreatedBy] [nvarchar](50) NOT NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedBy] [nvarchar](50) NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[Version] [int] NOT NULL,
 CONSTRAINT [PK_Vertical] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[TSO]  WITH CHECK ADD  CONSTRAINT [FK_TSO_CoreService] FOREIGN KEY([CoreServiceId])
REFERENCES [dbo].[CoreService] ([Id])
GO
ALTER TABLE [dbo].[TSO] CHECK CONSTRAINT [FK_TSO_CoreService]
GO
ALTER TABLE [dbo].[TSO]  WITH CHECK ADD  CONSTRAINT [FK_TSO_OperationalRisk] FOREIGN KEY([OperationalRiskId])
REFERENCES [dbo].[OperationalRisk] ([Id])
GO
ALTER TABLE [dbo].[TSO] CHECK CONSTRAINT [FK_TSO_OperationalRisk]
GO
ALTER TABLE [dbo].[TSO]  WITH CHECK ADD  CONSTRAINT [FK_TSO_RelevantRepository] FOREIGN KEY([RelevantRepositoryId])
REFERENCES [dbo].[RelevantRepository] ([Id])
GO
ALTER TABLE [dbo].[TSO] CHECK CONSTRAINT [FK_TSO_RelevantRepository]
GO
ALTER TABLE [dbo].[TSO]  WITH CHECK ADD  CONSTRAINT [FK_TSO_TSR] FOREIGN KEY([TSRId])
REFERENCES [dbo].[TSR] ([ID])
GO
ALTER TABLE [dbo].[TSO] CHECK CONSTRAINT [FK_TSO_TSR]
GO
ALTER TABLE [dbo].[TSO]  WITH CHECK ADD  CONSTRAINT [FK_TSO_User] FOREIGN KEY([TeamLeadId])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[TSO] CHECK CONSTRAINT [FK_TSO_User]
GO
ALTER TABLE [dbo].[TSOServiceDeliveryChain]  WITH CHECK ADD  CONSTRAINT [FK_TSOServiceDeliveryChain_ServiceDeliveryChain] FOREIGN KEY([ServiceDeliveryChainId])
REFERENCES [dbo].[ServiceDeliveryChain] ([Id])
GO
ALTER TABLE [dbo].[TSOServiceDeliveryChain] CHECK CONSTRAINT [FK_TSOServiceDeliveryChain_ServiceDeliveryChain]
GO
ALTER TABLE [dbo].[TSOServiceDeliveryChain]  WITH CHECK ADD  CONSTRAINT [FK_TSOServiceDeliveryChain_TSO] FOREIGN KEY([TSOId])
REFERENCES [dbo].[TSO] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[TSOServiceDeliveryChain] CHECK CONSTRAINT [FK_TSOServiceDeliveryChain_TSO]
GO
ALTER TABLE [dbo].[TSOServiceDeliveryChainTask]  WITH CHECK ADD  CONSTRAINT [FK_TSOServiceDeliveryChainTask_TSOServiceDeliveryChain] FOREIGN KEY([TSOServiceDeliveryChainId])
REFERENCES [dbo].[TSOServiceDeliveryChain] ([ID])
GO
ALTER TABLE [dbo].[TSOServiceDeliveryChainTask] CHECK CONSTRAINT [FK_TSOServiceDeliveryChainTask_TSOServiceDeliveryChain]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_ClientRegion] FOREIGN KEY([ClientRegionId])
REFERENCES [dbo].[ClientRegion] ([Id])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_ClientRegion]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_MarketOffering] FOREIGN KEY([MarketOfferingId])
REFERENCES [dbo].[MarketOffering] ([Id])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_MarketOffering]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_OperationalRisk] FOREIGN KEY([OperationalRiskId])
REFERENCES [dbo].[OperationalRisk] ([Id])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_OperationalRisk]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_Practice] FOREIGN KEY([PracticeId])
REFERENCES [dbo].[Practice] ([Id])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_Practice]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_SolutionCentre] FOREIGN KEY([SolutionCentreId])
REFERENCES [dbo].[SolutionCentre] ([Id])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_SolutionCentre]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_User_AccountManager] FOREIGN KEY([AccountManagerId])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_User_AccountManager]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_User_DeliveryManager] FOREIGN KEY([DeliveryManagerId])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_User_DeliveryManager]
GO
ALTER TABLE [dbo].[TSR]  WITH CHECK ADD  CONSTRAINT [FK_TSR_User_TestManager] FOREIGN KEY([TestManagerId])
REFERENCES [dbo].[User] ([ID])
GO
ALTER TABLE [dbo].[TSR] CHECK CONSTRAINT [FK_TSR_User_TestManager]
GO
ALTER TABLE [dbo].[TSRCoreServices]  WITH CHECK ADD  CONSTRAINT [FK_TSRCoreServices_CoreService] FOREIGN KEY([CoreServiceId])
REFERENCES [dbo].[CoreService] ([Id])
GO
ALTER TABLE [dbo].[TSRCoreServices] CHECK CONSTRAINT [FK_TSRCoreServices_CoreService]
GO
ALTER TABLE [dbo].[TSRCoreServices]  WITH CHECK ADD  CONSTRAINT [FK_TSRCoreServices_TSR] FOREIGN KEY([TSRId])
REFERENCES [dbo].[TSR] ([ID])
GO
ALTER TABLE [dbo].[TSRCoreServices] CHECK CONSTRAINT [FK_TSRCoreServices_TSR]
GO
ALTER TABLE [dbo].[TSRRelevantRepositories]  WITH CHECK ADD  CONSTRAINT [FK_TSRRelevantRepositories_RelevantRepository] FOREIGN KEY([RelevantRepositoryId])
REFERENCES [dbo].[RelevantRepository] ([Id])
GO
ALTER TABLE [dbo].[TSRRelevantRepositories] CHECK CONSTRAINT [FK_TSRRelevantRepositories_RelevantRepository]
GO
ALTER TABLE [dbo].[TSRRelevantRepositories]  WITH CHECK ADD  CONSTRAINT [FK_TSRRelevantRepositories_TSR] FOREIGN KEY([TSRId])
REFERENCES [dbo].[TSR] ([ID])
GO
ALTER TABLE [dbo].[TSRRelevantRepositories] CHECK CONSTRAINT [FK_TSRRelevantRepositories_TSR]
GO
ALTER TABLE [dbo].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_UserType] FOREIGN KEY([UserTypeID])
REFERENCES [dbo].[UserType] ([ID])
GO
ALTER TABLE [dbo].[User] CHECK CONSTRAINT [FK_User_UserType]
GO
