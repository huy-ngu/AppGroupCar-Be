IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [BusRoutes] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BusRoutes] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Companies] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyName] nvarchar(200) NOT NULL,
    [CompanyCode] nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NULL,
    [Phone] nvarchar(15) NOT NULL,
    [Email] nvarchar(100) NULL,
    [Address] nvarchar(255) NOT NULL,
    [ProvinceCode] nvarchar(20) NOT NULL,
    [DistrictCode] nvarchar(20) NULL,
    [BusinessLicenseNo] nvarchar(50) NOT NULL,
    [TaxCode] nvarchar(50) NULL,
    [LicenseIssuedDate] datetime2 NULL,
    [LicenseIssuedBy] nvarchar(200) NULL,
    [CompanyType] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [ApprovedAt] datetime2 NULL,
    [ApprovedBy] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Companies] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Locations] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Code] nvarchar(50) NULL,
    [LocationType] int NOT NULL,
    [ParentLocationId] uniqueidentifier NULL,
    [Address] nvarchar(300) NULL,
    [IsActive] bit NOT NULL,
    [Coordinate] geography NULL,
    [GooglePlaceId] nvarchar(100) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Locations] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Locations_Locations_ParentLocationId] FOREIGN KEY ([ParentLocationId]) REFERENCES [Locations] ([Id])
);
GO

CREATE TABLE [Roles] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [FullName] nvarchar(max) NOT NULL,
    [CustomerRatingAvg] decimal(18,2) NOT NULL,
    [CustomerRatingCount] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    [RatingAverage] decimal(18,2) NOT NULL,
    [RatingCount] int NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [VehicleSeatLayouts] (
    [Id] uniqueidentifier NOT NULL,
    [SeatCapacity] int NOT NULL,
    [LayoutJson] nvarchar(max) NOT NULL,
    [Description] nvarchar(255) NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_VehicleSeatLayouts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BusRoutePoints] (
    [Id] uniqueidentifier NOT NULL,
    [RouteId] uniqueidentifier NOT NULL,
    [LocationId] uniqueidentifier NOT NULL,
    [Sequence] int NOT NULL,
    [PickupAllowed] bit NOT NULL,
    [DropoffAllowed] bit NOT NULL,
    [LocationId1] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BusRoutePoints] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusRoutePoints_BusRoutes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [BusRoutes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BusRoutePoints_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BusRoutePoints_Locations_LocationId1] FOREIGN KEY ([LocationId1]) REFERENCES [Locations] ([Id])
);
GO

