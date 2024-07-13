using MainService.Models;

namespace MainService.Repositories.Interfaces;

public interface IApprovalRequestRepository : IRepository<ApprovalRequest>
{
    Task<IEnumerable<ApprovalRequest>> GetFilteredApprovalRequestsAsync(int? approverId, int? leaveRequestId, string status);
}