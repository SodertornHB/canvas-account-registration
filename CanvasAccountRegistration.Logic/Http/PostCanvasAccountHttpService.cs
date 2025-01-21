using CanvasAccountRegistration.Logic.Http;
using Logic.HttpModel;
using Microsoft.Extensions.Logging;

namespace Logic.Http
{
    public interface IPostCanvasAccountHttpService : IPostHttpService<PostCanvasAccountRequestModel, PostCanvasAccountResponseModel>
    {     
    }
    public class PostCanvasAccountHttpService : PostHttpService<PostCanvasAccountRequestModel, PostCanvasAccountResponseModel>, IPostCanvasAccountHttpService
    {
        public PostCanvasAccountHttpService(IHttpClient client,
            ILogger<HttpService<PostCanvasAccountRequestModel>> logger)
            : base(client, logger)
        {
        }
    }
}