CREATE TABLE [RoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] uniqueidentifier NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_RoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RoleClaims_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CompanyDrivers] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [LicenseNumber] nvarchar(20) NOT NULL,
    [LicenseClass] nvarchar(10) NOT NULL,
    [VerificationStatus] int NOT NULL,
    [VerifiedAt] datetime2 NULL,
    [VerifiedBy] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CompanyDrivers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyDrivers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyDrivers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [CompanyUsers] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] nvarchar(50) NOT NULL,
    [IsActive] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CompanyUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyUsers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyUsers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Drivers] (
    [UserId] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [IdentityNumber] nvarchar(20) NOT NULL,
    [LicenseNumber] nvarchar(20) NOT NULL,
    [LicenseClass] nvarchar(10) NOT NULL,
    [VerificationStatus] int NOT NULL,
    CONSTRAINT [PK_Drivers] PRIMARY KEY ([UserId]),
    CONSTRAINT [FK_Drivers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [RefreshTokens] (
    [Id] uniqueidentifier NOT NULL,
    [Token] nvarchar(max) NOT NULL,
    [JwtId] nvarchar(max) NOT NULL,
    [ExpiryDate] datetime2 NOT NULL,
    [Used] bit NOT NULL,
    [Invalidated] bit NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RefreshTokens] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RefreshTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [RideRequests] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [FromLocationId] uniqueidentifier NOT NULL,
    [ToLocationId] uniqueidentifier NOT NULL,
    [DesiredDepartureTime] datetime2 NOT NULL,
    [SeatsRequired] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_RideRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RideRequests_Locations_FromLocationId] FOREIGN KEY ([FromLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RideRequests_Locations_ToLocationId] FOREIGN KEY ([ToLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_RideRequests_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ShipmentRequests] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [PickupLocationId] uniqueidentifier NOT NULL,
    [DropoffLocationId] uniqueidentifier NOT NULL,
    [Weight] decimal(10,2) NOT NULL,
    [Volume] decimal(10,2) NOT NULL,
    [Description] nvarchar(500) NULL,
    [IsFragile] bit NOT NULL,
    [IsPerishable] bit NOT NULL,
    [RequiresCooler] bit NOT NULL,
    [AvoidSunLight] bit NOT NULL,
    [HandlingNote] nvarchar(500) NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ShipmentRequests] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ShipmentRequests_Locations_DropoffLocationId] FOREIGN KEY ([DropoffLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ShipmentRequests_Locations_PickupLocationId] FOREIGN KEY ([PickupLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ShipmentRequests_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [UserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] uniqueidentifier NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_UserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UserClaims_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_UserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_UserLogins_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserRoles] (
    [UserId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_UserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_UserRoles_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UserRoles_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [UserTokens] (
    [UserId] uniqueidentifier NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_UserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_UserTokens_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [CompanyVehicles] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyVehicleId] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [SeatLayoutId] uniqueidentifier NOT NULL,
    [PlateNumber] nvarchar(20) NOT NULL,
    [SeatCapacity] int NOT NULL,
    [VehicleType] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CompanyVehicles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyVehicles_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyVehicles_VehicleSeatLayouts_SeatLayoutId] FOREIGN KEY ([SeatLayoutId]) REFERENCES [VehicleSeatLayouts] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Vehicles] (
    [Id] uniqueidentifier NOT NULL,
    [DriverId] uniqueidentifier NOT NULL,
    [PlateNumber] nvarchar(20) NOT NULL,
    [Brand] nvarchar(50) NOT NULL,
    [SeatCapacity] int NOT NULL,
    [VehicleType] int NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Vehicles] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Vehicles_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([UserId]) ON DELETE CASCADE
);
GO

CREATE TABLE [BusShowtimes] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [CompanyVehicleId] uniqueidentifier NOT NULL,
    [RouteId] uniqueidentifier NOT NULL,
    [DepartureDate] date NOT NULL,
    [DepartureTime] time NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BusShowtimes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusShowtimes_BusRoutes_RouteId] FOREIGN KEY ([RouteId]) REFERENCES [BusRoutes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BusShowtimes_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_BusShowtimes_CompanyVehicles_CompanyVehicleId] FOREIGN KEY ([CompanyVehicleId]) REFERENCES [CompanyVehicles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Offers] (
    [Id] uniqueidentifier NOT NULL,
    [DriverId] uniqueidentifier NOT NULL,
    [VehicleId] uniqueidentifier NOT NULL,
    [ServiceType] int NOT NULL,
    [DepartureTime] datetime2 NOT NULL,
    [BasePrice] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Offers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Offers_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Offers_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ShipmentOffers] (
    [Id] uniqueidentifier NOT NULL,
    [RequestId] uniqueidentifier NOT NULL,
    [DriverId] uniqueidentifier NOT NULL,
    [VehicleId] uniqueidentifier NOT NULL,
    [ProposedPrice] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ShipmentOffers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ShipmentOffers_Drivers_DriverId] FOREIGN KEY ([DriverId]) REFERENCES [Drivers] ([UserId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_ShipmentOffers_ShipmentRequests_RequestId] FOREIGN KEY ([RequestId]) REFERENCES [ShipmentRequests] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ShipmentOffers_Vehicles_VehicleId] FOREIGN KEY ([VehicleId]) REFERENCES [Vehicles] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [BusBookings] (
    [Id] uniqueidentifier NOT NULL,
    [ShowtimeId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BusBookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusBookings_BusShowtimes_ShowtimeId] FOREIGN KEY ([ShowtimeId]) REFERENCES [BusShowtimes] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_BusBookings_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [ShowtimeLogs] (
    [Id] uniqueidentifier NOT NULL,
    [ShowtimeId] uniqueidentifier NOT NULL,
    [Action] nvarchar(50) NOT NULL,
    [OldStatus] int NULL,
    [NewStatus] int NULL,
    [CreatedBy] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_ShowtimeLogs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ShowtimeLogs_BusShowtimes_ShowtimeId] FOREIGN KEY ([ShowtimeId]) REFERENCES [BusShowtimes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Bookings] (
    [Id] uniqueidentifier NOT NULL,
    [OfferId] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [TotalPrice] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [PaymentStatus] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Bookings] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Bookings_Offers_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Offers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Bookings_Users_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [CargoOfferDetails] (
    [Id] uniqueidentifier NOT NULL,
    [MaxWeight] decimal(10,2) NOT NULL,
    [MaxVolume] decimal(10,2) NOT NULL,
    [AcceptFragile] bit NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_CargoOfferDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CargoOfferDetails_Offers_Id] FOREIGN KEY ([Id]) REFERENCES [Offers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [OfferRoutePoints] (
    [OfferId] uniqueidentifier NOT NULL,
    [Sequence] int NOT NULL,
    [LocationId] uniqueidentifier NOT NULL,
    [PickupAllowed] bit NOT NULL,
    [DropoffAllowed] bit NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_OfferRoutePoints] PRIMARY KEY ([OfferId], [Sequence]),
    CONSTRAINT [FK_OfferRoutePoints_Locations_LocationId] FOREIGN KEY ([LocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_OfferRoutePoints_Offers_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Offers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SharedRideOfferDetails] (
    [Id] uniqueidentifier NOT NULL,
    [TotalSeats] int NOT NULL,
    [AvailableSeats] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_SharedRideOfferDetails] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SharedRideOfferDetails_Offers_Id] FOREIGN KEY ([Id]) REFERENCES [Offers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Shipments] (
    [Id] uniqueidentifier NOT NULL,
    [ShipmentOfferId] uniqueidentifier NOT NULL,
    [ShipmentStatus] int NOT NULL,
    [PaymentStatus] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_Shipments] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Shipments_ShipmentOffers_ShipmentOfferId] FOREIGN KEY ([ShipmentOfferId]) REFERENCES [ShipmentOffers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BusBookingSeats] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [SeatNumber] int NOT NULL,
    [TicketCode] nvarchar(100) NOT NULL,
    [IsCheckedIn] bit NOT NULL,
    [CheckedInAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BusBookingSeats] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BusBookingSeats_BusBookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [BusBookings] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [BookingItems] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [Price] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NOT NULL,
    [DeleteAt] datetime2 NOT NULL,
    CONSTRAINT [PK_BookingItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_BookingItems_Bookings_BookingId] FOREIGN KEY ([BookingId]) REFERENCES [Bookings] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_BookingItems_BookingId] ON [BookingItems] ([BookingId]);
GO

CREATE INDEX [IX_Bookings_CustomerId] ON [Bookings] ([CustomerId]);
GO

CREATE INDEX [IX_Bookings_OfferId] ON [Bookings] ([OfferId]);
GO

CREATE INDEX [IX_BusBookings_ShowtimeId] ON [BusBookings] ([ShowtimeId]);
GO

CREATE INDEX [IX_BusBookings_UserId] ON [BusBookings] ([UserId]);
GO

CREATE UNIQUE INDEX [IX_BusBookingSeats_BookingId_SeatNumber] ON [BusBookingSeats] ([BookingId], [SeatNumber]);
GO

CREATE UNIQUE INDEX [IX_BusBookingSeats_TicketCode] ON [BusBookingSeats] ([TicketCode]);
GO

CREATE INDEX [IX_BusRoutePoints_LocationId] ON [BusRoutePoints] ([LocationId]);
GO

CREATE INDEX [IX_BusRoutePoints_LocationId1] ON [BusRoutePoints] ([LocationId1]);
GO

CREATE UNIQUE INDEX [IX_BusRoutePoints_RouteId_Sequence] ON [BusRoutePoints] ([RouteId], [Sequence]);
GO

CREATE INDEX [IX_BusShowtimes_CompanyId] ON [BusShowtimes] ([CompanyId]);
GO

CREATE UNIQUE INDEX [IX_BusShowtimes_CompanyVehicleId_DepartureDate_DepartureTime] ON [BusShowtimes] ([CompanyVehicleId], [DepartureDate], [DepartureTime]);
GO

CREATE INDEX [IX_BusShowtimes_RouteId] ON [BusShowtimes] ([RouteId]);
GO

CREATE INDEX [IX_CompanyDrivers_CompanyId] ON [CompanyDrivers] ([CompanyId]);
GO

CREATE INDEX [IX_CompanyDrivers_UserId] ON [CompanyDrivers] ([UserId]);
GO

CREATE INDEX [IX_CompanyUsers_CompanyId] ON [CompanyUsers] ([CompanyId]);
GO

CREATE INDEX [IX_CompanyUsers_UserId] ON [CompanyUsers] ([UserId]);
GO

CREATE INDEX [IX_CompanyVehicles_CompanyId] ON [CompanyVehicles] ([CompanyId]);
GO

CREATE INDEX [IX_CompanyVehicles_SeatLayoutId] ON [CompanyVehicles] ([SeatLayoutId]);
GO

CREATE INDEX [IX_Locations_ParentLocationId] ON [Locations] ([ParentLocationId]);
GO

CREATE INDEX [IX_OfferRoutePoints_LocationId] ON [OfferRoutePoints] ([LocationId]);
GO

CREATE INDEX [IX_Offers_DriverId] ON [Offers] ([DriverId]);
GO

CREATE INDEX [IX_Offers_VehicleId] ON [Offers] ([VehicleId]);
GO

CREATE INDEX [IX_RefreshTokens_UserId] ON [RefreshTokens] ([UserId]);
GO

CREATE INDEX [IX_RideRequests_CustomerId] ON [RideRequests] ([CustomerId]);
GO

CREATE INDEX [IX_RideRequests_FromLocationId] ON [RideRequests] ([FromLocationId]);
GO

CREATE INDEX [IX_RideRequests_ToLocationId] ON [RideRequests] ([ToLocationId]);
GO

CREATE INDEX [IX_RoleClaims_RoleId] ON [RoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [Roles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_ShipmentOffers_DriverId] ON [ShipmentOffers] ([DriverId]);
GO

CREATE INDEX [IX_ShipmentOffers_RequestId] ON [ShipmentOffers] ([RequestId]);
GO

CREATE INDEX [IX_ShipmentOffers_VehicleId] ON [ShipmentOffers] ([VehicleId]);
GO

CREATE INDEX [IX_ShipmentRequests_CustomerId] ON [ShipmentRequests] ([CustomerId]);
GO

CREATE INDEX [IX_ShipmentRequests_DropoffLocationId] ON [ShipmentRequests] ([DropoffLocationId]);
GO

CREATE INDEX [IX_ShipmentRequests_PickupLocationId] ON [ShipmentRequests] ([PickupLocationId]);
GO

CREATE UNIQUE INDEX [IX_Shipments_ShipmentOfferId] ON [Shipments] ([ShipmentOfferId]);
GO

CREATE INDEX [IX_ShowtimeLogs_ShowtimeId] ON [ShowtimeLogs] ([ShowtimeId]);
GO

CREATE INDEX [IX_UserClaims_UserId] ON [UserClaims] ([UserId]);
GO

CREATE INDEX [IX_UserLogins_UserId] ON [UserLogins] ([UserId]);
GO

CREATE INDEX [IX_UserRoles_RoleId] ON [UserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [Users] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [Users] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Vehicles_DriverId] ON [Vehicles] ([DriverId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260228154425_initDatabase', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VehicleSeatLayouts]') AND [c].[name] = N'LastUpdateAt');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [VehicleSeatLayouts] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [VehicleSeatLayouts] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[VehicleSeatLayouts]') AND [c].[name] = N'DeleteAt');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [VehicleSeatLayouts] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [VehicleSeatLayouts] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'LastUpdateAt');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Vehicles] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Vehicles]') AND [c].[name] = N'DeleteAt');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Vehicles] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Vehicles] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'LastUpdateAt');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [Users] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'DeleteAt');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [Users] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShowtimeLogs]') AND [c].[name] = N'LastUpdateAt');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [ShowtimeLogs] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [ShowtimeLogs] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var7 sysname;
SELECT @var7 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShowtimeLogs]') AND [c].[name] = N'DeleteAt');
IF @var7 IS NOT NULL EXEC(N'ALTER TABLE [ShowtimeLogs] DROP CONSTRAINT [' + @var7 + '];');
ALTER TABLE [ShowtimeLogs] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var8 sysname;
SELECT @var8 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'LastUpdateAt');
IF @var8 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var8 + '];');
ALTER TABLE [Shipments] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var9 sysname;
SELECT @var9 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'DeleteAt');
IF @var9 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var9 + '];');
ALTER TABLE [Shipments] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var10 sysname;
SELECT @var10 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'LastUpdateAt');
IF @var10 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var10 + '];');
ALTER TABLE [ShipmentRequests] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var11 sysname;
SELECT @var11 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'DeleteAt');
IF @var11 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var11 + '];');
ALTER TABLE [ShipmentRequests] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var12 sysname;
SELECT @var12 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentOffers]') AND [c].[name] = N'LastUpdateAt');
IF @var12 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentOffers] DROP CONSTRAINT [' + @var12 + '];');
ALTER TABLE [ShipmentOffers] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var13 sysname;
SELECT @var13 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentOffers]') AND [c].[name] = N'DeleteAt');
IF @var13 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentOffers] DROP CONSTRAINT [' + @var13 + '];');
ALTER TABLE [ShipmentOffers] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var14 sysname;
SELECT @var14 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SharedRideOfferDetails]') AND [c].[name] = N'LastUpdateAt');
IF @var14 IS NOT NULL EXEC(N'ALTER TABLE [SharedRideOfferDetails] DROP CONSTRAINT [' + @var14 + '];');
ALTER TABLE [SharedRideOfferDetails] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var15 sysname;
SELECT @var15 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SharedRideOfferDetails]') AND [c].[name] = N'DeleteAt');
IF @var15 IS NOT NULL EXEC(N'ALTER TABLE [SharedRideOfferDetails] DROP CONSTRAINT [' + @var15 + '];');
ALTER TABLE [SharedRideOfferDetails] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var16 sysname;
SELECT @var16 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RideRequests]') AND [c].[name] = N'LastUpdateAt');
IF @var16 IS NOT NULL EXEC(N'ALTER TABLE [RideRequests] DROP CONSTRAINT [' + @var16 + '];');
ALTER TABLE [RideRequests] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var17 sysname;
SELECT @var17 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RideRequests]') AND [c].[name] = N'DeleteAt');
IF @var17 IS NOT NULL EXEC(N'ALTER TABLE [RideRequests] DROP CONSTRAINT [' + @var17 + '];');
ALTER TABLE [RideRequests] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var18 sysname;
SELECT @var18 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RefreshTokens]') AND [c].[name] = N'LastUpdateAt');
IF @var18 IS NOT NULL EXEC(N'ALTER TABLE [RefreshTokens] DROP CONSTRAINT [' + @var18 + '];');
ALTER TABLE [RefreshTokens] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var19 sysname;
SELECT @var19 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[RefreshTokens]') AND [c].[name] = N'DeleteAt');
IF @var19 IS NOT NULL EXEC(N'ALTER TABLE [RefreshTokens] DROP CONSTRAINT [' + @var19 + '];');
ALTER TABLE [RefreshTokens] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var20 sysname;
SELECT @var20 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Offers]') AND [c].[name] = N'LastUpdateAt');
IF @var20 IS NOT NULL EXEC(N'ALTER TABLE [Offers] DROP CONSTRAINT [' + @var20 + '];');
ALTER TABLE [Offers] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var21 sysname;
SELECT @var21 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Offers]') AND [c].[name] = N'DeleteAt');
IF @var21 IS NOT NULL EXEC(N'ALTER TABLE [Offers] DROP CONSTRAINT [' + @var21 + '];');
ALTER TABLE [Offers] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var22 sysname;
SELECT @var22 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OfferRoutePoints]') AND [c].[name] = N'LastUpdateAt');
IF @var22 IS NOT NULL EXEC(N'ALTER TABLE [OfferRoutePoints] DROP CONSTRAINT [' + @var22 + '];');
ALTER TABLE [OfferRoutePoints] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var23 sysname;
SELECT @var23 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OfferRoutePoints]') AND [c].[name] = N'DeleteAt');
IF @var23 IS NOT NULL EXEC(N'ALTER TABLE [OfferRoutePoints] DROP CONSTRAINT [' + @var23 + '];');
ALTER TABLE [OfferRoutePoints] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var24 sysname;
SELECT @var24 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Locations]') AND [c].[name] = N'LastUpdateAt');
IF @var24 IS NOT NULL EXEC(N'ALTER TABLE [Locations] DROP CONSTRAINT [' + @var24 + '];');
ALTER TABLE [Locations] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var25 sysname;
SELECT @var25 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Locations]') AND [c].[name] = N'DeleteAt');
IF @var25 IS NOT NULL EXEC(N'ALTER TABLE [Locations] DROP CONSTRAINT [' + @var25 + '];');
ALTER TABLE [Locations] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var26 sysname;
SELECT @var26 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyVehicles]') AND [c].[name] = N'LastUpdateAt');
IF @var26 IS NOT NULL EXEC(N'ALTER TABLE [CompanyVehicles] DROP CONSTRAINT [' + @var26 + '];');
ALTER TABLE [CompanyVehicles] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var27 sysname;
SELECT @var27 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyVehicles]') AND [c].[name] = N'DeleteAt');
IF @var27 IS NOT NULL EXEC(N'ALTER TABLE [CompanyVehicles] DROP CONSTRAINT [' + @var27 + '];');
ALTER TABLE [CompanyVehicles] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var28 sysname;
SELECT @var28 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyUsers]') AND [c].[name] = N'LastUpdateAt');
IF @var28 IS NOT NULL EXEC(N'ALTER TABLE [CompanyUsers] DROP CONSTRAINT [' + @var28 + '];');
ALTER TABLE [CompanyUsers] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var29 sysname;
SELECT @var29 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyUsers]') AND [c].[name] = N'DeleteAt');
IF @var29 IS NOT NULL EXEC(N'ALTER TABLE [CompanyUsers] DROP CONSTRAINT [' + @var29 + '];');
ALTER TABLE [CompanyUsers] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var30 sysname;
SELECT @var30 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyDrivers]') AND [c].[name] = N'LastUpdateAt');
IF @var30 IS NOT NULL EXEC(N'ALTER TABLE [CompanyDrivers] DROP CONSTRAINT [' + @var30 + '];');
ALTER TABLE [CompanyDrivers] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var31 sysname;
SELECT @var31 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyDrivers]') AND [c].[name] = N'DeleteAt');
IF @var31 IS NOT NULL EXEC(N'ALTER TABLE [CompanyDrivers] DROP CONSTRAINT [' + @var31 + '];');
ALTER TABLE [CompanyDrivers] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var32 sysname;
SELECT @var32 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'LastUpdateAt');
IF @var32 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var32 + '];');
ALTER TABLE [Companies] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var33 sysname;
SELECT @var33 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'DeleteAt');
IF @var33 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var33 + '];');
ALTER TABLE [Companies] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var34 sysname;
SELECT @var34 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoOfferDetails]') AND [c].[name] = N'LastUpdateAt');
IF @var34 IS NOT NULL EXEC(N'ALTER TABLE [CargoOfferDetails] DROP CONSTRAINT [' + @var34 + '];');
ALTER TABLE [CargoOfferDetails] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var35 sysname;
SELECT @var35 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CargoOfferDetails]') AND [c].[name] = N'DeleteAt');
IF @var35 IS NOT NULL EXEC(N'ALTER TABLE [CargoOfferDetails] DROP CONSTRAINT [' + @var35 + '];');
ALTER TABLE [CargoOfferDetails] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var36 sysname;
SELECT @var36 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusShowtimes]') AND [c].[name] = N'LastUpdateAt');
IF @var36 IS NOT NULL EXEC(N'ALTER TABLE [BusShowtimes] DROP CONSTRAINT [' + @var36 + '];');
ALTER TABLE [BusShowtimes] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var37 sysname;
SELECT @var37 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusShowtimes]') AND [c].[name] = N'DeleteAt');
IF @var37 IS NOT NULL EXEC(N'ALTER TABLE [BusShowtimes] DROP CONSTRAINT [' + @var37 + '];');
ALTER TABLE [BusShowtimes] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var38 sysname;
SELECT @var38 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusRoutes]') AND [c].[name] = N'LastUpdateAt');
IF @var38 IS NOT NULL EXEC(N'ALTER TABLE [BusRoutes] DROP CONSTRAINT [' + @var38 + '];');
ALTER TABLE [BusRoutes] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var39 sysname;
SELECT @var39 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusRoutes]') AND [c].[name] = N'DeleteAt');
IF @var39 IS NOT NULL EXEC(N'ALTER TABLE [BusRoutes] DROP CONSTRAINT [' + @var39 + '];');
ALTER TABLE [BusRoutes] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var40 sysname;
SELECT @var40 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusRoutePoints]') AND [c].[name] = N'LastUpdateAt');
IF @var40 IS NOT NULL EXEC(N'ALTER TABLE [BusRoutePoints] DROP CONSTRAINT [' + @var40 + '];');
ALTER TABLE [BusRoutePoints] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var41 sysname;
SELECT @var41 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusRoutePoints]') AND [c].[name] = N'DeleteAt');
IF @var41 IS NOT NULL EXEC(N'ALTER TABLE [BusRoutePoints] DROP CONSTRAINT [' + @var41 + '];');
ALTER TABLE [BusRoutePoints] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var42 sysname;
SELECT @var42 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusBookingSeats]') AND [c].[name] = N'LastUpdateAt');
IF @var42 IS NOT NULL EXEC(N'ALTER TABLE [BusBookingSeats] DROP CONSTRAINT [' + @var42 + '];');
ALTER TABLE [BusBookingSeats] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var43 sysname;
SELECT @var43 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusBookingSeats]') AND [c].[name] = N'DeleteAt');
IF @var43 IS NOT NULL EXEC(N'ALTER TABLE [BusBookingSeats] DROP CONSTRAINT [' + @var43 + '];');
ALTER TABLE [BusBookingSeats] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var44 sysname;
SELECT @var44 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusBookings]') AND [c].[name] = N'LastUpdateAt');
IF @var44 IS NOT NULL EXEC(N'ALTER TABLE [BusBookings] DROP CONSTRAINT [' + @var44 + '];');
ALTER TABLE [BusBookings] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var45 sysname;
SELECT @var45 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusBookings]') AND [c].[name] = N'DeleteAt');
IF @var45 IS NOT NULL EXEC(N'ALTER TABLE [BusBookings] DROP CONSTRAINT [' + @var45 + '];');
ALTER TABLE [BusBookings] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var46 sysname;
SELECT @var46 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bookings]') AND [c].[name] = N'LastUpdateAt');
IF @var46 IS NOT NULL EXEC(N'ALTER TABLE [Bookings] DROP CONSTRAINT [' + @var46 + '];');
ALTER TABLE [Bookings] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var47 sysname;
SELECT @var47 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Bookings]') AND [c].[name] = N'DeleteAt');
IF @var47 IS NOT NULL EXEC(N'ALTER TABLE [Bookings] DROP CONSTRAINT [' + @var47 + '];');
ALTER TABLE [Bookings] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

