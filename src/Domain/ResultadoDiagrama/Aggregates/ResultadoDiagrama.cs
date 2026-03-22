using DataCriacaoResultadoDiagrama = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataCriacao;
using StatusResultadoDiagrama = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.Status;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Domain.ResultadoDiagrama.Entities;
using Domain.ResultadoDiagrama.Enums;
using Shared.Attributes;
using Shared.Exceptions;
using UUIDNext;

namespace Domain.ResultadoDiagrama.Aggregates;

[AggregateRoot]
public class ResultadoDiagrama
{
    public Guid Id { get; private set; }
    public Guid AnaliseDiagramaId { get; private set; }
    public StatusResultadoDiagrama Status { get; private set; } = null!;
    public AnaliseResultado? AnaliseResultado { get; private set; }
    public List<RelatorioGerado> Relatorios { get; private set; } = new();
    public List<ErroResultadoDiagrama> Erros { get; private set; } = new();
    public DataCriacaoResultadoDiagrama DataCriacao { get; private set; } = null!;

    private ResultadoDiagrama() { }

    public static ResultadoDiagrama Criar(Guid analiseDiagramaId)
    {
        return new ResultadoDiagrama
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            AnaliseDiagramaId = analiseDiagramaId,
            Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Recebido),
            AnaliseResultado = null,
            Relatorios = CriarRelatoriosPadrao(),
            Erros = new List<ErroResultadoDiagrama>(),
            DataCriacao = new DataCriacaoResultadoDiagrama(DateTimeOffset.UtcNow)
        };
    }

    public void MarcarEmProcessamento()
    {
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.EmProcessamento);
    }

    public void RegistrarAnalise(AnaliseResultado analiseResultado)
    {
        AnaliseResultado = analiseResultado;
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Analisado);
    }

    public bool AnaliseDisponivel()
    {
        return AnaliseResultado != null && Status.Valor == StatusAnaliseEnum.Analisado;
    }

    public void MarcarRelatorioSolicitado(TipoRelatorioEnum tipoRelatorio)
    {
        ObterRelatorio(tipoRelatorio).MarcarSolicitado();
    }

    public void MarcarRelatorioEmProcessamento(TipoRelatorioEnum tipoRelatorio)
    {
        ObterRelatorio(tipoRelatorio).MarcarEmProcessamento();
    }

    public void ConcluirRelatorio(TipoRelatorioEnum tipoRelatorio, ConteudosRelatorio conteudos)
    {
        ObterRelatorio(tipoRelatorio).Concluir(conteudos);
    }

    public void RegistrarFalhaProcessamento(string mensagem)
    {
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Erro);
        Erros.Add(ErroResultadoDiagrama.Criar(mensagem, null));
    }

    public void RegistrarFalhaRelatorio(TipoRelatorioEnum tipoRelatorio, string mensagem)
    {
        ObterRelatorio(tipoRelatorio).RegistrarErro();
        Erros.Add(ErroResultadoDiagrama.Criar(mensagem, tipoRelatorio));
    }

    public RelatorioGerado ObterRelatorio(TipoRelatorioEnum tipoRelatorio)
    {
        var relatorio = Relatorios.FirstOrDefault(item => item.Tipo.Valor == tipoRelatorio);
        if (relatorio == null)
            throw new DomainException($"Relatório do tipo '{tipoRelatorio}' não encontrado");

        return relatorio;
    }

    public ResultadoSolicitacaoGeracaoRelatorioEnum ObterResultadoSolicitacaoGeracaoRelatorio(TipoRelatorioEnum tipoRelatorio)
    {
        var relatorio = ObterRelatorio(tipoRelatorio);

        return relatorio.Status.Valor switch
        {
            StatusRelatorioEnum.Concluido => ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido,
            StatusRelatorioEnum.EmProcessamento or StatusRelatorioEnum.Solicitado => ResultadoSolicitacaoGeracaoRelatorioEnum.JaEmAndamento,
            _ => ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao
        };
    }

    private static List<RelatorioGerado> CriarRelatoriosPadrao()
    {
        return Enum.GetValues<TipoRelatorioEnum>()
            .Select(RelatorioGerado.Criar)
            .ToList();
    }
}
