-- ========================================
-- CLEAN DATABASE SETUP SCRIPT
-- ========================================
-- This script will clean up and recreate the database with proper constraints

USE master
GO

-- Close any existing connections to the database
IF EXISTS(SELECT * FROM sys.databases WHERE name = 'KitchenEquipmentManagement')
BEGIN
    ALTER DATABASE [KitchenEquipmentManagement] SET SINGLE_USER WITH ROLLBACK IMMEDIATE
    DROP DATABASE [KitchenEquipmentManagement]
    PRINT 'Existing database dropped successfully!'
END
GO

-- Create fresh database
CREATE DATABASE [KitchenEquipmentManagement]
GO

USE [KitchenEquipmentManagement]
GO

PRINT 'Creating fresh database schema...'
GO

-- Create Users Table
CREATE TABLE [dbo].[Users] (
    [user_id] INT IDENTITY(1,1) PRIMARY KEY,
    [user_type] NVARCHAR(50) NOT NULL,
    [first_name] NVARCHAR(100) NOT NULL,
    [last_name] NVARCHAR(100) NOT NULL,
    [email_address] NVARCHAR(255) NOT NULL,
    [user_name] NVARCHAR(100) NOT NULL,
    [password] NVARCHAR(255) NOT NULL,
    [created_date] DATETIME2 DEFAULT GETDATE(),
    [modified_date] DATETIME2 DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT [UK_Users_Email] UNIQUE ([email_address]),
    CONSTRAINT [UK_Users_Username] UNIQUE ([user_name])
)
GO

-- Create Sites Table
CREATE TABLE [dbo].[Sites] (
    [site_id] INT IDENTITY(1,1) PRIMARY KEY,
    [user_id] INT NOT NULL,
    [description] NVARCHAR(500) NOT NULL,
    [active] BIT NOT NULL DEFAULT 1,
    [created_date] DATETIME2 DEFAULT GETDATE(),
    [modified_date] DATETIME2 DEFAULT GETDATE(),
    
    -- Foreign Key
    CONSTRAINT [FK_Sites_Users] FOREIGN KEY ([user_id]) 
        REFERENCES [dbo].[Users]([user_id]) ON DELETE CASCADE
)
GO

-- Create Equipment Table
CREATE TABLE [dbo].[Equipment] (
    [equipment_id] INT IDENTITY(1,1) PRIMARY KEY,
    [serial_number] NVARCHAR(100) NOT NULL,
    [description] NVARCHAR(500) NOT NULL,
    [condition] NVARCHAR(50) NOT NULL,
    [user_id] INT NOT NULL,
    [created_date] DATETIME2 DEFAULT GETDATE(),
    [modified_date] DATETIME2 DEFAULT GETDATE(),
    
    -- Constraints
    CONSTRAINT [UK_Equipment_Serial] UNIQUE ([serial_number]),
    CONSTRAINT [FK_Equipment_Users] FOREIGN KEY ([user_id]) 
        REFERENCES [dbo].[Users]([user_id]) ON DELETE CASCADE
)
GO

-- Create RegisteredEquipment Table (with NO ACTION to avoid cascade conflicts)
CREATE TABLE [dbo].[RegisteredEquipment] (
    [id] INT IDENTITY(1,1) PRIMARY KEY,
    [equipment_id] INT NOT NULL,
    [site_id] INT NOT NULL,
    [registered_date] DATETIME2 DEFAULT GETDATE(),
    
    -- Constraints - Using NO ACTION to prevent cascade path conflicts
    CONSTRAINT [UK_RegisteredEquipment_Equipment] UNIQUE ([equipment_id]),
    CONSTRAINT [UK_RegisteredEquipment_Composite] UNIQUE ([equipment_id], [site_id]),
    CONSTRAINT [FK_RegisteredEquipment_Equipment] FOREIGN KEY ([equipment_id]) 
        REFERENCES [dbo].[Equipment]([equipment_id]) ON DELETE NO ACTION ON UPDATE NO ACTION,
    CONSTRAINT [FK_RegisteredEquipment_Sites] FOREIGN KEY ([site_id]) 
        REFERENCES [dbo].[Sites]([site_id]) ON DELETE NO ACTION ON UPDATE NO ACTION
)
GO

PRINT 'Database schema created successfully!'
PRINT 'Starting data insertion...'
GO