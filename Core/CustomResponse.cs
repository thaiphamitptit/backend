namespace backend.Core;

public class ErrorResponse {
  public required string Message { get; set; }
  public int Code { get; set; } = StatusCodes.Status500InternalServerError;
  public string Status { get; set; } = "error";
  public object? Errors { get; set; }
}

public class SuccessResponse {
  public required string Message { get; set; }
  public int Code { get; set; } = StatusCodes.Status200OK;
  public string Status { get; set; } = "success";
  public object? Metadata { get; set; }
}
