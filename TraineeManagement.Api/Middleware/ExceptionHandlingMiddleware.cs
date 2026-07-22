using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using TraineeManagement.Api.Constants;
using TraineeManagement.Api.Exceptions;
using TraineeManagement.Api.Exceptions.Base;
using TraineeManagement.Api.DTOs;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.Net.Mime;

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
            catch (ApiException ex)
            {
                _logger.LogWarning(ex.Message);

                await HandleExceptionAsync(context, ex.StatusCode, ex.Message);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, ExceptionMessages.InvalidJson);

                await HandleExceptionAsync(context, (int)HttpStatusCode.BadRequest, ExceptionMessages.InvalidJson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ExceptionMessages.UnexpectedError);

                await HandleExceptionAsync(context, (int)HttpStatusCode.InternalServerError, ExceptionMessages.UnexpectedError);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, string message)
        {
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.StatusCode = statusCode;

            string? result = JsonSerializer.Serialize(new ErrorResponse
            {
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            });

            return context.Response.WriteAsync(result);
        }
    }
}