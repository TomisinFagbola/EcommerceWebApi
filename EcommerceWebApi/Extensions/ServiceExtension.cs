using API.Middlewares;
using Domain.Entities.Identity;
using FluentValidation.AspNetCore;
using FluentValidation;
using Infrastructure;
using Infrastructure.Contracts;
using Infrastructure.Data.DbContext;
using Infrastructure.Entities.Identity;
using Infrastructure.Utils.Logger;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;
using Application.Validators;
using Infrastructure.Utils.Email;
using Application.Contracts;
using Application.Services;
using Application.Helpers;
using Infrastructure.Utils.Azure;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace API.Extensions
{
    public static class ServiceExtension
    {
        private static readonly ILoggerFactory ContextLoggerFactory = LoggerFactory.Create(builder => { builder.AddConsole(); });
        public static void ConfigureCors(this IServiceCollection serviceCollection) =>
                     serviceCollection.AddCors(options =>
                     {
                         options.AddPolicy("CorsPolicy", builder =>
                             builder.AllowAnyOrigin()
                                 .AllowAnyMethod()
                                 .AllowAnyHeader()
                                 .WithExposedHeaders("X-Pagination"));
                     });

      public static void ConfigureLoggerService(this IServiceCollection serviceCollection) =>
                                 serviceCollection.AddSingleton<ILoggerManager, LoggerManager>();

        public static void ConfigureEmail(this IServiceCollection services)
        {
            services.AddScoped<IEmailManager, EmailManager>();
        }

        public static void ConfigureAzureStorageServices(this IServiceCollection services) =>
        
            services.AddTransient<IAzureStorage, AzureStorage>();

        public static void ConfigureWebHelper(this IServiceCollection services) =>
             services.AddTransient<IWebHelper, WebHelper>();

        public static void ConfigureRepositoryManager(this IServiceCollection serviceCollection) =>
           serviceCollection.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection serviceCollection) =>
       serviceCollection.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);
            });
            services.AddVersionedApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'VVV";
                opt.SubstituteApiVersionInUrl = true;
            });
        }

        public static void ConfigureControllers(this IServiceCollection serviceCollection) =>
           serviceCollection.AddControllers(config =>
           {
           })
                           .AddXmlDataContractSerializerFormatters()
                           .AddJsonOptions(x =>
                           {
                               // serialize enums as strings in api responses (e.g. Role)
                               x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                               x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                           }
                           );
        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequireDigit = true;
                opt.Password.RequireLowercase = true;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<AppDbContext>()
                .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var jwtUserSecret = jwtSettings.GetSection("Secret").Value;

            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.GetSection("ValidIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("ValidAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtUserSecret))
                };
            });
        }
        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<AppDbContext>(opts =>
                opts.UseSqlServer(connString));
        }


        public static void ConfigureMvc(this IServiceCollection services)
        {
            services.AddMvc().ConfigureApiBehaviorOptions(o =>
            {
                o.InvalidModelStateResponseFactory = context => new ValidationFailedResult(context.ModelState);
            });
            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
            services.AddValidatorsFromAssemblyContaining<UserValidator>();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }

        //public static void ConfigureHangfire(this IServiceCollection services, IConfiguration configuration)
        //{
        //    services.AddHangfire(x =>
        //        x.UseSqlServerStorage(configuration.GetConnectionString("HangfireDbConnection")));
        //    services.AddHangfireServer();
        //}

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(s =>
            {
                s.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Ecommerce Web API",
                    Version = "v1",
                    Description = "Ecommerce Web API",
                    TermsOfService = new Uri("https://ecommerce.com/terms"),
                    Contact = new OpenApiContact
                    {
                        Name = "Tomisin Fagbola",
                        Email = "developer@developer.com",
                        Url = new Uri("https://ecommerce.com/tomisinfagbola")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "Ecommerce LICENCE",
                        Url = new Uri("https://ecommerce.com/developer-licence")
                    }
                });
              


                s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Add JWT with Bearer",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                s.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Name = "Bearer"
                    },
                    new List<string>()
                }
            });
            });
        }
    }
}
