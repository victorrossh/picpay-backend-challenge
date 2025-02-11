CREATE PROCEDURE sp_transfer_funds
    @payer_id UNIQUEIDENTIFIER,
    @payee_id UNIQUEIDENTIFIER,
    @amount DECIMAL(18, 2),
    @transaction_status INT OUTPUT,
    @transaction_id UNIQUEIDENTIFIER OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @balance DECIMAL(18, 2);
    SET @transaction_id = NEWID();

    BEGIN TRANSACTION;

    BEGIN TRY
        SELECT @balance = balance
        FROM tb_wallet WITH (UPDLOCK, HOLDLOCK)
        WHERE id = @payer_id;

        IF @balance < @amount
        BEGIN
            INSERT INTO tb_transaction (id, payer_id, payee_id, amount, status, created_at)
            VALUES (@transaction_id, @payer_id, @payee_id, @amount, 2, GETDATE());

            SET @transaction_status = 2;
            ROLLBACK TRANSACTION;
            RETURN;
        END;

        INSERT INTO tb_transaction (id, payer_id, payee_id, amount, status, created_at)
        VALUES (@transaction_id, @payer_id, @payee_id, @amount, 0, GETDATE());

        UPDATE tb_wallet SET balance = balance - @amount WHERE id = @payer_id;
        UPDATE tb_wallet SET balance = balance + @amount WHERE id = @payee_id;

        UPDATE tb_transaction SET status = 1 WHERE id = @transaction_id;

        COMMIT TRANSACTION;
        SET @transaction_status = 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        UPDATE tb_transaction SET status = 2 WHERE id = @transaction_id;

        SET @transaction_status = 2;
    END CATCH;
END;
