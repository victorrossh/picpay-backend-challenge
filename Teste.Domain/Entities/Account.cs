using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Teste.Domain.Enums;

namespace Teste.Domain.Entities;

[Table("tb_account")]
public sealed class Account
{
    [Key] public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    [Column("name", TypeName = "nvarchar(255)")]
    public required string Name { get; set; }

    [Required]
    [Column("identity", TypeName = "nvarchar(50)")]
    public required string Identity { get; set; }

    [Required]
    [Column("email", TypeName = "nvarchar(255)")]
    public required string Email { get; set; }

    [Required]
    [Column("password", TypeName = "nvarchar(255)")]
    public required string Password { get; set; }


    [Required] [Column("created_at")] public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}