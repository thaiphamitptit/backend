using FluentValidation;
using backend.Models;
using backend.Interfaces;

namespace backend.Validators;

public class DepartmentValidator: AbstractValidator<Department> {
  private readonly IDepartmentRepository _departmentRepository;

  public DepartmentValidator(IDepartmentRepository departmentRepository) {
     _departmentRepository = departmentRepository;

    RuleFor(department => department.DepartmentCode)
      .NotEmpty()
        .WithMessage("Mã phòng ban không được phép để trống")
      .Matches(@"^PB\d{5}$")
        .WithMessage("Mã phòng ban không hợp lệ")
      .MustAsync(async (code, cancellation) => !await _departmentRepository.CheckDepartmentCodeExist(code))
        .WithMessage("Mã phòng ban đã tồn tại trong hệ thống");
    RuleFor(department => department.DepartmentName)
      .NotEmpty()
        .WithMessage("Tên phòng ban không được phép để trống");
  }
}
