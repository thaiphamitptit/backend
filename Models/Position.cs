namespace backend.Models;

public class Position {
  public Guid PositionId { get; set; }
  public required string PositionCode { get; set; }
  public required string PositionName { get; set; }
  public string? Description { get; set; } = null;
  public DateTime? CreatedDate { get; set; } = DateTime.Now;
  public string? CreatedBy { get; set; } = null;
  public DateTime? ModifiedDate { get; set; } = DateTime.Now;
  public string? ModifiedBy { get; set; } = null;
}
