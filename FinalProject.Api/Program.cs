////////////using FinalProject.Core.Services;
////////////using FinalProject.Core;
////////////using FinalProject.Data.Repositories;
////////////using FinalProject.Service;
////////////using FinalProject;
////////////using Microsoft.AspNetCore.Authentication.JwtBearer;
////////////using Microsoft.EntityFrameworkCore;
////////////using Microsoft.IdentityModel.Tokens;
////////////using Microsoft.OpenApi.Models;
////////////using System.Text.Json.Serialization;
////////////using System.Text;
////////////using Microsoft.AspNetCore.Http.Features;
////////////using FinalProject.Core.Repositories;
////////////using Amazon.S3;
////////////using Amazon.Extensions.NETCore.Setup;
////////////using Amazon.Runtime;
////////////using Amazon;
////////////using dotenv.net;
////////////using Amazon.TranscribeService;
////////////using Google.Api;
////////////using DotNetEnv;
////////////using System.Text.Json;
////////////using MySqlConnector; // ודא שהוספת את החבילה

////////////public class Program
////////////{
////////////    public static void Main(string[] args)
////////////    {
////////////        Env.Load();
////////////        var builder = WebApplication.CreateBuilder(args);
////////////        var awsOptions = new AWSOptions
////////////        {
////////////            Credentials = new BasicAWSCredentials(
////////////              builder.Configuration["AWS_ACCESS_KEY"],
////////////              builder.Configuration["AWS_SECRET_KEY"]),
////////////            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
////////////        };

////////////        builder.Services.AddDefaultAWSOptions(awsOptions);
////////////        builder.Services.AddAWSService<IAmazonS3>();
////////////        builder.Services.AddAWSService<IAmazonTranscribeService>();
////////////        builder.Services.AddControllers().AddJsonOptions(options =>
////////////        {
////////////            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////////////            options.JsonSerializerOptions.WriteIndented = true;
////////////        });

////////////        // הגדרת מגבלת גודל קובץ
////////////        builder.Services.Configure<FormOptions>(options =>
////////////        {
////////////            options.MultipartBodyLengthLimit = 104857600; // 100MB
////////////        });

////////////        builder.Services.AddEndpointsApiExplorer();
////////////        builder.Services.AddAuthorization(options =>
////////////        {
////////////            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
////////////            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
////////////            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
////////////        });
////////////        builder.Services.AddControllers()
////////////       .AddJsonOptions(x =>
////////////       {
////////////            x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
////////////       });

////////////        builder.Services.AddSwaggerGen(options =>
////////////        {
////////////            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////////////            {
////////////                Scheme = "Bearer",
////////////                BearerFormat = "JWT",
////////////                In = ParameterLocation.Header,
////////////                Name = "Authorization",
////////////                Description = "Bearer Authentication with JWT Token",
////////////                Type = SecuritySchemeType.Http
////////////            });
////////////            options.AddSecurityRequirement(new OpenApiSecurityRequirement
////////////            {
////////////                    {
////////////                        new OpenApiSecurityScheme
////////////                        {
////////////                            Reference = new OpenApiReference
////////////                            {
////////////                                Id = "Bearer",
////////////                                Type = ReferenceType.SecurityScheme
////////////                            }
////////////                        },
////////////                        new List<string>()
////////////                    }
////////////            });

////////////        });

////////////        builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
////////////        {
////////////            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
////////////        }));

////////////        builder.Services.AddScoped<AuthService>();
////////////        builder.Services.AddScoped<IUserService, UserService>();
////////////        builder.Services.AddScoped<IUserRepository, UserRepository>();
////////////        builder.Services.AddScoped<IFolderService, FolderService>();
////////////        builder.Services.AddScoped<IFolderRepository, FolderRepository>();


////////////        builder.Services.AddDbContext<DataContext>(options =>
////////////        {
////////////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
////////////                ?? Environment.GetEnvironmentVariable("DefaultConnection");

////////////            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
////////////        });
////////////        //builder.Services.AddDbContext<DataContext>(options =>
////////////            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
////////////        builder.Services.AddAutoMapper(typeof(MappingProfile));

