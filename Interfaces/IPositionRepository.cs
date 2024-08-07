using backend.Models;

namespace backend.Interfaces;

public interface IPositionRepository {
  Task<bool> CheckPositionCodeExist(string code);
  Task<string> CreateNewPositionCode();
  Task<IEnumerable<Position>> GetAllPositions();
  Task<Position> GetPosition(Guid id);
  Task AddPosition(Position position);
  Task UpdatePosition(Position position);
  Task DeletePosition(Guid id);
}
