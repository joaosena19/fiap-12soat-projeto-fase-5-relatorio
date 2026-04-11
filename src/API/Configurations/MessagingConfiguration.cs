using Application.Contracts.Messaging.Dtos;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Filters;
using MassTransit;
using System.Text.Json.Serialization;

namespace API.Configurations;

/// <summary>
/// Configuração de mensageria usando MassTransit com Amazon SQS.
/// </summary>
public static class MessagingConfiguration
{
    /// <summary>
    /// Configura o MassTransit com Amazon SQS como transport
    /// </summary>
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UploadDiagramaConcluidoConsumer>();
            x.AddConsumer<UploadDiagramaRejeitadoConsumer>();
            x.AddConsumer<ProcessamentoDiagramaIniciadoConsumer>();
            x.AddConsumer<ProcessamentoDiagramaAnalisadoConsumer>();
            x.AddConsumer<ProcessamentoDiagramaErroConsumer>();
            x.AddConsumer<SolicitarGeracaoRelatoriosConsumer>();
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(false));

            x.UsingAmazonSqs((context, cfg) =>
            {
                var region = configuration["AWS:Region"] ?? throw new InvalidOperationException("Configuração AWS:Region não encontrada");

                cfg.Host(region, h =>
                {
                    var accessKey = configuration["AWS:AccessKeyId"];
                    var secretKey = configuration["AWS:SecretAccessKey"];

                    if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                    {
                        h.AccessKey(accessKey);
                        h.SecretKey(secretKey);
                    }
                });

                var topicoUploadConcluido = configuration["Mensageria:Topicos:UploadDiagramaConcluido"]!;
                var filaUploadConcluido = configuration["Mensageria:Filas:UploadDiagramaConcluido"] ?? topicoUploadConcluido;
                var topicoUploadRejeitado = configuration["Mensageria:Topicos:UploadDiagramaRejeitado"]!;
                var topicoProcessamentoIniciado = configuration["Mensageria:Topicos:ProcessamentoDiagramaIniciado"]!;
                var topicoProcessamentoAnalisado = configuration["Mensageria:Topicos:ProcessamentoDiagramaAnalisado"]!;
                var topicoProcessamentoErro = configuration["Mensageria:Topicos:ProcessamentoDiagramaErro"]!;
                var topicoSolicitarGeracao = configuration["Mensageria:Topicos:SolicitarGeracaoRelatorios"]!;

                cfg.Message<UploadDiagramaConcluidoDto>(m => m.SetEntityName(topicoUploadConcluido));
                cfg.Message<UploadDiagramaRejeitadoDto>(m => m.SetEntityName(topicoUploadRejeitado));
                cfg.Message<ProcessamentoDiagramaIniciadoDto>(m => m.SetEntityName(topicoProcessamentoIniciado));
                cfg.Message<ProcessamentoDiagramaAnalisadoDto>(m => m.SetEntityName(topicoProcessamentoAnalisado));
                cfg.Message<ProcessamentoDiagramaErroDto>(m => m.SetEntityName(topicoProcessamentoErro));
                cfg.Message<SolicitarGeracaoRelatoriosDto>(m => m.SetEntityName(topicoSolicitarGeracao));

                cfg.ReceiveEndpoint(filaUploadConcluido, e => e.ConfigureConsumer<UploadDiagramaConcluidoConsumer>(context));
                cfg.ReceiveEndpoint(topicoUploadRejeitado, e => e.ConfigureConsumer<UploadDiagramaRejeitadoConsumer>(context));
                cfg.ReceiveEndpoint(topicoProcessamentoIniciado, e => e.ConfigureConsumer<ProcessamentoDiagramaIniciadoConsumer>(context));
                cfg.ReceiveEndpoint(topicoProcessamentoAnalisado, e => e.ConfigureConsumer<ProcessamentoDiagramaAnalisadoConsumer>(context));
                cfg.ReceiveEndpoint(topicoProcessamentoErro, e => e.ConfigureConsumer<ProcessamentoDiagramaErroConsumer>(context));
                cfg.ReceiveEndpoint(topicoSolicitarGeracao, e => e.ConfigureConsumer<SolicitarGeracaoRelatoriosConsumer>(context));

                cfg.UseSendFilter(typeof(SendCorrelationIdFilter<>), context);
                cfg.UsePublishFilter(typeof(PublishCorrelationIdFilter<>), context);
                cfg.UseConsumeFilter(typeof(ConsumeCorrelationIdFilter<>), context);

                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.Converters.Add(new JsonStringEnumConverter());
                    return options;
                });
            });
        });

        return services;
    }
}
