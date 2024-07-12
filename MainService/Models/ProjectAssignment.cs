using System.ComponentModel.DataAnnotations.Schema;

namespace MainService.Models;
public class ProjectAssignment
{
    public int ProjectId { get; set; }
    public int EmployeeId { get; set; }

    [ForeignKey("ProjectId")]
    public Project Project { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; }
}
