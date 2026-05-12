USE BdiExamen;
GO

IF OBJECT_ID('dbo.spAgregar', 'P') IS NOT NULL
    DROP PROCEDURE dbo.spAgregar;
GO

CREATE PROCEDURE dbo.spAgregar
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

        IF EXISTS (SELECT 1 FROM dbo.tblExamen WHERE Id = @Id)
        BEGIN
            SELECT CAST(0 AS bit) AS Ok, 'Ya existe un registro con ese Id' AS Mensaje;
            RETURN;
        END

        INSERT INTO dbo.tblExamen (Id, Nombre, Descripcion)
        VALUES (@Id, @Nombre, @Descripcion);

        SELECT CAST(1 AS bit) AS Ok, 'Registro agregado correctamente' AS Mensaje;
    END TRY
    BEGIN CATCH
        SELECT CAST(0 AS bit) AS Ok, 'Error al agregar el registro' AS Mensaje;
    END CATCH
END
GO
