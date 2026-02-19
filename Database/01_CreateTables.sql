CREATE TABLE [dbo].[Amenities] (
    [AmenityId]   INT           IDENTITY (1, 1) NOT NULL,
    [AmenityName] NVARCHAR (80) NOT NULL,
    [IconKey]     NVARCHAR (40) NULL,
    [IsActive]    BIT           DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([AmenityId] ASC),
    UNIQUE NONCLUSTERED ([AmenityName] ASC)
);

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
    PRIMARY KEY CLUSTERED ([BookingId] ASC),
    FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    FOREIGN KEY ([GuestUserId]) REFERENCES [dbo].[Users] ([UserId]),
    CONSTRAINT [CK_Bookings_GuestCount] CHECK ([GuestCount]>=(1))
);

CREATE TABLE [dbo].[MembershipPlans] (
    [PlanId]             INT             IDENTITY (1, 1) NOT NULL,
    [PlanName]           NVARCHAR (40)   NOT NULL,
    [MonthlyFee]         DECIMAL (10, 2) DEFAULT ((0)) NOT NULL,
    [BookingDiscountPct] DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [PointsMultiplier]   DECIMAL (5, 2)  DEFAULT ((1.00)) NOT NULL,
    [HostServiceFeePct]  DECIMAL (5, 2)  DEFAULT ((0)) NOT NULL,
    [IsActive]           BIT             DEFAULT ((1)) NOT NULL,
    PRIMARY KEY CLUSTERED ([PlanId] ASC),
    UNIQUE NONCLUSTERED ([PlanName] ASC)
);

CREATE TABLE [dbo].[PointsTransactions] (
    [TxnId]     INT            IDENTITY (1, 1) NOT NULL,
    [UserId]    INT            NOT NULL,
    [TxnType]   NVARCHAR (20)  NOT NULL,
    [Points]    INT            NOT NULL,
    [Reference] NVARCHAR (120) NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([TxnId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

CREATE TABLE [dbo].[Reviews] (
    [ReviewId]   INT             IDENTITY (1, 1) NOT NULL,
    [SpaceId]    INT             NOT NULL,
    [UserId]     INT             NOT NULL,
    [Rating]     INT             NOT NULL,
    [Comment]    NVARCHAR (1000) NULL,
    [IsApproved] BIT             DEFAULT ((0)) NOT NULL,
    [CreatedAt]  DATETIME        DEFAULT (getdate()) NOT NULL,
    [Badges]     NVARCHAR (300)  NULL,
    PRIMARY KEY CLUSTERED ([ReviewId] ASC),
    FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    CHECK ([Rating]>=(1) AND [Rating]<=(5))
);

CREATE TABLE [dbo].[SpaceAmenities] (
    [SpaceId]   INT NOT NULL,
    [AmenityId] INT NOT NULL,
    PRIMARY KEY CLUSTERED ([SpaceId] ASC, [AmenityId] ASC),
    CONSTRAINT [FK_SpaceAmenities_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    CONSTRAINT [FK_SpaceAmenities_Amenities] FOREIGN KEY ([AmenityId]) REFERENCES [dbo].[Amenities] ([AmenityId])
);


GO
CREATE NONCLUSTERED INDEX [IX_SpaceAmenities_SpaceId]
    ON [dbo].[SpaceAmenities]([SpaceId] ASC);

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
    CONSTRAINT [CK_SpaceBlocks_DateRange] CHECK ([EndDate]>[StartDate])
);


GO
CREATE NONCLUSTERED INDEX [IX_SpaceBlocks_SpaceId_Dates]
    ON [dbo].[SpaceBlocks]([SpaceId] ASC, [StartDate] ASC, [EndDate] ASC, [IsActive] ASC);

CREATE TABLE [dbo].[SpacePhotos] (
    [PhotoId]   INT            IDENTITY (1, 1) NOT NULL,
    [SpaceId]   INT            NOT NULL,
    [PhotoUrl]  NVARCHAR (400) NOT NULL,
    [SortOrder] INT            DEFAULT ((0)) NOT NULL,
    [IsCover]   BIT            DEFAULT ((0)) NOT NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([PhotoId] ASC),
    CONSTRAINT [FK_SpacePhotos_Spaces] FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId])
);


GO
CREATE NONCLUSTERED INDEX [IX_SpacePhotos_SpaceId]
    ON [dbo].[SpacePhotos]([SpaceId] ASC);

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
    PRIMARY KEY CLUSTERED ([SpaceId] ASC),
    FOREIGN KEY ([HostUserId]) REFERENCES [dbo].[Users] ([UserId])
);

CREATE TABLE [dbo].[UserCoupons] (
    [UserCouponId]   INT             IDENTITY (1, 1) NOT NULL,
    [UserId]         INT             NULL,
    [CouponCode]     VARCHAR (50)    NULL,
    [DiscountAmount] DECIMAL (18, 2) NULL,
    [IsUsed]         BIT             DEFAULT ((0)) NULL,
    [RedeemedAt]     DATETIME        DEFAULT (getdate()) NULL,
    PRIMARY KEY CLUSTERED ([UserCouponId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);

CREATE TABLE [dbo].[UserMemberships] (
    [UserMembershipId] INT      IDENTITY (1, 1) NOT NULL,
    [UserId]           INT      NOT NULL,
    [PlanId]           INT      NOT NULL,
    [StartDate]        DATETIME DEFAULT (getdate()) NOT NULL,
    [EndDate]          DATETIME NULL,
    [IsActive]         BIT      DEFAULT ((1)) NOT NULL,
    [AutoRenew]        BIT      DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserMembershipId] ASC),
    FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId]),
    FOREIGN KEY ([PlanId]) REFERENCES [dbo].[MembershipPlans] ([PlanId])
);

CREATE TABLE [dbo].[Users] (
    [UserId]         INT            IDENTITY (1, 1) NOT NULL,
    [FullName]       NVARCHAR (100) NOT NULL,
    [Email]          NVARCHAR (120) NOT NULL,
    [PasswordHash]   NVARCHAR (256) NOT NULL,
    [Role]           NVARCHAR (20)  DEFAULT ('Guest') NOT NULL,
    [PointsBalance]  INT            DEFAULT ((0)) NOT NULL,
    [CreatedAt]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [MembershipTier] NVARCHAR (20)  CONSTRAINT [DF_Users_MembershipTier] DEFAULT ('Basic') NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);

CREATE TABLE [dbo].[PasswordOtps] (
    [OtpId]     INT            IDENTITY (1, 1) NOT NULL,
    [UserId]    INT            NOT NULL,
    [Purpose]   NVARCHAR (40)  NOT NULL,
    [OtpHash]   NVARCHAR (256) NOT NULL,
    [ExpiresAt] DATETIME       NOT NULL,
    [UsedAt]    DATETIME       NULL,
    [Attempts]  INT            DEFAULT ((0)) NOT NULL,
    [CreatedAt] DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([OtpId] ASC),
    CONSTRAINT [FK_PasswordOtps_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([UserId])
);


GO
CREATE NONCLUSTERED INDEX [IX_PasswordOtps_UserPurposeCreated]
    ON [dbo].[PasswordOtps]([UserId] ASC, [Purpose] ASC, [CreatedAt] DESC);

