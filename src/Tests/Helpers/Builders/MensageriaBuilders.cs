using Application.Contracts.Messaging.Dtos;

namespace Tests.Helpers.Builders;

public class ProcessamentoDiagramaAnalisadoDtoBuilder
{
    private string _correlationId = Guid.NewGuid().ToString();
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private string _descricaoAnalise = "Análise arquitetural completa";
    private List<string> _componentesIdentificados = ["API Gateway", "Worker Service"];
    private List<string> _riscosArquiteturais = ["Acoplamento elevado"];
    private List<string> _recomendacoesBasicas = ["Separar responsabilidades"];
    private DateTimeOffset _dataConclusao = DateTimeOffset.UtcNow;

    public ProcessamentoDiagramaAnalisadoDtoBuilder ComAnaliseDiagramaId(Guid valor) { _analiseDiagramaId = valor; return this; }
    public ProcessamentoDiagramaAnalisadoDtoBuilder ComDescricaoAnalise(string valor) { _descricaoAnalise = valor; return this; }
    public ProcessamentoDiagramaAnalisadoDtoBuilder ComComponentes(List<string> valor) { _componentesIdentificados = valor; return this; }
    public ProcessamentoDiagramaAnalisadoDtoBuilder ComRiscos(List<string> valor) { _riscosArquiteturais = valor; return this; }
    public ProcessamentoDiagramaAnalisadoDtoBuilder ComRecomendacoes(List<string> valor) { _recomendacoesBasicas = valor; return this; }

    public ProcessamentoDiagramaAnalisadoDto Build() => new()
    {
        CorrelationId = _correlationId,
        AnaliseDiagramaId = _analiseDiagramaId,
        DescricaoAnalise = _descricaoAnalise,
        ComponentesIdentificados = _componentesIdentificados,
        RiscosArquiteturais = _riscosArquiteturais,
        RecomendacoesBasicas = _recomendacoesBasicas,
        DataConclusao = _dataConclusao
    };
}

public class ProcessamentoDiagramaErroDtoBuilder
{
    private string _correlationId = Guid.NewGuid().ToString();
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private string _motivo = "Timeout no processamento de LLM";
    private int _tentativasRealizadas = 3;
    private DateTimeOffset _dataErro = DateTimeOffset.UtcNow;

    public ProcessamentoDiagramaErroDtoBuilder ComAnaliseDiagramaId(Guid valor) { _analiseDiagramaId = valor; return this; }
    public ProcessamentoDiagramaErroDtoBuilder ComMotivo(string valor) { _motivo = valor; return this; }
    public ProcessamentoDiagramaErroDtoBuilder ComTentativas(int valor) { _tentativasRealizadas = valor; return this; }

    public ProcessamentoDiagramaErroDto Build() => new()
    {
        CorrelationId = _correlationId,
        AnaliseDiagramaId = _analiseDiagramaId,
        Motivo = _motivo,
        TentativasRealizadas = _tentativasRealizadas,
        DataErro = _dataErro
    };
}
