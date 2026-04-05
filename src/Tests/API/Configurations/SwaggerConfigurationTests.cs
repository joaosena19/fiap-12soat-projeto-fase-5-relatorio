using API.Configurations.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using Swashbuckle.AspNetCore.Swagger;

namespace Tests.API.Configurations;

public class SwaggerConfigurationTests
{
    [Fact(DisplayName = "Deve registrar serviços de documentação do swagger")]
    [Trait("API", "SwaggerConfiguration")]
    public void AddSwaggerDocumentation_DeveRegistrarServicos_QuandoInvocado()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();

        // Act
        var retorno = services.AddSwaggerDocumentation();

        // Assert
        retorno.ShouldBe(services);
        services.Any(descriptor => descriptor.ServiceType == typeof(IApiDescriptionGroupCollectionProvider)).ShouldBeTrue();
        services.Any(descriptor => descriptor.ServiceType == typeof(ISwaggerProvider)).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve configurar middlewares do swagger quando aplicação é inicializada")]
    [Trait("API", "SwaggerConfiguration")]
    public void UseSwaggerDocumentation_DeveRetornarAplicacao_QuandoInvocado()
    {
        // Arrange
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddSwaggerDocumentation();
        var app = builder.Build();

        // Act
        var retorno = app.UseSwaggerDocumentation();

        // Assert
        retorno.ShouldBe(app);
    }

    [Fact(DisplayName = "Deve configurar definição de segurança Bearer no documento Swagger")]
    [Trait("API", "SwaggerConfiguration")]
    public void AddSwaggerDocumentation_DeveConfigurarSegurancaBearer_QuandoGerarDocumento()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSwaggerDocumentation();
        using var provider = services.BuildServiceProvider();
        var swaggerOptions = provider.GetRequiredService<IOptions<SwaggerGenOptions>>();

        // Act
        var schemaBearer = swaggerOptions.Value.SwaggerGeneratorOptions.SecuritySchemes["Bearer"];

        // Assert
        swaggerOptions.Value.SwaggerGeneratorOptions.SecuritySchemes.ContainsKey("Bearer").ShouldBeTrue();
        schemaBearer.Type.ShouldBe(SecuritySchemeType.ApiKey);
        swaggerOptions.Value.SwaggerGeneratorOptions.SecurityRequirements.Count.ShouldBeGreaterThan(0);
    }

    [Fact(DisplayName = "Deve incluir comentários XML quando arquivo de documentação existe")]
    [Trait("API", "SwaggerConfiguration")]
    public void AddSwaggerDocumentation_DeveIncluirComentariosXml_QuandoArquivoExiste()
    {
        // Arrange
        var xmlFile = "API.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        File.WriteAllText(xmlPath, "<doc><assembly><name>API</name></assembly><members></members></doc>");

        try
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSwaggerDocumentation();
            using var provider = services.BuildServiceProvider();
            var swaggerOptions = provider.GetRequiredService<IOptions<SwaggerGenOptions>>();

            // Act
            var acao = () => _ = swaggerOptions.Value;

            // Assert
            acao.ShouldNotThrow();
        }
        finally
        {
            if (File.Exists(xmlPath))
                File.Delete(xmlPath);
        }
    }
}
