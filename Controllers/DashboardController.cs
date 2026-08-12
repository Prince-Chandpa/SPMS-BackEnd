using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using spm_backend.Data;

namespace spm_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }
        
        #region 1) Display the total number of students registered in the system
        [HttpGet("TotalStudent")]
        public async Task<IActionResult> GetTotalStudent()
        {
            var result = await _context.Users.CountAsync(u => u.UserType.UserTypeName == "Student");
            return Ok(result);
        }
        #endregion
        
        #region 2) Display the total number of faculty members guiding projects.
        [HttpGet("TotalFacultyMember")]
        public async Task<IActionResult> GetTotalFacultyMember()
        {
            var result = await _context.Users.CountAsync(u => u.UserType.UserTypeName == "Faculty");
            return Ok(result);
        }
        #endregion

        #region 3) Display the total number of projects available in the system.
        [HttpGet("TotalProjectAvailable")]
        public async Task<IActionResult> GetTotalProjectAvailable()
        {
            var result = await _context.ProjectMasters.CountAsync();
            return Ok(result);
        }
        #endregion

        #region 4) Show how many tasks belong to each status category.
        [HttpGet("TotalTaskStatus")]
        public async Task<IActionResult> GetTaskStatusCategories()
        { 
            var result = await _context.Tasks.GroupBy(t => t.TaskStatus.TaskStatusName)
                .Select(g => new
                {
                    Status = g.Key,
                    TotalTasks = g.Count()
                }).ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 5) Show priority wise task count
        [HttpGet("PriorityWiseTaskCount")]
        public async Task<IActionResult> GetPriorityWiseTaskCount()
        {
            var result = await _context.Tasks.GroupBy(t => t.TaskPriority.TaskPriorityName)
                .Select(g => new
                {
                    Priority = g.Key,
                    TotalTasks = g.Count()
                }).ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 6) Show how many projects are assigned to each faculty member.
        [HttpGet("TotalProjectAssigned")]
        public async Task<IActionResult> GetTotalProjectAssigned()
        {
            var result = await _context.ProjectAllocations.GroupBy(f => f.UserFaculty.FullName)
                .Select(g => new
                {
                    Faculty = g.Key,
                    Projects = g.Count()
                }).ToListAsync();

            return Ok(result);
        }
        #endregion
        
        #region 7) Show how many tasks have been assigned to each student.
        [HttpGet("TotalTaskAssignedToStudent")]
        public async Task<IActionResult> GetTotalTaskAssignedToStudent()
        {
            var result = await _context.Tasks.GroupBy(s => s.ProjectAllocation.UserStudent.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    Tasks = g.Count()
                }).ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 8) Display the top 10 students having the highest average earned score.
        [HttpGet("HighestAverageScore")]
        public async Task<IActionResult> GetHighestAverageScore()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.UserStudent.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    AverageScore = g.Average(t => t.EarnedScore)
                })
                .OrderByDescending(score => score.AverageScore)
                .Take(10)
                .ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 9) Display the bottom 10 students based on average earned score.
        [HttpGet("LowestAverageScore")]
        public async Task<IActionResult> GetLowestAverageScore()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.UserStudent.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    AverageScore = g.Average(t => t.EarnedScore)
                })
                .OrderBy(score => score.AverageScore)
                .Take(10)
                .ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 10) Display all tasks whose due date has passed but are not completed.
        [HttpGet("OverDueTasks")]
        public async Task<IActionResult> GetOverDueTasks()
        {
            var result = await _context.Tasks.Where(t => t.TaskDueDate < DateTime.Now && t.TaskStatus.TaskStatusName != "Completed")
                .Select(t => new
                {
                    t.TaskTitle,
                    Student = t.ProjectAllocation.UserStudent.FullName,
                    Faculty = t.ProjectAllocation.UserFaculty.FullName,
                    t.TaskDueDate
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 11) Display tasks having follow-up dates within the next 7 days.
        [HttpGet("UpcomingFollowUpTasks")]
        public async Task<IActionResult> GetUpcomingFollowUpTasks()
        {
            var result = await _context.Tasks
                .Where(t => t.NextFollowUpDate >= DateTime.Today && t.NextFollowUpDate <= DateTime.Today.AddDays(7))
                .Select(t => new
                {
                    t.TaskTitle,
                    Student = t.ProjectAllocation.UserStudent.FullName,
                    Faculty = t.ProjectAllocation.UserFaculty.FullName,
                    t.NextFollowUpDate
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 12) Show how many students have obtained each grade.
        [HttpGet("TotalStudentByGrade")]
        public async Task<IActionResult> GetTotalStudentByGrade()
        {
            var result = await _context.ProjectAllocations.GroupBy(p => p.OverAllGrade)
                .Select(g => new
                {
                    Grade = g.Key,
                    Students = g.Count()
                }).ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 13) Show month-wise completed task count.
        [HttpGet("TotalMonthWiseCompletedTask")]
        public async Task<IActionResult> GetTotalMonthWiseCompletedTask()
        {
            var result = await _context.Tasks
                .Where(t => t.TaskCompletedDate != null).GroupBy(t => new
            {
                Year = t.TaskCompletedDate.Value.Year,
                Month = t.TaskCompletedDate.Value.Month
            })
            .Select(g => new {
                Year = g.Key.Year,
                Month = g.Key.Month,
                TotalCompletedTasks = g.Count()
            })
            .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 14) Display Role Wise Active User Count.
        [HttpGet("TotalRoleWiseActiveUser")]
        public async Task<IActionResult> GetTotalRoleWiseActiveUser()
        {
            var result = await _context.UserRoles.Where(u => u.User.IsActive)
                .GroupBy(r => r.Role.RoleName)
                .Select(g => new
                {
                    Role = g.Key,
                    ActiveUser = g.Count()
                })
                .OrderByDescending(g => g.ActiveUser)
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 15) Display each role with users assigned to it.
        [HttpGet("RoleByUsers")]
        public async Task<IActionResult> GetRoleByUsers()
        {
            var result = await _context.UserRoles.GroupBy(u => u.Role.RoleName)
                .Select(g => new
                {
                    RoleName = g.Key,
                    UserName = g.Select(u => u.User.FullName).ToList()
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion
        
        #region 16) List Roles Having More Than 10 Users.
        [HttpGet("RoleWithUsers")]
        public async Task<IActionResult> GetRoleWithUsers()
        {
            var result = await _context.UserRoles.GroupBy(u => u.Role.RoleName)
                .Select(g => new
                {
                    RoleName = g.Key,
                    TotalUsers = g.Count()
                })
                .Where(g => g.TotalUsers > 10)
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 17) Display role statistics.
        [HttpGet("RoleStatistics")]
        public async Task<IActionResult> GetRoleStatistics()
        {
            var result = await _context.UserRoles.GroupBy(u => u.Role.RoleName)
                .Select(g => new
                {
                    RoleName = g.Key,
                    TotalUsers = g.Count(),
                    ActiveUsers = g.Count(u => u.User.IsActive),
                    InactiveUsers = g.Count(u => !u.User.IsActive)
                })
                .OrderByDescending(g => g.TotalUsers)
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 18) Show tasks due within next 7 days.
        [HttpGet("DueSoonTasks")]
        public async Task<IActionResult> GetDueSoonTasks()
        {
            var result = await _context.Tasks.Where(t => t.TaskDueDate >= DateTime.Now && t.TaskDueDate <= DateTime.Today.AddDays(7))
                .Select(t => new
                {
                    t.TaskID,
                    t.TaskTitle,
                    t.TaskDueDate,
                    Student = t.ProjectAllocation.UserStudent.FullName,
                    RemainingDays = EF.Functions.DateDiffDay(DateTime.Today, t.TaskDueDate)
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 19) Display each project with total tasks, completed tasks, pending tasks, and average task progress.
        [HttpGet("TasksSummary")]
        public async Task<IActionResult> GetTasksSummary()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.ProjectMaster.ProjectTitle)
                .Select(g => new
                {
                    Project = g.Key,
                    TotalTask = g.Count(),
                    CompletedTask = g.Count(t => t.TaskStatus.TaskStatusName == "Completed"),
                    PendingTask = g.Count(t => t.TaskStatus.TaskStatusName == "Pending"),
                    AverageProgress = g.Average(t => t.ProgressPercentage)
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 20) Display project-wise total assigned score, earned score, and score percentage.
        [HttpGet("ProjectWiseScore")]
        public async Task<IActionResult> GetProjectWiseScore()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.ProjectMaster.ProjectTitle)
                .Select(g => new
                {
                    Project = g.Key,
                    TotalAssignedScore = g.Sum(t => t.AssignedScore),
                    TotalEarnedScore = g.Sum(t => t.EarnedScore),
                    ScorePercentage = g.Average(t => t.ProgressPercentage)
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 21) Display Top 10 projects based on average earned score.
        [HttpGet("Top10Projects")]
        public async Task<IActionResult> GetTop10Projects()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.ProjectMaster.ProjectTitle)
                .Select(g => new
                {
                    Project = g.Key,
                    AverageScore = g.Average(t => t.AssignedScore),
                })
                .OrderByDescending(g => g.AverageScore)
                .Take(10)
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 22) Show project count, task count, and average progress for each faculty.
        [HttpGet("FacultyProjectSummary")]
        public async Task<IActionResult> GetFacultyProjectSummary()
        {
            var result = await _context.ProjectAllocations.GroupBy(p => p.UserFaculty.FullName)
                .Select(g => new
                {
                    Faculty = g.Key,
                    TotalProjects = g.Count(),
                    TatalTasks = g.Sum(t => t.TotalTasksGiven),
                    AverageProgress = g.Average(t => t.ProgressPercentage)
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 23) Display task completion statistics and average score for each student.
        [HttpGet("TaskCompletionStatistics")]
        public async Task<IActionResult> GetTaskCompletionStatistics()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.UserStudent.FullName)
                .Select(g => new
                {
                    Student = g.Key,
                    TotalTasks = g.Count(),
                    CompletedTasks = g.Count(t => t.TaskStatus.TaskStatusName == "Completed"),
                    PendingTasks = g.Count(t => t.TaskStatus.TaskStatusName == "Pending"),
                    AverageScore = g.Average(t => t.EarnedScore)
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 24) Display projects whose expected completion date has passed but are still incomplete.
        [HttpGet("OverDueProjects")]
        public async Task<IActionResult> GetOverDueProjects()
        {
            var result = await _context.ProjectAllocations
                .Where(p => p.ProjectEndDate < DateTime.Now && p.ProgressPercentage < 100)
                .Select(p => new
                {
                    Project = p.ProjectMaster.ProjectTitle,
                    Student = p.UserStudent.FullName,
                    Faculty = p.UserFaculty.FullName,
                    EndDate = p.ProjectEndDate,
                    Progress = p.ProgressPercentage,
                }).ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        // #region 25) Show month-wise completed task count.
        // [HttpGet("MonthWiseCompletedTasks")]
        // public async Task<IActionResult> GetMonthWiseCompletedTasks()
        // {
        //     var result = await _context.Tasks.GroupBy(t => new
        //         {
        //             Year = t.TaskCompletedDate.Value.Year,
        //             Month = t.TaskCompletedDate.Value.Month
        //         })
        //         .Select(g => new
        //         {
        //             Year = g.Key.Year,
        //             Month = g.Key.Month,
        //             CompletedTasks = g.Count(t => t.TaskStatus.TaskStatusName == "Completed"),
        //         })
        //         .OrderBy(g => g.Year)
        //         .ThenBy(g => g.Month)
        //         .ToListAsync();
        //     return Ok(result);
        // }
        // #endregion 
        
        #region 26) Rank faculties based on average project progress.
        [HttpGet("RankFaculties")]
        public async Task<IActionResult> GetRankFaculties()
        {
            var result = await _context.ProjectAllocations.GroupBy(p => p.UserFaculty.FullName)
                .Select(g => new
                {
                    Faculty = g.Key,
                    AverageProgress = g.Average(p => p.ProgressPercentage)
                })
                .OrderByDescending(g => g.AverageProgress)
                .ToListAsync();
            return Ok(result);
        }
        #endregion 
        
        #region 27) Display task statistics for every project.
        [HttpGet("TaskStatistics")]
        public async Task<IActionResult> GetTaskStatistics()
        {
            var result = await _context.Tasks.GroupBy(t => t.ProjectAllocation.ProjectMaster.ProjectTitle)
                .Select(g => new
                {
                    Project = g.Key,
                    TotalTasks = g.Count(),
                    CompletedTasks = g.Count(t => t.TaskStatus.TaskStatusName == "Completed"),
                    PendingTasks = g.Count(t => t.TaskStatus.TaskStatusName == "Pending"),
                    OverdueTasks = g.Count(t =>
                        t.TaskDueDate < DateTime.Now && t.TaskStatus.TaskStatusName != "Completed"),
                })
                .ToListAsync();
            return Ok(result);
        }
        #endregion
    }
}
