-- ==========================================
-- Script para crear la base de datos ThemeParkDb
-- ==========================================

-- INSTRUCCIONES:
-- 1. Primero ejecuta SOLO las líneas 9-13 para crear la base de datos
-- 2. Luego selecciona ThemeParkDb en el dropdown de bases de datos de DBeaver
-- 3. Finalmente ejecuta el resto del script (desde la línea 18 en adelante)

-- PASO 1: Crear la base de datos
IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'ThemeParkDb')
BEGIN
    CREATE DATABASE ThemeParkDb;
END;

-- PASO 2: Después de crear la BD, selecciona ThemeParkDb en DBeaver y ejecuta desde aquí:

-- ==========================================
-- TABLAS BASE (sin dependencias)
-- ==========================================

-- Tabla Attractions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Attractions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Attractions (
        Nombre nvarchar(100) NOT NULL,
        Tipo int NOT NULL,
        EdadMinima int NOT NULL,
        CapacidadMaxima int NOT NULL,
        Descripcion nvarchar(500) NOT NULL,
        FechaCreacion datetime2 NOT NULL,
        FechaModificacion datetime2 NULL,
        AforoActual int NOT NULL,
        TieneIncidenciaActiva bit NOT NULL,
        Points int NOT NULL,
        CONSTRAINT PK_Attractions PRIMARY KEY (Nombre)
    );
END;

-- Tabla Configuraciones
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Configuraciones' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Configuraciones (
        Id uniqueidentifier NOT NULL,
        CONSTRAINT PK_Configuraciones PRIMARY KEY (Id)
    );
END;

-- Tabla Events
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Events' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Events (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(100) NOT NULL,
        Fecha datetime2 NOT NULL,
        Hora time NOT NULL,
        Aforo int NOT NULL,
        CostoAdicional decimal(18,2) NOT NULL,
        CONSTRAINT PK_Events PRIMARY KEY (Id)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Events_Name' AND object_id = OBJECT_ID('dbo.Events'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_Events_Name ON dbo.Events (Name ASC);
    END;
END;

-- Tabla Incidents
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Incidents' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Incidents (
        Id uniqueidentifier NOT NULL,
        AttractionName nvarchar(MAX) NOT NULL,
        Descripcion nvarchar(MAX) NOT NULL,
        FechaCreacion datetime2 NOT NULL,
        FechaResolucion datetime2 NULL,
        IsActive bit NOT NULL,
        MaintenanceId uniqueidentifier NULL,
        FechaProgramada datetime2 NULL,
        HoraProgramada time NULL,
        CONSTRAINT PK_Incidents PRIMARY KEY (Id)
    );
END;

-- Tabla Maintenances
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Maintenances' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Maintenances (
        Id uniqueidentifier NOT NULL,
        AttractionName nvarchar(200) NOT NULL,
        Fecha datetime2 NOT NULL,
        HoraInicio time NOT NULL,
        DuracionMinutos int NOT NULL,
        Descripcion nvarchar(500) NOT NULL,
        IncidentId uniqueidentifier NOT NULL,
        CONSTRAINT PK_Maintenances PRIMARY KEY (Id)
    );
END;

-- Tabla Rewards
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Rewards' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Rewards (
        Id int IDENTITY(1,1) NOT NULL,
        Nombre nvarchar(200) NOT NULL,
        Descripcion nvarchar(500) NOT NULL,
        CostoPuntos int NOT NULL,
        CantidadDisponible int NOT NULL,
        NivelMembresiaRequerido nvarchar(MAX) NULL,
        Activa bit NOT NULL,
        FechaCreacion datetime2 NOT NULL,
        CONSTRAINT PK_Rewards PRIMARY KEY (Id)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Rewards_Nombre' AND object_id = OBJECT_ID('dbo.Rewards'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_Rewards_Nombre ON dbo.Rewards (Nombre ASC);
    END;
END;

