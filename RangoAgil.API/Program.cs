using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

var app = builder.Build();

app.MapGet("/rango/{id}", async(RangoDbContext rangoDbContext, int id) =>
{
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
});

app.MapGet("/rangos", async (RangoDbContext rangoDbContext) =>
{
    return await rangoDbContext.Rangos.ToListAsync();
});

app.Run();
 