////////////        builder.Services.AddAuthentication(options =>
////////////        {
////////////            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
////////////            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
////////////        })
////////////        .AddJwtBearer(options =>
////////////        {
////////////            options.TokenValidationParameters = new TokenValidationParameters
////////////            {
////////////                ValidateIssuer = true,
////////////                ValidateAudience = true,
////////////                ValidateLifetime = true,
////////////                ValidateIssuerSigningKey = true,
////////////                ValidIssuer = builder.Configuration["JWT:Issuer"],
////////////                ValidAudience = builder.Configuration["JWT:Audience"],
////////////                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
////////////            };
////////////        });

////////////        //builder.Services.AddCors(options =>
////////////        //{
////////////        //    options.AddPolicy("AllowNextJS", policy =>
////////////        //    {
////////////        //        policy.WithOrigins("http://localhost:3000")
////////////        //              .AllowAnyHeader()
////////////        //              .AllowAnyMethod()
////////////        //              .AllowCredentials();
////////////        //    });
////////////        //});


////////////        var app = builder.Build();
////////////        //app.UseCors("AllowNextJS");
////////////        // Configure the HTTP request pipeline.
////////////        if (app.Environment.IsDevelopment())
////////////        {
////////////            app.UseSwagger();
////////////            app.UseSwaggerUI();
////////////        }

////////////        app.UseHttpsRedirection();
////////////        app.UseCors("MyPolicy");
////////////        app.UseAuthentication();
////////////        app.UseAuthorization();
////////////        app.MapControllers();
////////////        app.MapGet("/", () => "API is running!");
////////////        app.Run();
////////////    }
////////////}
//////////using FinalProject.Core.Services;
//////////using FinalProject.Core;
//////////using FinalProject.Data.Repositories;
//////////using FinalProject.Service;
//////////using FinalProject;
//////////using Microsoft.AspNetCore.Authentication.JwtBearer;
//////////using Microsoft.EntityFrameworkCore;
//////////using Microsoft.IdentityModel.Tokens;
//////////using Microsoft.OpenApi.Models;
//////////using System.Text.Json.Serialization;
//////////using System.Text;
//////////using Microsoft.AspNetCore.Http.Features;
//////////using FinalProject.Core.Repositories;
//////////using Amazon.S3;
//////////using Amazon.Extensions.NETCore.Setup;
//////////using Amazon.Runtime;
//////////using Amazon;
//////////using dotenv.net;
//////////using Amazon.TranscribeService;
//////////using DotNetEnv;
//////////using System.Text.Json;
//////////using MySqlConnector;

//////////public class Program
//////////{
//////////    public static void Main(string[] args)
//////////    {
//////////        Env.Load();
//////////        var builder = WebApplication.CreateBuilder(args);

//////////        // AWS
//////////        var awsOptions = new AWSOptions
//////////        {
//////////            Credentials = new BasicAWSCredentials(
//////////              builder.Configuration["AWS_ACCESS_KEY"],
//////////              builder.Configuration["AWS_SECRET_KEY"]),
//////////            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
//////////        };
//////////        builder.Services.AddDefaultAWSOptions(awsOptions);
//////////        builder.Services.AddAWSService<IAmazonS3>();
//////////        builder.Services.AddAWSService<IAmazonTranscribeService>();

//////////        // Controllers + JSON Options
//////////        builder.Services.AddControllers().AddJsonOptions(options =>
//////////        {
//////////            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//////////            options.JsonSerializerOptions.WriteIndented = true;
//////////            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//////////        });

//////////        // CORS – מתאים ל־Render
//////////        builder.Services.AddCors(options =>
//////////        {
//////////            options.AddPolicy("RenderPolicy", policy =>
//////////            {
//////////                policy.WithOrigins("https://learnahead.onrender.com")
//////////                      .AllowAnyHeader()
//////////                      .AllowAnyMethod();
//////////            });
//////////        });

//////////        // File Upload Limit
//////////        builder.Services.Configure<FormOptions>(options =>
//////////        {
//////////            options.MultipartBodyLengthLimit = 104857600; // 100MB
//////////        });

