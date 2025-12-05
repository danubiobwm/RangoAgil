using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.Models;

namespace RangoAgil.API.EndpointHandlers
{
    public static class IngredientesHandlers
    {
        public static void MapIngredientesEndpoints(this IEndpointRouteBuilder app)
        {
            // O grupo de rotas é definido como /api/v1/rangos, pois ingredientes 
            // sempre estarão aninhados em um Rango.
            var ingredientesEndponts = app.MapGroup("/api/v1/rangos/{rangoId:int}/ingredientes");

            // GET /api/v1/rangos/{rangoId}/ingredientes
            ingredientesEndponts.MapGet("/", async (
                RangoDbContext rangoDbContext,
                IMapper mapper,
                int rangoId) =>
            {
                // Se o rangoId não for encontrado, esta expressão retornará null,
                // e o Map retornará uma coleção vazia, que é aceitável para listar.
                var ingredientes = (await rangoDbContext.Rangos
                    .Include(rango => rango.Ingredientes)
                    .FirstOrDefaultAsync(rango => rango.Id == rangoId))?.Ingredientes;

                return mapper.Map<IEnumerable<IngredienteDTO>>(ingredientes);
            });

        }
    }
}