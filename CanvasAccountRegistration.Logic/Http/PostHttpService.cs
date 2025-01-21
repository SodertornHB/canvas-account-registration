using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CanvasAccountRegistration.Logic.Http
{
    public interface IPostHttpService<TRequestModel, TResonseModel> : IPostHttpServiceBase
    {
        Task<TResonseModel> Post(string url, TRequestModel model);
    }

    public class PostHttpService<TRequestModel, TResonseModel> : PostHttpServiceBase, IPostHttpService<TRequestModel, TResonseModel>
    {
        public PostHttpService(IHttpClient client,
            ILogger<HttpService<TRequestModel>> logger)
            : base(client, logger)
        { }

        public virtual async Task<TResonseModel> Post(string url, TRequestModel model)
        {
            try
            {
                var content = JsonConvert.SerializeObject(model);
                var response = await client.Post(new Uri(url), content);
                response.CheckStatus();
                logger.LogDebug($"Post data to {url}: {response.Content}");
                return JsonConvert.DeserializeObject<TResonseModel>(response.Content);
            }
            catch (HttpRequestException e)
            {
                LogHttpException(e);
                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, e.Message);
                throw;
            }
        }
    }
}
