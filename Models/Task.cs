using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace spm_backend.Models;

public class Task
{
    [Key]
    public int TaskID {get; set;}
    
    [ForeignKey(nameof(ProjectAllocation)), Required]
    public int ProjectAllocationID { get; set; }
    public ProjectAllocation ProjectAllocation { get; set; }
    
    [Required,MaxLength(200)]
    public string TaskTitle { get; set; } = string.Empty;
    
    [MaxLength(250)]
    public string TaskDescription { get; set; } = string.Empty;
    
    [ForeignKey(nameof(TaskStatus)), Required]
    public int TaskStatusID { get; set; }
    public TaskStatus TaskStatus { get; set; }
    
    [ForeignKey(nameof(TaskPriority)), Required]
    public int TaskPriorityID { get; set; }
    public TaskPriority TaskPriority { get; set; }
    
    [Required]
    public decimal AssignedScore { get; set; }
    
    public decimal? EarnedScore { get; set; }
    
    [Required]
    public decimal ProgressPercentage {get; set;}
    
    [Required]
    public DateTime TaskAssignedDate { get; set; }
    
    public DateTime? TaskStartDate { get; set; }
    public DateTime? TaskDueDate { get; set; }
    public DateTime? TaskCompletedDate { get; set; }
    public DateTime? NextFollowUpDate { get; set; }
    
    [MaxLength(500)]
    public string FacultyRemarks {get; set;} = string.Empty;
    
    [MaxLength(500)]
    public string StudentRemarks {get; set;} = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public bool IsDeleted { get; set; } = false;
}