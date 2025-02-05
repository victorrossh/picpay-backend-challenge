using System;
using System.ComponentModel.DataAnnotations.Schema;
using Teste.Domain.Enums;

namespace Teste.Domain.Entities;

[Table("TB_Account")]
public class Account
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Identity { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public TypeAccount Type { get; set; }
    public DateTime CreatedAt { get; set; }
}