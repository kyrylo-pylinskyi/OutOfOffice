using MainService.Models;

namespace MainService.Repositories.Interfaces;

public interface ILeaveRequestRepository : IRepository<LeaveRequest>
{
    Task<IEnumerable<LeaveRequest>> GetFilteredLeaveRequestsAsync(int? employeeId, string reason, string status);
}