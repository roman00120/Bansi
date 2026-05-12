USE BdiExamen;
GO

BEGIN TRY
    IF OBJECT_ID('dbo.tblExamen', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.tblExamen
        (
            Id INT NOT NULL PRIMARY KEY,
            Nombre VARCHAR(100) NOT NULL,
            Descripcion VARCHAR(255) NOT NULL
        );

        PRINT 'Tabla tblExamen creada correctamente';
    END
    ELSE
    BEGIN
        PRINT 'La tabla tblExamen ya existe';
    END
END TRY
BEGIN CATCH
    PRINT 'Error al crear la tabla tblExamen';
    PRINT ERROR_MESSAGE();
END CATCH
GO