//////////        // Swagger
//////////        builder.Services.AddSwaggerGen(options =>
//////////        {
//////////            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//////////            {
//////////                Scheme = "Bearer",
//////////                BearerFormat = "JWT",
//////////                In = ParameterLocation.Header,
//////////                Name = "Authorization",
//////////                Description = "Bearer Authentication with JWT Token",
//////////                Type = SecuritySchemeType.Http
//////////            });
//////////            options.AddSecurityRequirement(new OpenApiSecurityRequirement
//////////            {
//////////                {
//////////                    new OpenApiSecurityScheme
//////////                    {
//////////                        Reference = new OpenApiReference
//////////                        {
//////////                            Id = "Bearer",
//////////                            Type = ReferenceType.SecurityScheme
//////////                        }
//////////                    },
//////////                    new List<string>()
//////////                }
//////////            });
//////////        });

//////////        // Roles Policies
//////////        builder.Services.AddAuthorization(options =>
//////////        {
//////////            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//////////            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
//////////            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
//////////        });

//////////        // Auth + Repos
//////////        builder.Services.AddScoped<AuthService>();
//////////        builder.Services.AddScoped<IUserService, UserService>();
//////////        builder.Services.AddScoped<IUserRepository, UserRepository>();
//////////        builder.Services.AddScoped<IFolderService, FolderService>();
//////////        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

//////////        // EF Core + MySQL
//////////        builder.Services.AddDbContext<DataContext>(options =>
//////////        {
//////////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//////////                ?? Environment.GetEnvironmentVariable("DefaultConnection");

//////////            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//////////        });

//////////        builder.Services.AddAutoMapper(typeof(MappingProfile));

//////////        // JWT Auth
//////////        builder.Services.AddAuthentication(options =>
//////////        {
//////////            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//////////            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//////////        })
//////////        .AddJwtBearer(options =>
//////////        {
//////////            options.TokenValidationParameters = new TokenValidationParameters
//////////            {
//////////                ValidateIssuer = true,
//////////                ValidateAudience = true,
//////////                ValidateLifetime = true,
//////////                ValidateIssuerSigningKey = true,
//////////                ValidIssuer = builder.Configuration["JWT:Issuer"],
//////////                ValidAudience = builder.Configuration["JWT:Audience"],
//////////                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
//////////            };
//////////        });

//////////        var app = builder.Build();

//////////        if (app.Environment.IsDevelopment())
//////////        {
//////////            app.UseSwagger();
//////////            app.UseSwaggerUI();
//////////        }

//////////        app.UseHttpsRedirection();
//////////        app.UseCors("RenderPolicy"); // CORS חייב להיות כאן
//////////        app.UseAuthentication();
//////////        app.UseAuthorization();

//////////        app.MapControllers();
//////////        app.MapGet("/", () => "API is running!");

//////////        app.Run();
//////////    }
//////////}
////////using FinalProject.Core.Services;
////////using FinalProject.Core;
////////using FinalProject.Data.Repositories;
////////using FinalProject.Service;
////////using FinalProject;
////////using Microsoft.AspNetCore.Authentication.JwtBearer;
////////using Microsoft.EntityFrameworkCore;
////////using Microsoft.IdentityModel.Tokens;
////////using Microsoft.OpenApi.Models;
////////using System.Text.Json.Serialization;
////////using System.Text;
////////using Microsoft.AspNetCore.Http.Features;
////////using FinalProject.Core.Repositories;
////////using Amazon.S3;
////////using Amazon.Extensions.NETCore.Setup;
////////using Amazon.Runtime;
////////using Amazon;
////////using dotenv.net;
////////using Amazon.TranscribeService;
////////using DotNetEnv;
////////using System.Text.Json;
////////using MySqlConnector;

////////public class Program
////////{
////////    public static void Main(string[] args)
////////    {
////////        Env.Load();
////////        var builder = WebApplication.CreateBuilder(args);

