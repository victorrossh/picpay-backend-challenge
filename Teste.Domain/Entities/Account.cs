using System;
using System.ComponentModel.DataAnnotations.Schema;
using Teste.Domain.Enums;

namespace Teste.Domain.Entities;

[Table("tb_account")]
public class Account
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Identity { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public AccountRole Role { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
}