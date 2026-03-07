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

            // 1. KI?M TRA LO?I L?I ?? X? LÝ VÀ LOG CHO G?N
            if (exception is AppException appEx)
            {
                // ?ây là l?i nghi?p v? do c?u ch? ??ng ném ra (VD: 400 Bad Request)
                statusCode = appEx.StatusCode;
                message = appEx.Message;
                data = appEx.Details;

                // CH? GHI WARNING: Không truy?n exception vào, màn hình console s? không b? rác b?i StackTrace
                _logger.LogWarning("L?i nghi?p v? [{StatusCode}]: {Message}", statusCode, message);
            }
            else
            {
                // ?ây là l?i v? m?m (500 Internal Server Error) do code ?u ho?c l?i h? th?ng
                statusCode = (int)HttpStatusCode.InternalServerError;

                // Super Senior Tip: KHÔNG BAO GI? tr? exception.Message c?a l?i 500 ra cho Client, d? b? hack.
                message = "?ã x?y ra l?i h? th?ng nghiêm tr?ng. Vui lòng th? l?i sau.";

                // GHI ERROR KÈM EXCEPTION: Màn hình console s? hi?n màu ?? và in ra StackTrace ?? c?u debug
                _logger.LogError(exception, "L?I H? TH?NG CHÍ M?NG: {Message}", exception.Message);
            }

            // 2. TR? V? RESPONSE CHO CLIENT
            var response = new ApiResponse<object>
            {
                Code = statusCode,
                Message = message,
                Data = data
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            // Format JSON cho ??p theo chu?n camelCase c?a API
            var jsonOptions = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var payload = JsonSerializer.Serialize(response, jsonOptions);

            await context.Response.WriteAsync(payload);
        }
    }
}