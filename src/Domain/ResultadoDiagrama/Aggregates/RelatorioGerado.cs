using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using DataGeracaoRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.DataGeracao;
using StatusRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Status;
using TipoRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Tipo;
using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;

namespace Domain.ResultadoDiagrama.Aggregates;

[AggregateMember]
public class RelatorioGerado
{
    public TipoRelatorio Tipo { get; private set; } = null!;
    public StatusRelatorio Status { get; private set; } = null!;
    public ConteudosRelatorio Conteudos { get; private set; } = null!;
    public DataGeracaoRelatorio? DataGeracao { get; private set; }

    private RelatorioGerado() { }

    public static RelatorioGerado Criar(TipoRelatorioEnum tipo)
    {
        return new RelatorioGerado
        {
            Tipo = new TipoRelatorio(tipo),
            Status = new StatusRelatorio(StatusRelatorioEnum.NaoSolicitado),
            Conteudos = ConteudosRelatorio.Vazio(),
            DataGeracao = null
        };
    }

    public static RelatorioGerado Reidratar(TipoRelatorio tipo, StatusRelatorio status, ConteudosRelatorio conteudos, DataGeracaoRelatorio? dataGeracao)
    {
        return new RelatorioGerado
        {
            Tipo = tipo,
            Status = status,
            Conteudos = conteudos,
            DataGeracao = dataGeracao
        };
    }

    public bool PodeGerar()
    {
        return Status.Valor == StatusRelatorioEnum.NaoSolicitado || Status.Valor == StatusRelatorioEnum.Solicitado || Status.Valor == StatusRelatorioEnum.Erro;
    }

    public void MarcarSolicitado()
    {
        Status = new StatusRelatorio(StatusRelatorioEnum.Solicitado);
    }

    public void MarcarEmProcessamento()
    {
        Status = new StatusRelatorio(StatusRelatorioEnum.EmProcessamento);
    }

    public void Concluir(ConteudosRelatorio conteudos)
    {
        Conteudos = conteudos;
        DataGeracao = new DataGeracaoRelatorio(DateTimeOffset.UtcNow);
        Status = new StatusRelatorio(StatusRelatorioEnum.Concluido);
    }

    public void RegistrarErro()
    {
        Status = new StatusRelatorio(StatusRelatorioEnum.Erro);
    }
}