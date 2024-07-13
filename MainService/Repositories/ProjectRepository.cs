using MainService.Data;
using MainService.Models;
using MainService.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MainService.Repositories;

public class ProjectRepository : Repository<Project>, IProjectRepository
{
    public ProjectRepository(ApplicationDbContext context) : base(context) { }

    public async Task<IEnumerable<Project>> GetFilteredProjectsAsync(string type, int? managerId, string status)
    {
        var query = _context.Projects.AsQueryable();

        if (!string.IsNullOrEmpty(type))
            query = query.Where(p => p.ProjectType == type);

        if (managerId.HasValue)
            query = query.Where(p => p.ProjectManagerId == managerId.Value);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(p => p.Status == status);

        return await query.ToListAsync();
    }
}