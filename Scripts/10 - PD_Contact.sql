USE [ISyncContacts]
GO
CREATE PROC PD_Contact
(
	@ID int
)
AS
BEGIN
	DELETE FROM Contacts
	WHERE ID=@ID
END
