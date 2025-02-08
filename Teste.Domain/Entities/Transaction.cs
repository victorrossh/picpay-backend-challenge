using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teste.Domain.Enums;

namespace Teste.Domain.Entities;

[Table("tb_transaction")]
public sealed class Transaction
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required] [Column("payer_id")] public Guid PayerId { get; set; }

    [Required] [Column("payee_id")] public Guid PayeeId { get; set; }

    [Required]
    [Column("amount", TypeName = "decimal(18,2)")]
    public decimal Amount { get; set; }

    [Required] [Column("status")] public Status Status { get; set; }

    [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}