DECLARE @var48 sysname;
SELECT @var48 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BookingItems]') AND [c].[name] = N'LastUpdateAt');
IF @var48 IS NOT NULL EXEC(N'ALTER TABLE [BookingItems] DROP CONSTRAINT [' + @var48 + '];');
ALTER TABLE [BookingItems] ALTER COLUMN [LastUpdateAt] datetime2 NULL;
GO

DECLARE @var49 sysname;
SELECT @var49 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BookingItems]') AND [c].[name] = N'DeleteAt');
IF @var49 IS NOT NULL EXEC(N'ALTER TABLE [BookingItems] DROP CONSTRAINT [' + @var49 + '];');
ALTER TABLE [BookingItems] ALTER COLUMN [DeleteAt] datetime2 NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260228155519_fixBaseEntity', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[CompanyDrivers].[VerificationStatus]', N'DriverVerificationStatus', N'COLUMN';
GO

ALTER TABLE [Vehicles] ADD [RegistrationDocumentUrl] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Vehicles] ADD [UrlImage] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Users] ADD [AvatarUrl] nvarchar(max) NOT NULL DEFAULT N'';
GO

ALTER TABLE [ShipmentRequests] ADD [ImageUrl] nvarchar(500) NULL;
GO

ALTER TABLE [Drivers] ADD [LicenseDocumentUrl] nvarchar(500) NULL;
GO

