using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace spm_backend.Models;

public class User
{
    [Key]
    public int UserID { get; set; }
    
    [ForeignKey("UserTypeID")]
    public int UserTypeID { get; set; }
    
    [JsonIgnore]
    public UserType? UserType { get; set; }
    
    [Required,MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string UserCode { get; set; } = string.Empty;
    
    [Required,MaxLength(150)]
    public string Email { get; set; } = string.Empty;
    
    [Required,StringLength(255)]
    public string Password { get; set; } = string.Empty;
    
    [Required,Phone,MaxLength(15)]
    public string MobileNumber { get; set; } = string.Empty;
    
    [MaxLength(500)]
    public string ProfilePicturePath { get; set; } = string.Empty;
    
    [Required]
    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; } = false;
    
    // public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}