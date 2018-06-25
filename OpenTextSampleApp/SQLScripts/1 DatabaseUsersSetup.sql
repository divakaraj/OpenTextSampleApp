
USE [master]
GO
create database OpenTextSampleDB
GO
CREATE LOGIN OpenTextSampleUser WITH PASSWORD=N'password-1', DEFAULT_DATABASE=[master], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO
CREATE USER OpenTextSampleUser FOR LOGIN OpenTextSampleUser
GO

use OpenTextSampleDB
CREATE USER OpenTextSampleUser FOR LOGIN OpenTextSampleUser
go
Grant connect,execute to OpenTextSampleUser
GO
EXEC sp_addrolemember N'db_datareader', N'OpenTextSampleUser'
GO
EXEC sp_addrolemember N'db_datawriter', N'OpenTextSampleUser'
GO