ALTER TABLE [CompanyDrivers] ADD [LicenseDocumentUrl] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [Companies] ADD [LogoUrl] nvarchar(500) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260302153123_UpdateApplicationUserFields', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var50 sysname;
SELECT @var50 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Users]') AND [c].[name] = N'AvatarUrl');
IF @var50 IS NOT NULL EXEC(N'ALTER TABLE [Users] DROP CONSTRAINT [' + @var50 + '];');
ALTER TABLE [Users] ALTER COLUMN [AvatarUrl] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260302153505_MakeAvatarUrlNullable', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var51 sysname;
SELECT @var51 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'ImageUrl');
IF @var51 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var51 + '];');
ALTER TABLE [ShipmentRequests] ALTER COLUMN [ImageUrl] nvarchar(max) NULL;
GO

DECLARE @var52 sysname;
SELECT @var52 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Drivers]') AND [c].[name] = N'LicenseDocumentUrl');
IF @var52 IS NOT NULL EXEC(N'ALTER TABLE [Drivers] DROP CONSTRAINT [' + @var52 + '];');
ALTER TABLE [Drivers] ALTER COLUMN [LicenseDocumentUrl] nvarchar(max) NULL;
GO

ALTER TABLE [Drivers] ADD [CreatedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [Drivers] ADD [DeleteAt] datetime2 NULL;
GO

ALTER TABLE [Drivers] ADD [LastUpdateAt] datetime2 NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260305113140_update_driver', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var53 sysname;
SELECT @var53 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'CreatedBy');
IF @var53 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var53 + '];');
ALTER TABLE [Companies] DROP COLUMN [CreatedBy];
GO

