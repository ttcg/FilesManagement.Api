using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace FilesManagement.Api.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                Serilog
                    .Log
                    .Error(ex, "Global error {Message}", ex.Message);
            }
        }
    }
}
