CREATE TABLE tb_transaction (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    payer_id UNIQUEIDENTIFIER NOT NULL,
    payee_id UNIQUEIDENTIFIER NOT NULL,
    amount DECIMAL(18, 2) NOT NULL CHECK (amount > 0),
    status INT NOT NULL CHECK (status IN (0, 1, 2, 3)),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT fk_tb_transaction_payer FOREIGN KEY (payer_id) REFERENCES tb_wallet(id),
    CONSTRAINT fk_tb_transaction_payee FOREIGN KEY (payee_id) REFERENCES tb_wallet(id),
    CONSTRAINT ck_tb_transaction_amount CHECK (amount > 0)
);
