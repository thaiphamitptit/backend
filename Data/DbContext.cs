using System.Data;
using MySql.Data.MySqlClient;

namespace backend.Data;

public class DbContext {
  private readonly string? _connectionString;

  public DbContext(IConfiguration configuration) {
    _connectionString = configuration.GetConnectionString("DefaultConnection");
  }

  public IDbConnection DbConnection() => new MySqlConnection(_connectionString);
}
