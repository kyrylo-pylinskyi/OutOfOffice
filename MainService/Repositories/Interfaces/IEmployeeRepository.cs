using MainService.Models;

namespace MainService.Repositories.Interfaces;

public interface IEmployeeRepository : IRepository<Employee>
{
    Task<IEnumerable<Employee>> GetFilteredEmployeesAsync(string name, string subdivision, string position, bool? isActive);
}