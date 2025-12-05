using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Entities;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers
{
    public static class RangosHandlers
    {
        public static void MapRangosEndpoints(this IEndpointRouteBuilder app)
        {
          
            var rangosEndponts = app.MapGroup("/api/v1");

            // GET /api/v1/rangos
            rangosEndponts.MapGet("/rangos", async Task<IResult> (RangoDbContext rangoDbContext, [FromQuery(Name = "name")] string? rangoNome) =>
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

            // GET /api/v1/rangos/{rangoId}/ingredientes
            rangosEndponts.MapGet("/rangos/{rangoId:int}/ingredientes", async (
                RangoDbContext rangoDbContext,
                IMapper mapper,
                int rangoId) =>
            {
                return mapper.Map<IEnumerable<IngredienteDTO>>((await rangoDbContext.Rangos
                    .Include(rango => rango.Ingredientes)
                    .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes);
            });

            // GET /api/v1/rangos/{rangoId}
            rangosEndponts.MapGet("/rangos/{rangoId:int}", async (
                RangoDbContext rangoDbContext,
                IMapper mapper,
                int rangoId) =>
            {
                return mapper.Map<RangoDTO>(await rangoDbContext.Rangos
                    .Include(rango => rango.Ingredientes)
                    .FirstOrDefaultAsync(rango => rango.Id == rangoId));
            });

            // POST /api/v1/rangos
            rangosEndponts.MapPost("/rangos", async (
                RangoDbContext rangoDbContext,
                IMapper mapper, [FromBody] RangoForCreateDTO rangoForCreateDTO) => {

                    var rangoEntity = mapper.Map<Rango>(rangoForCreateDTO);
                    rangoDbContext.Add(rangoEntity);
                    await rangoDbContext.SaveChangesAsync();

                    var rangoToReturn = mapper.Map<RangoDTO>(rangoEntity);
                    return TypedResults.Created($"/rango/{rangoToReturn.Id}", rangoToReturn);
                });

            // PUT /api/v1/rangos/{rangoId}
            rangosEndponts.MapPut("/rangos/{rangoId:int}", async Task<Results<NotFound, Ok>> (
                RangoDbContext rangoDbContext,
                IMapper mapper,
                int rangoId,
                [FromBody] RangoForUpdateDTO rangoForUpdateDTO) =>
            {
                var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

                if (rangosEntity == null)
                {
                    return TypedResults.NotFound();
                }
                mapper.Map(rangoForUpdateDTO, rangosEntity);

                await rangoDbContext.SaveChangesAsync();

                return TypedResults.Ok();
            });

            // DELETE /api/v1/rangos/{rangoId}
            rangosEndponts.MapDelete("/rangos/{rangoId:int}", async Task<Results<NotFound, NoContent>> (
                RangoDbContext rangoDbContext,
                int rangoId) =>
            {
                var rangosEntity = await rangoDbContext.Rangos.FirstOrDefaultAsync(x => x.Id == rangoId);

                if (rangosEntity == null)
                {
                    return TypedResults.NotFound();
                }

                rangoDbContext.Rangos.Remove(rangosEntity);

                await rangoDbContext.SaveChangesAsync();

                return TypedResults.NoContent();
            });
        }
    }
}