-- Tabla Sessions
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sessions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Sessions (
        Id uniqueidentifier NOT NULL,
        UserId uniqueidentifier NOT NULL,
        Token nvarchar(255) NOT NULL,
        ExpirationDate datetime2 NOT NULL,
        CreatedAt datetime2 NOT NULL,
        CONSTRAINT PK_Sessions PRIMARY KEY (Id)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Sessions_Token' AND object_id = OBJECT_ID('dbo.Sessions'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_Sessions_Token ON dbo.Sessions (Token ASC);
    END;
END;

-- Tabla SystemDateTimes
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'SystemDateTimes' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.SystemDateTimes (
        Id int IDENTITY(1,1) NOT NULL,
        CurrentDateTime datetime2 NULL,
        CONSTRAINT PK_SystemDateTimes PRIMARY KEY (Id)
    );
END;

-- Tabla Tickets
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Tickets' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Tickets (
        Id uniqueidentifier NOT NULL,
        CodigoQR uniqueidentifier NOT NULL,
        FechaVisita datetime2 NOT NULL,
        CodigoIdentificacionUsuario nvarchar(50) NOT NULL,
        FechaCompra datetime2 NOT NULL,
        TipoTicket nvarchar(8) NOT NULL,
        EventoId uniqueidentifier NULL,
        CONSTRAINT PK_Tickets PRIMARY KEY (Id)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Tickets_CodigoQR' AND object_id = OBJECT_ID('dbo.Tickets'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_Tickets_CodigoQR ON dbo.Tickets (CodigoQR ASC);
    END;
END;

-- Tabla Users
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Users (
        Id uniqueidentifier NOT NULL,
        Nombre nvarchar(100) NOT NULL,
        Email nvarchar(255) NOT NULL,
        Apellido nvarchar(100) NOT NULL,
        Contraseña nvarchar(255) NOT NULL,
        FechaNacimiento datetime2 NOT NULL,
        FechaRegistro datetime2 NOT NULL,
        CodigoIdentificacion nvarchar(MAX) NOT NULL,
        CONSTRAINT PK_Users PRIMARY KEY (Id)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email' AND object_id = OBJECT_ID('dbo.Users'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_Users_Email ON dbo.Users (Email ASC);
    END;
END;

-- Tabla __EFMigrationsHistory
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '__EFMigrationsHistory' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.[__EFMigrationsHistory] (
        MigrationId nvarchar(150) NOT NULL,
        ProductVersion nvarchar(32) NOT NULL,
        CONSTRAINT PK___EFMigrationsHistory PRIMARY KEY (MigrationId)
    );
END;

-- ==========================================
-- TABLAS DEPENDIENTES
-- ==========================================

-- Tabla ConfiguracionesPorAtraccion (depende de Configuraciones)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionesPorAtraccion' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ConfiguracionesPorAtraccion (
        Id uniqueidentifier NOT NULL,
        Valores text NOT NULL,
        CONSTRAINT PK_ConfiguracionesPorAtraccion PRIMARY KEY (Id),
        CONSTRAINT FK_ConfiguracionesPorAtraccion_Configuraciones_Id
            FOREIGN KEY (Id) REFERENCES dbo.Configuraciones(Id) ON DELETE CASCADE
    );
END;

-- Tabla ConfiguracionesPorCombo (depende de Configuraciones)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionesPorCombo' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ConfiguracionesPorCombo (
        Id uniqueidentifier NOT NULL,
        VentanaTemporalMinutos int NOT NULL,
        BonusMultiplicador int NOT NULL,
        MinimoAtracciones int NOT NULL,
        CONSTRAINT PK_ConfiguracionesPorCombo PRIMARY KEY (Id),
        CONSTRAINT FK_ConfiguracionesPorCombo_Configuraciones_Id
            FOREIGN KEY (Id) REFERENCES dbo.Configuraciones(Id) ON DELETE CASCADE
    );
END;

-- Tabla ConfiguracionesPorEvento (depende de Configuraciones)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ConfiguracionesPorEvento' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ConfiguracionesPorEvento (
        Id uniqueidentifier NOT NULL,
        Puntos int NOT NULL,
        Evento nvarchar(200) NOT NULL,
        CONSTRAINT PK_ConfiguracionesPorEvento PRIMARY KEY (Id),
        CONSTRAINT FK_ConfiguracionesPorEvento_Configuraciones_Id
            FOREIGN KEY (Id) REFERENCES dbo.Configuraciones(Id) ON DELETE CASCADE
    );
END;

