using Dapper;
using backend.Data;
using backend.Models;
using backend.Interfaces;
using backend.Utils;

namespace backend.Repositories;

public class PositionRepository : IPositionRepository {
  private readonly DbContext _context;
  private readonly string _defaultCode = "VT00000";

  public PositionRepository(DbContext context) {
    _context = context;
  }

  public async Task<bool> CheckPositionCodeExist(string code) {
    using (var connection = _context.DbConnection()) {
      var query = "SELECT COUNT(1) FROM Position WHERE PositionCode = @Code";
      var count = await connection.ExecuteScalarAsync<int>(query, new { Code = code });
      return count > 0;
    }
  }

  public async Task<string> CreateNewPositionCode() {
    using (var connection = _context.DbConnection()) {
      var query = "SELECT PositionCode FROM Position ORDER BY PositionCode DESC LIMIT 1";
      var positionCode = await connection.QueryFirstOrDefaultAsync<string>(query) ?? _defaultCode;
      var newPositionCode = CommonUtils.AutoGenerateCode(positionCode);
      return newPositionCode;
    }
  }

  public async Task<IEnumerable<Position>> GetAllPositions() {
    using (var connection = _context.DbConnection()) {
      var query = @"SELECT * FROM Position";
      return await connection.QueryAsync<Position>(query);
    }
  }

  public async Task<Position> GetPosition(Guid id) {
    using (var connection = _context.DbConnection()) {
      var query = @"SELECT * FROM Position WHERE PositionId = @Id";
      return await connection.QueryFirstOrDefaultAsync<Position>(query, new { Id = id });
    }
  }

  public async Task AddPosition(Position position) {
    position.PositionId = Guid.NewGuid();
    using (var connection = _context.DbConnection()) {
      var query = @"
        INSERT INTO 
          Position (
            PositionId, 
            PositionCode, 
            PositionName, 
            Description, 
            CreatedDate, 
            ModifiedDate, 
            CreatedBy, 
            ModifiedBy
          ) 
        VALUES ( 
          @PositionId, 
          @PositionCode, 
          @PositionName, 
          @Description, 
          @CreatedDate, 
          @ModifiedDate, 
          @CreatedBy, 
          @ModifiedBy
        )";
      await connection.ExecuteAsync(query, position);
    }
  }

  public async Task UpdatePosition(Position position) {
    position.ModifiedDate = DateTime.Now;
    using (var connection = _context.DbConnection()) {
      var query = @"
        UPDATE 
          Position 
        SET 
          PositionCode = @PositionCode, 
          PositionName = @PositionName, 
          Description = @Description, 
          CreatedDate = @CreatedDate, 
          ModifiedDate = @ModifiedDate, 
          CreatedBy = @CreatedBy, 
          ModifiedBy = @ModifiedBy 
        WHERE 
          PositionId = @PositionId";
      await connection.ExecuteAsync(query, position);
    }
  }

  public async Task DeletePosition(Guid id) {
    using (var connection = _context.DbConnection()) {
      var query = @"DELETE FROM Position WHERE PositionId = @Id";
      await connection.ExecuteAsync(query, new { Id = id });
    }
  }
}
