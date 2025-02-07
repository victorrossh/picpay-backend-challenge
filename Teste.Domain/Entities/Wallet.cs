using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Domain.Entities;

[Table("tb_wallet")]
public class Wallet
{
    public Guid Id { get; } = Guid.NewGuid();
    public Guid AccountId  { get; set; }
    public double Balance { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}