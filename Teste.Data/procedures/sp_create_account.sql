CREATE PROCEDURE sp_create_account
    @name VARCHAR(255),
    @identity VARCHAR(255),
    @email VARCHAR(255),
    @password TEXT,
    @role INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @account_id UNIQUEIDENTIFIER;
    DECLARE @initial_balance DECIMAL(18,2);

    BEGIN TRY
        BEGIN TRANSACTION;

        IF EXISTS (SELECT 1 FROM tb_account WHERE email = @email OR [identity] = @identity)
        BEGIN
            THROW 50000, 'Error: Email or identity already registered.', 1;
        END;

        SET @initial_balance = CASE WHEN @role = 0 THEN 10.00
                                    ELSE 0.00
                                END;

        SET @account_id = NEWID();
        INSERT INTO tb_account (id, name, [identity], email, password, created_at)
        VALUES (@account_id, @name, @identity, @email, @password, GETDATE());

        INSERT INTO tb_wallet (id, account_id, role, balance, created_at)
        VALUES (NEWID(), @account_id, @role, @initial_balance, GETDATE());

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;

        THROW;
    END CATCH
END;
