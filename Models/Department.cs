namespace backend.Models;

public class Department {
  public Guid DepartmentId { get; set; }
  public required string DepartmentCode { get; set; }
  public required string DepartmentName { get; set; }
  public string? Description { get; set; } = null;
  public DateTime? CreatedDate { get; set; } = DateTime.Now;
  public string? CreatedBy { get; set; } = null;
  public DateTime? ModifiedDate { get; set; } = DateTime.Now;
  public string? ModifiedBy { get; set; } = null;
}
