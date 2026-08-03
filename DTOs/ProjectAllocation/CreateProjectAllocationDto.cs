namespace spm_backend.DTOs.ProjectAllocation;

public class CreateProjectAllocationDto
{
    public int ProjectID { get; set; }
    
    public int StudentID { get; set; }
    
    public int FacultyID { get; set; }
    
    public DateTime AssignedDate { get; set; }
    
    public DateTime ProjectStartDate { get; set; }
    
    public DateTime ProjectEndDate { get; set; }
    
    public int TotalTasksGiven {get; set;}
    
    public int TotalCompletedTasks {get; set;}
    
    public decimal ProgressPercentage {get; set;}
    
    public string OverAllGrade { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}