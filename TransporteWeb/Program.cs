using Blazored.SessionStorage;
using Domain.Dto;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using TransporteWeb.Components;
using TransporteWeb.Repository.Interfaz;
using TransporteWeb.Repository.Repositories;
using TransporteWeb.Repository.Utils;
using TransporteWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// ✅ AGREGAR AUTENTICACIÓN CON COOKIES (NECESARIO PARA BLAZOR SERVER)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "BlazorAuth";
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/auth/logout";
        options.AccessDeniedPath = "/auth/accessdenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<IBaseRepository<CentroDistribucion, CentroDistribucionDto>, CdRepository>();
builder.Services.AddScoped<IBaseRepository<Rol, RolDto>, RolRepository>();
builder.Services.AddScoped<IBaseRepository<Usuario, UsuarioDto>, UsuarioRepository>();
builder.Services.AddScoped<IBaseRepository<Propietario, PropietarioDto>, PropietarioRepository>();
builder.Services.AddScoped<IPropietarioRepository, PropietarioRepository>();
builder.Services.AddScoped<IBaseRepository<Camion, CamionDto>, CamionRepository>();
builder.Services.AddScoped<IBaseRepository<Conductor, ConductorDto>, ConductorRepository>();
builder.Services.AddScoped<IBaseRepository<Guia, GuiaDto>, GuiasRepository>();
builder.Services.AddHttpClient();
builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection("ApiSettings"));
builder.Services.AddMudServices();

builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<CustomAuthenticationStateProvider>());  // Este es el que se usa por defecto

builder.Services.AddHttpClient<AuthService>(options =>
{
    options.BaseAddress = new Uri("https://localhost:7162/api");
});



builder.Services.AddBlazoredSessionStorage();

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.UseAuthentication();  // 👈 Siempre antes que Authorization
app.UseAuthorization();   // 👈

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
