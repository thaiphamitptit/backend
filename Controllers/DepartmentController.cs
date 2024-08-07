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

public class DepartmentController : ControllerBase {
  private readonly IDepartmentRepository _departmentRepository;
  private readonly IValidator<Department> _departmentValidator;

  public DepartmentController(IDepartmentRepository departmentRepository, IValidator<Department> departmentValidator) {
    _departmentRepository = departmentRepository;
    _departmentValidator = departmentValidator;
  }
  
  [HttpGet]
  public async Task<ActionResult<IEnumerable<Department>>> GetAllDepartments() {
    var departments = await _departmentRepository.GetAllDepartments();
    dynamic metadata = new ExpandoObject();
    metadata.departments = departments;
    return Ok(
      new SuccessResponse { 
        Message = "Lấy thông tin danh sách phòng ban thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata
      }
    );
  }

  [HttpGet("newDepartmentCode")]
  public async Task<ActionResult> CreateNewDepartmentCode() {
    var departmentCode = await _departmentRepository.CreateNewDepartmentCode();
    dynamic metadata = new ExpandoObject();
    metadata.departmentCode = departmentCode;
    return Ok(
      new SuccessResponse { 
        Message = "Lấy mã phòng ban mới thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata
      }
    );
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Department>> GetDepartment(Guid id) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    var department = await _departmentRepository.GetDepartment(id);
    dynamic metadata = new ExpandoObject();
    metadata.department = department;
    if (department == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy phòng ban hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    return Ok(
      new SuccessResponse { 
        Message = "Lấy thông tin phòng ban thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata
      }
    );
  }

  [HttpPost]
  public async Task<ActionResult> AddDepartment([FromBody] Department department) {
    var validationResult = await _departmentValidator.ValidateAsync(department);
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
    await _departmentRepository.AddDepartment(department);
    dynamic metadata = new ExpandoObject();
    metadata.department = department;
    return CreatedAtAction(
      nameof(GetDepartment), 
      new { id = department.DepartmentId }, 
      new SuccessResponse {
        Message = "Thêm phòng ban thành công",
        Code = StatusCodes.Status201Created,
        Metadata = metadata,
      }
    );
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> UpdateDepartment(Guid id, [FromBody] Department department) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    if (await _departmentRepository.GetDepartment(id) == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy phòng ban hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    var validationResult = await _departmentValidator.ValidateAsync(department);
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
    department.DepartmentId = id;
    await _departmentRepository.UpdateDepartment(department);
    return Ok(
      new SuccessResponse { 
        Message = "Cập nhật thông tin phòng ban thành công" 
      }
    );
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteDepartment(Guid id) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    if (await _departmentRepository.GetDepartment(id) == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy phòng ban hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    await _departmentRepository.DeleteDepartment(id);
    return Ok(
      new SuccessResponse { 
        Message = "Xoá phòng ban thành công" 
      }
    );
  }
}
