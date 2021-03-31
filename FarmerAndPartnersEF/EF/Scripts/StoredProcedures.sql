USE [FarmerAndPartnersCore]
GO

CREATE TYPE [dbo].[CompaniesType] AS TABLE
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [ContractStatusId] INT NOT NULL
)

CREATE TYPE [dbo].[UsersType] AS TABLE
(
    [Id] INT NOT NULL,
    [Name] NVARCHAR(50) NOT NULL,
    [Login] NVARCHAR(50) NOT NULL,
    [Password] NVARCHAR(50) NOT NULL,
    [CompanyId] INT NOT NULL
)

GO

CREATE PROCEDURE [dbo].[UsersMerge_proc]
(
    @UsersType [dbo].[UsersType] READONLY
)
AS
BEGIN
    MERGE INTO [Users] AS target
    USING @UsersType AS source
    ON target.[Id] = source.[Id]
    WHEN MATCHED THEN
       UPDATE
       SET target.[Name] = source.[Name],
           target.[Login] = source.[Login],
           target.[Password] = source.[Password],
           target.[CompanyId] = source.[CompanyId]
    WHEN NOT MATCHED BY target THEN
        INSERT ([Id], [Name], [Login], [Password], [CompanyId])
        VALUES (source.[Id], source.[Name], source.[Login], source.[Password], source.[CompanyId]);
END

GO

CREATE PROCEDURE [dbo].[CompaniesMerge_proc] 
(
    @CompaniesType [dbo].[CompaniesType] READONLY,
    @UsersType [dbo].[UsersType] READONLY
)
AS
BEGIN
    MERGE INTO [Companies] AS target
    USING @CompaniesType AS source
    ON target.[Id] = source.[Id]
    WHEN MATCHED THEN 
        UPDATE
        SET target.[Name] = source.[Name],
            target.[ContractStatusId] = source.[ContractStatusId]    
    WHEN NOT MATCHED BY target THEN
        INSERT ([Id], [Name], [ContractStatusId])
        VALUES (source.[Id], source.[Name], source.[ContractStatusId]);
        
    IF EXISTS (SELECT * FROM @UsersType)
        EXEC UsersMerge_proc @UsersType
END

GO

