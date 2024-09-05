using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SassProject.Data;
using SassProject.IRepos;
using SassProject.Repos;
using SassProject.SassMapper;
using System.Text.Json.Serialization;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("ContextConnection") ?? throw new InvalidOperationException("Connection string 'ContextConnection' not found.");

builder.Services.AddDbContext<Context>(options => options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<Context>();

// Add services to the container.


builder.Services.AddScoped<ICategoryRepo, CategoryRepo>();
builder.Services.AddScoped<IimageRepo, ImageRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();

builder.Services.AddAutoMapper(typeof(DataMapper));





builder.Services.AddControllers()
    .AddJsonOptions(x =>
        x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
