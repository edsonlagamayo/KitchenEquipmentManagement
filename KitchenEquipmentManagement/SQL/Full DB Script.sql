USE [KitchenEquipmentManagement]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sites]') AND type in (N'U'))
ALTER TABLE [dbo].[Sites] DROP CONSTRAINT IF EXISTS [FK_Sites_Users]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisteredEquipment]') AND type in (N'U'))
ALTER TABLE [dbo].[RegisteredEquipment] DROP CONSTRAINT IF EXISTS [FK_RegisteredEquipment_Sites]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisteredEquipment]') AND type in (N'U'))
ALTER TABLE [dbo].[RegisteredEquipment] DROP CONSTRAINT IF EXISTS [FK_RegisteredEquipment_Equipment]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Equipment]') AND type in (N'U'))
ALTER TABLE [dbo].[Equipment] DROP CONSTRAINT IF EXISTS [FK_Equipment_Users]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [DF__Users__modified___3A81B327]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [DF__Users__created_d__398D8EEE]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sites]') AND type in (N'U'))
ALTER TABLE [dbo].[Sites] DROP CONSTRAINT IF EXISTS [DF__Sites__modified___3F466844]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sites]') AND type in (N'U'))
ALTER TABLE [dbo].[Sites] DROP CONSTRAINT IF EXISTS [DF__Sites__created_d__3E52440B]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sites]') AND type in (N'U'))
ALTER TABLE [dbo].[Sites] DROP CONSTRAINT IF EXISTS [DF__Sites__active__3D5E1FD2]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisteredEquipment]') AND type in (N'U'))
ALTER TABLE [dbo].[RegisteredEquipment] DROP CONSTRAINT IF EXISTS [DF__Registere__regis__4AB81AF0]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Equipment]') AND type in (N'U'))
ALTER TABLE [dbo].[Equipment] DROP CONSTRAINT IF EXISTS [DF__Equipment__modif__44FF419A]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Equipment]') AND type in (N'U'))
ALTER TABLE [dbo].[Equipment] DROP CONSTRAINT IF EXISTS [DF__Equipment__creat__440B1D61]
GO
/****** Object:  Index [UK_Users_Username]    Script Date: 25/08/2025 2:00:06 pm ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [UK_Users_Username]
GO
/****** Object:  Index [UK_Users_Email]    Script Date: 25/08/2025 2:00:06 pm ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [UK_Users_Email]
GO
/****** Object:  Index [UK_RegisteredEquipment_Equipment]    Script Date: 25/08/2025 2:00:06 pm ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisteredEquipment]') AND type in (N'U'))
ALTER TABLE [dbo].[RegisteredEquipment] DROP CONSTRAINT IF EXISTS [UK_RegisteredEquipment_Equipment]
GO
/****** Object:  Index [UK_RegisteredEquipment_Composite]    Script Date: 25/08/2025 2:00:06 pm ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[RegisteredEquipment]') AND type in (N'U'))
ALTER TABLE [dbo].[RegisteredEquipment] DROP CONSTRAINT IF EXISTS [UK_RegisteredEquipment_Composite]
GO
/****** Object:  Index [UK_Equipment_Serial]    Script Date: 25/08/2025 2:00:06 pm ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Equipment]') AND type in (N'U'))
ALTER TABLE [dbo].[Equipment] DROP CONSTRAINT IF EXISTS [UK_Equipment_Serial]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 25/08/2025 2:00:06 pm ******/
DROP TABLE IF EXISTS [dbo].[Users]
GO
/****** Object:  Table [dbo].[Sites]    Script Date: 25/08/2025 2:00:06 pm ******/
DROP TABLE IF EXISTS [dbo].[Sites]
GO
/****** Object:  Table [dbo].[RegisteredEquipment]    Script Date: 25/08/2025 2:00:06 pm ******/
DROP TABLE IF EXISTS [dbo].[RegisteredEquipment]
GO
/****** Object:  Table [dbo].[Equipment]    Script Date: 25/08/2025 2:00:06 pm ******/
DROP TABLE IF EXISTS [dbo].[Equipment]
GO
USE [master]
GO
/****** Object:  Database [KitchenEquipmentManagement]    Script Date: 25/08/2025 2:00:06 pm ******/
DROP DATABASE IF EXISTS [KitchenEquipmentManagement]
GO
/****** Object:  Database [KitchenEquipmentManagement]    Script Date: 25/08/2025 2:00:06 pm ******/
CREATE DATABASE [KitchenEquipmentManagement]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'KitchenEquipmentManagement', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLSERVER2022\MSSQL\DATA\KitchenEquipmentManagement.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'KitchenEquipmentManagement_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.SQLSERVER2022\MSSQL\DATA\KitchenEquipmentManagement_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [KitchenEquipmentManagement] SET COMPATIBILITY_LEVEL = 160
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [KitchenEquipmentManagement].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ARITHABORT OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET  ENABLE_BROKER 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET RECOVERY FULL 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET  MULTI_USER 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [KitchenEquipmentManagement] SET DB_CHAINING OFF 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [KitchenEquipmentManagement] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
EXEC sys.sp_db_vardecimal_storage_format N'KitchenEquipmentManagement', N'ON'
GO
ALTER DATABASE [KitchenEquipmentManagement] SET QUERY_STORE = ON
GO
ALTER DATABASE [KitchenEquipmentManagement] SET QUERY_STORE (OPERATION_MODE = READ_WRITE, CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 30), DATA_FLUSH_INTERVAL_SECONDS = 900, INTERVAL_LENGTH_MINUTES = 60, MAX_STORAGE_SIZE_MB = 1000, QUERY_CAPTURE_MODE = AUTO, SIZE_BASED_CLEANUP_MODE = AUTO, MAX_PLANS_PER_QUERY = 200, WAIT_STATS_CAPTURE_MODE = ON)
GO
USE [KitchenEquipmentManagement]
GO
/****** Object:  Table [dbo].[Equipment]    Script Date: 25/08/2025 2:00:07 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Equipment](
	[equipment_id] [int] IDENTITY(1,1) NOT NULL,
	[serial_number] [nvarchar](100) NOT NULL,
	[description] [nvarchar](500) NOT NULL,
	[condition] [nvarchar](50) NOT NULL,
	[user_id] [int] NOT NULL,
	[created_date] [datetime2](7) NULL,
	[modified_date] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[equipment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[RegisteredEquipment]    Script Date: 25/08/2025 2:00:07 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RegisteredEquipment](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[equipment_id] [int] NOT NULL,
	[site_id] [int] NOT NULL,
	[registered_date] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sites]    Script Date: 25/08/2025 2:00:07 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sites](
	[site_id] [int] IDENTITY(1,1) NOT NULL,
	[user_id] [int] NOT NULL,
	[description] [nvarchar](500) NOT NULL,
	[active] [bit] NOT NULL,
	[created_date] [datetime2](7) NULL,
	[modified_date] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[site_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 25/08/2025 2:00:07 pm ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[user_id] [int] IDENTITY(1,1) NOT NULL,
	[user_type] [nvarchar](50) NOT NULL,
	[first_name] [nvarchar](100) NOT NULL,
	[last_name] [nvarchar](100) NOT NULL,
	[email_address] [nvarchar](255) NOT NULL,
	[user_name] [nvarchar](100) NOT NULL,
	[password] [nvarchar](255) NOT NULL,
	[created_date] [datetime2](7) NULL,
	[modified_date] [datetime2](7) NULL,
PRIMARY KEY CLUSTERED 
(
	[user_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Equipment] ON 
GO
INSERT [dbo].[Equipment] ([equipment_id], [serial_number], [description], [condition], [user_id], [created_date], [modified_date]) VALUES (21, N'1231-1231', N'Bread Maker', N'Working', 7, CAST(N'2025-08-24T23:49:08.5763326' AS DateTime2), CAST(N'2025-08-24T23:49:08.5764120' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[Equipment] OFF
GO
SET IDENTITY_INSERT [dbo].[RegisteredEquipment] ON 
GO
INSERT [dbo].[RegisteredEquipment] ([id], [equipment_id], [site_id], [registered_date]) VALUES (18, 21, 10, CAST(N'2025-08-25T13:29:30.6099454' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[RegisteredEquipment] OFF
GO
SET IDENTITY_INSERT [dbo].[Sites] ON 
GO
INSERT [dbo].[Sites] ([site_id], [user_id], [description], [active], [created_date], [modified_date]) VALUES (10, 7, N'Jollibee', 1, CAST(N'2025-08-24T23:48:42.7785895' AS DateTime2), CAST(N'2025-08-24T23:48:42.7786602' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[Sites] OFF
GO
SET IDENTITY_INSERT [dbo].[Users] ON 
GO
INSERT [dbo].[Users] ([user_id], [user_type], [first_name], [last_name], [email_address], [user_name], [password], [created_date], [modified_date]) VALUES (1, N'SuperAdmin', N'Super', N'Admin', N'admin@kitchenequipment.com', N'admin', N'$2a$12$Y6pnPwDviFEZQB33AAnhc.EtMLN4qSj6kS5XsNw65PQbyJjw/kFwW', CAST(N'2025-08-24T21:19:03.6500000' AS DateTime2), CAST(N'2025-08-25T13:55:19.7168752' AS DateTime2))
GO
INSERT [dbo].[Users] ([user_id], [user_type], [first_name], [last_name], [email_address], [user_name], [password], [created_date], [modified_date]) VALUES (6, N'Admin', N'Edson', N'Lagamayo', N'edson@kitchenequipment.com', N'edson', N'$2a$12$oJ8p2HCO.zGF5wuHNdItsO7Wq8.uCo6GfePkNHknMOhKh8YUtbhOm', CAST(N'2025-08-24T23:47:13.0500000' AS DateTime2), CAST(N'2025-08-25T13:55:36.4869511' AS DateTime2))
GO
  
INSERT [dbo].[Users] ([user_id], [user_type], [first_name], [last_name], [email_address], [user_name], [password], [created_date], [modified_date]) VALUES (11, N'Admin', N'John', N'Doe', N'johndoe@yahoo.com', N'john', N'$2a$12$gZM4J8qAIZpI4ZDNV7OBW.5gdLlb7EzBbfOlRPhxyC3WqY2pu3lsy', CAST(N'2025-08-25T13:56:37.0105608' AS DateTime2), CAST(N'2025-08-25T13:56:37.0106412' AS DateTime2))
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_Equipment_Serial]    Script Date: 25/08/2025 2:00:07 pm ******/
ALTER TABLE [dbo].[Equipment] ADD  CONSTRAINT [UK_Equipment_Serial] UNIQUE NONCLUSTERED 
(
	[serial_number] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UK_RegisteredEquipment_Composite]    Script Date: 25/08/2025 2:00:07 pm ******/
