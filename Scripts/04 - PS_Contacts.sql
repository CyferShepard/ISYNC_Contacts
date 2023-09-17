USE [ISyncContacts]
GO

CREATE PROC PS_Contacts
(
	@ID int = 0,
	@Active bit = null,
	@FirstName varchar(50) =null,
	@CategoryId int =null,
	@EMail varchar(200)=null
)
AS
BEGIN
	SELECT 
	co.ID
	,CategoryId
	,Name
	,FirstName
	,LastName
	,DateOfBirth
	,CellNumber
	,EMail
	,[Image]
	,DateCreated
	,co.Active
	FROM Contacts co
	JOIN Categories ca
	on co.CategoryId=ca.ID
	WHERE co.Active=ISNULL(@Active,co.Active)
	AND co.ID=IIF(@ID>0,@ID,co.ID)
	AND FirstName like '%'+ISNULL(@FirstName,FirstName)+'%'
	AND CategoryId = IIF(@CategoryId>1 AND @CategoryId IS NOT NULL,@CategoryId,CategoryId)
	AND LOWER(EMail) like '%'+ISNULL(@EMail,LOWER(EMail))+'%'


END