////////        // AWS
////////        var awsOptions = new AWSOptions
////////        {
////////            Credentials = new BasicAWSCredentials(
////////              builder.Configuration["AWS_ACCESS_KEY"],
////////              builder.Configuration["AWS_SECRET_KEY"]),
////////            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
////////        };
////////        builder.Services.AddDefaultAWSOptions(awsOptions);
////////        builder.Services.AddAWSService<IAmazonS3>();
////////        builder.Services.AddAWSService<IAmazonTranscribeService>();

////////        // Controllers + JSON Options
////////        builder.Services.AddControllers().AddJsonOptions(options =>
////////        {
////////            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////////            options.JsonSerializerOptions.WriteIndented = true;
////////            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
////////        });

////////        // CORS – מתאים ל־Render
////////        builder.Services.AddCors(options =>
////////        {
////////            options.AddPolicy("RenderPolicy", policy =>
////////            {
////////                policy.WithOrigins("https://learnahead.onrender.com")
////////                      .AllowAnyHeader()
////////                      .AllowAnyMethod();
////////                // אם בעתיד תצטרך Cookie/Authorization:
////////                // policy.AllowCredentials();
////////            });
////////        });

////////        // File Upload Limit
////////        builder.Services.Configure<FormOptions>(options =>
////////        {
////////            options.MultipartBodyLengthLimit = 104857600; // 100MB
////////        });

////////        // Swagger
////////        builder.Services.AddSwaggerGen(options =>
////////        {
////////            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////////            {
////////                Scheme = "Bearer",
////////                BearerFormat = "JWT",
////////                In = ParameterLocation.Header,
////////                Name = "Authorization",
////////                Description = "Bearer Authentication with JWT Token",
////////                Type = SecuritySchemeType.Http
////////            });
////////            options.AddSecurityRequirement(new OpenApiSecurityRequirement
////////            {
////////                {
////////                    new OpenApiSecurityScheme
////////                    {
////////                        Reference = new OpenApiReference
////////                        {
////////                            Id = "Bearer",
////////                            Type = ReferenceType.SecurityScheme
////////                        }
////////                    },
////////                    new List<string>()
////////                }
////////            });
////////        });

////////        // Roles Policies
////////        builder.Services.AddAuthorization(options =>
////////        {
////////            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
////////            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
////////            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
////////        });

////////        // Auth + Repos
////////        builder.Services.AddScoped<AuthService>();
////////        builder.Services.AddScoped<IUserService, UserService>();
////////        builder.Services.AddScoped<IUserRepository, UserRepository>();
////////        builder.Services.AddScoped<IFolderService, FolderService>();
////////        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

////////        // EF Core + MySQL
////////        builder.Services.AddDbContext<DataContext>(options =>
////////        {
////////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
////////                ?? Environment.GetEnvironmentVariable("DefaultConnection");

////////            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
////////        });

////////        builder.Services.AddAutoMapper(typeof(MappingProfile));

////////        // JWT Auth
////////        builder.Services.AddAuthentication(options =>
////////        {
////////            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
////////            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
////////        })
////////        .AddJwtBearer(options =>
////////        {
////////            options.TokenValidationParameters = new TokenValidationParameters
////////            {
////////                ValidateIssuer = true,
////////                ValidateAudience = true,
////////                ValidateLifetime = true,
////////                ValidateIssuerSigningKey = true,
////////                ValidIssuer = builder.Configuration["JWT:Issuer"],
////////                ValidAudience = builder.Configuration["JWT:Audience"],
////////                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
////////            };
////////        });

////////        var app = builder.Build();

////////        if (app.Environment.IsDevelopment())
////////        {
////////            app.UseSwagger();
////////            app.UseSwaggerUI();
////////        }

////////        app.UseHttpsRedirection();

////////        // חשוב: UseCors חייב לבוא לפני UseAuthentication ו-UseAuthorization
////////        app.UseCors("RenderPolicy");

////////        app.UseAuthentication();
////////        app.UseAuthorization();

////////        app.MapControllers();

////////        app.MapGet("/", () => "API is running!");

