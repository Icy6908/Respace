CREATE TABLE [dbo].[Users] (
    [UserId]        INT            IDENTITY (1, 1) NOT NULL,
    [FullName]      NVARCHAR (100) NOT NULL,
    [Email]         NVARCHAR (120) NOT NULL,
    [PasswordHash]  NVARCHAR (256) NOT NULL,
    [Role]          NVARCHAR (20)  DEFAULT ('Guest') NOT NULL,
    [PointsBalance] INT            DEFAULT ((0)) NOT NULL,
    [CreatedAt]     DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Email] ASC)
);


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
    PRIMARY KEY CLUSTERED ([SpaceId] ASC),
    FOREIGN KEY ([HostUserId]) REFERENCES [dbo].[Users] ([UserId])
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

CREATE TABLE [dbo].[Bookings] (
    [BookingId]     INT             IDENTITY (1, 1) NOT NULL,
    [SpaceId]       INT             NOT NULL,
    [GuestUserId]   INT             NOT NULL,
    [StartDateTime] DATETIME        NOT NULL,
    [EndDateTime]   DATETIME        NOT NULL,
    [TotalPrice]    DECIMAL (10, 2) NOT NULL,
    [Status]        NVARCHAR (20)   DEFAULT ('Confirmed') NOT NULL,
    [CreatedAt]     DATETIME        DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([BookingId] ASC),
    FOREIGN KEY ([SpaceId]) REFERENCES [dbo].[Spaces] ([SpaceId]),
    FOREIGN KEY ([GuestUserId]) REFERENCES [dbo].[Users] ([UserId])
);

