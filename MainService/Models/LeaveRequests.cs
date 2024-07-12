using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainService.Models;

public class LeaveRequest
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    public int EmployeeId { get; set; }

    [Required]
    [MaxLength(50)]
    public string AbsenceReason { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public string Comment { get; set; }

    [Required]
    [MaxLength(20)]
    public string Status { get; set; }

    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; }

    public ICollection<ApprovalRequest> ApprovalRequests { get; set; }
}
