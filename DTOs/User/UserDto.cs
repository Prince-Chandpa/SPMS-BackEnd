namespace spm_backend.DTOs.User;

public class UserDto
{
    public int UserID { get; set; }
    
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string UserCode { get; set; } = string.Empty;

    public string MobileNumber { get; set; } = string.Empty;

    public string ProfilePicturePath { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public int UserTypeID { get; set; }
}