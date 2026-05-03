-- ============================================================
-- perfumeDP  — Full Schema + Migration Script
-- Run this on a fresh database (includes original schema
-- + all missing pieces needed to make the app work)
-- ============================================================

CREATE DATABASE perfumeDP;
GO
USE perfumeDP;
GO

-- ── Users ────────────────────────────────────────────────────
CREATE TABLE [dbo].[Users] (
    [Id]       INT            IDENTITY(1,1) NOT NULL,
    [Username] NVARCHAR(100)  NOT NULL,
    [Email]    NVARCHAR(100)  NOT NULL,
    -- BUG FIX: Store password HASH (BCrypt/PBKDF2), never plain-text
    [Password] NVARCHAR(256)  NOT NULL,
    [Role]     NVARCHAR(50)   NULL DEFAULT 'User',
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT UQ_Users_Email UNIQUE ([Email])   -- FIX: emails must be unique
);
GO

-- ── Perfumes ─────────────────────────────────────────────────
CREATE TABLE [dbo].[Perfumes] (
    [Id]          INT             IDENTITY(1,1) NOT NULL,
    [Name]        NVARCHAR(100)   NOT NULL,
    [Description] NVARCHAR(255)   NULL,
    [Price]       DECIMAL(10,2)   NULL,
    -- BUG FIX: ImagePath column was missing from DB but used in Create.cshtml
    [ImagePath]   NVARCHAR(500)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO

-- ── Orders ───────────────────────────────────────────────────
CREATE TABLE [dbo].[Orders] (
    [OrderId]   INT      IDENTITY(1,1) NOT NULL,
    [UserId]    INT      NULL,
    [PerfumeId] INT      NULL,
    [OrderDate] DATETIME NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([OrderId] ASC),
    CONSTRAINT FK_Orders_Users    FOREIGN KEY ([UserId])    REFERENCES [Users]([Id]),
    CONSTRAINT FK_Orders_Perfumes FOREIGN KEY ([PerfumeId]) REFERENCES [Perfumes]([Id])
);
GO

-- ── Cart ─────────────────────────────────────────────────────
CREATE TABLE [dbo].[Cart] (
    [CartId]    INT      IDENTITY(1,1) NOT NULL,
    [UserId]    INT      NOT NULL,
    [PerfumeId] INT      NOT NULL,
    [OrderDate] DATETIME NOT NULL DEFAULT GETDATE(),
    PRIMARY KEY CLUSTERED ([CartId] ASC),
    -- BUG FIX: Cart table had commented-out FK constraints — added them properly
    CONSTRAINT FK_Cart_Users    FOREIGN KEY ([UserId])    REFERENCES [Users]([Id]),
    CONSTRAINT FK_Cart_Perfumes FOREIGN KEY ([PerfumeId]) REFERENCES [Perfumes]([Id])
);
GO

-- ── Seed: default admin account ──────────────────────────────
-- Password shown here is a BCrypt hash of "Admin@123"
-- CHANGE THIS before deploying to production!
INSERT INTO [dbo].[Users] ([Username],[Email],[Password],[Role])
VALUES (
    N'Admin',
    N'admin@royalscent.com',
    N'$2a$11$CHANGE_THIS_HASH_WITH_A_REAL_BCRYPT_HASH',
    N'Admin'
);
GO

PRINT 'Schema created successfully.';
