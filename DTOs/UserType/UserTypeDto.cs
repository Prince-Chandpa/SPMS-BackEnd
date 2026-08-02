namespace spm_backend.DTOs.UserType;

public class UserTypeDto
{
    public int UserTypeID { get; set; }
    
    public string UserTypeName { get; set; } = string.Empty;
    
    public string Description { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; }
}