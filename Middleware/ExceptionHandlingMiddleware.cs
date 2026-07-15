using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace TraineeManagement.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Validation Exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, ex.Message);
            }
            catch (JsonException jsonEx)
            {
                _logger.LogWarning(jsonEx, "A client passed malformed JSON.");
                await HandleExceptionAsync(context, HttpStatusCode.BadRequest, "The request payload format is invalid.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "A critical unhandled exception occurred.");
                await HandleExceptionAsync(context, HttpStatusCode.InternalServerError, "An unexpected error occurred. Please try again later.");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, HttpStatusCode statusCode, string message)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            string? result = JsonSerializer.Serialize(new { message = message });

            return context.Response.WriteAsync(result);
        }
    }
}