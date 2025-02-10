CREATE TABLE tb_account (
    id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    name VARCHAR(255) NOT NULL,
    [identity] VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL,
    password TEXT NOT NULL,
    role INT NOT NULL CHECK (role IN (0, 1)),
    created_at DATETIME NOT NULL DEFAULT GETDATE(),
    CONSTRAINT uq_tb_account_identity UNIQUE ([identity]),
    CONSTRAINT uq_tb_account_email UNIQUE (email)
);