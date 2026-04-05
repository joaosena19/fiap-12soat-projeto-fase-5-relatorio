using Amazon.S3;
using Amazon.S3.Model;

namespace Tests.Helpers.MockExtensions;

public static class AmazonS3MockExtensions
{
    public static S3PutObjectSetup AoSalvarObjeto(this Mock<IAmazonS3> mock) => new(mock);

    public static void DeveTerSalvadoObjeto(this Mock<IAmazonS3> mock, Action<PutObjectRequest>? validacao = null)
    {
        if (validacao is not null)
            mock.Verify(x => x.PutObjectAsync(It.Is<PutObjectRequest>(r => ValidarRequest(r, validacao)), default), Times.Once);
        else
            mock.Verify(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default), Times.Once);
    }

    private static bool ValidarRequest(PutObjectRequest request, Action<PutObjectRequest> validacao)
    {
        validacao(request);
        return true;
    }

    public class S3PutObjectSetup
    {
        private readonly Mock<IAmazonS3> _mock;

        public S3PutObjectSetup(Mock<IAmazonS3> mock) => _mock = mock;

        public void Retorna(Action<PutObjectRequest>? callback = null)
        {
            var setup = _mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default));

            if (callback is not null)
                setup.Callback<PutObjectRequest, CancellationToken>((req, _) => callback(req)).ReturnsAsync(new PutObjectResponse());
            else
                setup.ReturnsAsync(new PutObjectResponse());
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.PutObjectAsync(It.IsAny<PutObjectRequest>(), default)).ThrowsAsync(excecao);
        }
    }
}
