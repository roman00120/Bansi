USE BdiExamen;
GO

IF OBJECT_ID('dbo.spConsultar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spConsultar;
GO

CREATE PROCEDURE dbo.spConsultar
    @Id INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id IS NOT NULL AND @Id <= 0
        BEGIN
            SELECT
                CAST(0 AS bit) AS Ok,
                'El Id no es valido' AS Mensaje,
                CAST(NULL AS INT) AS Id,
                CAST(NULL AS VARCHAR(100)) AS Nombre,
                CAST(NULL AS VARCHAR(255)) AS Descripcion;
            RETURN;
        END

        IF @Id IS NULL
        BEGIN
            IF EXISTS (SELECT 1 FROM dbo.tblExamen)
            BEGIN
                SELECT
                    CAST(1 AS bit) AS Ok,
                    'Consulta realizada correctamente' AS Mensaje,
                    Id,
                    Nombre,
                    Descripcion
                FROM dbo.tblExamen
                ORDER BY Id;
            END
            ELSE
            BEGIN
                SELECT
                    CAST(1 AS bit) AS Ok,
                    'No hay registros' AS Mensaje,
                    CAST(NULL AS INT) AS Id,
                    CAST(NULL AS VARCHAR(100)) AS Nombre,
                    CAST(NULL AS VARCHAR(255)) AS Descripcion;
            END

            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.tblExamen WHERE Id = @Id)
        BEGIN
            SELECT
                CAST(0 AS bit) AS Ok,
                'No se encontro el registro' AS Mensaje,
                CAST(NULL AS INT) AS Id,
                CAST(NULL AS VARCHAR(100)) AS Nombre,
                CAST(NULL AS VARCHAR(255)) AS Descripcion;
            RETURN;
        END

        SELECT
            CAST(1 AS bit) AS Ok,
            'Consulta realizada correctamente' AS Mensaje,
            Id,
            Nombre,
            Descripcion
        FROM dbo.tblExamen
        WHERE Id = @Id;
    END TRY
    BEGIN CATCH
        SELECT
            CAST(0 AS bit) AS Ok,
            'Error al consultar los registros' AS Mensaje,
            CAST(NULL AS INT) AS Id,
            CAST(NULL AS VARCHAR(100)) AS Nombre,
            CAST(NULL AS VARCHAR(255)) AS Descripcion;
    END CATCH
END
GO