ALTER TABLE [dbo].[RegisteredEquipment] ADD  CONSTRAINT [UK_RegisteredEquipment_Composite] UNIQUE NONCLUSTERED 
(
	[equipment_id] ASC,
	[site_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [UK_RegisteredEquipment_Equipment]    Script Date: 25/08/2025 2:00:07 pm ******/
ALTER TABLE [dbo].[RegisteredEquipment] ADD  CONSTRAINT [UK_RegisteredEquipment_Equipment] UNIQUE NONCLUSTERED 
(
	[equipment_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_Users_Email]    Script Date: 25/08/2025 2:00:07 pm ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [UK_Users_Email] UNIQUE NONCLUSTERED 
(
	[email_address] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [UK_Users_Username]    Script Date: 25/08/2025 2:00:07 pm ******/
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [UK_Users_Username] UNIQUE NONCLUSTERED 
(
	[user_name] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Equipment] ADD  DEFAULT (getdate()) FOR [created_date]
GO
ALTER TABLE [dbo].[Equipment] ADD  DEFAULT (getdate()) FOR [modified_date]
GO
ALTER TABLE [dbo].[RegisteredEquipment] ADD  DEFAULT (getdate()) FOR [registered_date]
GO
ALTER TABLE [dbo].[Sites] ADD  DEFAULT ((1)) FOR [active]
GO
ALTER TABLE [dbo].[Sites] ADD  DEFAULT (getdate()) FOR [created_date]
GO
ALTER TABLE [dbo].[Sites] ADD  DEFAULT (getdate()) FOR [modified_date]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [created_date]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [modified_date]
GO
ALTER TABLE [dbo].[Equipment]  WITH CHECK ADD  CONSTRAINT [FK_Equipment_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([user_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Equipment] CHECK CONSTRAINT [FK_Equipment_Users]
GO
ALTER TABLE [dbo].[RegisteredEquipment]  WITH CHECK ADD  CONSTRAINT [FK_RegisteredEquipment_Equipment] FOREIGN KEY([equipment_id])
REFERENCES [dbo].[Equipment] ([equipment_id])
GO
ALTER TABLE [dbo].[RegisteredEquipment] CHECK CONSTRAINT [FK_RegisteredEquipment_Equipment]
GO
ALTER TABLE [dbo].[RegisteredEquipment]  WITH CHECK ADD  CONSTRAINT [FK_RegisteredEquipment_Sites] FOREIGN KEY([site_id])
REFERENCES [dbo].[Sites] ([site_id])
GO
ALTER TABLE [dbo].[RegisteredEquipment] CHECK CONSTRAINT [FK_RegisteredEquipment_Sites]
GO
ALTER TABLE [dbo].[Sites]  WITH CHECK ADD  CONSTRAINT [FK_Sites_Users] FOREIGN KEY([user_id])
REFERENCES [dbo].[Users] ([user_id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Sites] CHECK CONSTRAINT [FK_Sites_Users]
GO
USE [master]
GO
ALTER DATABASE [KitchenEquipmentManagement] SET  READ_WRITE 
GO
