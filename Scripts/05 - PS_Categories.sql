USE [ISyncContacts]
GO
CREATE PROC PS_Categories

AS
BEGIN
	SELECT 
	ID
	,[Name]
	,Active
	FROM Categories

END