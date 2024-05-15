using Microsoft.EntityFrameworkCore;
using NeighbourhoodHelp.Data;
using System;

namespace NeighbourhoodHelp.Api.Extensions
{
    public static class DBRegistryExtension
    {
        private static string GetRenderConnectionString()
        {
            // Get the Database URL from the ENV variables in Render
            string connectionUrl = $"postgres://neighbourhood_help_user:rmxhYDlGSXX7f7Rj6rBsdkRPQAlTaIxO@dpg-cp2d37779t8c73frl7a0-a/neighbourhood_help";

            // parse the connection string
            var databaseUri = new Uri(connectionUrl);
            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

            var x = $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port=5432;" +
                   $"Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
            return x;
        }

        public static void AddDbContextAndConfigurations(this IServiceCollection services, IWebHostEnvironment env, IConfiguration config)
        {
            var ConnectionString = GetRenderConnectionString();
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                string connStr;

                if (env.IsDevelopment())
                {
                    connStr = GetRenderConnectionString();
                    // options.UseNpgsql(connStr);
                    connStr = config.GetConnectionString("LocalConnection");
                    options.UseNpgsql(connStr);
                }
                else
                {
                    connStr = config.GetConnectionString("DefaultConnection");
                    options.UseNpgsql(connStr);
                }
            });
        }
    }
}
