CREATE PROCEDURE [dbo].[Insert_Ledger]
	@Id INT,
	@Name VARCHAR(250),
	@Remarks VARCHAR(250),
	@GroupId INT,
	@AccountTypeId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO Ledger (Name, Remarks, GroupId, AccountTypeId, Status)
		VALUES (@Name, @Remarks, @GroupId, @AccountTypeId, @Status);
	END

	ELSE
	BEGIN
		UPDATE Ledger
		SET Name = @Name,
			Remarks = @Remarks,
			GroupId = @GroupId,
			AccountTypeId = @AccountTypeId,
			Status = @Status
		WHERE Id = @Id;
	END
END