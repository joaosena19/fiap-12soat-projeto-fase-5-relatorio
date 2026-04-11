using Infrastructure.Database;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Repositories;

public class ResultadoDiagramaRepositoryTests : IDisposable
{
    private readonly AppDbContext _contexto;
    private readonly ResultadoDiagramaRepository _repository;

    public ResultadoDiagramaRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"repo-roundtrip-{Guid.NewGuid()}")
            .Options;

        _contexto = new AppDbContext(options);
        _repository = new ResultadoDiagramaRepository(_contexto);
    }

    [Fact(DisplayName = "Deve salvar e recuperar ResultadoDiagrama com AnaliseResultado")]
    [Trait("Infrastructure", "ResultadoDiagramaRepository")]
    public async Task SalvarERecuperar_DevePersistirAnaliseResultado_QuandoAnalisado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .ComAnaliseDiagramaId(analiseDiagramaId)
            .Analisado()
            .Build();

        // Act
        await _repository.SalvarAsync(resultadoDiagrama);
        var recuperado = await _repository.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        recuperado.ShouldNotBeNull();
        recuperado.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        recuperado.DeveEstarAnalisadoComDescricao("Análise arquitetural gerada para testes");
        recuperado.AnaliseResultado!.ComponentesIdentificados.ShouldNotBeEmpty();
        recuperado.AnaliseResultado.RiscosArquiteturais.ShouldNotBeEmpty();
        recuperado.AnaliseResultado.RecomendacoesBasicas.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "Deve salvar e recuperar ResultadoDiagrama com relatório concluído")]
    [Trait("Infrastructure", "ResultadoDiagramaRepository")]
    public async Task SalvarERecuperar_DevePersistirRelatorios_QuandoRelatorioConcluido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .ComAnaliseDiagramaId(analiseDiagramaId)
            .Analisado()
            .ComRelatorioConcluido(TipoRelatorioEnum.Json)
            .Build();

        // Act
        await _repository.SalvarAsync(resultadoDiagrama);
        var recuperado = await _repository.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        recuperado.ShouldNotBeNull();
        var relatorio = recuperado.ObterRelatorio(TipoRelatorioEnum.Json);
        relatorio.Status.Valor.ShouldBe(StatusRelatorioEnum.Concluido);
        relatorio.Conteudos.Valores.ShouldNotBeEmpty();
    }

    [Fact(DisplayName = "Deve salvar e recuperar ResultadoDiagrama com erros")]
    [Trait("Infrastructure", "ResultadoDiagramaRepository")]
    public async Task SalvarERecuperar_DevePersistirErros_QuandoFalhaProcessamento()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .ComAnaliseDiagramaId(analiseDiagramaId)
            .ComFalhaProcessamento()
            .Build();

        // Act
        await _repository.SalvarAsync(resultadoDiagrama);
        var recuperado = await _repository.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        recuperado.ShouldNotBeNull();
        recuperado.DeveConterErroComMensagem("Falha de processamento simulada");
    }

    [Fact(DisplayName = "Deve atualizar ResultadoDiagrama existente sem duplicar")]
    [Trait("Infrastructure", "ResultadoDiagramaRepository")]
    public async Task SalvarAsync_DeveAtualizar_QuandoResultadoJaExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var resultadoDiagrama = new ResultadoDiagramaBuilder()
            .ComAnaliseDiagramaId(analiseDiagramaId)
            .EmProcessamento()
            .Build();

        await _repository.SalvarAsync(resultadoDiagrama);

        resultadoDiagrama.RegistrarAnalise(global::Domain.ResultadoDiagrama.Entities.AnaliseResultado.Criar(
            "Análise após processamento",
            ["Componente A"],
            ["Risco B"],
            ["Recomendação C"]));

        // Act
        await _repository.SalvarAsync(resultadoDiagrama);
        var recuperado = await _repository.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        recuperado.ShouldNotBeNull();
        recuperado.DeveEstarAnalisadoComDescricao("Análise após processamento");
        _contexto.ResultadosDiagrama.Count(item => item.AnaliseDiagramaId == analiseDiagramaId).ShouldBe(1);
    }

    [Fact(DisplayName = "Deve retornar nulo quando resultado não existir")]
    [Trait("Infrastructure", "ResultadoDiagramaRepository")]
    public async Task ObterPorAnaliseDiagramaIdAsync_DeveRetornarNulo_QuandoNaoExistir()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();

        // Act
        var resultado = await _repository.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        // Assert
        resultado.ShouldBeNull();
    }

    public void Dispose()
    {
        _contexto.Dispose();
    }
}
