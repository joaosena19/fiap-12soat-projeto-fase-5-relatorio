using Domain.ResultadoDiagrama.Enums;
using Tests.Helpers.Fixtures;

namespace Tests.Features.ResultadoDiagrama.SolicitarGeracaoRelatorios;

public class SolicitarGeracaoRelatoriosUseCaseTests
{
    private readonly SolicitarGeracaoRelatoriosUseCaseTestFixture _fixture;

    public SolicitarGeracaoRelatoriosUseCaseTests()
    {
        _fixture = new SolicitarGeracaoRelatoriosUseCaseTestFixture();
    }

    [Fact(DisplayName = "Deve apresentar erro quando lista de tipos está vazia")]
    [Trait("UseCase", "SolicitarGeracaoRelatorios")]
    public async Task ExecutarAsync_DeveApresentarErro_QuandoListaTiposVazia()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio = [];

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.InvalidInput);
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoSolicitacaoGeracao();
    }

    [Fact(DisplayName = "Deve apresentar erro quando resultado não existe")]
    [Trait("UseCase", "SolicitarGeracaoRelatorios")]
    public async Task ExecutarAsync_DeveApresentarErro_QuandoResultadoNaoExiste()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new[] { TipoRelatorioEnum.Markdown };
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).NaoRetornaNada();

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.ResourceNotFound);
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoSolicitacaoGeracao();
    }

    [Fact(DisplayName = "Deve aceitar geração quando relatório ainda não foi solicitado")]
    [Trait("UseCase", "SolicitarGeracaoRelatorios")]
    public async Task ExecutarAsync_DeveApresentarSucesso_QuandoRelatorioNaoSolicitadoAntes()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new[] { TipoRelatorioEnum.Markdown };
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);
        _fixture.GatewayMock.AoSalvar().RetornaMesmoObjeto();

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoSucesso();
        _fixture.PresenterMock.NaoDeveTerApresentadoErro();
        _fixture.GatewayMock.DeveTerSalvo();
        _fixture.MessagePublisherMock.DeveTerPublicadoSolicitacaoGeracaoComTipos(TipoRelatorioEnum.Markdown);
        _fixture.PresenterMock.DeveTerApresentadoSucessoCom(resultadoSolicitacaoRelatoriosDto =>
            resultadoSolicitacaoRelatoriosDto.Relatorios.Any(item =>
                item.Tipo == TipoRelatorioEnum.Markdown && item.Resultado == ResultadoSolicitacaoGeracaoRelatorioEnum.AceitoParaGeracao));
    }

    [Fact(DisplayName = "Deve retornar concluído quando relatório já foi concluído")]
    [Trait("UseCase", "SolicitarGeracaoRelatorios")]
    public async Task ExecutarAsync_DeveApresentarSucesso_QuandoRelatorioJaConcluido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new[] { TipoRelatorioEnum.Markdown };
        var resultadoDiagrama = new ResultadoDiagramaBuilder().ComAnaliseDiagramaId(analiseDiagramaId).Analisado().ComRelatorioConcluido(TipoRelatorioEnum.Markdown).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).Retorna(resultadoDiagrama);

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoSucessoCom(resultadoSolicitacaoRelatoriosDto =>
            resultadoSolicitacaoRelatoriosDto.Relatorios.Any(item =>
                item.Tipo == TipoRelatorioEnum.Markdown && item.Resultado == ResultadoSolicitacaoGeracaoRelatorioEnum.Concluido));
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoSolicitacaoGeracao();
    }

    [Fact(DisplayName = "Deve apresentar erro inesperado quando exceção não prevista ocorre")]
    [Trait("UseCase", "SolicitarGeracaoRelatorios")]
    public async Task ExecutarAsync_DeveApresentarErroInesperado_QuandoExcecaoNaoPrevista()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var tiposRelatorio = new[] { TipoRelatorioEnum.Markdown };
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(analiseDiagramaId).LancaExcecao(new Exception("erro inesperado"));

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.UnexpectedError);
        _fixture.LoggerMock.DeveTerLogadoErrorComException();
    }

    [Fact(DisplayName = "Deve apresentar erro quando tipos de relatório inválidos são informados")]
    [Trait("UseCase", "SolicitarGeracaoRelatorios")]
    public async Task ExecutarAsync_DeveApresentarErro_QuandoTiposRelatorioInvalidos()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        IReadOnlyCollection<TipoRelatorioEnum> tiposRelatorio = [(TipoRelatorioEnum)999];

        // Act
        await _fixture.ExecutarAsync(analiseDiagramaId, tiposRelatorio);

        // Assert
        _fixture.PresenterMock.DeveTerApresentadoErro(ErrorType.InvalidInput);
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoSolicitacaoGeracao();
    }
}