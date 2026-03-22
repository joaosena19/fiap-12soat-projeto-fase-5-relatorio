using MensagemErro = Domain.AnaliseDiagrama.ValueObjects.ErroResultadoDiagrama.Mensagem;
using TipoRelatorioErro = Domain.AnaliseDiagrama.ValueObjects.ErroResultadoDiagrama.TipoRelatorio;
using DataOcorrenciaErro = Domain.AnaliseDiagrama.ValueObjects.ErroResultadoDiagrama.DataOcorrencia;
using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;

namespace Domain.AnaliseDiagrama.Aggregates;

[AggregateMember]
public class ErroResultadoDiagrama
{
    public MensagemErro Mensagem { get; set; } = null!;
    public TipoRelatorioErro TipoRelatorio { get; set; } = null!;
    public DataOcorrenciaErro DataOcorrencia { get; set; } = null!;

    private ErroResultadoDiagrama() { }

    public static ErroResultadoDiagrama Criar(string mensagem, TipoRelatorioEnum? tipoRelatorio)
    {
        return new ErroResultadoDiagrama
        {
            Mensagem = new MensagemErro(mensagem),
            TipoRelatorio = new TipoRelatorioErro(tipoRelatorio),
            DataOcorrencia = new DataOcorrenciaErro(DateTimeOffset.UtcNow)
        };
    }
}
