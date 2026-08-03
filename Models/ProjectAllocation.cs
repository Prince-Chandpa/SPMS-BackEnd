using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spm_backend.Models;

public class ProjectAllocation
{
    [Key] 
    public int ProjectAllocationID { get; set; }
    
    [ForeignKey(nameof(ProjectMaster)), Required]
    public int ProjectID { get; set; }
    public ProjectMaster ProjectMaster { get; set; }
    
    [ForeignKey(nameof(UserStudent)), Required]
    public int StudentID { get; set; }
    public User UserStudent { get; set; }
    
    [ForeignKey(nameof(UserFaculty)), Required]
    public int FacultyID { get; set; }
    public User UserFaculty { get; set; }
    
    [Required]
    public DateTime AssignedDate { get; set; } = DateTime.Now;
    
    [Required]
    public DateTime ProjectStartDate { get; set; }
    
    [Required]
    public DateTime ProjectEndDate { get; set; }
    
    [Required]
    public int TotalTasksGiven {get; set;}
    
    [Required]
    public int TotalCompletedTasks {get; set;}
    
    [Required]
    public decimal ProgressPercentage {get; set;}

    [MaxLength(1)] 
    public string OverAllGrade { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;
    
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}