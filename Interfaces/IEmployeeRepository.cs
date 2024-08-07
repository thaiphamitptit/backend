using backend.Models;

namespace backend.Interfaces;

public interface IEmployeeRepository {
  Task<bool> CheckEmployeeCodeExist(string code);
  Task<string> CreateNewEmployeeCode();
  Task<IEnumerable<Employee>> GetAllEmployees();
  Task<EmployeeFiltered> GetFilteredEmployees(EmployeeQueryParams employeeQueryParams);
  Task<Employee> GetEmployee(Guid id);
  Task AddEmployee(Employee employee);
  Task UpdateEmployee(Employee employee);
  Task DeleteEmployee(Guid id);
}
