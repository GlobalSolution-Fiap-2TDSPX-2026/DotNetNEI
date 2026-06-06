using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace NEI;

[Table("DB_USERS")]
[Index(nameof(Email), IsUnique = true)]
[Index(nameof(Username), IsUnique = true)]
public class User
{

    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Required]
    [MaxLength(80)]
    [Column("USERNAME")]
    public string Username { get; set; }

    [Required]
    [MaxLength(150)]
    [Column("EMAIL")]
    public string Email { get; set; }

    [Required]
    [Column("ROLE")]
    public Role Role { get; set; }

    public void Update(string username, string email, Role role)
    {
        this.Username = username;
        this.Email = email;
        this.Role = role;
    }
}
