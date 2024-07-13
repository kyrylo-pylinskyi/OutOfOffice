using MainService.Data;
using MainService.Models;
using MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MainService.Repositories;

public class LeaveRequestRepository : Repository<LeaveRequest>, ILeaveRequestRepository
{
    public LeaveRequestRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<LeaveRequest>> GetFilteredLeaveRequestsAsync(int? employeeId, string reason, string status)
    {
        var query = _context.LeaveRequests.AsQueryable();

        if (employeeId.HasValue)
            query = query.Where(lr => lr.EmployeeId == employeeId.Value);

        if (!string.IsNullOrEmpty(reason))
            query = query.Where(lr => lr.AbsenceReason == reason);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(lr => lr.Status == status);

        return await query.ToListAsync();
    }
}