DECLARE @var54 sysname;
SELECT @var54 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'ImageUrl');
IF @var54 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var54 + '];');
ALTER TABLE [ShipmentRequests] ALTER COLUMN [ImageUrl] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260305174005_DB_GOUPCARAPP_V2', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260311093109_update_company', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260311094125_company_v3', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var55 sysname;
SELECT @var55 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Companies]') AND [c].[name] = N'ApprovedBy');
IF @var55 IS NOT NULL EXEC(N'ALTER TABLE [Companies] DROP CONSTRAINT [' + @var55 + '];');
ALTER TABLE [Companies] DROP COLUMN [ApprovedBy];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260311094602_company_v4', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var56 sysname;
SELECT @var56 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BookingItems]') AND [c].[name] = N'Quantity');
IF @var56 IS NOT NULL EXEC(N'ALTER TABLE [BookingItems] DROP CONSTRAINT [' + @var56 + '];');
ALTER TABLE [BookingItems] DROP COLUMN [Quantity];
GO

ALTER TABLE [BookingItems] ADD [DropoffLocationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

ALTER TABLE [BookingItems] ADD [PickupLocationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

CREATE INDEX [IX_BookingItems_DropoffLocationId] ON [BookingItems] ([DropoffLocationId]);
GO

CREATE INDEX [IX_BookingItems_PickupLocationId] ON [BookingItems] ([PickupLocationId]);
GO

ALTER TABLE [BookingItems] ADD CONSTRAINT [FK_BookingItems_Locations_DropoffLocationId] FOREIGN KEY ([DropoffLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [BookingItems] ADD CONSTRAINT [FK_BookingItems_Locations_PickupLocationId] FOREIGN KEY ([PickupLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260316043759_updateBookingItem', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [CompanyDrivers] DROP CONSTRAINT [FK_CompanyDrivers_Companies_CompanyId];
GO

ALTER TABLE [CompanyDrivers] DROP CONSTRAINT [FK_CompanyDrivers_Users_UserId];
GO

DROP INDEX [IX_CompanyDrivers_CompanyId] ON [CompanyDrivers];
GO

DROP INDEX [IX_CompanyDrivers_UserId] ON [CompanyDrivers];
GO

DECLARE @var57 sysname;
SELECT @var57 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyDrivers]') AND [c].[name] = N'CompanyId');
IF @var57 IS NOT NULL EXEC(N'ALTER TABLE [CompanyDrivers] DROP CONSTRAINT [' + @var57 + '];');
ALTER TABLE [CompanyDrivers] DROP COLUMN [CompanyId];
GO

DECLARE @var58 sysname;
SELECT @var58 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyDrivers]') AND [c].[name] = N'UserId');
IF @var58 IS NOT NULL EXEC(N'ALTER TABLE [CompanyDrivers] DROP CONSTRAINT [' + @var58 + '];');
ALTER TABLE [CompanyDrivers] DROP COLUMN [UserId];
GO

ALTER TABLE [CompanyVehicles] ADD [RegistrationDocumentUrl] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [CompanyVehicles] ADD [UrlImage] nvarchar(500) NOT NULL DEFAULT N'';
GO

ALTER TABLE [BusRoutes] ADD [CompanyId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

CREATE INDEX [IX_BusRoutes_CompanyId] ON [BusRoutes] ([CompanyId]);
GO

ALTER TABLE [BusRoutes] ADD CONSTRAINT [FK_BusRoutes_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE;
GO

ALTER TABLE [BusShowtimes] ADD CONSTRAINT [FK_BusShowtimes_Drivers_Id] FOREIGN KEY ([Id]) REFERENCES [Drivers] ([UserId]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324025507_bus04', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324102329_bus04_2', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var59 sysname;
SELECT @var59 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyUsers]') AND [c].[name] = N'RoleId');
IF @var59 IS NOT NULL EXEC(N'ALTER TABLE [CompanyUsers] DROP CONSTRAINT [' + @var59 + '];');
ALTER TABLE [CompanyUsers] DROP COLUMN [RoleId];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324102453_bus05', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [CompanyUsers] ADD [JoinedAt] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324102947_bus05_2', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [BusShowtimes] DROP CONSTRAINT [FK_BusShowtimes_Drivers_Id];
GO

DROP TABLE [CompanyDrivers];
GO

ALTER TABLE [BusBookingSeats] ADD [SeatId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

CREATE TABLE [Old_CompanyDrivers] (
    [Id] uniqueidentifier NOT NULL,
    [LicenseNumber] nvarchar(20) NOT NULL,
    [LicenseClass] nvarchar(10) NOT NULL,
    [DriverVerificationStatus] int NOT NULL,
    [LicenseDocumentUrl] nvarchar(500) NOT NULL,
    [VerifiedAt] datetime2 NULL,
    [VerifiedBy] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Old_CompanyDrivers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Seats] (
    [SeatId] uniqueidentifier NOT NULL,
    [ShowtimeId] uniqueidentifier NOT NULL,
    [SeatNumber] nvarchar(10) NOT NULL,
    [Status] int NOT NULL,
    [LastUpdatedAt] datetime2 NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Seats] PRIMARY KEY ([SeatId]),
    CONSTRAINT [FK_Seats_BusShowtimes_ShowtimeId] FOREIGN KEY ([ShowtimeId]) REFERENCES [BusShowtimes] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [SeatStatusHistories] (
    [HistoryId] uniqueidentifier NOT NULL,
    [SeatId] uniqueidentifier NOT NULL,
    [OldStatus] int NOT NULL,
    [NewStatus] int NOT NULL,
    [Reason] nvarchar(255) NULL,
    [ChangedBy] uniqueidentifier NOT NULL,
    [ChangedAt] datetime2 NOT NULL,
    [Id] uniqueidentifier NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_SeatStatusHistories] PRIMARY KEY ([HistoryId]),
    CONSTRAINT [FK_SeatStatusHistories_Seats_SeatId] FOREIGN KEY ([SeatId]) REFERENCES [Seats] ([SeatId]) ON DELETE NO ACTION,
    CONSTRAINT [FK_SeatStatusHistories_Users_ChangedBy] FOREIGN KEY ([ChangedBy]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_BusBookingSeats_SeatId] ON [BusBookingSeats] ([SeatId]);
GO

CREATE INDEX [IX_Seats_ShowtimeId] ON [Seats] ([ShowtimeId]);
GO

CREATE INDEX [IX_SeatStatusHistories_ChangedAt] ON [SeatStatusHistories] ([ChangedAt]);
GO

CREATE INDEX [IX_SeatStatusHistories_ChangedBy] ON [SeatStatusHistories] ([ChangedBy]);
GO

CREATE INDEX [IX_SeatStatusHistories_SeatId] ON [SeatStatusHistories] ([SeatId]);
GO

ALTER TABLE [BusBookingSeats] ADD CONSTRAINT [FK_BusBookingSeats_Seats_SeatId] FOREIGN KEY ([SeatId]) REFERENCES [Seats] ([SeatId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260324180326_bu5_fantasy', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Bookings] ADD [Quantity] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260327032152_add quantity booking', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var60 sysname;
SELECT @var60 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'AvoidSunLight');
IF @var60 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var60 + '];');
ALTER TABLE [ShipmentRequests] DROP COLUMN [AvoidSunLight];
GO

DECLARE @var61 sysname;
SELECT @var61 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'IsPerishable');
IF @var61 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var61 + '];');
ALTER TABLE [ShipmentRequests] DROP COLUMN [IsPerishable];
GO

DECLARE @var62 sysname;
SELECT @var62 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentRequests]') AND [c].[name] = N'RequiresCooler');
IF @var62 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentRequests] DROP CONSTRAINT [' + @var62 + '];');
ALTER TABLE [ShipmentRequests] DROP COLUMN [RequiresCooler];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260331025446_update shipmentOffer', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260402005506_seatlayout_v1', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [VehicleSeatLayouts] ADD [SeatLayoutName] nvarchar(max) NOT NULL DEFAULT N'';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260402005534_seatlayou_v2', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_Shipments_ShipmentOfferId] ON [Shipments];
GO

DECLARE @var63 sysname;
SELECT @var63 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'ShipmentOfferId');
IF @var63 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var63 + '];');
ALTER TABLE [Shipments] ALTER COLUMN [ShipmentOfferId] uniqueidentifier NULL;
GO

ALTER TABLE [Shipments] ADD [Description] nvarchar(500) NULL;
GO

ALTER TABLE [Shipments] ADD [DropoffLocationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

ALTER TABLE [Shipments] ADD [HandlingNote] nvarchar(500) NULL;
GO

ALTER TABLE [Shipments] ADD [ImageUrl] nvarchar(max) NULL;
GO

ALTER TABLE [Shipments] ADD [IsFragile] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Shipments] ADD [OfferId] uniqueidentifier NULL;
GO

ALTER TABLE [Shipments] ADD [PickupLocationId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

ALTER TABLE [Shipments] ADD [Volume] decimal(10,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Shipments] ADD [Weight] decimal(10,2) NOT NULL DEFAULT 0.0;
GO

CREATE INDEX [IX_Shipments_DropoffLocationId] ON [Shipments] ([DropoffLocationId]);
GO

CREATE INDEX [IX_Shipments_OfferId] ON [Shipments] ([OfferId]);
GO

CREATE INDEX [IX_Shipments_PickupLocationId] ON [Shipments] ([PickupLocationId]);
GO

CREATE UNIQUE INDEX [IX_Shipments_ShipmentOfferId] ON [Shipments] ([ShipmentOfferId]) WHERE [ShipmentOfferId] IS NOT NULL;
GO

ALTER TABLE [Shipments] ADD CONSTRAINT [CK_Shipment_Must_Have_Only_One_Offer_Source] CHECK ((OfferId IS NOT NULL AND ShipmentOfferId IS NULL) OR (OfferId IS NULL AND ShipmentOfferId IS NOT NULL));
GO

ALTER TABLE [Shipments] ADD CONSTRAINT [FK_Shipments_Locations_DropoffLocationId] FOREIGN KEY ([DropoffLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Shipments] ADD CONSTRAINT [FK_Shipments_Locations_PickupLocationId] FOREIGN KEY ([PickupLocationId]) REFERENCES [Locations] ([Id]) ON DELETE NO ACTION;
GO

ALTER TABLE [Shipments] ADD CONSTRAINT [FK_Shipments_Offers_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Offers] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260402050739_update_table_shipments', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Shipments] ADD [DeliveryDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

ALTER TABLE [ShipmentRequests] ADD [DeliveryDate] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260402082207_add_deliveryDate_shipment', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE UNIQUE INDEX [IX_CompanyVehicles_PlateNumber] ON [CompanyVehicles] ([PlateNumber]) WHERE [DeleteAt] IS NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260403113746_AddUniquePlateNumberForCompanyVehicle', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Shipments] DROP CONSTRAINT [FK_Shipments_Locations_DropoffLocationId];
GO

ALTER TABLE [Shipments] DROP CONSTRAINT [FK_Shipments_Locations_PickupLocationId];
GO

ALTER TABLE [Shipments] DROP CONSTRAINT [FK_Shipments_Offers_OfferId];
GO

DROP INDEX [IX_Shipments_DropoffLocationId] ON [Shipments];
GO

DROP INDEX [IX_Shipments_OfferId] ON [Shipments];
GO

DROP INDEX [IX_Shipments_PickupLocationId] ON [Shipments];
GO

ALTER TABLE [Shipments] DROP CONSTRAINT [CK_Shipment_Must_Have_Only_One_Offer_Source];
GO

DECLARE @var64 sysname;
SELECT @var64 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'DeliveryDate');
IF @var64 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var64 + '];');
ALTER TABLE [Shipments] DROP COLUMN [DeliveryDate];
GO

