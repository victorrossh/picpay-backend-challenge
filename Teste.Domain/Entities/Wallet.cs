using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Teste.Domain.Entities;

[Table("tb_wallet")]
public sealed class Wallet
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required] 
    [Column("account_id")] 
    public Guid AccountId { get; set; }

    [Required] 
    [Column("balance", TypeName = "decimal(18,2)")] 
    public decimal Balance { get; set; }

    [Column("created_at")] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}