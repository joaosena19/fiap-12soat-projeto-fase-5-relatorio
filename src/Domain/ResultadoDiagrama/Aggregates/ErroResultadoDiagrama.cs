using MensagemErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.Mensagem;
using TipoRelatorioErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.TipoRelatorio;
using OrigemErroVO = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.OrigemErro;
using NumeroTentativaVO = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.NumeroTentativa;
using DataOcorrenciaErro = Domain.ResultadoDiagrama.ValueObjects.ErroResultadoDiagrama.DataOcorrencia;
using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;

namespace Domain.ResultadoDiagrama.Aggregates;

[AggregateMember]
public class ErroResultadoDiagrama
{
    public MensagemErro Mensagem { get; private set; } = null!;
    public TipoRelatorioErro TipoRelatorio { get; private set; } = null!;
    public OrigemErroVO OrigemErro { get; private set; } = null!;
    public NumeroTentativaVO NumeroTentativa { get; private set; } = null!;
    public DataOcorrenciaErro DataOcorrencia { get; private set; } = null!;

    private ErroResultadoDiagrama() { }

    public static ErroResultadoDiagrama Criar(string mensagem, TipoRelatorioEnum? tipoRelatorio, OrigemErroEnum? origemErro = null, int? numeroTentativa = null)
    {
        return new ErroResultadoDiagrama
        {
            Mensagem = new MensagemErro(mensagem),
            TipoRelatorio = new TipoRelatorioErro(tipoRelatorio),
            OrigemErro = new OrigemErroVO(origemErro),
            NumeroTentativa = new NumeroTentativaVO(numeroTentativa),
            DataOcorrencia = new DataOcorrenciaErro(DateTimeOffset.UtcNow)
        };
    }

    public static ErroResultadoDiagrama Reidratar(string mensagem, TipoRelatorioEnum? tipoRelatorio, OrigemErroEnum? origemErro, int? numeroTentativa, DateTimeOffset dataOcorrencia)
    {
        return new ErroResultadoDiagrama
        {
            Mensagem = new MensagemErro(mensagem),
            TipoRelatorio = new TipoRelatorioErro(tipoRelatorio),
            OrigemErro = new OrigemErroVO(origemErro),
            NumeroTentativa = new NumeroTentativaVO(numeroTentativa),
            DataOcorrencia = new DataOcorrenciaErro(dataOcorrencia)
        };
    }
}
