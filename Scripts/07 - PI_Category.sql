USE [ISyncContacts]
GO
CREATE PROC PI_Category
(
	@Name VARCHAR(50),
	@Active bit
)
AS
BEGIN
	INSERT INTO Categories
	(
		[Name],
		Active
	)
	SELECT
	@Name,
	@Active

END
