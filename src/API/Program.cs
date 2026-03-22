using API.Configurations;
using API.Configurations.Swagger;
using API.Middleware;
using Application.Contracts.Gateways;
using Application.Contracts.Messaging;
using Application.Contracts.Relatorios;
using Infrastructure.Messaging.Publishers;
using Infrastructure.Repositories;
using Infrastructure.Relatorios;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using Serilog;
using NewRelic.LogEnrichers.Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.With<Infrastructure.Monitoramento.Correlation.CorrelationIdEnricher>()
    .Enrich.WithNewRelicLogsInContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

var licenseKey = configuration["NEW_RELIC_LICENSE_KEY"];
var appName = configuration["NEW_RELIC_APP_NAME"] ?? "RelatorioService";
var newRelicEndpoint = configuration["NEW_RELIC_LOG_ENDPOINT_URL"] ?? "https://log-api.newrelic.com/log/v1";

if (!string.IsNullOrWhiteSpace(licenseKey))
{
    loggerConfig.WriteTo.NewRelicLogs(
        endpointUrl: newRelicEndpoint,
        applicationName: appName,
        licenseKey: licenseKey
    );
}

Log.Logger = loggerConfig.CreateLogger();

QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddApiControllers();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMonitoring();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddHealthChecks(builder.Configuration);
builder.Services.AddArmazenamento(builder.Configuration);
builder.Services.AddScoped<IResultadoDiagramaGateway, ResultadoDiagramaRepository>();
builder.Services.AddScoped<IRelatorioMessagePublisher, RelatorioMessagePublisher>();
builder.Services.AddScoped<IRelatorioStrategy, RelatorioMarkdownStrategy>();
builder.Services.AddScoped<IRelatorioStrategy, RelatorioPdfStrategy>();
builder.Services.AddScoped<IRelatorioStrategy, RelatorioJsonStrategy>();
builder.Services.AddScoped<IRelatorioStrategyResolver, RelatorioStrategyResolver>();


var app = builder.Build();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwaggerDocumentation();
app.UseHttpsRedirection();
app.UseSecurityHeadersConfiguration();
app.UseAuthentication();
app.UseAuthorization();
app.UseHealthCheckEndpoints();
app.MapControllers();


// Aplicar migrações automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Infrastructure.Database.AppDbContext>();
    dbContext.Database.Migrate();
}

await app.RunAsync();

//Necessário para testes de integração
public partial class Program
{
    protected Program() { }
}
