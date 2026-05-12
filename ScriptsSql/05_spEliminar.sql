USE BdiExamen;
GO

IF OBJECT_ID('dbo.spEliminar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spEliminar;
GO

CREATE PROCEDURE dbo.spEliminar
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id IS NULL OR @Id <= 0
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'El Id es obligatorio' AS Mensaje;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.tblExamen WHERE Id = @Id)
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'No se encontro el registro' AS Mensaje;
            RETURN;
        END

        DELETE FROM dbo.tblExamen
        WHERE Id = @Id;

        SELECT CAST(1 AS bit) AS Ok, 'Registro eliminado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS bit) AS Ok, 'Error al eliminar el registro' AS Mensaje;
    END CATCH
END
GO
