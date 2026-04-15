using DataCriacaoResultadoDiagrama = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataCriacao;
using DataUltimaTentativaResultadoDiagrama = Domain.ResultadoDiagrama.ValueObjects.ResultadoDiagrama.DataUltimaTentativa;
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
    public DataUltimaTentativaResultadoDiagrama DataUltimaTentativa { get; private set; } = null!;

    private ResultadoDiagrama() { }

    public static ResultadoDiagrama Criar(Guid analiseDiagramaId)
    {
        var agora = DateTimeOffset.UtcNow;

        return new ResultadoDiagrama
        {
            Id = Uuid.NewDatabaseFriendly(Database.PostgreSql),
            AnaliseDiagramaId = analiseDiagramaId,
            Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Recebido),
            AnaliseResultado = null,
            Relatorios = CriarRelatoriosPadrao(),
            Erros = new List<ErroResultadoDiagrama>(),
            DataCriacao = new DataCriacaoResultadoDiagrama(agora),
            DataUltimaTentativa = new DataUltimaTentativaResultadoDiagrama(agora)
        };
    }

    public void MarcarEmProcessamento()
    {
        if (Status.Valor == StatusAnaliseEnum.EmProcessamento)
            return;

        if (Status.Valor != StatusAnaliseEnum.Recebido)
            throw new DomainException($"Só é possível marcar EmProcessamento quando o status atual for Recebido. Status atual: {Status.Valor}");

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

    public void RegistrarFalhaProcessamento(string mensagem, OrigemErroEnum? origemErro = null, int? numeroTentativa = null)
    {
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Erro);
        Erros.Add(ErroResultadoDiagrama.Criar(mensagem, null, origemErro, numeroTentativa));
    }

    public void RegistrarRejeicao(string mensagem, OrigemErroEnum? origemErro = null, int? numeroTentativa = null)
    {
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.Rejeitado);
        Erros.Add(ErroResultadoDiagrama.Criar(mensagem, null, origemErro, numeroTentativa));
    }

    public void RegistrarFalhaRelatorio(TipoRelatorioEnum tipoRelatorio, string mensagem)
    {
        ObterRelatorio(tipoRelatorio).RegistrarErro();
        Erros.Add(ErroResultadoDiagrama.Criar(mensagem, tipoRelatorio, OrigemErroEnum.GeracaoRelatorio, null));
    }

    /// <summary>
    /// Prepara o aggregate para reprocessamento após falha. Reseta relatórios e status, mantendo histórico de erros.
    /// </summary>
    /// <exception cref="DomainException">Status atual não permite reprocessamento.</exception>
    public void PrepararParaReprocessamento()
    {
        if (Status.Valor != StatusAnaliseEnum.Erro)
            throw new DomainException($"Só é possível preparar para reprocessamento quando o status atual for Erro. Status atual: {Status.Valor}");

        AnaliseResultado = null;
        Relatorios = CriarRelatoriosPadrao();
        Status = new StatusResultadoDiagrama(StatusAnaliseEnum.EmProcessamento);
        DataUltimaTentativa = new DataUltimaTentativaResultadoDiagrama(DateTimeOffset.UtcNow);
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
        if (!AnaliseDisponivel())
            return ResultadoSolicitacaoGeracaoRelatorioEnum.AnaliseNaoDisponivel;

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
            .Select(tipo => RelatorioGerado.Criar(tipo, Constants.TiposRelatorioAutomatico.Tipos.Contains(tipo) ? StatusRelatorioEnum.Automatico : StatusRelatorioEnum.NaoSolicitado))
            .ToList();
    }
}
