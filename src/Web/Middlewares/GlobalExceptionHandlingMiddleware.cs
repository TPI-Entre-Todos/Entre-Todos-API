using System.Net;
using Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Web.Middlewares
{
    public class GlobalExceptionHandlingMiddleware : IMiddleware
    {
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;

        public GlobalExceptionHandlingMiddleware(ILogger<GlobalExceptionHandlingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title, detail) = exception switch
            {
                BadRequestException ex => (HttpStatusCode.BadRequest, "Bad request", ex.Message),
                NotFoundException ex => (HttpStatusCode.NotFound, "Not found", ex.Message),
                UnauthorizedException ex => (HttpStatusCode.Unauthorized, "Unauthorized", ex.Message),
                Domain.Exceptions.UnauthorizedAccessException ex => (HttpStatusCode.Forbidden, "Forbidden", ex.Message),
                ArgumentException ex => (HttpStatusCode.BadRequest, "Bad request", ex.Message),
                _ => (HttpStatusCode.InternalServerError, "Server error", "Ocurrio un error inesperado.")
            };

            if (statusCode == HttpStatusCode.InternalServerError)
                _logger.LogError(exception, exception.Message);
            else
                _logger.LogWarning(exception, exception.Message);

            context.Response.StatusCode = (int)statusCode;
            context.Response.ContentType = "application/problem+json";

            var problem = new ProblemDetails
            {
                Status = (int)statusCode,
                Title = title,
                Detail = detail,
                Instance = context.Request.Path
            };

            await context.Response.WriteAsJsonAsync(problem);
        }
    }
}
