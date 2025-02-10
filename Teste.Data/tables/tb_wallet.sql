CREATE TABLE tb_wallet (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    account_id UNIQUEIDENTIFIER NOT NULL,
    balance DECIMAL(18, 2) NOT NULL DEFAULT 0.00 CHECK (balance >= 0),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_tb_wallet_account FOREIGN KEY (account_id) REFERENCES tb_account(id) ON DELETE CASCADE,
    CONSTRAINT ck_tb_wallet_balance CHECK (balance >= 0)
);