DECLARE @var65 sysname;
SELECT @var65 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'Description');
IF @var65 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var65 + '];');
ALTER TABLE [Shipments] DROP COLUMN [Description];
GO

DECLARE @var66 sysname;
SELECT @var66 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'DropoffLocationId');
IF @var66 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var66 + '];');
ALTER TABLE [Shipments] DROP COLUMN [DropoffLocationId];
GO

DECLARE @var67 sysname;
SELECT @var67 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'HandlingNote');
IF @var67 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var67 + '];');
ALTER TABLE [Shipments] DROP COLUMN [HandlingNote];
GO

DECLARE @var68 sysname;
SELECT @var68 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'ImageUrl');
IF @var68 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var68 + '];');
ALTER TABLE [Shipments] DROP COLUMN [ImageUrl];
GO

DECLARE @var69 sysname;
SELECT @var69 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'IsFragile');
IF @var69 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var69 + '];');
ALTER TABLE [Shipments] DROP COLUMN [IsFragile];
GO

DECLARE @var70 sysname;
SELECT @var70 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'OfferId');
IF @var70 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var70 + '];');
ALTER TABLE [Shipments] DROP COLUMN [OfferId];
GO

DECLARE @var71 sysname;
SELECT @var71 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'PickupLocationId');
IF @var71 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var71 + '];');
ALTER TABLE [Shipments] DROP COLUMN [PickupLocationId];
GO

