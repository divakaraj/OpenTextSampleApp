use OpenTextSampleDB
GO

create Table AuditRecord
(
	 UniqueID int Primary key identity(1,1)
	,DateTimeOfEntry datetime not null
	,SenderAddress nvarchar(50) not null
)
GO

create table RecipientRecord
(
	 UniqueChildID int Primary key identity(1,1)
	,RecipientAddress nvarchar(50)
	,UniqueID int FOREIGN KEY REFERENCES AuditRecord(UniqueID)
)
GO