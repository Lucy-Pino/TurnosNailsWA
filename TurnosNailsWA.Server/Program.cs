using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using TurnosNailsWA.BD.Datos;
using TurnosNailsWA.Repositorio;
using TurnosNailsWA.Repositorio.Repositorios;
using TurnosNailsWA.Server;

var builder = WebApplication.CreateBuilder(args);

#region Servicios 
    var connectionString = builder.Configuration
                       .GetConnectionString("connSql") ?? 
                       throw new InvalidOperationException("La conexion a la base de datos no funciona.");

     builder.Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(connectionString)); //Conexion a la base de datos

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "EliBeautyCare.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllersWithViews(); //Maneja controllers 
builder.Services.AddRazorPages();
builder.Services.AddSwaggerGen(); //Genera documentación de la API
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IServicioRepositorio, ServicioRepositorio>();
builder.Services.AddScoped<IReservaRepositorio, ReservaRepositorio>();
builder.Services.AddScoped<ITurnoRepositorio, TurnoRepositorio>();
builder.Services.AddScoped<IPagoRepositorio, PagoRepositorio>();
builder.Services.AddScoped<IHorarioAtencionRepositorio, HorarioAtencionRepositorio>();
builder.Services.AddHostedService<PagoVencidoService>(); //Revisa los pagos vencidos cada minuto

#endregion

var app = builder.Build();

#region Middleware 
if (!app.Environment.IsDevelopment())
{
      app.UseExceptionHandler("/Error");
      app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
   app.UseBlazorFrameworkFiles();
   app.UseStaticFiles();

   app.UseRouting();

   app.UseAuthentication();
   app.UseAuthorization();

   app.MapRazorPages();
   app.MapControllers();
   app.MapFallbackToFile("index.html");
#endregion

app.Run();
