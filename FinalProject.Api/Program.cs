////using FinalProject.Core.Repositories;
////using FinalProject.Core.Services;
////using FinalProject.Data;
////using FinalProject.Data.Repositories;
////using FinalProject.Infrastructure;
////using FinalProject.Service;
////using System.Text.Json.Serialization;

////namespace TipatChalav
////{
////    public class Program
////    {
////        public static void Main(string[] args)
////        {
////            var builder = WebApplication.CreateBuilder(args);

////            // Add services to the container.

////            builder.Services.AddControllers().AddJsonOptions(options =>
////            {
////                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
////                options.JsonSerializerOptions.WriteIndented = true;
////            });
////            builder.Services.AddEndpointsApiExplorer();
////            builder.Services.AddSwaggerGen();

////            builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
////            {
////                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
////            }));

////            // builder.Services.AddTransient<IDataContext,DataContext>();

////            //builder.Services.AddScoped<IUserRepository, UserService>();
////            builder.Services.AddScoped<IUserRepository, UserRepository>();

////            //builder.Services.AddScoped<ITeacherService, TeacherService>();
////            //builder.Services.AddScoped<ITeacherRepository, TeacherRepository>();

////            builder.Services.AddScoped<IRecordService, RecordService>();
////            builder.Services.AddScoped<IRecordRepository, RecordRepository>();

////            builder.Services.AddDbContext<DataContext>();
////            // builder.Services.AddSingleton<DataContext>();
////            //builder.Services.AddAutoMapper(typeof(MappingProfile));
////            //builder.Services.AddSingleton<Mapping>();

////            var app = builder.Build();

////            // Configure the HTTP request pipeline.
////            if (app.Environment.IsDevelopment())
////            {
////                app.UseSwagger();
////                app.UseSwaggerUI();
////            }

////            app.UseHttpsRedirection();

////            app.UseCors("MyPolicy");

////            app.UseAuthorization();

////            app.MapControllers();

////            app.Run();
////        }
////    }
////}

//////using Microsoft.AspNetCore.Hosting;
//////using Microsoft.Extensions.Hosting;

//////public class Program
//////{
//////    public static void Main(string[] args)
//////    {
//////        CreateHostBuilder(args).Build().Run();
//////    }

//////    public static IHostBuilder CreateHostBuilder(string[] args) =>
//////        Host.CreateDefaultBuilder(args)
//////            .ConfigureWebHostDefaults(webBuilder =>
//////            {
//////                webBuilder.UseStartup<Startup>();
//////            });
//////}

//using FinalProject;
//using FinalProject.Core;
//using FinalProject.Core.Repositories;
//using FinalProject.Core.Services;
//using FinalProject.Data.Repositories;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.OpenApi.Models;

////using FinalProject.Infrastructure;
//using System.Text.Json.Serialization;

//namespace FinalProject
//{
//    public class Program
//    {
//        public static void Main(string[] args)
//        {
//            var builder = WebApplication.CreateBuilder(args);

//            // Add services to the container.

//            builder.Services.AddControllers().AddJsonOptions(options =>
//            {
//                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
//                options.JsonSerializerOptions.WriteIndented = true;
//            });

//            builder.Services.AddEndpointsApiExplorer();
//            builder.Services.AddSwaggerGen();

//            builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
//            {
//                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
//            }));

//            // builder.Services.AddTransient<IDataContext,DataContext>();

//            builder.Services.AddScoped<IUserService, UserService>();
//            builder.Services.AddScoped<IUserRepository, UserRepository>();

//            //builder.Services.AddScoped<IBabyService, BabyService>();
//            //builder.Services.AddScoped<IBabyRepository, BabyRepository>();

//            //builder.Services.AddScoped<INurseService, NurseService>();
//            //builder.Services.AddScoped<INurseRepository, NurseRepository>();

//            builder.Services.AddDbContext<DataContext>();
//            // builder.Services.AddSingleton<DataContext>();
//            builder.Services.AddAutoMapper(typeof(MappingProfile));
//            //builder.Services.AddSingleton<Mapping>();

//            //builder.Services.AddDbContext<DataContext>(options =>
//            //options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
//            //new MySqlServerVersion(new Version(8, 0, 21)))); // יש להתאים לגרסה שלך


//            var app = builder.Build();

//            // Configure the HTTP request pipeline.
//            if (app.Environment.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();

//            app.UseCors("MyPolicy");

//            app.UseAuthorization();

//            app.MapControllers();

//            app.Run();
//        }
//    }
//}

using FinalProject;
using FinalProject.Core;
using FinalProject.Core.Repositories;
using FinalProject.Core.Services;
using FinalProject.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FinalProject.Service;


namespace FinalProject
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                options.JsonSerializerOptions.WriteIndented = true;
            });

            builder.Services.AddEndpointsApiExplorer();
            //builder.Services.AddSwaggerGen();
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
            builder.Services.AddCors(opt => opt.AddPolicy("MyPolicy", policy =>
            {
                policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
            }));

            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IFolderService, FolderService>();
            builder.Services.AddScoped<IFolderRepository, FolderRepository>();
            // הוספת DataContext עם קונפיגורציה
            //builder.Services.AddDbContext<DataContext>(options =>
            //    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
            //    new MySqlServerVersion(new Version(8, 0, 21)))); // יש להתאים לגרסה שלך
            builder.Services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAutoMapper(typeof(MappingProfile));

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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/", () => "API is running!");
            app.Run();
        }
    }
}
