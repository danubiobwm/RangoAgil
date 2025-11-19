using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

app.MapGet("/rangos", async Task<IResult> (RangoDbContext rangoDbContext, [FromQuery(Name = "name")] string? rangoNome) =>
{
    var rangosEntity = await rangoDbContext.Rangos
        .Where(x => rangoNome == null || x.Nome.ToLower().Contains(rangoNome.ToLower()))
        .ToListAsync();

    if (rangosEntity.Count <= 0 || rangosEntity == null)
    {
        return TypedResults.NoContent();
    }
    else
    {
        return TypedResults.Ok(rangosEntity);
    }
});

app.MapGet("/rango/{id}", async (RangoDbContext rangoDbContext, int id) =>
{
    return await rangoDbContext.Rangos.FirstOrDefaultAsync(rango => rango.Id == id);
});

app.MapGet("/rangos", async (RangoDbContext rangoDbContext) =>
{
    return await rangoDbContext.Rangos.ToListAsync();
});

app.Run();
