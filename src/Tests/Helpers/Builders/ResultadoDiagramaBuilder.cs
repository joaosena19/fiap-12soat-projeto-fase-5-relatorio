using Domain.ResultadoDiagrama.Aggregates;
using Domain.ResultadoDiagrama.Entities;
using ConteudosRelatorio = Domain.ResultadoDiagrama.ValueObjects.RelatorioGerado.Conteudos;

namespace Tests.Helpers.Builders;

public class ResultadoDiagramaBuilder
{
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private AnaliseResultado? _analiseResultado;
    private readonly List<(TipoRelatorioEnum Tipo, ConteudosRelatorio Conteudo)> _relatoriosConcluidos = [];
    private readonly List<TipoRelatorioEnum> _relatoriosSolicitados = [];
    private readonly List<TipoRelatorioEnum> _relatoriosEmProcessamento = [];
    private readonly List<TipoRelatorioEnum> _relatoriosComErro = [];
    private bool _emProcessamento;
    private bool _comFalhaProcessamento;

    public ResultadoDiagramaBuilder ComAnaliseDiagramaId(Guid analiseDiagramaId)
    {
        _analiseDiagramaId = analiseDiagramaId;
        return this;
    }

    public ResultadoDiagramaBuilder EmProcessamento()
    {
        _emProcessamento = true;
        return this;
    }

    public ResultadoDiagramaBuilder Analisado()
    {
        _analiseResultado = CriarAnaliseResultadoPadrao();
        return this;
    }

    public ResultadoDiagramaBuilder Analisado(AnaliseResultado analiseResultado)
    {
        _analiseResultado = analiseResultado;
        return this;
    }

    public ResultadoDiagramaBuilder ComFalhaProcessamento()
    {
        _comFalhaProcessamento = true;
        return this;
    }

    public ResultadoDiagramaBuilder ComRelatorioSolicitado(TipoRelatorioEnum tipoRelatorio)
    {
        _relatoriosSolicitados.Add(tipoRelatorio);
        return this;
    }

    public ResultadoDiagramaBuilder ComRelatorioEmProcessamento(TipoRelatorioEnum tipoRelatorio)
    {
        _relatoriosEmProcessamento.Add(tipoRelatorio);
        return this;
    }

    public ResultadoDiagramaBuilder ComRelatorioConcluido(TipoRelatorioEnum tipoRelatorio)
    {
        _relatoriosConcluidos.Add((tipoRelatorio, CriarConteudoPadrao()));
        return this;
    }

    public ResultadoDiagramaBuilder ComRelatorioConcluido(TipoRelatorioEnum tipoRelatorio, ConteudosRelatorio conteudos)
    {
        _relatoriosConcluidos.Add((tipoRelatorio, conteudos));
        return this;
    }

    public ResultadoDiagramaBuilder ComRelatorioComErro(TipoRelatorioEnum tipoRelatorio)
    {
        _relatoriosComErro.Add(tipoRelatorio);
        return this;
    }

    public ResultadoDiagrama Build()
    {
        var resultadoDiagrama = ResultadoDiagrama.Criar(_analiseDiagramaId);

        if (_emProcessamento || _analiseResultado != null || _comFalhaProcessamento)
            resultadoDiagrama.MarcarEmProcessamento();

        if (_analiseResultado != null)
            resultadoDiagrama.RegistrarAnalise(_analiseResultado);

        if (_comFalhaProcessamento)
            resultadoDiagrama.RegistrarFalhaProcessamento("Falha de processamento simulada");

        foreach (var tipoRelatorio in _relatoriosSolicitados)
            resultadoDiagrama.MarcarRelatorioSolicitado(tipoRelatorio);

        foreach (var tipoRelatorio in _relatoriosEmProcessamento)
            resultadoDiagrama.MarcarRelatorioEmProcessamento(tipoRelatorio);

        foreach (var relatorio in _relatoriosConcluidos)
            resultadoDiagrama.ConcluirRelatorio(relatorio.Tipo, relatorio.Conteudo);

        foreach (var tipoRelatorio in _relatoriosComErro)
            resultadoDiagrama.RegistrarFalhaRelatorio(tipoRelatorio, "Falha de geração simulada");

        return resultadoDiagrama;
    }

    private static AnaliseResultado CriarAnaliseResultadoPadrao()
    {
        return AnaliseResultado.Criar(
            "Análise arquitetural gerada para testes",
            ["API Gateway", "Worker Service"],
            ["Acoplamento elevado entre módulos"],
            ["Separar responsabilidades por bounded context"]);
    }

    private static ConteudosRelatorio CriarConteudoPadrao()
    {
        return ConteudosRelatorio.Criar(new Dictionary<string, string>
        {
            ["conteudo"] = "Relatorio gerado"
        });
    }
}