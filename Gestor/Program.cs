using Gestor.Models;
using Gestor.Servicios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);
var politicaDeUsuariosAutenticados = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
//Add policiy
builder.Services.AddControllersWithViews(opciones=>
{opciones.Filters.Add(new AuthorizeFilter(politicaDeUsuariosAutenticados));});
// Add services to the container.
builder.Services.AddControllersWithViews();
//Agregamos los servicios creados
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioTipoUsuarios>();
builder.Services.AddTransient<IRepositorioTipoCuentas, RepositorioTipoCuentas>();
builder.Services.AddTransient<IRepositorioCuentas, RepositorioCuentas>();
builder.Services.AddTransient<IRepositorioCategorias, RepositorioCategorias>();
builder.Services.AddTransient<IRepositorioTransacciones, RepositorioTransacciones>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IServicioReportes, ServicioReportes>();
builder.Services.AddTransient<IRepositorioUsuarios, RepositorioTipoUsuarios>();
builder.Services.AddTransient<IUserStore<TipoUsuarios>, UsuarioStore>();
builder.Services.AddTransient<SignInManager<TipoUsuarios>>();
builder.Services.AddIdentityCore<TipoUsuarios>( opciones =>
{
    opciones.Password.RequireDigit = false;
    opciones.Password.RequireLowercase = false;
    opciones.Password.RequireUppercase = false;
    opciones.Password.RequireNonAlphanumeric = false;

}).AddErrorDescriber<ManejoDeErrorIdentity>();

builder.Services.AddAuthentication(option=>
{
    option.DefaultAuthenticateScheme = IdentityConstants.ApplicationScheme;
    option.DefaultChallengeScheme = IdentityConstants.ApplicationScheme;
    option.DefaultSignOutScheme = IdentityConstants.ApplicationScheme;
}).AddCookie(IdentityConstants.ApplicationScheme, opciones =>
    {
        opciones.LoginPath = "/usuarios/login";
    }
    ); 




//Configuro auto mapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Transacciones}/{action=Calendario}/{id?}");

app.Run();
