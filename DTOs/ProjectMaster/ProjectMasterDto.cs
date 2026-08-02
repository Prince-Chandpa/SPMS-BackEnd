namespace spm_backend.DTOs.ProjectMaster;

public class ProjectMasterDto
{
    public int ProjectMasterID { get; set; }
    
    public string ProjectTitle { get; set; } =  string.Empty;
    
    public string Description { get; set; } =  string.Empty;
    
    public bool IsActive { get; set; }
}