////////        app.Run();
////////    }
////////}
using FinalProject.Core.Services;
using FinalProject.Core;
using FinalProject.Data.Repositories;
using FinalProject.Service;
using FinalProject;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using FinalProject.Core.Repositories;
using Amazon.S3;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime;
using Amazon;
using dotenv.net;
using Amazon.TranscribeService;
using DotNetEnv;
using System.Text.Json;
using MySqlConnector;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);

        // AWS
        var awsOptions = new AWSOptions
        {
            Credentials = new BasicAWSCredentials(
              builder.Configuration["AWS_ACCESS_KEY"],
              builder.Configuration["AWS_SECRET_KEY"]),
            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
        };
        builder.Services.AddDefaultAWSOptions(awsOptions);
        builder.Services.AddAWSService<IAmazonS3>();
        builder.Services.AddAWSService<IAmazonTranscribeService>();

        // Controllers + JSON Options
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            options.JsonSerializerOptions.WriteIndented = true;
            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        // CORS – מתאים ל־Render
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("RenderPolicy", policy =>
            {
                policy.WithOrigins("https://learnahead.onrender.com")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
                // אם תשתמש ב־cookies:
                // policy.AllowCredentials();
            });
        });

        // File Upload Limit
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 104857600; // 100MB
        });

        // Swagger
        builder.Services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = "Authorization",
                Description = "Bearer Authentication with JWT Token",
                Type = SecuritySchemeType.Http
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Id = "Bearer",
                            Type = ReferenceType.SecurityScheme
                        }
                    },
                    new List<string>()
                }
            });
        });

        // Roles Policies
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
        });

        // Services and Repositories
        builder.Services.AddScoped<AuthService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IFolderService, FolderService>();
        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

        // EF Core + MySQL
        builder.Services.AddDbContext<DataContext>(options =>
        {
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? Environment.GetEnvironmentVariable("DefaultConnection");

            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        builder.Services.AddAutoMapper(typeof(MappingProfile));

        // JWT Auth
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["JWT:Issuer"],
                ValidAudience = builder.Configuration["JWT:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
            };
        });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        // פתרון שגיאת Preflight CORS
        app.Use(async (context, next) =>
        {
            if (context.Request.Method == HttpMethods.Options)
            {
                context.Response.StatusCode = 200;
                await context.Response.CompleteAsync();
                return;
            }

            await next();
        });

        // סדר קריטי: CORS לפני Auth ו-Authz
        app.UseCors("RenderPolicy");

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapGet("/", () => "API is running!");

        app.Run();
    }
}
////using FinalProject.Core.Services;
////using FinalProject.Core;
////using FinalProject.Data.Repositories;
////using FinalProject.Service;
////using FinalProject;
////using Microsoft.AspNetCore.Authentication.JwtBearer;
////using Microsoft.EntityFrameworkCore;
////using Microsoft.IdentityModel.Tokens;
////using Microsoft.OpenApi.Models;
////using System.Text.Json.Serialization;
////using System.Text;
////using Microsoft.AspNetCore.Http.Features;
////using FinalProject.Core.Repositories;
////using Amazon.S3;
////using Amazon.Extensions.NETCore.Setup;
////using Amazon.Runtime;
////using Amazon;
////using dotenv.net;
////using Amazon.TranscribeService;
////using DotNetEnv;
////using System.Text.Json;
////using MySqlConnector;

////public class Program
////{
////    public static void Main(string[] args)
////    {
////        Env.Load();
////        var builder = WebApplication.CreateBuilder(args);

////        // AWS Configuration
////        var awsOptions = new AWSOptions
////        {
////            Credentials = new BasicAWSCredentials(
////              builder.Configuration["AWS_ACCESS_KEY"],
////              builder.Configuration["AWS_SECRET_KEY"]),
////            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
////        };
////        builder.Services.AddDefaultAWSOptions(awsOptions);
////        builder.Services.AddAWSService<IAmazonS3>();
////        builder.Services.AddAWSService<IAmazonTranscribeService>();

////        // JSON Options
////        builder.Services.AddControllers().AddJsonOptions(options =>
////        {
////            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////            options.JsonSerializerOptions.WriteIndented = true;
////            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
////        });

