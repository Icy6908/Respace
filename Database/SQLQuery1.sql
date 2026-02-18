/* ========= DROP (safe order) ========= */
IF OBJECT_ID('dbo.SpaceAmenities','U') IS NOT NULL DROP TABLE dbo.SpaceAmenities;
IF OBJECT_ID('dbo.SpacePhotos','U')    IS NOT NULL DROP TABLE dbo.SpacePhotos;
IF OBJECT_ID('dbo.SpaceBlocks','U')    IS NOT NULL DROP TABLE dbo.SpaceBlocks;
IF OBJECT_ID('dbo.Reviews','U')        IS NOT NULL DROP TABLE dbo.Reviews;
IF OBJECT_ID('dbo.Bookings','U')       IS NOT NULL DROP TABLE dbo.Bookings;
IF OBJECT_ID('dbo.PointsTransactions','U') IS NOT NULL DROP TABLE dbo.PointsTransactions;
IF OBJECT_ID('dbo.UserMemberships','U') IS NOT NULL DROP TABLE dbo.UserMemberships;
IF OBJECT_ID('dbo.Amenities','U')      IS NOT NULL DROP TABLE dbo.Amenities;
IF OBJECT_ID('dbo.MembershipPlans','U') IS NOT NULL DROP TABLE dbo.MembershipPlans;
IF OBJECT_ID('dbo.Spaces','U')         IS NOT NULL DROP TABLE dbo.Spaces;
IF OBJECT_ID('dbo.Users','U')          IS NOT NULL DROP TABLE dbo.Users;
GO

/* ========= CREATE TABLES (correct order) ========= */

CREATE TABLE [dbo].[Users] (
    [UserId]         INT            IDENTITY (1, 1) NOT NULL,
    [FullName]       NVARCHAR (100) NOT NULL,
    [Email]          NVARCHAR (120) NOT NULL,
    [PasswordHash]   NVARCHAR (256) NOT NULL,
    [Role]           NVARCHAR (20)  DEFAULT ('Guest') NOT NULL,
    [PointsBalance]  INT            DEFAULT ((0)) NOT NULL,
    [CreatedAt]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [MembershipTier] NVARCHAR (20)  CONSTRAINT [DF_Users_MembershipTier] DEFAULT ('Basic') NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED ([UserId] ASC),
    CONSTRAINT [UQ_Users_Email] UNIQUE NONCLUSTERED ([Email] ASC)
);
GO

