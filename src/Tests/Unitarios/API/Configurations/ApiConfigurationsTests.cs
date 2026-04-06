using Amazon.S3;
using API.Configurations;
using Application.Contracts.Armazenamento;
using Application.Contracts.Monitoramento;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using Serilog;
using Tests.Helpers.Fixtures;

namespace Tests.API.Configurations;

public class ApiConfigurationsTests
{
    [Fact(DisplayName = "Deve registrar serviços de monitoramento quando configuração é aplicada")]
    [Trait("API", "MonitoringConfiguration")]
    public void AddMonitoring_DeveRegistrarDependencias_QuandoInvocado()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture();

        // Act
        var retorno = fixture.Services.AddMonitoring();
        using var provider = fixture.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Assert
        retorno.ShouldBe(fixture.Services);
        scope.ServiceProvider.GetService<ICorrelationIdAccessor>().ShouldNotBeNull();
        provider.GetService<Microsoft.AspNetCore.Http.IHttpContextAccessor>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve registrar serviços de armazenamento com região configurada")]
    [Trait("API", "ArmazenamentoConfiguration")]
    public void AddArmazenamento_DeveRegistrarDependencias_QuandoRegiaoConfigurada()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoAwsValida();

        // Act
        var retorno = fixture.Services.AddArmazenamento(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<IAmazonS3>().ShouldNotBeNull();
        fixture.Services.Any(descriptor => descriptor.ServiceType == typeof(IArmazenamentoArquivoService)).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve registrar serviços de armazenamento com credenciais explícitas")]
    [Trait("API", "ArmazenamentoConfiguration")]
    public void AddArmazenamento_DeveRegistrarS3_QuandoCredenciaisInformadas()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoAwsValida()
            .ComCredenciaisAws();

        // Act
        fixture.Services.AddArmazenamento(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();

        // Assert
        provider.GetService<IAmazonS3>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve lançar exceção quando região AWS não está configurada")]
    [Trait("API", "ArmazenamentoConfiguration")]
    public void AddArmazenamento_DeveLancarExcecao_QuandoRegiaoNaoConfigurada()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture();

        // Act
        var acao = () => fixture.Services.AddArmazenamento(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve registrar health checks quando configuração é aplicada")]
    [Trait("API", "HealthCheckConfiguration")]
    public void AddHealthChecks_DeveRegistrarServico_QuandoInvocado()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture();
        fixture.Services.AddDbContext<global::Infrastructure.Database.AppDbContext>(options => options.UseInMemoryDatabase($"relatorio-config-{Guid.NewGuid()}"));

        // Act
        var retorno = fixture.Services.AddHealthChecks(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<HealthCheckService>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve registrar controllers com filtro de autorização")]
    [Trait("API", "ControllersConfiguration")]
    public void AddApiControllers_DeveRegistrarFiltroAutorizacao_QuandoInvocado()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture();

        // Act
        var retorno = fixture.Services.AddApiControllers();
        using var provider = fixture.BuildServiceProvider();
        var mvcOptions = provider.GetRequiredService<IOptions<MvcOptions>>().Value;

        // Assert
        retorno.ShouldBe(fixture.Services);
        mvcOptions.Filters.OfType<AuthorizeFilter>().Any().ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve registrar autenticação JWT quando configuração é válida")]
    [Trait("API", "AuthenticationConfiguration")]
    public void AddJwtAuthentication_DeveRegistrarAutenticacao_QuandoConfiguracaoValida()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoJwtValida();

        // Act
        var retorno = fixture.Services.AddJwtAuthentication(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<IAuthenticationSchemeProvider>().ShouldNotBeNull();
        provider.GetService<IOptionsMonitor<JwtBearerOptions>>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve lançar exceção quando configuração JWT está ausente")]
    [Trait("API", "AuthenticationConfiguration")]
    public void AddJwtAuthentication_DeveLancarExcecao_QuandoConfiguracaoAusente()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoJwtValida()
            .SemConfiguracaoJwt("Key");

        // Act
        var acao = () => fixture.Services.AddJwtAuthentication(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve registrar DbContext quando configuração de banco é válida")]
    [Trait("API", "DatabaseConfiguration")]
    public void AddDatabase_DeveRegistrarDbContext_QuandoConfiguracaoValida()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoDatabaseValida();

        // Act
        var retorno = fixture.Services.AddDatabase(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        fixture.Services.Any(d => d.ServiceType == typeof(global::Infrastructure.Database.AppDbContext)).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve lançar exceção quando configuração de banco está incompleta")]
    [Trait("API", "DatabaseConfiguration")]
    public void AddDatabase_DeveLancarExcecao_QuandoConfiguracaoIncompleta()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoDatabaseValida()
            .SemConfiguracaoDatabase("Host");

        // Act
        var acao = () => fixture.Services.AddDatabase(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve configurar logger global do Serilog")]
    [Trait("API", "SerilogConfiguration")]
    public void ConfigurarSerilog_DeveConfigurarLoggerGlobal_QuandoInvocado()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture();
        var loggerAnterior = Log.Logger;

        try
        {
            // Act
            SerilogConfiguration.ConfigurarSerilog(fixture.BuildConfiguration());

            // Assert
            Log.Logger.ShouldNotBeNull();
            ReferenceEquals(Log.Logger, loggerAnterior).ShouldBeFalse();
        }
        finally
        {
            Log.CloseAndFlush();
            Log.Logger = loggerAnterior;
        }
    }

    [Fact(DisplayName = "Deve registrar estratégias de relatório quando configuração é aplicada")]
    [Trait("API", "RelatorioConfiguration")]
    public void AddRelatorios_DeveRegistrarEstrategias_QuandoInvocado()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture();

        // Act
        var retorno = fixture.Services.AddRelatorios();
        using var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        fixture.Services.Any(d => d.ServiceType == typeof(global::Application.Contracts.Relatorios.IRelatorioStrategy)).ShouldBeTrue();
        fixture.Services.Any(d => d.ServiceType == typeof(global::Application.Contracts.Relatorios.IRelatorioStrategyResolver)).ShouldBeTrue();
        fixture.Services.Any(d => d.ServiceType == typeof(global::Application.Contracts.Messaging.IRelatorioMessagePublisher)).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve registrar serviços de mensageria quando invocado")]
    [Trait("API", "MessagingConfiguration")]
    public void AddMessaging_DeveRegistrarServicos_QuandoInvocado()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoAwsValida()
            .ComConfiguracaoMensageriaValida();

        // Act
        var retorno = fixture.Services.AddMessaging(fixture.BuildConfiguration());
        var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        fixture.Services.Any(d => d.ServiceType == typeof(MassTransit.IBus)).ShouldBeTrue();
        provider.GetService<MassTransit.IBusControl>().ShouldNotBeNull();
        provider.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    [Fact(DisplayName = "Deve registrar serviços de mensageria quando credenciais AWS são informadas")]
    [Trait("API", "MessagingConfiguration")]
    public void AddMessaging_DeveRegistrarServicos_QuandoCredenciaisInformadas()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoAwsValida()
            .ComCredenciaisAws()
            .ComConfiguracaoMensageriaValida();

        // Act
        var retorno = fixture.Services.AddMessaging(fixture.BuildConfiguration());
        var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        fixture.Services.Any(d => d.ServiceType == typeof(MassTransit.IBus)).ShouldBeTrue();
        provider.GetService<MassTransit.IBusControl>().ShouldNotBeNull();
        provider.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

    [Fact(DisplayName = "Deve falhar ao materializar bus quando região AWS não é informada")]
    [Trait("API", "MessagingConfiguration")]
    public void AddMessaging_DeveFalharMaterializacao_QuandoRegiaoAwsNaoInformada()
    {
        // Arrange
        var fixture = new ApiConfigurationTestFixture()
            .ComConfiguracaoMensageriaValida();

        fixture.Services.AddMessaging(fixture.BuildConfiguration());
        var provider = fixture.BuildServiceProvider();

        // Act
        var acao = () => provider.GetRequiredService<MassTransit.IBusControl>();

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
        provider.DisposeAsync().AsTask().GetAwaiter().GetResult();
    }

}