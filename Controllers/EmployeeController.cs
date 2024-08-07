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

public class EmployeeController : ControllerBase {
  private readonly IEmployeeRepository _employeeRepository;
  private readonly IValidator<Employee> _employeeValidator;

  public EmployeeController(IEmployeeRepository employeeRepository, IValidator<Employee> employeeValidator) {
    _employeeRepository = employeeRepository;
    _employeeValidator = employeeValidator;
  }

  [HttpGet]
  public async Task<ActionResult<IEnumerable<Employee>>> GetAllEmployees() {
    var employees = await _employeeRepository.GetAllEmployees();
    dynamic metadata = new ExpandoObject();
    metadata.employees = employees;
    return Ok(
      new SuccessResponse {
        Message = "Lấy thông tin danh sách nhân viên thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata 
      }
    );
  }

  [HttpGet("filter")]
  public async Task<ActionResult> GetFilteredEmployees(
    [FromQuery] int? pageSize, 
    [FromQuery] int? pageNumber,
    [FromQuery] string? employeeFilter, 
    [FromQuery] Guid? departmentId, 
    [FromQuery] Guid? positionId) {
      var employeeQueryParams = new EmployeeQueryParams {
        PageSize = pageSize,
        PageNumber = pageNumber,
        EmployeeFilter = employeeFilter,
        DepartmentId = departmentId,
        PositionId = positionId
      };
      if (employeeQueryParams.DepartmentId.HasValue && employeeQueryParams.DepartmentId.Value == Guid.Empty) {
        return BadRequest(
          new ErrorResponse { 
            Message = "Vui lòng chọn phòng ban hợp lệ", 
            Code = StatusCodes.Status400BadRequest, 
          }
        );
      }
      if (employeeQueryParams.PositionId.HasValue && employeeQueryParams.PositionId.Value == Guid.Empty) {
        return BadRequest(
          new ErrorResponse { 
            Message = "Vui lòng chọn vị trí hợp lệ", 
            Code = StatusCodes.Status400BadRequest, 
          }
        );
      }
      if (employeeQueryParams.PageSize.HasValue) {
        if (pageSize <= 0) {
          return BadRequest(
            new ErrorResponse { 
              Message = "Vui lòng chọn số bản ghi/trang hợp lệ", 
              Code = StatusCodes.Status400BadRequest, 
            }
          );
        }
      }
      if (employeeQueryParams.PageNumber.HasValue) {
        if (pageNumber <= 0) {
          return BadRequest(
            new ErrorResponse { 
              Message = "Vui lòng chọn số trang hợp lệ", 
              Code = StatusCodes.Status400BadRequest, 
            }
          );
        }
      }
      var metadata = await _employeeRepository.GetFilteredEmployees(employeeQueryParams);
      return Ok(
        new SuccessResponse {
          Message = "Lấy thông tin danh sách nhân viên thành công", 
          Code = StatusCodes.Status200OK, 
          Metadata = metadata 
        }
      );
  }

  [HttpGet("newEmployeeCode")]
  public async Task<ActionResult> CreateNewEmployeeCode() {
    var employeeCode = await _employeeRepository.CreateNewEmployeeCode();
    dynamic metadata = new ExpandoObject();
    metadata.employeeCode = employeeCode;
    return Ok(
      new SuccessResponse { 
        Message = "Lấy mã nhân viên mới thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata
      }
    );
  }

  [HttpGet("{id}")]
  public async Task<ActionResult<Employee>> GetEmployee(Guid id) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    var employee = await _employeeRepository.GetEmployee(id);
    dynamic metadata = new ExpandoObject();
    metadata.employee = employee; 
    if (employee == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy nhân viên hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    return Ok(
      new SuccessResponse {
        Message = "Lấy thông tin nhân viên thành công", 
        Code = StatusCodes.Status200OK, 
        Metadata = metadata 
      }
    );
  }

  [HttpPost]
  public async Task<ActionResult> AddEmployee([FromBody] Employee employee) {
    var validationResult = await _employeeValidator.ValidateAsync(employee);
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
    await _employeeRepository.AddEmployee(employee);
    dynamic metadata = new ExpandoObject();
    metadata.employee = employee;
    return CreatedAtAction(
      nameof(GetEmployee), 
      new { id = employee.EmployeeId }, 
      new SuccessResponse {
        Message = "Thêm nhân viên thành công",
        Code = StatusCodes.Status201Created,
        Metadata = metadata,
      }
    );
  }

  [HttpPut("{id}")]
  public async Task<ActionResult> UpdateEmployee(Guid id, [FromBody] Employee employee) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    if (await _employeeRepository.GetEmployee(id) == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy nhân viên hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    var validationResult = await _employeeValidator.ValidateAsync(employee);
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
    employee.EmployeeId = id;
    await _employeeRepository.UpdateEmployee(employee);
    return Ok(
      new SuccessResponse { 
        Message = "Cập nhật thông tin nhân viên thành công" 
      }
    );
  }

  [HttpDelete("{id}")]
  public async Task<ActionResult> DeleteEmployee(Guid id) {
    if (!Guid.TryParse(id.ToString(), out Guid guid)) {
      return BadRequest(
        new ErrorResponse { 
          Message = "Vui lòng chọn uuid hợp lệ", 
          Code = StatusCodes.Status400BadRequest, 
        }
      );
    }
    if (await _employeeRepository.GetEmployee(id) == null) {
      return NotFound(
        new ErrorResponse { 
          Message = "Không tìm thấy nhân viên hợp lệ", 
          Code = StatusCodes.Status404NotFound 
        }
      );
    }
    await _employeeRepository.DeleteEmployee(id);
    return Ok(
      new SuccessResponse {
        Message = "Xoá nhân viên thành công" 
      }
    );
  }
}
