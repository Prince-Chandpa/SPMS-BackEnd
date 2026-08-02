using System.ComponentModel.DataAnnotations;

namespace spm_backend.Models;

public class ProjectMaster
{
    [Key] 
    public int ProjectMasterID { get; set; }
    
    [Required,MaxLength(200)] 
    public string ProjectTitle { get; set; } =  string.Empty;
    
    [MaxLength(Int32.MaxValue)]
    public string Description { get; set; } =  string.Empty;
}