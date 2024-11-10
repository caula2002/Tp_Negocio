USE [Tp_Negocio]
GO

/****** Objeto: Table [dbo].[Chimis] Fecha del script: 31/10/2024 21:38:32 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Chimis] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Nombre]       NVARCHAR (MAX) NULL,
    [Cantidad]     NVARCHAR (MAX) NULL,
    [Ingredientes] NVARCHAR (MAX) NULL,
    [NombreFoto]   NVARCHAR (MAX) NULL
);