DECLARE @var72 sysname;
SELECT @var72 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'Volume');
IF @var72 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var72 + '];');
ALTER TABLE [Shipments] DROP COLUMN [Volume];
GO

DECLARE @var73 sysname;
SELECT @var73 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Shipments]') AND [c].[name] = N'Weight');
IF @var73 IS NOT NULL EXEC(N'ALTER TABLE [Shipments] DROP CONSTRAINT [' + @var73 + '];');
ALTER TABLE [Shipments] DROP COLUMN [Weight];
GO

ALTER TABLE [ShipmentOffers] ADD [OfferId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

CREATE INDEX [IX_ShipmentOffers_OfferId] ON [ShipmentOffers] ([OfferId]);
GO

ALTER TABLE [ShipmentOffers] ADD CONSTRAINT [FK_ShipmentOffers_Offers_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Offers] ([Id]) ON DELETE CASCADE;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260405163427_update_table_shipmentoffer_shipment', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ShipmentOffers] DROP CONSTRAINT [FK_ShipmentOffers_Offers_OfferId];
GO

ALTER TABLE [ShipmentOffers] ADD CONSTRAINT [FK_ShipmentOffers_Offers_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Offers] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260406035717_update_table_shipmentOffer_PK', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var74 sysname;
SELECT @var74 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentOffers]') AND [c].[name] = N'OfferId');
IF @var74 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentOffers] DROP CONSTRAINT [' + @var74 + '];');
ALTER TABLE [ShipmentOffers] ALTER COLUMN [OfferId] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260406041023_update_table_shipmentOffer_PK2', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [BusShowtimes] DROP CONSTRAINT [FK_BusShowtimes_CompanyVehicles_CompanyVehicleId];
GO

ALTER TABLE [CompanyVehicles] DROP CONSTRAINT [PK_CompanyVehicles];
GO

DECLARE @var75 sysname;
SELECT @var75 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[CompanyVehicles]') AND [c].[name] = N'Id');
IF @var75 IS NOT NULL EXEC(N'ALTER TABLE [CompanyVehicles] DROP CONSTRAINT [' + @var75 + '];');
ALTER TABLE [CompanyVehicles] DROP COLUMN [Id];
GO

ALTER TABLE [CompanyVehicles] ADD CONSTRAINT [PK_CompanyVehicles] PRIMARY KEY ([CompanyVehicleId]);
GO

CREATE UNIQUE INDEX [IX_Seats_ShowtimeId_SeatNumber] ON [Seats] ([ShowtimeId], [SeatNumber]) WHERE [DeleteAt] IS NULL;
GO

ALTER TABLE [BusShowtimes] ADD CONSTRAINT [FK_BusShowtimes_CompanyVehicles_CompanyVehicleId] FOREIGN KEY ([CompanyVehicleId]) REFERENCES [CompanyVehicles] ([CompanyVehicleId]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260406175026_CompanyVehicle_UseBusinessKey', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP INDEX [IX_BusBookingSeats_SeatId] ON [BusBookingSeats];
GO

DROP INDEX [IX_BusBookingSeats_BookingId_SeatNumber] ON [BusBookingSeats];
DECLARE @var76 sysname;
SELECT @var76 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusBookingSeats]') AND [c].[name] = N'SeatNumber');
IF @var76 IS NOT NULL EXEC(N'ALTER TABLE [BusBookingSeats] DROP CONSTRAINT [' + @var76 + '];');
ALTER TABLE [BusBookingSeats] ALTER COLUMN [SeatNumber] nvarchar(10) NOT NULL;
CREATE UNIQUE INDEX [IX_BusBookingSeats_BookingId_SeatNumber] ON [BusBookingSeats] ([BookingId], [SeatNumber]);
GO

ALTER TABLE [BusBookings] ADD [ExpireAt] datetime2 NULL;
GO

CREATE UNIQUE INDEX [IX_BusBookingSeats_SeatId] ON [BusBookingSeats] ([SeatId]) WHERE [DeleteAt] IS NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260407113450_updatedbbooking', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Shipments] ADD [DeliveredDate] datetime2 NULL;
GO

ALTER TABLE [Shipments] ADD [Price] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260408094815_add_datetime_price_table_shipment', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260408095411_add_type_return_shipment', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [ShipmentOffers] DROP CONSTRAINT [FK_ShipmentOffers_Offers_OfferId];
GO

CREATE TABLE [Payments] (
    [Id] uniqueidentifier NOT NULL,
    [BookingId] uniqueidentifier NOT NULL,
    [Code] nvarchar(64) NOT NULL,
    [PaymentMethod] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Payments] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Payment_Amount_Positive] CHECK ([Amount] > 0)
);
GO

CREATE TABLE [Wallets] (
    [Id] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [Status] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Wallets] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Wallet_Balance_NonNegative] CHECK ([Balance] >= 0),
    CONSTRAINT [FK_Wallets_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Deposits] (
    [Id] uniqueidentifier NOT NULL,
    [WalletId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Code] nvarchar(64) NOT NULL,
    [Provider] int NOT NULL,
    [ProviderTransactionId] nvarchar(128) NULL,
    [Status] int NOT NULL,
    [ExpiredAt] datetime2 NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Deposits] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Deposit_Amount_Positive] CHECK ([Amount] > 0),
    CONSTRAINT [FK_Deposits_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [WalletTransactions] (
    [Id] uniqueidentifier NOT NULL,
    [WalletId] uniqueidentifier NOT NULL,
    [Type] int NOT NULL,
    [Status] int NOT NULL,
    [ReferenceId] uniqueidentifier NOT NULL,
    [ReferenceType] nvarchar(64) NULL,
    [Description] nvarchar(500) NULL,
    [Amount] decimal(18,2) NOT NULL,
    [BalanceBefore] decimal(18,2) NOT NULL,
    [BalanceAfter] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_WalletTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_WalletTransaction_Amount_Positive] CHECK ([Amount] > 0),
    CONSTRAINT [CK_WalletTransaction_BalanceAfter_NonNegative] CHECK ([BalanceAfter] >= 0),
    CONSTRAINT [FK_WalletTransactions_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Withdrawals] (
    [Id] uniqueidentifier NOT NULL,
    [WalletId] uniqueidentifier NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [BankAcc] nvarchar(32) NOT NULL,
    [BankName] nvarchar(100) NOT NULL,
    [AccName] nvarchar(100) NOT NULL,
    [Status] int NOT NULL,
    [Note] nvarchar(500) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Withdrawals] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Withdraw_Amount_Positive] CHECK ([Amount] > 0),
    CONSTRAINT [FK_Withdrawals_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Deposits_Code] ON [Deposits] ([Code]);
