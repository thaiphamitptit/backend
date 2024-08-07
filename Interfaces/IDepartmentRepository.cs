using backend.Models;

namespace backend.Interfaces;

public interface IDepartmentRepository {
  Task<bool> CheckDepartmentCodeExist(string code);
  Task<string> CreateNewDepartmentCode();
  Task<IEnumerable<Department>> GetAllDepartments();
  Task<Department> GetDepartment(Guid id);
  Task AddDepartment(Department department);
  Task UpdateDepartment(Department department);
  Task DeleteDepartment(Guid id);
}
