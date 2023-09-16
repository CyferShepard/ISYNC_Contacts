USE [ISyncContacts]
GO
CREATE PROC PI_Contact
(
	@CategoryId int,
	@FirstName VARCHAR(50),
	@LastName VARCHAR(50),
	@DateOfBirth Date = null,
	@CellNumber VARCHAR(13) = null,
	@EMail VARCHAR(200),
	@Image VARBINARY(MAX) = null
)
AS
BEGIN
	INSERT INTO Contacts
	(
		 CategoryId
		,FirstName
		,LastName
		,DateOfBirth
		,CellNumber
		,EMail
		,[Image]
	
	)
	SELECT
	@CategoryId,
	@FirstName,
	@LastName,
	@DateOfBirth,
	@CellNumber,
	@EMail,
	@Image

END
