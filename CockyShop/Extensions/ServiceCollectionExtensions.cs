using System;
using System.Text;
using CockyShop.Infrastucture;
using CockyShop.Middlewares;
using CockyShop.Models.Identity;
using CockyShop.Services;
using CockyShop.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace CockyShop.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services,
            IConfiguration configuration)
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
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IOrdersService, OrdersService>();
            services.AddTransient<ExceptionHandlerMiddleware>();

            return services;
        }


        public static IServiceCollection AddAuthorizationServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireAdministratorRole",
                    policy => policy.RequireRole("Admin"));
            });

            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(conf =>
                {
                    conf.RequireHttpsMetadata = false;
                    conf.SaveToken = true;
                    conf.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = configuration["Jwt:JwtIssuer"],
                        ValidAudience = configuration["Jwt:JwtIssuer"],
                        IssuerSigningKey =
                            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:JwtKey"])),
                        ClockSkew = TimeSpan.Zero // remove delay of token when expire
                    };
                });

            return services;
        }
    }
}