////        // CORS for client domain
////        builder.Services.AddCors(options =>
////        {
////            options.AddPolicy("RenderPolicy", policy =>
////            {
////                policy.WithOrigins("https://learnahead.onrender.com")
////                      .AllowAnyHeader()
////                      .AllowAnyMethod();
////            });
////        });

////        // File Upload Limit
////        builder.Services.Configure<FormOptions>(options =>
////        {
////            options.MultipartBodyLengthLimit = 104857600; // 100MB
////        });

////        // Swagger (OpenAPI)
////        builder.Services.AddSwaggerGen(options =>
////        {
////            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
////            {
////                Scheme = "Bearer",
////                BearerFormat = "JWT",
////                In = ParameterLocation.Header,
////                Name = "Authorization",
////                Description = "Bearer Authentication with JWT Token",
////                Type = SecuritySchemeType.Http
////            });
////            options.AddSecurityRequirement(new OpenApiSecurityRequirement
////            {
////                {
////                    new OpenApiSecurityScheme
////                    {
////                        Reference = new OpenApiReference
////                        {
////                            Id = "Bearer",
////                            Type = ReferenceType.SecurityScheme
////                        }
////                    },
////                    new List<string>()
////                }
////            });
////        });

////        // Authorization Policies
////        builder.Services.AddAuthorization(options =>
////        {
////            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
////            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
////            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
////        });

////        // Services and Repositories
////        builder.Services.AddScoped<AuthService>();
////        builder.Services.AddScoped<IUserService, UserService>();
////        builder.Services.AddScoped<IUserRepository, UserRepository>();
////        builder.Services.AddScoped<IFolderService, FolderService>();
////        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

////        // Database Configuration (MySQL)
////        builder.Services.AddDbContext<DataContext>(options =>
////        {
////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
////                ?? Environment.GetEnvironmentVariable("DefaultConnection");

////            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
////        });

////        builder.Services.AddAutoMapper(typeof(MappingProfile));

////        // JWT Authentication
////        builder.Services.AddAuthentication(options =>
////        {
////            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
////            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
////        })
////        .AddJwtBearer(options =>
////        {
////            options.TokenValidationParameters = new TokenValidationParameters
////            {
////                ValidateIssuer = true,
////                ValidateAudience = true,
////                ValidateLifetime = true,
////                ValidateIssuerSigningKey = true,
////                ValidIssuer = builder.Configuration["JWT:Issuer"],
////                ValidAudience = builder.Configuration["JWT:Audience"],
////                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
////            };
////        });

////        var app = builder.Build();

////        // Middleware
////        //if (app.Environment.IsDevelopment())
////        //{
////            app.UseSwagger();
////            app.UseSwaggerUI();
////        //}

////        app.UseHttpsRedirection();

////        // Middleware to respond to OPTIONS requests quickly (CORS Preflight)
////        app.Use(async (context, next) =>
////        {
////            if (context.Request.Method == HttpMethods.Options)
////            {
////                context.Response.StatusCode = 200;
////                await context.Response.CompleteAsync();
////                return;
////            }

////            await next();
////        });

////        // CORS must come before Auth
////        app.UseCors("RenderPolicy");

////        app.UseAuthentication();
////        app.UseAuthorization();

////        app.MapControllers();

////        app.MapGet("/", () => "API is running!");

////        app.Run();
////    }
////}
//using FinalProject.Core.Services;
//using FinalProject.Core;
//using FinalProject.Data.Repositories;
//using FinalProject.Service;
//using FinalProject;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.Text.Json.Serialization;
//using System.Text;
//using Microsoft.AspNetCore.Http.Features;
//using FinalProject.Core.Repositories;
//using Amazon.S3;
//using Amazon.Extensions.NETCore.Setup;
//using Amazon.Runtime;
//using Amazon;
//using dotenv.net;
//using Amazon.TranscribeService;
//using DotNetEnv;
//using System.Text.Json;
//using MySqlConnector;

//public class Program
//{
//    public static void Main(string[] args)
//    {
//        Env.Load();
//        var builder = WebApplication.CreateBuilder(args);

