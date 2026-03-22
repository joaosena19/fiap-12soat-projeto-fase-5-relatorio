using MensagemErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.Mensagem;
using TipoRelatorioErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.TipoRelatorio;
using DataOcorrenciaErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.DataOcorrencia;
using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;

namespace Domain.ResultadoDiagrama.Aggregates;

[AggregateMember]
public class ErroResultadoDiagrama
{
    public MensagemErro Mensagem { get; private set; } = null!;
    public TipoRelatorioErro TipoRelatorio { get; private set; } = null!;
    public DataOcorrenciaErro DataOcorrencia { get; private set; } = null!;

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
