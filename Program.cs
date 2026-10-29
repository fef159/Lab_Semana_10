using Eventos.Web.Data;
using Eventos.Web.Models;
using Eventos.Web.Repositorios;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<EventosContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("EventosConnection")));

builder.Services.AddScoped<EventoRepositorio>();
builder.Services.AddScoped<AsistenteRepositorio>();
builder.Services.AddScoped<InscripcionRepositorio>();

var app = builder.Build();

// Datos iniciales: se ejecutan después de aplicar las migraciones y solo si la tabla está vacía.
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EventosContext>();
    await context.Database.MigrateAsync();

    if (!await context.Categorias.AnyAsync())
    {
        context.Categorias.AddRange(
            new Categoria { Nombre = "Conferencia" },
            new Categoria { Nombre = "Taller" },
            new Categoria { Nombre = "Concierto" });
        await context.SaveChangesAsync();
    }

    if (!await context.Eventos.AnyAsync())
    {
        var categorias = await context.Categorias.ToDictionaryAsync(c => c.Nombre, c => c.Id);
        var hoy = DateTime.Today;

        context.Eventos.AddRange(
            new Evento { Titulo = "Congreso de Tecnología 2026", Fecha = hoy.AddDays(15).AddHours(9), Lugar = "Centro de Convenciones", Aforo = 200, Precio = 120.00m, CategoriaId = categorias["Conferencia"] },
            new Evento { Titulo = "Taller de ASP.NET Core", Fecha = hoy.AddDays(30).AddHours(14), Lugar = "Laboratorio 3", Aforo = 3, Precio = 50.00m, Modalidad = "Virtual", CategoriaId = categorias["Taller"] },
            new Evento { Titulo = "Concierto Sinfónico", Fecha = hoy.AddDays(45).AddHours(20), Lugar = "Teatro Municipal", Aforo = 500, Precio = 80.50m, CategoriaId = categorias["Concierto"] },
            new Evento { Titulo = "Seminario de Inteligencia Artificial", Fecha = hoy.AddDays(60).AddHours(10), Lugar = "Auditorio Principal", Aforo = 150, Precio = 0m, Modalidad = "Virtual", CategoriaId = categorias["Conferencia"] });
        await context.SaveChangesAsync();
    }

    if (!await context.Asistentes.AnyAsync())
    {
        context.Asistentes.AddRange(
            new Asistente { Nombres = "Ana Torres Quispe", Dni = "70123456", Correo = "ana.torres@correo.com", Telefono = "987654321" },
            new Asistente { Nombres = "Luis Ramírez Soto", Dni = "70234567", Correo = "luis.ramirez@correo.com", Telefono = "976543210" },
            new Asistente { Nombres = "María Fernández Díaz", Dni = "70345678", Correo = "maria.fernandez@correo.com" },
            new Asistente { Nombres = "Carlos Mendoza Rojas", Dni = "70456789", Correo = "carlos.mendoza@correo.com" },
            new Asistente { Nombres = "Lucía Vargas Paredes", Dni = "70567890", Correo = "lucia.vargas@correo.com" });
        await context.SaveChangesAsync();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Eventos}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
