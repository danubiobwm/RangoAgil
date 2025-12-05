using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RangoAgil.API.DbContexts;
using RangoAgil.API.EndpointHandlers; // Adiciona o namespace do handler

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RangoDbContext>(
    o => o.UseSqlite(builder.Configuration["ConnectionStrings:RangoDbConStr"])
);

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();


app.MapRangosEndpoints();

app.Run();