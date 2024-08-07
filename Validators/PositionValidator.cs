using FluentValidation;
using backend.Models;
using backend.Interfaces;

namespace backend.Validators;

public class PositionValidator: AbstractValidator<Position> {
  private readonly IPositionRepository _positionRepository;

  public PositionValidator(IPositionRepository positionRepository) {
    _positionRepository = positionRepository;

    RuleFor(position => position.PositionCode)
      .NotEmpty()
        .WithMessage("Mã vị trí không được phép để trống")
      .Matches(@"^VT\d{5}$")
        .WithMessage("Mã vị trí không hợp lệ")
      .MustAsync(async (code, cancellation) => !await _positionRepository.CheckPositionCodeExist(code))
        .WithMessage("Mã vị trí đã tồn tại trong hệ thống");
    RuleFor(position => position.PositionName)
      .NotEmpty()
        .WithMessage("Tên vị trí không được phép để trống");
  }
}
