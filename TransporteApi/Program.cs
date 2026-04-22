using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySql.EntityFrameworkCore.Extensions;
using TransporteApi.Models;
using TransporteApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies()); ;

builder.Services.AddScoped(typeof(IServices<,>), typeof(BaseService<,>));
builder.Services.AddScoped<RolService>(); 
builder.Services.AddScoped<RolPermisoService>(); 
builder.Services.AddScoped<UsuarioService>(); 
builder.Services.AddScoped<PropietarioService>(); 
builder.Services.AddScoped<CamionService>(); 
builder.Services.AddScoped<ConductorService>(); 
builder.Services.AddScoped<CentroDistribucionService>(); 
builder.Services.AddScoped<GuiaService>(); 


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySQL(builder.Configuration.GetConnectionString("MySQL")));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    
    app.Urls.Add("http://*:5071");
    app.Urls.Add("https://*:7162");
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
