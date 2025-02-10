CREATE PROCEDURE sp_transfer_funds
    @payer_id UNIQUEIDENTIFIER,
    @payee_id UNIQUEIDENTIFIER,
    @amount DECIMAL(18, 2),
    @transaction_status INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @balance DECIMAL(18, 2);
    DECLARE @transaction_id UNIQUEIDENTIFIER;

    SET @transaction_id = NEWID();

    BEGIN TRY
        -- Verificar o saldo do pagador antes de registrar a transação
        SELECT @balance = balance
        FROM tb_wallet
        WHERE id = @payer_id;

        -- Caso o saldo seja insuficiente, registre a transação com status 2 (falha)
        IF @balance < @amount
        BEGIN
            INSERT INTO tb_transaction (id, payer_id, payee_id, amount, status, created_at)
            VALUES (@transaction_id, @payer_id, @payee_id, @amount, 2, GETDATE());

            SET @transaction_status = 2; -- Falha por saldo insuficiente
            RETURN;
        END

        -- Inserir a transação com status 0 (pendente) se o saldo for suficiente
        INSERT INTO tb_transaction (id, payer_id, payee_id, amount, status, created_at)
        VALUES (@transaction_id, @payer_id, @payee_id, @amount, 0, GETDATE());

        IF @@TRANCOUNT = 0 BEGIN TRANSACTION;

        -- Realizar a transferência
        UPDATE tb_wallet SET balance = balance - @amount WHERE id = @payer_id;
        UPDATE tb_wallet SET balance = balance + @amount WHERE id = @payee_id;

        -- Atualizar o status da transação para 1 (sucesso)
        UPDATE tb_transaction SET status = 1 WHERE id = @transaction_id;

        IF @@TRANCOUNT > 0 COMMIT TRANSACTION;

        SET @transaction_status = 1; -- Sucesso
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRANSACTION;

        -- Atualizar a transação com status 2 (falha) em caso de erro
        UPDATE tb_transaction SET status = 2 WHERE id = @transaction_id;

        SET @transaction_status = 2; -- Falha
    END CATCH;
END;
