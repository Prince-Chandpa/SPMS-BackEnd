using System.ComponentModel.DataAnnotations;

namespace spm_backend.DTOs.User;

public class UpdateUserDto
{
    [Required]
    public int UserTypeID { get; set; }
    
    [Required]
    [MaxLength(150)]
    public string FullName { get; set; } = string.Empty;
    
    [MaxLength(100)]
    public string UserCode { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;

    [Required]
    [Phone]
    public string MobileNumber { get; set; } = string.Empty;

    public string ProfilePicturePath { get; set; } = string.Empty;

    public bool IsActive { get; set; }
}