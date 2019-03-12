CREATE TABLE [dbo].[Bikes]
(
	[BikeNumber] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Model] NVARCHAR(MAX) NOT NULL, 
    [Manufacturer] NVARCHAR(MAX) NOT NULL, 
    [Mileage] INT NOT NULL
)
