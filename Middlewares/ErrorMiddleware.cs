using backend.Core;

namespace backend.Middlewares;

public class ErrorMiddleware {
  private readonly RequestDelegate _next;
  private readonly ILogger<ErrorMiddleware> _logger;

  public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger) {
    _next = next;
    _logger = logger;
  }

  public async Task InvokeAsync(HttpContext context) {
    try {
      await _next(context);
    } catch (Exception e) {
      _logger.LogError(e, "Internal server error");
      await context.Response.WriteAsJsonAsync(
        new ErrorResponse { 
          Message = "Có lỗi xảy ra từ hệ thống, vui lòng thử lại sau" 
        }
      );
    }
  }
}
