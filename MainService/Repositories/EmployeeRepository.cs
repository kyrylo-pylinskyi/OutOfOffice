using MainService.Data;
using MainService.Models;
using MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MainService.Repositories;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Employee>> GetFilteredEmployeesAsync(string name, string subdivision, string position, bool? isActive)
    {
        var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(e => e.FullName.Contains(name));

        if (!string.IsNullOrEmpty(subdivision))
            query = query.Where(e => e.Subdivision == subdivision);

        if (!string.IsNullOrEmpty(position))
            query = query.Where(e => e.Position == position);

        if (isActive.HasValue)
            query = query.Where(e => e.Status == (isActive.Value ? "Active" : "Inactive"));

        return await query.ToListAsync();
    }
}