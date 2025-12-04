CREATE PROCEDURE sp_GetAllGeneric
    @TableName NVARCHAR(128)
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @SQL NVARCHAR(MAX);
    
    IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @TableName)
    BEGIN
        RAISERROR('Invalid table name', 16, 1);
        RETURN;
    END
    
    SET @SQL = N'SELECT * FROM ' + QUOTENAME(@TableName);
    
    EXEC sp_executesql @SQL;
END
GO
