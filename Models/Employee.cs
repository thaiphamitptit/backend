namespace backend.Models;

public class Employee {
  public Guid EmployeeId { get; set; }
  public required string EmployeeCode { get; set; }
  public required string FullName { get; set; }
  public required string Email { get; set; }
  public required string PhoneNumber { get; set; }
  public required string IdentityNumber { get; set; }
  public Guid PositionId { get; set; }
  public Guid DepartmentId { get; set; }
  public DateTime? DateOfBirth { get; set; } = null;
  public int? Gender { get; set; } = null;
  public string? TelephoneNumber { get; set; } = null;
  public string? Address { get; set; } = null;
  public DateTime? IdentityDate { get; set; } = null;
  public string? IdentityPlace { get; set; } = null;
  public string? BankAccount { get; set; } = null;
  public string? BankName { get; set; } = null;
  public string? Branch { get; set; } = null;
  public DateTime? CreatedDate { get; set; } = DateTime.Now;
  public string? CreatedBy { get; set; } = null;
  public DateTime? ModifiedDate { get; set; } = DateTime.Now;
  public string? ModifiedBy { get; set; } = null;
}

public class EmployeeQueryParams {
  public int? PageSize { get; set; }
  public int? PageNumber { get; set; }
  public string? EmployeeFilter { get; set; }
  public Guid? DepartmentId { get; set; }
  public Guid? PositionId { get; set; }
}

public class EmployeeFiltered {
  public int TotalRecords { get; set; }
  public int TotalPages { get; set; }
  public int CurrentPageRecords { get; set; }
  public int CurrentPageNumber { get; set; }
  public required IEnumerable<Employee> Employees { get; set; }
}
