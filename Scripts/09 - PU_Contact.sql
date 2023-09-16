USE [ISyncContacts]
GO
CREATE PROC PU_Contact
(
	@ID int,
	@CategoryId int = null,
	@FirstName VARCHAR(50) = null,
	@LastName VARCHAR(50) = null,
	@DateOfBirth Date = null,
	@CellNumber VARCHAR(13) = null,
	@EMail VARCHAR(200) = null,
	@Image VARBINARY(MAX) = null,
	@Active bit = null
)
AS
BEGIN
	UPDATE Contacts
	SET
		CategoryId = ISNULL(@CategoryId,CategoryId),
		FirstName = ISNULL(@FirstName,FirstName),
		LastName = ISNULL(@LastName,LastName),
		DateOfBirth = ISNULL(@DateOfBirth,DateOfBirth),
		CellNumber = ISNULL(@CellNumber,CellNumber),
		EMail = ISNULL(@EMail,EMail),
		[Image] = ISNULL(@Image,[Image]),
		Active= ISNULL(@Active,Active)

	WHERE ID=@ID

END
