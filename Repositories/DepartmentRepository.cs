using Dapper;
using backend.Data;
using backend.Models;
using backend.Interfaces;
using backend.Utils;

namespace backend.Repositories;

public class DepartmentRepository : IDepartmentRepository {
  private readonly DbContext _context;
  private readonly string _defaultCode = "PB0000";

  public DepartmentRepository(DbContext context) {
    _context = context;
  }

  public async Task<bool> CheckDepartmentCodeExist(string code) {
    using (var connection = _context.DbConnection()) {
      var query = "SELECT COUNT(1) FROM Department WHERE DepartmentCode = @Code";
      var count = await connection.ExecuteScalarAsync<int>(query, new { Code = code });
      return count > 0;
    }
  }

  public async Task<string> CreateNewDepartmentCode() {
    using (var connection = _context.DbConnection()) {
      var query = "SELECT DepartmentCode FROM Department ORDER BY DepartmentCode DESC LIMIT 1";
      var departmentCode = await connection.QueryFirstOrDefaultAsync<string>(query) ?? _defaultCode;
      var newDepartmentCode = CommonUtils.AutoGenerateCode(departmentCode);
      return newDepartmentCode;
    }
  }

  public async Task<IEnumerable<Department>> GetAllDepartments() {
    using (var connection = _context.DbConnection()) {
      var query = @"SELECT * FROM Department";
      return await connection.QueryAsync<Department>(query);
    }
  }

  public async Task<Department> GetDepartment(Guid id) {
    using (var connection = _context.DbConnection()) {
      var query = @"SELECT * FROM Department WHERE DepartmentId = @Id";
      return await connection.QueryFirstOrDefaultAsync<Department>(query, new { Id = id });
    }
  }

  public async Task AddDepartment(Department department) {
    department.DepartmentId = Guid.NewGuid();
    using (var connection = _context.DbConnection()) {
      var query = @"
        INSERT INTO 
          Department (
            DepartmentId, 
            DepartmentCode, 
            DepartmentName, 
            Description, 
            CreatedDate, 
            ModifiedDate, 
            CreatedBy, 
            ModifiedBy
          ) 
        VALUES ( 
          @DepartmentId, 
          @DepartmentCode, 
          @DepartmentName, 
          @Description, 
          @CreatedDate, 
          @ModifiedDate, 
          @CreatedBy, 
          @ModifiedBy
        )";
      await connection.ExecuteAsync(query, department);
    }
  }

  public async Task UpdateDepartment(Department department) {
    department.ModifiedDate = DateTime.Now;
    using (var connection = _context.DbConnection()) {
      var query = @"
        UPDATE 
          Department 
        SET 
          DepartmentCode = @DepartmentCode, 
          DepartmentName = @DepartmentName, 
          Description = @Description, 
          CreatedDate = @CreatedDate, 
          ModifiedDate = @ModifiedDate, 
          CreatedBy = @CreatedBy, 
          ModifiedBy = @ModifiedBy 
        WHERE 
          DepartmentId = @DepartmentId";
      await connection.ExecuteAsync(query, department);
    }
  }

  public async Task DeleteDepartment(Guid id) {
    using (var connection = _context.DbConnection()) {
      var query = @"DELETE FROM Department WHERE DepartmentId = @Id";
      await connection.ExecuteAsync(query, new { Id = id });
    }
  }
}