//        // AWS Configuration
//        var awsOptions = new AWSOptions
//        {
//            Credentials = new BasicAWSCredentials(
//              builder.Configuration["AWS_ACCESS_KEY"],
//              builder.Configuration["AWS_SECRET_KEY"]),
//            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
//        };
//        builder.Services.AddDefaultAWSOptions(awsOptions);
//        builder.Services.AddAWSService<IAmazonS3>();
//        builder.Services.AddAWSService<IAmazonTranscribeService>();

//        // JSON Options
//        builder.Services.AddControllers().AddJsonOptions(options =>
//        {
//            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//            options.JsonSerializerOptions.WriteIndented = true;
//            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//        });

//        // CORS for client domain
//        builder.Services.AddCors(options =>
//        {
//            options.AddPolicy("RenderPolicy", policy =>
//            {
//                policy.WithOrigins("https://learnahead.onrender.com")
//                      .AllowAnyHeader()
//                      .AllowAnyMethod();
//                //.AllowCredentials(); // רק אם תשתמשי עם cookies/credentials
//            });
//        });

//        // File Upload Limit
//        builder.Services.Configure<FormOptions>(options =>
//        {
//            options.MultipartBodyLengthLimit = 104857600; // 100MB
//        });

//        // Swagger (OpenAPI)
//        builder.Services.AddSwaggerGen(options =>
//        {
//            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//            {
//                Scheme = "Bearer",
//                BearerFormat = "JWT",
//                In = ParameterLocation.Header,
//                Name = "Authorization",
//                Description = "Bearer Authentication with JWT Token",
//                Type = SecuritySchemeType.Http
//            });
//            options.AddSecurityRequirement(new OpenApiSecurityRequirement
//            {
//                {
//                    new OpenApiSecurityScheme
//                    {
//                        Reference = new OpenApiReference
//                        {
//                            Id = "Bearer",
//                            Type = ReferenceType.SecurityScheme
//                        }
//                    },
//                    new List<string>()
//                }
//            });
//        });

//        // Authorization Policies
//        builder.Services.AddAuthorization(options =>
//        {
//            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
//            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
//        });

//        // Services and Repositories
//        builder.Services.AddScoped<AuthService>();
//        builder.Services.AddScoped<IUserService, UserService>();
//        builder.Services.AddScoped<IUserRepository, UserRepository>();
//        builder.Services.AddScoped<IFolderService, FolderService>();
//        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

//        // Database Configuration (MySQL)
//        builder.Services.AddDbContext<DataContext>(options =>
//        {
//            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//                ?? Environment.GetEnvironmentVariable("DefaultConnection");

//            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//        });

//        builder.Services.AddAutoMapper(typeof(MappingProfile));

//        // JWT Authentication
//        builder.Services.AddAuthentication(options =>
//        {
//            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//        })
//        .AddJwtBearer(options =>
//        {
//            options.TokenValidationParameters = new TokenValidationParameters
//            {
//                ValidateIssuer = true,
//                ValidateAudience = true,
//                ValidateLifetime = true,
//                ValidateIssuerSigningKey = true,
//                ValidIssuer = builder.Configuration["JWT:Issuer"],
//                ValidAudience = builder.Configuration["JWT:Audience"],
//                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
//            };
//        });

//        var app = builder.Build();

//        // Middleware
//        app.UseSwagger();
//        app.UseSwaggerUI();

//        app.UseHttpsRedirection();

//        // CORS must come before Authentication and Authorization
//        app.UseCors("RenderPolicy");

//        app.UseAuthentication();
//        app.UseAuthorization();

//        app.MapControllers();

//        app.MapGet("/", () => "API is running!");

//        app.Run();
//    }
//}
//using FinalProject.Data;
//using FinalProject.Service;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using System.Text.Json.Serialization;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using DotNetEnv;
//using FinalProject.Core.Services;
//using FinalProject;
//using FinalProject.Core.Repositories;
//using FinalProject.Data.Repositories;

//var builder = WebApplication.CreateBuilder(args);

//// ------------------- Load environment variables -------------------
//Env.Load(); // Ensure DotNetEnv is installed via NuGet

