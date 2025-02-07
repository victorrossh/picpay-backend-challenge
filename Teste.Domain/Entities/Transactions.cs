using System;
using System.ComponentModel.DataAnnotations.Schema;
using Teste.Domain.Enums;

namespace Teste.Domain.Entities;

[Table("tb_transaction")]
public class Transaction
{
    public Guid Id { get; } = Guid.NewGuid();
    public string PayerWalletId { get; set; }
    public string PayeeWalletId { get; set; }
    public double Amount { get; set; }
    public TransactionStatus Status { get; set; } = TransactionStatus.Pending;
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}