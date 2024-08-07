using Dapper;
using backend.Data;
using backend.Models;
using backend.Interfaces;
using backend.Utils;
using System.Text;

namespace backend.Repositories;

public class EmployeeRepository : IEmployeeRepository {
  private readonly DbContext _context;
  private readonly string _defaultCode = "NV00000";
  private readonly int _defaultPageNumber = 1;
  private readonly int _defaultPageSize = 20;

  public EmployeeRepository(DbContext context) {
    _context = context;
  }

  public async Task<bool> CheckEmployeeCodeExist(string code) {
    using (var connection = _context.DbConnection()) {
      var query = "SELECT COUNT(1) FROM Employee WHERE EmployeeCode = @Code";
      var count = await connection.ExecuteScalarAsync<int>(query, new { Code = code });
      return count > 0;
    }
  }

  public async Task<string> CreateNewEmployeeCode() {
    using (var connection = _context.DbConnection()) {
      var query = "SELECT EmployeeCode FROM Employee ORDER BY EmployeeCode DESC LIMIT 1";
      var employeeCode = await connection.QueryFirstOrDefaultAsync<string>(query) ?? _defaultCode;
      var newEmployeeCode = CommonUtils.AutoGenerateCode(employeeCode);
      return newEmployeeCode;
    }
  }

  public async Task<IEnumerable<Employee>> GetAllEmployees() {
    using (var connection = _context.DbConnection()) {
      var query = @"SELECT * FROM Employee";
      return await connection.QueryAsync<Employee>(query);
    }
  }

  public async Task<EmployeeFiltered> GetFilteredEmployees(EmployeeQueryParams employeeQueryParams) {
    using (var connection = _context.DbConnection()) {
      var parameters = new DynamicParameters();
      var query = new StringBuilder("SELECT * FROM Employee WHERE 1 = 1");
      if (!string.IsNullOrEmpty(employeeQueryParams.EmployeeFilter)) {
        query.Append(" AND (FullName LIKE @EmployeeFilter OR EmployeeCode LIKE @EmployeeFilter)");
        parameters.Add("EmployeeFilter", $"%{employeeQueryParams.EmployeeFilter}%");
      }
      if (employeeQueryParams.DepartmentId.HasValue) {
        query.Append(" AND DepartmentId = @DepartmentId");
        parameters.Add("DepartmentId", employeeQueryParams.DepartmentId);
      }
      if (employeeQueryParams.PositionId.HasValue) {
        query.Append(" AND PositionId = @PositionId");
        parameters.Add("PositionId", employeeQueryParams.PositionId);
      }
      var totalEmployees = await connection.QueryAsync<Employee>(query.ToString(), parameters);
      var pageSize = employeeQueryParams.PageSize ?? _defaultPageSize;
      var pageNumber = employeeQueryParams.PageNumber ?? _defaultPageNumber;
      query.Append(" ORDER BY EmployeeCode LIMIT @PageSize OFFSET @Offset");
      parameters.Add("Offset", (pageNumber - 1) * pageSize);
      parameters.Add("PageSize", pageSize);
      var currentPageEmployees = await connection.QueryAsync<Employee>(query.ToString(), parameters);
      var totalRecords = totalEmployees.Count();
      var currentPageRecords = currentPageEmployees.Count();
      return new EmployeeFiltered {
        TotalRecords = totalRecords,
        TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize),
        CurrentPageRecords = currentPageRecords,
        CurrentPageNumber = pageNumber,
        Employees = currentPageEmployees
      };
    }
  }

  public async Task<Employee> GetEmployee(Guid id) {
    using (var connection = _context.DbConnection()) {
      var query = @"SELECT * FROM Employee WHERE EmployeeId = @Id";
      return await connection.QueryFirstOrDefaultAsync<Employee>(query, new { Id = id });
    }
  }

  public async Task AddEmployee(Employee employee) {
    employee.EmployeeId = Guid.NewGuid();
    using (var connection = _context.DbConnection()) {
      var query = @"
        INSERT INTO 
          Employee (
            EmployeeId, 
            EmployeeCode, 
            FullName, 
            Email,
            PhoneNumber,
            IdentityNumber,
            PositionId,
            DepartmentId,
            DateOfBirth,
            Gender,
            TelephoneNumber,
            Address,
            IdentityDate,
            IdentityPlace,
            BankAccount,
            BankName,
            Branch, 
            CreatedDate, 
            ModifiedDate, 
            CreatedBy, 
            ModifiedBy
          ) 
        VALUES ( 
          @EmployeeId, 
          @EmployeeCode, 
          @FullName, 
          @Email,
          @PhoneNumber,
          @IdentityNumber,
          @PositionId,
          @DepartmentId,
          @DateOfBirth,
          @Gender,
          @TelephoneNumber,
          @Address,
          @IdentityDate,
          @IdentityPlace,
          @BankAccount,
          @BankName,
          @Branch,  
          @CreatedDate, 
          @ModifiedDate, 
          @CreatedBy, 
          @ModifiedBy
        )";
      await connection.ExecuteAsync(query, employee);
    }
  }

  public async Task UpdateEmployee(Employee employee) {
    employee.ModifiedDate = DateTime.Now;
    using (var connection = _context.DbConnection()) {
      var query = @"
        UPDATE 
          Employee 
        SET 
          EmployeeCode = @EmployeeCode, 
          FullName = @FullName, 
          Email = @Email,
          PhoneNumber = @PhoneNumber,
          IdentityNumber = @IdentityNumber,
          PositionId = @PositionId,
          DepartmentId = @DepartmentId,
          DateOfBirth = @DateOfBirth,
          Gender = @Gender,
          TelephoneNumber = @TelephoneNumber,
          Address = @Address,
          IdentityDate = @IdentityDate,
          IdentityPlace = @IdentityPlace,
          BankAccount = @BankAccount,
          BankName = @BankName,
          Branch = @Branch, 
          CreatedDate = @CreatedDate, 
          ModifiedDate = @ModifiedDate, 
          CreatedBy = @CreatedBy, 
          ModifiedBy = @ModifiedBy
        WHERE 
          EmployeeId = @EmployeeId";
      await connection.ExecuteAsync(query, employee);
    }
  }

  public async Task DeleteEmployee(Guid id) {
    using (var connection = _context.DbConnection()) {
      var query = @"DELETE FROM Employee WHERE EmployeeId = @Id";
      await connection.ExecuteAsync(query, new { Id = id });
    }
  }
}
