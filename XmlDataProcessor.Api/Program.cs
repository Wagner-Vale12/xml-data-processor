using XmlDataProcessor.Infrastructure.Persistence.Connection;
using XmlDataProcessor.Application.Abstractions.Repositories;
using XmlDataProcessor.Infrastructure.Persistence.Repositories;
using XmlDataProcessor.Application.Abstractions.Xml;
using XmlDataProcessor.Infrastructure.Xml;
using XmlDataProcessor.Application.UseCases.Importacoes;
using XmlDataProcessor.Api.ExceptionHandling;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddProblemDetails();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' não encontrada.");

builder.Services.AddSingleton<ISqlConnectionFactory>(
    new SqlConnectionFactory(connectionString));

builder.Services.AddScoped<
    ILeitorMovimentosXml,
    LeitorMovimentosXml>();

builder.Services.AddScoped<IImportacaoRepository, ImportacaoRepository>();

builder.Services.AddScoped<
    IMovimentoRepository,
    MovimentoRepository>();

builder.Services.AddScoped<IniciarImportacaoService>();

builder.Services.AddScoped<ProcessarImportacaoService>();

builder.Services.AddScoped<ObterImportacaoPorIdService>();

builder.Services.AddScoped<ListarImportacoesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();