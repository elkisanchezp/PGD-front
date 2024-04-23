using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Tiempo de expiración de 30 minutos
        options.SlidingExpiration = true; // La expiración se renueva con cada solicitud
        options.Cookie.HttpOnly = true; // Las cookies solo son accesibles desde el servidor
        options.Cookie.SameSite = SameSiteMode.Strict; // Restringir las cookies a solicitudes del mismo sitio
        options.LoginPath = "/Login/Index"; // Ruta de inicio de sesión
        options.AccessDeniedPath = "/Home/AccessDenied"; // Ruta de acceso denegado
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();


app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});



app.Run();