GO

CREATE UNIQUE INDEX [IX_Deposits_ProviderTransactionId] ON [Deposits] ([ProviderTransactionId]) WHERE [ProviderTransactionId] IS NOT NULL;
GO

CREATE INDEX [IX_Deposits_WalletId] ON [Deposits] ([WalletId]);
GO

CREATE INDEX [IX_Payments_BookingId] ON [Payments] ([BookingId]);
GO

CREATE UNIQUE INDEX [IX_Payments_Code] ON [Payments] ([Code]);
GO

CREATE UNIQUE INDEX [IX_Wallets_UserId] ON [Wallets] ([UserId]);
GO

CREATE INDEX [IX_WalletTransactions_ReferenceType_ReferenceId] ON [WalletTransactions] ([ReferenceType], [ReferenceId]);
GO

CREATE INDEX [IX_WalletTransactions_WalletId] ON [WalletTransactions] ([WalletId]);
GO

CREATE INDEX [IX_Withdrawals_WalletId] ON [Withdrawals] ([WalletId]);
GO

ALTER TABLE [ShipmentOffers] ADD CONSTRAINT [FK_ShipmentOffers_Offers_OfferId] FOREIGN KEY ([OfferId]) REFERENCES [Offers] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260408190849_AddWalletPaymentModule', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260420022857_update_table_driver', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260420024725_update_table_driver2', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var77 sysname;
SELECT @var77 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[ShipmentOffers]') AND [c].[name] = N'OfferId');
IF @var77 IS NOT NULL EXEC(N'ALTER TABLE [ShipmentOffers] DROP CONSTRAINT [' + @var77 + '];');
ALTER TABLE [ShipmentOffers] ALTER COLUMN [OfferId] uniqueidentifier NULL;
GO

CREATE TABLE [Reviews] (
    [Id] uniqueidentifier NOT NULL,
    [ReferenceId] uniqueidentifier NOT NULL,
    [ServiceType] int NOT NULL,
    [ReviewerId] uniqueidentifier NOT NULL,
    [RevieweeId] uniqueidentifier NOT NULL,
    [Rating] int NOT NULL,
    [Comment] nvarchar(1000) NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_Reviews] PRIMARY KEY ([Id]),
    CONSTRAINT [CK_Review_Rating_Range] CHECK ([Rating] >= 1 AND [Rating] <= 5),
    CONSTRAINT [FK_Reviews_Users_RevieweeId] FOREIGN KEY ([RevieweeId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Reviews_Users_ReviewerId] FOREIGN KEY ([ReviewerId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE UNIQUE INDEX [IX_Review_OnePerUserPerTrip] ON [Reviews] ([ReferenceId], [ServiceType], [ReviewerId]);
GO

CREATE INDEX [IX_Reviews_ReferenceId_ServiceType] ON [Reviews] ([ReferenceId], [ServiceType]);
GO

CREATE INDEX [IX_Reviews_RevieweeId] ON [Reviews] ([RevieweeId]);
GO

CREATE INDEX [IX_Reviews_ReviewerId] ON [Reviews] ([ReviewerId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260512104320_add_review', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Drivers] ADD [DriverRatingAverage] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Drivers] ADD [DriverRatingCount] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260512155412_update_driver_add_rating', N'8.0.23');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DROP TABLE [Old_CompanyDrivers];
GO

DECLARE @var78 sysname;
SELECT @var78 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OfferRoutePoints]') AND [c].[name] = N'DropoffAllowed');
IF @var78 IS NOT NULL EXEC(N'ALTER TABLE [OfferRoutePoints] DROP CONSTRAINT [' + @var78 + '];');
ALTER TABLE [OfferRoutePoints] DROP COLUMN [DropoffAllowed];
GO

DECLARE @var79 sysname;
SELECT @var79 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[OfferRoutePoints]') AND [c].[name] = N'PickupAllowed');
IF @var79 IS NOT NULL EXEC(N'ALTER TABLE [OfferRoutePoints] DROP CONSTRAINT [' + @var79 + '];');
ALTER TABLE [OfferRoutePoints] DROP COLUMN [PickupAllowed];
GO

DECLARE @var80 sysname;
SELECT @var80 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusRoutePoints]') AND [c].[name] = N'DropoffAllowed');
IF @var80 IS NOT NULL EXEC(N'ALTER TABLE [BusRoutePoints] DROP CONSTRAINT [' + @var80 + '];');
ALTER TABLE [BusRoutePoints] DROP COLUMN [DropoffAllowed];
GO

DECLARE @var81 sysname;
SELECT @var81 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[BusRoutePoints]') AND [c].[name] = N'PickupAllowed');
IF @var81 IS NOT NULL EXEC(N'ALTER TABLE [BusRoutePoints] DROP CONSTRAINT [' + @var81 + '];');
ALTER TABLE [BusRoutePoints] DROP COLUMN [PickupAllowed];
GO

ALTER TABLE [OfferRoutePoints] ADD [StopType] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Companies] ADD [RatingCompanyAverage] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

ALTER TABLE [Companies] ADD [RatingCompanyCount] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [BusShowtimes] ADD [CompanyDriverId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
GO

ALTER TABLE [BusRoutePoints] ADD [StopType] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [CompanyDrivers] (
    [Id] uniqueidentifier NOT NULL,
    [CompanyId] uniqueidentifier NOT NULL,
    [UserId] uniqueidentifier NOT NULL,
    [LicenseNumber] nvarchar(20) NOT NULL,
    [LicenseClass] nvarchar(10) NOT NULL,
    [DriverVerificationStatus] int NOT NULL,
    [LicenseDocumentUrl] nvarchar(500) NOT NULL,
    [VerifiedAt] datetime2 NULL,
    [VerifiedBy] uniqueidentifier NULL,
    [CreatedAt] datetime2 NOT NULL,
    [LastUpdateAt] datetime2 NULL,
    [DeleteAt] datetime2 NULL,
    CONSTRAINT [PK_CompanyDrivers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CompanyDrivers_Companies_CompanyId] FOREIGN KEY ([CompanyId]) REFERENCES [Companies] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_CompanyDrivers_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_BusShowtimes_CompanyDriverId] ON [BusShowtimes] ([CompanyDriverId]);
GO

CREATE INDEX [IX_CompanyDrivers_CompanyId] ON [CompanyDrivers] ([CompanyId]);
GO

CREATE INDEX [IX_CompanyDrivers_UserId] ON [CompanyDrivers] ([UserId]);
GO

ALTER TABLE [BusShowtimes] ADD CONSTRAINT [FK_BusShowtimes_CompanyDrivers_CompanyDriverId] FOREIGN KEY ([CompanyDriverId]) REFERENCES [CompanyDrivers] ([Id]) ON DELETE NO ACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260521092310_RouteStopTypeAndRouteRules', N'8.0.23');
GO

COMMIT;
GO

