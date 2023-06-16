using MarvelApp.Application.Automapper;
using MarvelApp.Application.Interfaces;
using MarvelApp.Application.Interfaces.ApiRestServices;
using MarvelApp.Application.Services;
using MarvelApp.Infrastructure;
using MarvelApp.Infrastructure.Interfaces;
using MarvelApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRouting(routing => routing.LowercaseUrls = true);

builder.Services.AddDbContext<ApplicationContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddAutoMapper(typeof(MappingProfiles));

builder.Logging.ClearProviders();
builder.Logging.AddLog4Net();

builder.Services
    .AddScoped<IMarvelService, MarvelService>()
    .AddScoped<ICharacterService, CharacterService>();

builder.Services
    .AddScoped<ICharacterRepository, CharacterRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
