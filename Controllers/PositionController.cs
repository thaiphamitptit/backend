using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using System.Dynamic;
using backend.Models;
using backend.Core;
using backend.Utils;
using backend.Interfaces;

namespace backend.Controllers;

[Route("api/v1/[controller]s")]
[ApiController]

public class PositionController : ControllerBase {
  private readonly IPositionRepository _positionRepository;
  private readonly IValidator<Position> _positionValidator;

  public PositionController(IPositionRepository positionRepository, IValidator<Position> positionValidator) {
    _positionRepository = positionRepository;
    _positionValidator = positionValidator;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Position>>> GetAllPositions() {
    var positions = await _positionRepository.GetAllPositions();
    dynamic metadata = new ExpandoObject();
    metadata.positions = positions;
    return Ok(
      new SuccessResponse {
        Message = "Lấy thông tin danh sách vị trí thành công", 
        Code = StatusCodes.Status200OK,
        Metadata = metadata
      }
    );
  }

  [HttpGet("newPositionCode")]
  public async Task<ActionResult> CreateNewPositionCode() {
    var positionCode = await _positionRepository.CreateNewPositionCode();
    dynamic metadata = new ExpandoObject();
    metadata.positionCode = positionCode;
    return Ok(
      new SuccessResponse { 
        Message = "Lấy mã vị trí mới thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata
      }
    );
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Position>> GetPosition(Guid id) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    var position = await _positionRepository.GetPosition(id);
    dynamic metadata = new ExpandoObject();
    metadata.position = position;
    if (position == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy vị trí hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    return Ok(
      new SuccessResponse {
        Message = "Lấy thông tin vị trí thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata
      }
    );
  }

  [HttpPost]
  public async Task<ActionResult> AddPosition([FromBody] Position position) {
    var validationResult = await _positionValidator.ValidateAsync(position);
    if (!validationResult.IsValid) {
      var errors = ValidationUtils.ExtractErrors(validationResult);
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng nhập lại các trường không hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
          Errors = errors
        }
      );
    }
    await _positionRepository.AddPosition(position);
    dynamic metadata = new ExpandoObject();
    metadata.position = position;
    return CreatedAtAction(
      nameof(GetPosition), 
      new { id = position.PositionId }, 
      new SuccessResponse {
        Message = "Thêm vị trí thành công",
        Code = StatusCodes.Status201Created,
        Metadata = metadata,
      }
    );
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> UpdatePosition(Guid id, [FromBody] Position position) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    if (await _positionRepository.GetPosition(id) == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy vị trí hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    var validationResult = await _positionValidator.ValidateAsync(position);
    if (!validationResult.IsValid) {
      var errors = ValidationUtils.ExtractErrors(validationResult);
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng nhập lại các trường không hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
          Errors = errors
        }
      );
    }
    position.PositionId = id;
    await _positionRepository.UpdatePosition(position);
    return Ok(
      new SuccessResponse { 
        Message = "Cập nhật thông tin vị trí thành công" 
      }
    );
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeletePosition(Guid id) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
     if (await _positionRepository.GetPosition(id) == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy vị trí hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    await _positionRepository.DeletePosition(id);
    return Ok(
      new SuccessResponse { 
        Message = "Xoá vị trí thành công" 
      }
    );
  }
}
