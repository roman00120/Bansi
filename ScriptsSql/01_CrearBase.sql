USE master;
GO

BEGIN TRY
    IF DB_ID('BdiExamen') IS NULL
    BEGIN
        CREATE DATABASE BdiExamen;
        PRINT 'Base de datos creada correctamente';
    END
    ELSE
    BEGIN
        PRINT 'La base de datos BdiExamen ya existe';
    END
END TRY
BEGIN CATCH
    PRINT 'Error al crear la base de datos BdiExamen';
    PRINT ERROR_MESSAGE();
END CATCH
GO
