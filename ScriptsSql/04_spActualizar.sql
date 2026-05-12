USE BdiExamen;
GO

IF OBJECT_ID('dbo.spActualizar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spActualizar;
GO

CREATE PROCEDURE dbo.spActualizar
    @Id INT,
    @Nombre VARCHAR(100),
    @Descripcion VARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        IF @Id IS NULL OR @Id <= 0
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'El Id es obligatorio' AS Mensaje;
            RETURN;
        END

        IF @Nombre IS NULL OR LTRIM(RTRIM(@Nombre)) = ''
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'El nombre es obligatorio' AS Mensaje;
            RETURN;
        END

        IF @Descripcion IS NULL OR LTRIM(RTRIM(@Descripcion)) = ''
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'La descripcion es obligatoria' AS Mensaje;
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.tblExamen WHERE Id = @Id)
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'No se encontro el registro' AS Mensaje;
            RETURN;
        END

        UPDATE dbo.tblExamen
        SET
            Nombre = @Nombre,
            Descripcion = @Descripcion
        WHERE Id = @Id;

        SELECT CAST(1 AS bit) AS Ok, 'Registro actualizado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS bit) AS Ok, 'Error al actualizar el registro' AS Mensaje;
    END CATCH
END
GO
