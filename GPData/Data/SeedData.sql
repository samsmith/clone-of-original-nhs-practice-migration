/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

IF NOT EXISTS (SELECT * FROM [dbo].[EntityType])
BEGIN
	-- Disable constraints for all tables:
	EXEC sp_msforeachtable 'ALTER TABLE ? NOCHECK CONSTRAINT all'

	INSERT [dbo].[EntityType] ([EntityType], [EntityName]) VALUES (1,N'Organization')
	INSERT [dbo].[EntityType] ([EntityType], [EntityName]) VALUES (2,N'Location')
	INSERT [dbo].[EntityType] ([EntityType], [EntityName]) VALUES (3,N'Patient')
	INSERT [dbo].[EntityType] ([EntityType], [EntityName]) VALUES (4,N'Practitioner')
	
	-- Re-enable constraints for all tables:
	EXEC sp_msforeachtable 'ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all';	

END