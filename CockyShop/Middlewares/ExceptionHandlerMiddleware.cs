using System;
using System.Net;
using System.Threading.Tasks;
using CockyShop.Exceptions;
using CockyShop.Models.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace CockyShop.Middlewares
{
    public class ExceptionHandlerMiddleware : IMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger)
        {
            _logger = logger;
          
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception.Message);

                var apiError = ConvertToError(exception);

                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = apiError.Status;

                await response.WriteAsJsonAsync(apiError);
            }
        }

        private ApiError ConvertToError(Exception exception)
        {
            return exception switch
            {
                EntityNotFoundException e => new ApiError
                {
                    Description = e.Description,
                    Status = (int) HttpStatusCode.NotFound
                },
                DomainException e => new ApiError()
                {
                    Description = e.Description,
                    Status = (int) HttpStatusCode.BadRequest
                },
                _ => new ApiError
                {
                    Description = String.Empty,
                    Status = (int) HttpStatusCode.InternalServerError
                }
            };
        }
    }
}