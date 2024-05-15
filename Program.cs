using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NeighbourhoodHelp.Data;
using NeighbourhoodHelp.Data.IRepository;
using NeighbourhoodHelp.Data.Repository;
using NeighbourhoodHelp.Infrastructure.AutoMapper;
using NeighbourhoodHelp.Infrastructure.Helpers;
using NeighbourhoodHelp.Infrastructure.Interfaces;
using NeighbourhoodHelp.Infrastructure.Services;
using NeighbourhoodHelp.Model.Entities;
using System;
using System.Text.Json.Serialization;
using NeighbourhoodHelp.Core.IServices;
using NeighbourhoodHelp.Core.Services;
using NeighbourhoodHelp.Api.Extensions;

namespace NeighbourhoodHelp.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var env = builder.Environment;
            var config = builder.Configuration;
            // Add services to the container.
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUserServices, UserServices>();
            builder.Services.AddScoped<ITokenService, TokenService>();

            builder.Services.AddScoped<IAgentRepository, AgentRepository>();
           builder.Services.AddScoped<IAgentServices, AgentServices>();
           builder.Services.AddScoped<ICloudService, CloudService>();
            builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            //builder.Services.AddControllers();
           
            builder.Services.AddControllers().AddJsonOptions(options =>

            {

                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;

            });



            //Addidng of services
            builder.Services.AddScoped<IErrandRepository, ErrandRepository>();
            builder.Services.AddScoped<IAgentRepository, AgentRepository>();
            builder.Services.AddScoped<IAgentServices, AgentServices>();
            builder.Services.AddScoped<IErrandServices, ErrandServices>();
            builder.Services.AddScoped<IPriceServices, PriceServices>();
            builder.Services.AddScoped<IPriceRepository, PriceRepository>();


            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();

            //Adding Swagger Documention
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Neighbourhood Help", Version = "v1.0.0" });
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema);
                var securityRequirement = new OpenApiSecurityRequirement
                {
                    { securitySchema, new[] { "Bearer" } }
                };
                c.AddSecurityRequirement(securityRequirement);
            });

            builder.Services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddDbContextAndConfigurations(env, config);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            //Registering the Identity
            builder.Services.AddIdentity<AppUser, IdentityRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders(); 


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            
            app.UseSwagger();

            app.UseSwaggerUI();
            
            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseCors("AllowSpecificOrigin");

            //Get the service scope and obtain the necessary services
            using var scope = app.Services.CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            UserandRolesInitializedData.SeedData(context, userManager, roleManager).Wait();

            app.MapControllers();

            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "text/plain";

                    var exceptionHandlerPathFeature =
                        context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

                    if (exceptionHandlerPathFeature?.Error is not null)
                    {
                        await context.Response.WriteAsync($"An error occurred: {exceptionHandlerPathFeature.Error.Message}");
                    }
                });
            });

            app.Run();
        }
    }
}
