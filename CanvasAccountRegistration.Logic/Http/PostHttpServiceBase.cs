using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;

namespace CanvasAccountRegistration.Logic.Http
{
    public interface IPostHttpServiceBase
    {
        void OverrideDefaultBearerToken(string token);
    }

    public class PostHttpServiceBase : IPostHttpServiceBase
    {
        protected IHttpClient client;
        protected readonly ILogger logger;

        public PostHttpServiceBase(IHttpClient client,
            ILogger logger)
        {
            this.client = client;
            this.logger = logger;
        }
       
        
        public void OverrideDefaultBearerToken(string token)
        {
            client.SetBearerToken(token);
        }

        protected static Uri CombineUrls(string url, string id)
        {
            if (!url.EndsWith("/")) url += "/";
            return new Uri(url + id);
        }

        protected void LogHttpException(HttpRequestException e)
        {
            logger.LogError(e, $"{e.Message} Status code: {e.StatusCode}");
        }

    }
}
