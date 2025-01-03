using System.Text;
using ExcalibPayments.Orders.Application.Abstractions;
using ExcalibPayments.Orders.Application.Services;
using ExcalibPayments.Orders.Domain;
using ExcalibPayments.Orders.Domain.Entities;
using ExcalibPayments.Orders.Domain.Models;
using ExcalibPayments.Orders.Domain.Options;
using ExcalibPayments.Orders.Web.BackgroundServices;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace ExcalibPayments.Orders.Web.Extensions;

public static class ServiceCollectionsExtensions
{
    public static WebApplicationBuilder AddSwagger(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Orders API", 
                Version = "v1"
            });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return builder;
    }

    public static WebApplicationBuilder AddData(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<OrdersDbContext>(opt => 
            opt.UseNpgsql(builder.Configuration.GetConnectionString("Orders")));
        
        return builder;
    }

    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ICartsService, CartsService>();
        builder.Services.AddScoped<IOrdersService, OrdersService>();
        builder.Services.AddScoped<IMerchantsService, MerchantsService>();
        
        return builder;
    }
    
    public static WebApplicationBuilder AddIntegrationServices(this WebApplicationBuilder builder)
    {
        return builder;
    }

    public static WebApplicationBuilder AddBackgroundService(this WebApplicationBuilder builder)
    {
        builder.Services.AddHostedService<CreateOrderConsumer>();
        
        return builder;
    }

    public static WebApplicationBuilder AddBearerAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.UseSecurityTokenValidators = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(
                        builder.Configuration["Authentication:TokenPrivateKey"]!)),
                    ValidIssuer = "test",
                    ValidAudience = "test",
                    // ValidateIssuer = true,
                    // ValidateAudience = true,
                    // ValidateLifetime = true,
                    // ValidateIssuerSigningKey = true
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ValidateIssuerSigningKey = false
                };
            });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireRole(RoleConsts.Admin));
            options.AddPolicy("Merchant", policy => policy.RequireRole(RoleConsts.Merchant));
            options.AddPolicy("User", policy => policy.RequireRole(RoleConsts.User));
        });
        builder.Services.AddTransient<IAuthService, AuthService>();
        builder.Services.AddDefaultIdentity<UserEntity>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddEntityFrameworkStores<OrdersDbContext>()
            .AddUserManager<UserManager<UserEntity>>()
            .AddUserStore<UserStore<UserEntity, IdentityRoleEntity, OrdersDbContext, long>>();

        return builder;
    }

    public static WebApplicationBuilder AddOptions(this WebApplicationBuilder builder)
    {
        builder.Services.Configure<AuthOptions>(builder.Configuration.GetSection("Authentication"));
        builder.Services.Configure<RabbitMqOptions>(builder.Configuration.GetSection("RabbitMQ"));

        return builder;
    }
}