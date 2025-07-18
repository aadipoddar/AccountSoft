﻿CREATE PROCEDURE [dbo].[Insert_Accounting]
	@Id INT OUTPUT,
	@CompanyId INT,
	@ReferenceNo VARCHAR(20),
	@VoucherId INT,
	@Remarks VARCHAR(500),
	@AccountingDate DATE,
	@FinancialYearId INT,
	@Status BIT
AS
BEGIN
	IF @Id = 0
	BEGIN
		INSERT INTO [dbo].[Accounting]
		(
			CompanyId,
			ReferenceNo,
			VoucherId,
			Remarks,
			AccountingDate,
			FinancialYearId,
			Status
		) VALUES
		(
			@CompanyId,
			@ReferenceNo,
			@VoucherId,
			@Remarks,
			@AccountingDate,
			@FinancialYearId,
			@Status
		)

		SET @Id = SCOPE_IDENTITY();
	END

	ELSE
	BEGIN
		UPDATE [dbo].[Accounting]
		SET
			CompanyId = @CompanyId,
			ReferenceNo = @ReferenceNo,
			VoucherId = @VoucherId,
			Remarks = @Remarks,
			AccountingDate = @AccountingDate,
			FinancialYearId = @FinancialYearId,
			Status = @Status
		WHERE Id = @Id
	END

	SELECT @Id AS Id
END