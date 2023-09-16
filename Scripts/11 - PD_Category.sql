USE [ISyncContacts]
GO
CREATE PROC PD_Category
(
	@ID int
)
AS
BEGIN
	DELETE FROM Categories
	WHERE ID=@ID
END
