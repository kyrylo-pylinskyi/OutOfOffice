using MainService.Models;

namespace MainService.Repositories.Interfaces;

public interface IProjectRepository : IRepository<Project>
{
    Task<IEnumerable<Project>> GetFilteredProjectsAsync(string type, int? managerId, string status);
}