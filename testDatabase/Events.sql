CREATE TABLE [dbo].[Events]
(
	[Id] INT NOT NULL PRIMARY KEY, 
    [Name] NVARCHAR(MAX) NOT NULL, 
    [Description] NVARCHAR(MAX) NOT NULL, 
    [MaxParticipants] INT NOT NULL, 
    [Date] DATETIME NULL, 
    [Track] INT NOT NULL FOREIGN KEY ([Track]) REFERENCES [Tracks]([Id]), 
)
