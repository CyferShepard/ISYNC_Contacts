USE [ISyncContacts]


CREATE TABLE [dbo].[Contacts](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CategoryId] [int] NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[DateOfBirth] [date] NULL,
	[CellNumber] [varchar](13) NULL,
	[EMail] [varchar](200)UNIQUE NOT NULL ,
	[Image] [varbinary](max) NULL,
	[DateCreated] [datetime] NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_Contacts] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Contacts] ADD  CONSTRAINT [FK_Contacts_Category]   FOREIGN KEY ([CategoryId]) REFERENCES [dbo].[Categories]([ID])
GO

ALTER TABLE [dbo].[Contacts] ADD  CONSTRAINT [DF_Contacts_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]
GO

ALTER TABLE [dbo].[Contacts] ADD  CONSTRAINT [DF_Contacts_Active]  DEFAULT ((1)) FOR [Active]
GO

ALTER TABLE [dbo].[Contacts] ADD  CONSTRAINT [DF_Contacts_Category]  DEFAULT ((1)) FOR [CategoryId]
GO


