/*
Plantilla de script posterior a la implementación							
--------------------------------------------------------------------------------------
 Este archivo contiene instrucciones de SQL que se anexarán al script de compilación.		
 Use la sintaxis de SQLCMD para incluir un archivo en el script posterior a la implementación.			
 Ejemplo:      :r .\miArchivo.sql								
 Use la sintaxis de SQLCMD para hacer referencia a una variable en el script posterior a la implementación.		
 Ejemplo:      :setvar TableName miTabla							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO Bikes VALUES ('YZF600R','Yamaha',20000)
INSERT INTO Bikes VALUES ('SuperDuke1290','KTM',20000)

INSERT INTO Tracks VALUES ('Brands Hatch','United Kingdom',0)
INSERT INTO Tracks VALUES ('Suzuka','Japan',0)

INSERT INTO [Events] VALUES ('Event 1','Event 1 description',30,GETDATE(),2)
INSERT INTO [Events] VALUES ('Event 2','Event 2 description',30,GETDATE(),1)