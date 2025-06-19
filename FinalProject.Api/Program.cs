//////using FinalProject.Core.Services;
//////using FinalProject.Core;
//////using FinalProject.Data.Repositories;
//////using FinalProject.Service;
//////using FinalProject;
//////using Microsoft.AspNetCore.Authentication.JwtBearer;
//////using Microsoft.EntityFrameworkCore;
//////using Microsoft.IdentityModel.Tokens;
//////using Microsoft.OpenApi.Models;
//////using System.Text.Json.Serialization;
//////using System.Text;
//////using Microsoft.AspNetCore.Http.Features;
//////using FinalProject.Core.Repositories;
//////using Amazon.S3;
//////using Amazon.Extensions.NETCore.Setup;
//////using Amazon.Runtime;
//////using Amazon;
//////using dotenv.net;
//////using Amazon.TranscribeService;
//////using Google.Api;
//////using DotNetEnv;
//////using System.Text.Json;
//////using MySqlConnector; // ודא שהוספת את החבילה

//////public class Program
//////{
//////    public static void Main(string[] args)
//////    {
//////        Env.Load();
//////        var builder = WebApplication.CreateBuilder(args);
//////        var awsOptions = new AWSOptions
//////        {
//////            Credentials = new BasicAWSCredentials(
//////              builder.Configuration["AWS_ACCESS_KEY"],
//////              builder.Configuration["AWS_SECRET_KEY"]),
//////            Region = RegionEndpoint.GetBySystemName(builder.Configuration["AWS_REGION"])
//////        };

//////        builder.Services.AddDefaultAWSOptions(awsOptions);
//////        builder.Services.AddAWSService<IAmazonS3>();
//////        builder.Services.AddAWSService<IAmazonTranscribeService>();
//////        builder.Services.AddControllers().AddJsonOptions(options =>
//////        {
//////            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//////            options.JsonSerializerOptions.WriteIndented = true;
//////        });

//////        // הגדרת מגבלת גודל קובץ
//////        builder.Services.Configure<FormOptions>(options =>
//////        {
//////            options.MultipartBodyLengthLimit = 104857600; // 100MB
//////        });

//////        builder.Services.AddEndpointsApiExplorer();
//////        builder.Services.AddAuthorization(options =>
//////        {
//////            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//////            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
//////            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
//////        });
//////        builder.Services.AddControllers()
//////       .AddJsonOptions(x =>
//////       {
//////            x.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//////       });

//////        builder.Services.AddSwaggerGen(options =>
//////        {
//////            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//////            {
//////                Scheme = "Bearer",
//////                BearerFormat = "JWT",
//////                In = ParameterLocation.Header,
//////                Name = "Authorization",
//////                Description = "Bearer Authentication with JWT Token",
//////                Type = SecuritySchemeType.Http
//////            });
//////            options.AddSecurityRequirement(new OpenApiSecurityRequirement
//////            {
//////                    {
//////                        new OpenApiSecurityScheme
//////                        {
//////                            Reference = new OpenApiReference
//////                            {
//////                                Id = "Bearer",
//////                                Type = ReferenceType.SecurityScheme
//////                            }
//////                        },
//////                        new List<string>()
//////                    }
//////            });

//////        });

//////        builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
//////        {
//////            policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//////        }));

//////        builder.Services.AddScoped<AuthService>();
//////        builder.Services.AddScoped<IUserService, UserService>();
//////        builder.Services.AddScoped<IUserRepository, UserRepository>();
//////        builder.Services.AddScoped<IFolderService, FolderService>();
//////        builder.Services.AddScoped<IFolderRepository, FolderRepository>();


//////        builder.Services.AddDbContext<DataContext>(options =>
//////        {
//////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//////                ?? Environment.GetEnvironmentVariable("DefaultConnection");

//////            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//////        });
//////        //builder.Services.AddDbContext<DataContext>(options =>
//////            //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//////        builder.Services.AddAutoMapper(typeof(MappingProfile));

//////        builder.Services.AddAuthentication(options =>
//////        {
//////            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//////            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//////        })
//////        .AddJwtBearer(options =>
//////        {
//////            options.TokenValidationParameters = new TokenValidationParameters
//////            {
//////                ValidateIssuer = true,
//////                ValidateAudience = true,
//////                ValidateLifetime = true,
//////                ValidateIssuerSigningKey = true,
//////                ValidIssuer = builder.Configuration["JWT:Issuer"],
//////                ValidAudience = builder.Configuration["JWT:Audience"],
//////                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
//////            };
//////        });

