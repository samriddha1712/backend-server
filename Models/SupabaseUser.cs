using Postgrest.Attributes;
using Postgrest.Models;

namespace AISmartSheet.API.Models;

[Table("users")]
public class SupabaseUser : BaseModel
{
    [PrimaryKey("id")]
    public Guid Id { get; set; }

    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Column("full_name")]
    public string FullName { get; set; } = string.Empty;

    [Column("password_hash")]
    public string? PasswordHash { get; set; }

    [Column("role")]
    public string Role { get; set; } = "developer";

    [Column("is_active")]
    public bool IsActive { get; set; } = true;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }
}
