using System;
using CockyShop.Infrastucture;
using CockyShop.Middlewares;
using CockyShop.Models.Identity;
using CockyShop.Services;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CockyShop.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration configuration)
        {
              var connectionString = configuration.GetConnectionString("DefaultConnection");
              
              services.AddIdentity<AppUser, AppRole>(
              o =>
                  {
                      o.Password.RequiredLength = 8;
                      o.Password.RequireDigit = false;
                      o.Password.RequireLowercase = false;
                      o.Password.RequireNonAlphanumeric = false;
                      o.Password.RequireUppercase = false;
                      o.User.RequireUniqueEmail = true;
                  })
                  .AddEntityFrameworkStores<AppDbContext>()
                  .AddDefaultTokenProviders();
              
              services.ConfigureApplicationCookie(options =>
              {
                  // Cookie settings
                  options.Cookie.HttpOnly = true;
                  options.ExpireTimeSpan = TimeSpan.FromMinutes(5);

                  options.LoginPath = "/Identity/Account/Login";
                  options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                  options.SlidingExpiration = true;
              });

              return services;
        }

        public static IServiceCollection AddAppServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
                        
            services.AddDbContext<AppDbContext>(o => 
                o.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
            
            services.AddScoped<IProductsService, ProductsService>();
            services.AddTransient<ExceptionHandlerMiddleware>();
            
            return services;
        }
    }
}