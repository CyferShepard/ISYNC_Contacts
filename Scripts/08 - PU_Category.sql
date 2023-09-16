USE [ISyncContacts]
GO
CREATE PROC PU_Category
(
	@ID int,
	@Name VARCHAR(50) = null,
	@Active bit = null
)
AS
BEGIN
	UPDATE Categories
	SET [Name] = ISNULL(@Name,[Name]),
		Active = ISNULL(@Active,Active)
	WHERE ID=@ID

END
