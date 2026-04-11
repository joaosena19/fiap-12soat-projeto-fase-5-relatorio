using API.Configurations;
using API.Configurations.Swagger;
using API.Middleware;
using QuestPDF.Infrastructure;
using Serilog;

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

SerilogConfiguration.ConfigurarSerilog(configuration);

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
builder.Services.AddRelatorios();

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

app.AplicarMigracoes();

await app.RunAsync();

//Necessário para testes de integração
public partial class Program
{
    protected Program() { }
}
