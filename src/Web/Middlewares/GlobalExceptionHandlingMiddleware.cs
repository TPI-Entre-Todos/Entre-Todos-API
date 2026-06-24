using System.Net;
using System.Text.Json;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        public GlobalExceptionHandlingMiddleware(
            ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }


        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (NotFoundException ex)
            {
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.NotFound;

                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Server error",
                    Title = "Server error",
                    Detail = ex.Message
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }

            catch (AppValidationException ex)
            {
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.BadRequest;

                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Server error",
                    Title = "Server error",
                    Detail = ex.Message
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }

            catch (ArgumentException ex)
            {
                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.BadRequest;

                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Server error",
                    Title = "Server error",
                    Detail = "An internal server"
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);
            }
            catch (Exception ex)
            {

                _logger.LogError(ex, ex.Message);

                int statusCode = (int)HttpStatusCode.InternalServerError;

                context.Response.StatusCode = statusCode;

                ProblemDetails problem = new()
                {
                    Status = statusCode,
                    Type = "Server error",
                    Title = "Server error",
                    Detail = ex.Message
                };

                string json = JsonSerializer.Serialize(problem);

                context.Response.ContentType = "application/json";

                await context.Response.WriteAsync(json);

            }
        }
    }

    [Serializable]
    internal class AppValidationException : Exception
    {
        public AppValidationException()
        {
        }

        public AppValidationException(string? message) : base(message)
        {
        }

        public AppValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}