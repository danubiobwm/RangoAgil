using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;


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

app.MapGet("/rango/{rangoId:int}/ingredientes", async (
    RangoDbContext rangoDbContext,
    IMapper mapper, 
    int rangoId) =>
{
     return mapper.Map<IEnumerable<IngredienteDTO>> ((await rangoDbContext.Rangos
        .Include(rango => rango.Ingredientes)
        .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
});

app.MapGet("/rango/{id:int}", async (
    RangoDbContext rangoDbContext, 
    IMapper mapper ,
    int id) =>
{
    return mapper.Map<RangoDTO>(await rangoDbContext.Rangos
        .Include(rango => rango.Ingredientes)
        .FirstOrDefaultAsync(rango => rango.Id == id));
});

app.MapPost("/rango", async (
    RangoDbContext rangoDbContext, 
    IMapper mapper, [FromBody] RangoForCreateDTO rangoForCreateDTO) => { 
  
        var rangoEntity = mapper.Map<Rango>(rangoForCreateDTO);
        rangoDbContext.Add(rangoEntity);
        await rangoDbContext.SaveChangesAsync();

        var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);
        return TypedResults.Created($"/rango/{rangoToReturn.Id}", rangoToReturn);
    });

app.MapPut("/rango/{id:int}", async Task<Results<NotFound, Ok>>(
    RangoDbContext rangoDbContext,
    IMapper mapper,
    int id,
    [FromBody] RangoForUpdateDTO rangoForUpdateDTO) =>
{
    var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x=>x.Id==id);
       

    if (rangosEntity == null)
    {
        return TypedResults.NotFound();
    }
    mapper.Map(rangoForUpdateDTO, rangosEntity);

    await rangoDbContext.SaveChangesAsync();

    return TypedResults.Ok();
});

app.MapDelete("/rango/{id:int}", async Task<Results<NotFound, NoContent>> (
    RangoDbContext rangoDbContext,
    int id) =>
{
    var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == id);


    if (rangosEntity == null)
    {
        return TypedResults.NotFound();
    }

    rangoDbContext.Rangos.Remove(rangosEntity);

    await rangoDbContext.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.Run();