//////        //builder.Services.AddCors(options =>
//////        //{
//////        //    options.AddPolicy("AllowNextJS", policy =>
//////        //    {
//////        //        policy.WithOrigins("http://localhost:3000")
//////        //              .AllowAnyHeader()
//////        //              .AllowAnyMethod()
//////        //              .AllowCredentials();
//////        //    });
//////        //});


//////        var app = builder.Build();
//////        //app.UseCors("AllowNextJS");
//////        // Configure the HTTP request pipeline.
//////        if (app.Environment.IsDevelopment())
//////        {
//////            app.UseSwagger();
//////            app.UseSwaggerUI();
//////        }

//////        app.UseHttpsRedirection();
//////        app.UseCors("MyPolicy");
//////        app.UseAuthentication();
//////        app.UseAuthorization();
//////        app.MapControllers();
//////        app.MapGet("/", () => "API is running!");
//////        app.Run();
//////    }
//////}
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

////        // AWS
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

////        // Controllers + JSON Options
////        builder.Services.AddControllers().AddJsonOptions(options =>
////        {
////            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////            options.JsonSerializerOptions.WriteIndented = true;
////            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
////        });

////        // CORS – מתאים ל־Render
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

////        // Swagger
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

////        // Roles Policies
////        builder.Services.AddAuthorization(options =>
////        {
////            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
////            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
////            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
////        });

////        // Auth + Repos
////        builder.Services.AddScoped<AuthService>();
////        builder.Services.AddScoped<IUserService, UserService>();
////        builder.Services.AddScoped<IUserRepository, UserRepository>();
////        builder.Services.AddScoped<IFolderService, FolderService>();
////        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

////        // EF Core + MySQL
////        builder.Services.AddDbContext<DataContext>(options =>
////        {
////            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
////                ?? Environment.GetEnvironmentVariable("DefaultConnection");

////            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
////        });

////        builder.Services.AddAutoMapper(typeof(MappingProfile));

////        // JWT Auth
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

////        if (app.Environment.IsDevelopment())
////        {
////            app.UseSwagger();
////            app.UseSwaggerUI();
////        }

////        app.UseHttpsRedirection();
////        app.UseCors("RenderPolicy"); // CORS חייב להיות כאן
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

//        // AWS
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

//        // Controllers + JSON Options
//        builder.Services.AddControllers().AddJsonOptions(options =>
//        {
//            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//            options.JsonSerializerOptions.WriteIndented = true;
//            options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
//        });

//        // CORS – מתאים ל־Render
//        builder.Services.AddCors(options =>
//        {
//            options.AddPolicy("RenderPolicy", policy =>
//            {
//                policy.WithOrigins("https://learnahead.onrender.com")
//                      .AllowAnyHeader()
//                      .AllowAnyMethod();
//                // אם בעתיד תצטרך Cookie/Authorization:
//                // policy.AllowCredentials();
//            });
//        });

//        // File Upload Limit
//        builder.Services.Configure<FormOptions>(options =>
//        {
//            options.MultipartBodyLengthLimit = 104857600; // 100MB
//        });

//        // Swagger
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

//        // Roles Policies
//        builder.Services.AddAuthorization(options =>
//        {
//            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
//            options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
//            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
//        });

//        // Auth + Repos
//        builder.Services.AddScoped<AuthService>();
//        builder.Services.AddScoped<IUserService, UserService>();
//        builder.Services.AddScoped<IUserRepository, UserRepository>();
//        builder.Services.AddScoped<IFolderService, FolderService>();
//        builder.Services.AddScoped<IFolderRepository, FolderRepository>();

//        // EF Core + MySQL
//        builder.Services.AddDbContext<DataContext>(options =>
//        {
//            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
//                ?? Environment.GetEnvironmentVariable("DefaultConnection");

//            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
//        });

//        builder.Services.AddAutoMapper(typeof(MappingProfile));

//        // JWT Auth
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

//        if (app.Environment.IsDevelopment())
//        {
//            app.UseSwagger();
//            app.UseSwaggerUI();
//        }

//        app.UseHttpsRedirection();

//        // חשוב: UseCors חייב לבוא לפני UseAuthentication ו-UseAuthorization
//        app.UseCors("RenderPolicy");

//        app.UseAuthentication();
//        app.UseAuthorization();

//        app.MapControllers();

//        app.MapGet("/", () => "API is running!");

//        app.Run();
//    }
//}
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