//var dbConnectionString = Environment.GetEnvironmentVariable("DefaultConnection");
//var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer");
//var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience");
//var jwtKey = Environment.GetEnvironmentVariable("JWT__Key");

//// ------------------- Configure DB -------------------
//builder.Services.AddDbContext<DataContext>(options =>
//    options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

//// ------------------- Configure CORS -------------------
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy
//            .WithOrigins("https://learnahead.onrender.com") // Your frontend URL
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

//// ------------------- Configure Controllers -------------------
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//// ------------------- Swagger -------------------
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinalProject API", Version = "v1" });
//});

//// ------------------- Dependency Injection -------------------
////builder.Services.AddScoped<IUserService, UserService>();
////builder.Services.AddScoped<IFolderService, FolderService>();
////builder.Services.AddScoped<IUploadService, UploadService>();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IFolderService, FolderService>();
//builder.Services.AddScoped<IFolderRepository, FolderRepository>();
//// ------------------- Authentication & JWT -------------------
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = jwtIssuer,

//            ValidateAudience = true,
//            ValidAudience = jwtAudience,

//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),

//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero
//        };
//    });

//// ------------------- Build & Run -------------------
//var app = builder.Build();

//app.UseCors("AllowFrontend");

//app.UseAuthentication();
//app.UseAuthorization();

//app.MapControllers();


//app.UseSwagger();
//app.UseSwaggerUI();

//app.MapGet("/", () => "API is running!");

//app.Run();





//using FinalProject.Data;
//using FinalProject.Service;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.OpenApi.Models;
//using System.Text.Json.Serialization;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;
//using DotNetEnv;
//using FinalProject.Core.Services;
//using FinalProject;
//using FinalProject.Core.Repositories;
//using FinalProject.Data.Repositories;

//var builder = WebApplication.CreateBuilder(args);

//// ------------------- Load environment variables -------------------
//Env.Load(); // Ensure DotNetEnv is installed via NuGet

//var dbConnectionString = Environment.GetEnvironmentVariable("DefaultConnection");
//var jwtIssuer = Environment.GetEnvironmentVariable("JWT__Issuer");
//var jwtAudience = Environment.GetEnvironmentVariable("JWT__Audience");
//var jwtKey = Environment.GetEnvironmentVariable("JWT__Key");

//// ------------------- Configure DB -------------------
//builder.Services.AddDbContext<DataContext>(options =>
//    options.UseMySql(dbConnectionString, ServerVersion.AutoDetect(dbConnectionString)));

//// ------------------- Configure CORS -------------------
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowFrontend", policy =>
//    {
//        policy
//            .WithOrigins("https://learnahead.onrender.com")
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

//// ------------------- Configure Controllers -------------------
//builder.Services.AddControllers()
//    .AddJsonOptions(options =>
//        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

//// ------------------- Swagger -------------------
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FinalProject API", Version = "v1" });
//});

//// ------------------- Dependency Injection -------------------
//builder.Services.AddScoped<AuthService>();
//builder.Services.AddScoped<IUserService, UserService>();
//builder.Services.AddScoped<IUserRepository, UserRepository>();
//builder.Services.AddScoped<IFolderService, FolderService>();
//builder.Services.AddScoped<IFolderRepository, FolderRepository>();

//// ------------------- Authentication & JWT -------------------
//builder.Services.AddAuthentication("Bearer")
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.RequireHttpsMetadata = false;
//        options.SaveToken = true;

//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidIssuer = jwtIssuer,
//            ValidateAudience = true,
//            ValidAudience = jwtAudience,
//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!)),
//            ValidateLifetime = true,
//            ClockSkew = TimeSpan.Zero
//        };
//    });

//// ------------------- Build & Configure -------------------
//var app = builder.Build();

//// ✅ סדר תקין של קריאות middleware
//app.UseRouting(); // ← חובה לפני CORS/Auth

//app.UseCors("AllowFrontend");

//app.UseAuthentication();
//app.UseAuthorization();

//app.UseSwagger();
//app.UseSwaggerUI();

//app.MapControllers();
//app.MapGet("/", () => "API is running!");

//app.Run();
