using Moq;
using Moq.Protected;
using Movies.Client;
using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace TestProject1
{
    public class TestAPInotFoundException
    {
        [Fact]
        public async Task Test_NotFoundExceptonFor_MovieAPI()
        {
            var httpclient = new HttpClient(new NotfoundAPIHandler())
            {
                BaseAddress = new Uri("http://localhost:57863")
            };

            var testapi = new TestableAPIExample(httpclient);

            await Assert.ThrowsAsync<ResourseNotFoundException>(
                () => testapi.GetMovieTestAPI());
        }

        [Fact]
        public async Task Test_UnauthrisedTestUsingMoq_MovieAPI()
        {
            var mokMessageHandler = new Mock<HttpMessageHandler>();

            mokMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
               .ReturnsAsync(() => new HttpResponseMessage() { StatusCode = HttpStatusCode.Unauthorized });

            var httpclient = new HttpClient(mokMessageHandler.Object)
            {
                BaseAddress = new Uri("http://localhost:57863")
            };

            var testapi = new TestableAPIExample(httpclient);

            await Assert.ThrowsAsync<UnauthorisedRequestException>(() => testapi.GetMovieTestAPI());

        }



    }
}
