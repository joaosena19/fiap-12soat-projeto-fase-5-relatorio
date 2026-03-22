using DataCriacaoResultadoDiagrama = Domain.AnaliseDiagrama.ValueObjects.ResultadoDiagrama.DataCriacao;
using StatusResultadoDiagrama = Domain.AnaliseDiagrama.ValueObjects.ResultadoDiagrama.Status;
using ConteudosRelatorio = Domain.AnaliseDiagrama.ValueObjects.RelatorioGerado.Conteudos;
using Domain.AnaliseDiagrama.Entities;
using Domain.AnaliseDiagrama.Enums;
using Shared.Attributes;
using Shared.Exceptions;
using UUIDNext;

namespace Domain.AnaliseDiagrama.Aggregates;

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

    public void RegistrarAnalise(AnaliseResultado analiseResultado, string jsonString)
    {
        AnaliseResultado = analiseResultado;
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Analisado);

        var conteudos = ConteudosRelatorio.Vazio().Adicionar(Shared.Constants.ConteudoRelatorioChaves.JsonString, jsonString);
        ObterRelatorio(TipoRelatorioEnum.Json).Concluir(conteudos);
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

        if (relatorio.Status.Valor == StatusRelatorioEnum.Concluido)
            return ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido;

        return ResultadoSolicitacaoGeracaoRelatorioEnum.EmProcessamento;
    }

    private static List<RelatorioGerado> CriarRelatoriosPadrao()
    {
        return Enum.GetValues<TipoRelatorioEnum>()
            .Select(RelatorioGerado.Criar)
            .ToList();
    }
}
