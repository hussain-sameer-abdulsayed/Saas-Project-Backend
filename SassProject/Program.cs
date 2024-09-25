using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SassProject.Data;
using SassProject.IRepos;
using SassProject.Repos;
using SassProject.SassMapper;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("ContextConnection") ?? throw new InvalidOperationException("Connection string ContextConnection not found.");

        builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connectionString));
        // Add Identity and RoleManager
        builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddEntityFrameworkStores<Context>()
            .AddDefaultTokenProviders();

        // Add services to the container.


        builder.Services.AddScoped<IUserRepo, UserRepo>();
        builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
        builder.Services.AddScoped<IimageRepo, ImageRepo>();
        builder.Services.AddScoped<IProductRepo, ProductRepo>();
        builder.Services.AddScoped<IOrderRepo, OrderRepo>();
        builder.Services.AddScoped<IJWTMangerRepo, JWTManagerRepo>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();

        

        builder.Services.AddAutoMapper(typeof(DataMapper));


        builder.Services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            var Key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
            o.SaveToken = true;
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };
        });

            builder.Services.AddControllers()
            .AddJsonOptions(x =>
            {
                x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());// convrt enum numbers to string
                x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            });



        // request limiter
        builder.Services.AddRateLimiter(options =>
        {
            options.AddConcurrencyLimiter("concurrencyPolicy", opt =>
            {
                opt.PermitLimit = 5;
                opt.QueueLimit = 5;
                opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
            }).RejectionStatusCode = 429;
        });


        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Saas",
                Version = "v1"
            });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter Bearer [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 1safsfsdfdfd\"",
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        {
            new OpenApiSecurityScheme {
                Reference = new OpenApiReference {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
        });
        // frontend seetup
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("reactApp", policy =>
            {
                policy.WithOrigins("http://localhost:3000")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });


        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseStaticFiles(); // Ensure static files are served (the problem i encounterd for image loading)

        app.UseHttpsRedirection();

        app.UseCors("reactApp"); // frontend seetup
        app.UseAuthentication();
        app.UseAuthorization();

        
        app.UseRateLimiter(); // request limiter

        app.MapControllers();

        app.Run();
    }
}