CREATE TABLE [dbo].[Spaces] (
    [SpaceId]      INT             IDENTITY (1, 1) NOT NULL,
    [HostUserId]   INT             NOT NULL,
    [Name]         NVARCHAR (120)  NOT NULL,
    [Location]     NVARCHAR (80)   NOT NULL,
    [Type]         NVARCHAR (80)   NOT NULL,
    [Description]  NVARCHAR (2000) NULL,
    [PricePerHour] DECIMAL (10, 2) DEFAULT ((0)) NOT NULL,
    [Capacity]     INT             DEFAULT ((1)) NOT NULL,
    [Status]       NVARCHAR (20)   DEFAULT ('Pending') NOT NULL,
    [AdminRemarks] NVARCHAR (500)  NULL,
    [CreatedAt]    DATETIME        DEFAULT (getdate()) NOT NULL,
    [IsDeleted]    BIT             CONSTRAINT [DF_Spaces_IsDeleted] DEFAULT ((0)) NOT NULL,
    [AddressLine]  NVARCHAR (200)  NULL,
    [City]         NVARCHAR (80)   NULL,
    [Postcode]     NVARCHAR (20)   NULL,
    [State]        NVARCHAR (80)   NULL,
    [Country]      NVARCHAR (80)   NULL,
    [Latitude]     DECIMAL (9, 6)  NULL,
    [Longitude]    DECIMAL (9, 6)  NULL,
    CONSTRAINT [PK_Spaces] PRIMARY KEY CLUSTERED ([SpaceId] ASC),
    CONSTRAINT [FK_Spaces_Users] FOREIGN KEY ([HostUserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO

CREATE TABLE [dbo].[Amenities] (
    [AmenityId]   INT           IDENTITY (1, 1) NOT NULL,
    [AmenityName] NVARCHAR (80) NOT NULL,
    [IconKey]     NVARCHAR (40) NULL,
    [IsActive]    BIT           DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Amenities] PRIMARY KEY CLUSTERED ([AmenityId] ASC),
    CONSTRAINT [UQ_Amenities_AmenityName] UNIQUE NONCLUSTERED ([AmenityName] ASC)
);
GO

CREATE TABLE [dbo].[SpaceAmenities] (
    [SpaceId]   INT NOT NULL,
    [AmenityId] INT NOT NULL,
    CONSTRAINT [PK_SpaceAmenities] PRIMARY KEY CLUSTERED ([SpaceId] ASC, [AmenityId] ASC),
    CONSTRAINT [FK_SpaceAmenities_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    CONSTRAINT [FK_SpaceAmenities_Amenities] FOREIGN KEY ([AmenityId]) REFERENCES [dbo].[Amenities] ([AmenityId])
);
GO

CREATE NONCLUSTERED INDEX [IX_SpaceAmenities_SpaceId]
    ON [dbo].[SpaceAmenities]([SpaceId] ASC);
GO

CREATE TABLE [dbo].[SpaceBlocks] (
    [BlockId]    INT            IDENTITY (1, 1) NOT NULL,
    [SpaceId]    INT            NOT NULL,
    [HostUserId] INT            NOT NULL,
    [StartDate]  DATE           NOT NULL,
    [EndDate]    DATE           NOT NULL,
    [Reason]     NVARCHAR (255) NULL,
    [IsActive]   BIT            CONSTRAINT [DF_SpaceBlocks_IsActive] DEFAULT ((1)) NOT NULL,
    [CreatedAt]  DATETIME       CONSTRAINT [DF_SpaceBlocks_CreatedAt] DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SpaceBlocks] PRIMARY KEY CLUSTERED ([BlockId] ASC),
    CONSTRAINT [FK_SpaceBlocks_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    CONSTRAINT [FK_SpaceBlocks_Users] FOREIGN KEY ([HostUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [CK_SpaceBlocks_DateRange] CHECK ([EndDate] > [StartDate])
);
GO

CREATE NONCLUSTERED INDEX [IX_SpaceBlocks_SpaceId_Dates]
    ON [dbo].[SpaceBlocks]([SpaceId] ASC, [StartDate] ASC, [EndDate] ASC, [IsActive] ASC);
GO

CREATE TABLE [dbo].[SpacePhotos] (
    [PhotoId]   INT            IDENTITY (1, 1) NOT NULL,
    [SpaceId]   INT            NOT NULL,
    [PhotoUrl]  NVARCHAR (400) NOT NULL,
    [SortOrder] INT            DEFAULT ((0)) NOT NULL,
    [IsCover]   BIT            DEFAULT ((0)) NOT NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_SpacePhotos] PRIMARY KEY CLUSTERED ([PhotoId] ASC),
    CONSTRAINT [FK_SpacePhotos_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId])
);
GO

CREATE NONCLUSTERED INDEX [IX_SpacePhotos_SpaceId]
    ON [dbo].[SpacePhotos]([SpaceId] ASC);
GO

CREATE TABLE [dbo].[Bookings] (
    [BookingId]     INT             IDENTITY (1, 1) NOT NULL,
    [SpaceId]       INT             NOT NULL,
    [GuestUserId]   INT             NOT NULL,
    [StartDateTime] DATETIME        NOT NULL,
    [EndDateTime]   DATETIME        NOT NULL,
    [TotalPrice]    DECIMAL (10, 2) NOT NULL,
    [Status]        NVARCHAR (20)   DEFAULT ('Confirmed') NOT NULL,
    [CreatedAt]     DATETIME        DEFAULT (getdate()) NOT NULL,
    [CancelComment] NVARCHAR (500)  NULL,
    [CancelledBy]   NVARCHAR (20)   NULL,
    [CancelledAt]   DATETIME        NULL,
    [CancelReason]  NVARCHAR (255)  NULL,
    [GuestCount]    INT             CONSTRAINT [DF_Bookings_GuestCount] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY CLUSTERED ([BookingId] ASC),
    CONSTRAINT [FK_Bookings_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    CONSTRAINT [FK_Bookings_Users_Guest] FOREIGN KEY ([GuestUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [CK_Bookings_GuestCount] CHECK ([GuestCount] >= (1))
);
GO

CREATE TABLE [dbo].[Reviews] (
    [ReviewId]   INT             IDENTITY (1, 1) NOT NULL,
    [SpaceId]    INT             NOT NULL,
    [UserId]     INT             NOT NULL,
    [Rating]     INT             NOT NULL,
    [Comment]    NVARCHAR (1000) NULL,
    [IsApproved] BIT             DEFAULT ((0)) NOT NULL,
    [CreatedAt]  DATETIME        DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY CLUSTERED ([ReviewId] ASC),
    CONSTRAINT [FK_Reviews_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    CONSTRAINT [FK_Reviews_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [CK_Reviews_Rating] CHECK ([Rating] >= (1) AND [Rating] <= (5))
);
GO

CREATE TABLE [dbo].[MembershipPlans] (
    [PlanId]             INT             IDENTITY (1, 1) NOT NULL,
    [PlanName]           NVARCHAR (40)   NOT NULL,
    [MonthlyFee]         DECIMAL (10, 2) DEFAULT ((0)) NOT NULL,
    [BookingDiscountPct] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [PointsMultiplier]   DECIMAL (5, 2)  DEFAULT ((1.00)) NOT NULL,
    [HostServiceFeePct]  DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [IsActive]           BIT             DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_MembershipPlans] PRIMARY KEY CLUSTERED ([PlanId] ASC),
    CONSTRAINT [UQ_MembershipPlans_PlanName] UNIQUE NONCLUSTERED ([PlanName] ASC)
);
GO

CREATE TABLE [dbo].[UserMemberships] (
    [UserMembershipId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]           INT      NOT NULL,
    [PlanId]           INT      NOT NULL,
    [StartDate]        DATETIME DEFAULT (getdate()) NOT NULL,
    [EndDate]          DATETIME NULL,
    [IsActive]         BIT      DEFAULT ((1)) NOT NULL,
    [AutoRenew]        BIT      DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_UserMemberships] PRIMARY KEY CLUSTERED ([UserMembershipId] ASC),
    CONSTRAINT [FK_UserMemberships_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [FK_UserMemberships_Plans] FOREIGN KEY ([PlanId]) REFERENCES [dbo].[MembershipPlans] ([PlanId])
);
GO

CREATE TABLE [dbo].[PointsTransactions] (
    [TxnId]     INT            IDENTITY (1, 1) NOT NULL,
    [UserId]    INT            NOT NULL,
    [TxnType]   NVARCHAR (20)  NOT NULL,
    [Points]    INT            NOT NULL,
    [Reference] NVARCHAR (120) NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    CONSTRAINT [PK_PointsTransactions] PRIMARY KEY CLUSTERED ([TxnId] ASC),
    CONSTRAINT [FK_PointsTransactions_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);
GO[14:42, 18/02/2026] HL: IF NOT EXISTS (SELECT 1 FROM dbo.Amenities)
BEGIN
  INSERT INTO dbo.Amenities (AmenityName, IconKey, IsActive) VALUES
  ('Wi-Fi', 'wifi', 1),
  ('Air Conditioning', 'ac', 1),
  ('Projector', 'projector', 1),
  ('Whiteboard', 'whiteboard', 1),
  ('Parking', 'parking', 1),
  ('Restroom', 'restroom', 1),
  ('Sound System', 'sound', 1),
  ('TV / Display', 'tv', 1),
  ('Kitchen / Pantry', 'kitchen', 1);
END

/* ===== Seed Membership Plans ===== */
IF NOT EXISTS (SELECT 1 FROM dbo.MembershipPlans)
BEGIN
  INSERT INTO dbo.MembershipPlans (PlanName, MonthlyFee, BookingDiscountPct, PointsMultiplier, HostServiceFeePct, IsActive) VALUES
  ('Basic', 0, 0, 1.00, 0, 1),
  ('Plus', 9.90, 5, 1.25, 2.5, 1),
  ('Pro', 19.90, 10, 1.50, 5.0, 1);
END
GO
[14:42, 18/02/2026] HL: IF COL_LENGTH('dbo.Spaces','AddressLine') IS NULL ALTER TABLE dbo.Spaces ADD AddressLine NVARCHAR(200) NULL;
IF COL_LENGTH('dbo.Spaces','City')        IS NULL ALTER TABLE dbo.Spaces ADD City        NVARCHAR(80)  NULL;
IF COL_LENGTH('dbo.Spaces','Postcode')    IS NULL ALTER TABLE dbo.Spaces ADD Postcode    NVARCHAR(20)  NULL;
IF COL_LENGTH('dbo.Spaces','State')       IS NULL ALTER TABLE dbo.Spaces ADD State       NVARCHAR(80)  NULL;
IF COL_LENGTH('dbo.Spaces','Country')     IS NULL ALTER TABLE dbo.Spaces ADD Country     NVARCHAR(80)  NULL;
IF COL_LENGTH('dbo.Spaces','Latitude')    IS NULL ALTER TABLE dbo.Spaces ADD Latitude    DECIMAL(9,6)  NULL;
IF COL_LENGTH('dbo.Spaces','Longitude')   IS NULL ALTER TABLE dbo.Spaces ADD Longitude   DECIMAL(9,6)  NULL;