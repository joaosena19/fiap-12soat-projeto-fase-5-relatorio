using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using DataGeracaoRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.DataGeracao;
using StatusRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Status;
using TipoRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Tipo;
using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;

namespace Domain.AnaliseDiagrama.Aggregates;

[AggregateMember]
public class RelatorioGerado
{
    public TipoRelatorio Tipo { get; set; } = null!;
    public StatusRelatorio Status { get; set; } = null!;
    public ConteudosRelatorio Conteudos { get; set; } = null!;
    public DataGeracaoRelatorio? DataGeracao { get; set; }

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
        return Status.Valor == StatusRelatorioEnum.NaoSolicitado || Status.Valor == StatusRelatorioEnum.Erro;
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