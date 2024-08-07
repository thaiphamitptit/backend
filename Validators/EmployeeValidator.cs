using FluentValidation;
using backend.Models;
using backend.Interfaces;

namespace backend.Validators;

public class EmployeeValidator: AbstractValidator<Employee> {
  private readonly IEmployeeRepository _employeeRepository;

  public EmployeeValidator(IEmployeeRepository employeeRepository) {
    _employeeRepository = employeeRepository;

    RuleFor(employee => employee.EmployeeCode)
      .NotEmpty()
        .WithMessage("Mã nhân viên không được phép để trống")
      .Matches(@"^NV\d{5}$")
        .WithMessage("Mã nhân viên không hợp lệ")
      .MustAsync(async (code, cancellation) => !await _employeeRepository.CheckEmployeeCodeExist(code))
        .WithMessage("Mã nhân viên đã tồn tại trong hệ thống");
    RuleFor(employee => employee.FullName)
      .NotEmpty()
        .WithMessage("Họ tên không được phép để trống");
    RuleFor(employee => employee.Email)
      .NotEmpty()
        .WithMessage("Email không được phép để trống")
      .EmailAddress()
        .WithMessage("Email không hợp lệ");;
    RuleFor(employee => employee.PhoneNumber)
      .NotEmpty()
        .WithMessage("SĐT không được phép để trống");
    RuleFor(employee => employee.IdentityNumber)
      .NotEmpty()
        .WithMessage("Số CMTND không được phép để trống");
    RuleFor(employee => employee.DepartmentId)
      .NotEmpty()
        .WithMessage("Phòng ban không được phép để trống");
    RuleFor(employee => employee.PositionId)
      .NotEmpty()
        .WithMessage("Vị trí không được phép để trống");
    RuleFor(employee => employee.DateOfBirth)
      .LessThan(DateTime.Now)
        .WithMessage("Ngày sinh không hợp lệ");
    RuleFor(employee => employee.IdentityDate)
      .LessThan(DateTime.Now)
        .WithMessage("Ngày cấp CMTND không hợp lệ");
  }
}
