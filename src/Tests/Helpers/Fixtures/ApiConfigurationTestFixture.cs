using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Helpers.Fixtures;

public class ApiConfigurationTestFixture
{
    private readonly Dictionary<string, string?> _configuracoes = [];

    public IServiceCollection Services { get; } = new ServiceCollection();

    public ApiConfigurationTestFixture()
    {
        Services.AddLogging();
    }

    public ApiConfigurationTestFixture ComConfiguracaoAwsValida()
    {
        _configuracoes["AWS:Region"] = "us-east-1";
        return this;
    }

    public ApiConfigurationTestFixture ComCredenciaisAws()
    {
        _configuracoes["AWS:AccessKeyId"] = "access-key";
        _configuracoes["AWS:SecretAccessKey"] = "secret-key";
        return this;
    }

    public ApiConfigurationTestFixture ComConfiguracaoMensageriaValida()
    {
        _configuracoes["Mensageria:Topicos:UploadDiagramaConcluido"] = "upload-diagrama-concluido";
        _configuracoes["Mensageria:Filas:UploadDiagramaConcluido"] = "upload-diagrama-concluido-fila";
        _configuracoes["Mensageria:Topicos:UploadDiagramaRejeitado"] = "upload-diagrama-rejeitado";
        _configuracoes["Mensageria:Topicos:ProcessamentoDiagramaIniciado"] = "processamento-diagrama-iniciado";
        _configuracoes["Mensageria:Topicos:ProcessamentoDiagramaAnalisado"] = "processamento-diagrama-analisado";
        _configuracoes["Mensageria:Topicos:ProcessamentoDiagramaErro"] = "processamento-diagrama-erro";
        _configuracoes["Mensageria:Topicos:SolicitarGeracaoRelatorios"] = "solicitar-geracao-relatorios";
        return this;
    }

    public ApiConfigurationTestFixture SemConfiguracaoAws(string chave)
    {
        _configuracoes.Remove($"AWS:{chave}");
        return this;
    }

    public ApiConfigurationTestFixture ComConfiguracaoJwtValida()
    {
        _configuracoes["Jwt:Key"] = "chave-super-segura-com-tamanho-minimo-123456";
        _configuracoes["Jwt:Issuer"] = "relatorio-api";
        _configuracoes["Jwt:Audience"] = "relatorio-client";
        return this;
    }

    public ApiConfigurationTestFixture SemConfiguracaoJwt(string chave)
    {
        _configuracoes.Remove($"Jwt:{chave}");
        return this;
    }

    public ApiConfigurationTestFixture ComConfiguracaoDatabaseValida()
    {
        _configuracoes["DatabaseConnection:Host"] = "localhost";
        _configuracoes["DatabaseConnection:Port"] = "5432";
        _configuracoes["DatabaseConnection:DatabaseName"] = "relatorio_db";
        _configuracoes["DatabaseConnection:User"] = "postgres";
        _configuracoes["DatabaseConnection:Password"] = "postgres";
        return this;
    }

    public ApiConfigurationTestFixture SemConfiguracaoDatabase(string chave)
    {
        _configuracoes.Remove($"DatabaseConnection:{chave}");
        return this;
    }

    public IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(_configuracoes)
            .Build();
    }

    public ServiceProvider BuildServiceProvider()
    {
        return Services.BuildServiceProvider();
    }
}