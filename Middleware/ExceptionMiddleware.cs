using System.Net;
using System.Text.Json;
using AppGroupCar.DTO.ResponseDTO;
using AppGroupCar.Exceptions;

namespace AppGroupCar.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            int statusCode;
            string message;
            object data = null;

            if (exception is AppException appEx)
            {
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                data = appEx.Details;

                _logger.LogWarning("Lỗi nghiệp vụ [{StatusCode}]: {Message}", statusCode, message);
            }
            else
            {
                statusCode = (int)HttpStatusCode.InternalServerError;

                message = "Đã xảy ra lỗi hệ thống. Vui lòng thử lại sau.";

                _logger.LogError(exception, "Lỗi hệ thống: {Message}", exception.Message);
            }

            var response = new ApiResponse<object>
            {
                Code = statusCode,
                Message = message,
                Data = data
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(response, jsonOptions);

            await context.Response.WriteAsync(payload);
        }
    }
}