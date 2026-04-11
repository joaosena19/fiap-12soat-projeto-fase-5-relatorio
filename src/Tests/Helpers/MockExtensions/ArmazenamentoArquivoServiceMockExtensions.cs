using Application.Contracts.Armazenamento;

namespace Tests.Helpers.MockExtensions;

public static class ArmazenamentoArquivoServiceMockExtensions
{
    public static ArmazenamentoSetup AoArmazenar(this Mock<IArmazenamentoArquivoService> mock) => new(mock);

    public static void DeveTerArmazenado(this Mock<IArmazenamentoArquivoService> mock, string? contentType = null)
    {
        if (contentType is not null)
            mock.Verify(x => x.ArmazenarAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), contentType), Times.Once);
        else
            mock.Verify(x => x.ArmazenarAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    public class ArmazenamentoSetup
    {
        private readonly Mock<IArmazenamentoArquivoService> _mock;

        public ArmazenamentoSetup(Mock<IArmazenamentoArquivoService> mock) => _mock = mock;

        public void Retorna(string url, string? contentType = null)
        {
            if (contentType is not null)
                _mock.Setup(x => x.ArmazenarAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), contentType)).ReturnsAsync(url);
            else
                _mock.Setup(x => x.ArmazenarAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(url);
        }

        public void LancaExcecao(Exception excecao, string? contentType = null)
        {
            if (contentType is not null)
                _mock.Setup(x => x.ArmazenarAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), contentType)).ThrowsAsync(excecao);
            else
                _mock.Setup(x => x.ArmazenarAsync(It.IsAny<Guid>(), It.IsAny<byte[]>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(excecao);
        }
    }
}