-- Tabla EventAttractions (depende de Attractions y Events)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EventAttractions' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.EventAttractions (
        AtraccionesNombre nvarchar(100) NOT NULL,
        EventId uniqueidentifier NOT NULL,
        CONSTRAINT PK_EventAttractions PRIMARY KEY (AtraccionesNombre, EventId),
        CONSTRAINT FK_EventAttractions_Attractions_AtraccionesNombre
            FOREIGN KEY (AtraccionesNombre) REFERENCES dbo.Attractions(Nombre) ON DELETE CASCADE,
        CONSTRAINT FK_EventAttractions_Events_EventId
            FOREIGN KEY (EventId) REFERENCES dbo.Events(Id) ON DELETE CASCADE
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_EventAttractions_EventId' AND object_id = OBJECT_ID('dbo.EventAttractions'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_EventAttractions_EventId ON dbo.EventAttractions (EventId ASC);
    END;
END;

-- Tabla RewardExchanges (depende de Rewards y Users)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RewardExchanges' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.RewardExchanges (
        Id int IDENTITY(1,1) NOT NULL,
        RewardId int NOT NULL,
        UserId uniqueidentifier NOT NULL,
        PuntosDescontados int NOT NULL,
        PuntosRestantesUsuario int NOT NULL,
        FechaCanje datetime2 NOT NULL,
        Estado nvarchar(50) NOT NULL,
        CONSTRAINT PK_RewardExchanges PRIMARY KEY (Id)
    );
END;

-- Tabla Roles (depende de Users)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Roles (
        Id uniqueidentifier NOT NULL,
        UserId uniqueidentifier NOT NULL,
        Discriminator nvarchar(34) NOT NULL,
        NivelMembresia nvarchar(MAX) NULL,
        CONSTRAINT PK_Roles PRIMARY KEY (Id),
        CONSTRAINT FK_Roles_Users_UserId
            FOREIGN KEY (UserId) REFERENCES dbo.Users(Id) ON DELETE CASCADE
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Roles_UserId' AND object_id = OBJECT_ID('dbo.Roles'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Roles_UserId ON dbo.Roles (UserId ASC);
    END;
END;

-- Tabla ScoringStrategies (depende de Configuraciones)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ScoringStrategies' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.ScoringStrategies (
        Id uniqueidentifier NOT NULL,
        Name nvarchar(200) NOT NULL,
        Description nvarchar(500) NOT NULL,
        ConfiguracionId uniqueidentifier NULL,
        Active bit NOT NULL,
        [Type] nvarchar(MAX) NULL,
        PluginTypeIdentifier nvarchar(200) NULL,
        ConfigurationJson nvarchar(MAX) NULL,
        CreatedDate datetime2 NOT NULL,
        CONSTRAINT PK_ScoringStrategies PRIMARY KEY (Id),
        CONSTRAINT FK_ScoringStrategies_Configuraciones_ConfiguracionId
            FOREIGN KEY (ConfiguracionId) REFERENCES dbo.Configuraciones(Id)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ScoringStrategies_ConfiguracionId' AND object_id = OBJECT_ID('dbo.ScoringStrategies'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_ScoringStrategies_ConfiguracionId
            ON dbo.ScoringStrategies (ConfiguracionId ASC)
            WHERE [ConfiguracionId] IS NOT NULL;
    END;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_ScoringStrategies_Name' AND object_id = OBJECT_ID('dbo.ScoringStrategies'))
    BEGIN
        CREATE UNIQUE NONCLUSTERED INDEX IX_ScoringStrategies_Name ON dbo.ScoringStrategies (Name ASC);
    END;
END;

-- Tabla Visits (depende de Attractions)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Visits' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    CREATE TABLE dbo.Visits (
        Id uniqueidentifier NOT NULL,
        UserId uniqueidentifier NOT NULL,
        AttractionName nvarchar(200) NOT NULL,
        AttractionNombre nvarchar(100) NULL,
        EntryTime datetime2 NOT NULL,
        ExitTime datetime2 NULL,
        IsActive bit NOT NULL,
        Points int NOT NULL,
        ScoringStrategyName nvarchar(MAX) NULL,
        CONSTRAINT PK_Visits PRIMARY KEY (Id),
        CONSTRAINT FK_Visits_Attractions_AttractionNombre
            FOREIGN KEY (AttractionNombre) REFERENCES dbo.Attractions(Nombre)
    );

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Visits_AttractionNombre' AND object_id = OBJECT_ID('dbo.Visits'))
    BEGIN
        CREATE NONCLUSTERED INDEX IX_Visits_AttractionNombre ON dbo.Visits (AttractionNombre ASC);
    END;
END;
