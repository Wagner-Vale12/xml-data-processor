using XmlDataProcessor.Infrastructure.Persistence.Connection;
using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Infrastructure.Persistence.Repositories;
using XmlDataProcessor.Application.UseCases.Importacoes;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada.");

builder.Services.AddSingleton<ISqlConnectionFactory>(
    new SqlConnectionFactory(connectionString));

builder.Services.AddScoped<IImportacaoRepository, ImportacaoRepository>();

builder.Services.AddScoped<IniciarImportacaoService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();