using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService.Models;

public class Project
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string ProjectType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    [Required]
    public int ProjectManagerId { get; set; }

    public string Comment { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; }

    [ForeignKey("ProjectManagerId")]
    public Employee ProjectManager { get; set; }

    public ICollection<ProjectAssignment> ProjectAssignments { get; set; }
}
