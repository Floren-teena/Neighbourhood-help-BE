using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using NeighbourhoodHelp.Infrastructure.Interfaces;
using NeighbourhoodHelp.Infrastructure.Services;

namespace NeighbourhoodHelp.Api.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureLoggerService(this IServiceCollection service)
            => service.AddSingleton<ILoggerManagerService, LoggerManagerService>();

        public static void ConfigureExceptionHandler(this IApplicationBuilder app, ILoggerManagerService logger)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context => {
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        context.Response.ContentType = "application/json";


                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (contextFeature != null)
                        {
                            logger.logError($"Something went wrong: {contextFeature.Error}");
                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = "Internal server error"
                            }.ToString());

                        }
                    }



                );


            });
        }


    }

    
}
