using MainService.Data;
using MainService.Models;
using MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MainService.Repositories;

public class ApprovalRequestRepository : Repository<ApprovalRequest>, IApprovalRequestRepository
{
    public ApprovalRequestRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<ApprovalRequest>> GetFilteredApprovalRequestsAsync(int? approverId, int? leaveRequestId, string status)
    {
        var query = _context.ApprovalRequests.AsQueryable();

        if (approverId.HasValue)
            query = query.Where(ar => ar.ApproverId == approverId.Value);

        if (leaveRequestId.HasValue)
            query = query.Where(ar => ar.LeaveRequestId == leaveRequestId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(ar => ar.Status == status);

        return await query.ToListAsync();
    }
}