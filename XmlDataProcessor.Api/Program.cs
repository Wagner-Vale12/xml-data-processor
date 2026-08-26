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

var diretorioArquivos = Path.GetFullPath(
    Path.Combine(
        builder.Environment.ContentRootPath,
        "..",
        "Arquivos"));

builder.Services.AddScoped<ILeitorMovimentosXml>(
    _ => new LeitorMovimentosXml(diretorioArquivos));

builder.Services.AddScoped<IImportacaoRepository, ImportacaoRepository>();

builder.Services.AddScoped<
    IMovimentoRepository,
    MovimentoRepository>();

builder.Services.AddScoped<IniciarImportacaoService>();

builder.Services.AddScoped<ProcessarImportacaoService>();

builder.Services.AddScoped<ObterImportacaoPorIdService>();

builder.Services.AddScoped<ListarImportacoesService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors("Frontend");
    
app.UseAuthorization();

app.MapControllers();

app.Run();