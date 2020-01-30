using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;
using Octokit;
using Octokit.Internal;

using ProtoVerify.Common;

namespace ProtoVerify.AzureFuctionApp
{
    public static class GitHubWebHook
    {
        [FunctionName("GitHubWebHook")]
        public static async Task<HttpResponseMessage> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "POST")] HttpRequestMessage request,
            ILogger logger,
            ExecutionContext context)
        {
            var services = GithubServices.GetServicesFromContext(context);

            string eventName = request.Headers.GetValueOrDefault("X-GitHub-Event");
            string deliveryId = request.Headers.GetValueOrDefault("X-GitHub-Delivery");
            string signature = request.Headers.GetValueOrDefault("X-Hub-Signature");

            logger.LogInformation("Webhook delivery: Delivery id = '{DeliveryId}', Event name = '{EventName}'", deliveryId, eventName);

            var payloadBytes = await request.Content.ReadAsByteArrayAsync();
            if (!services.PayloadValidator.IsPayloadSignatureValid(payloadBytes, signature))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest, "Invalid signature");
            }

            if(eventName=="pull_request")
            {
                var pullRequestPayload = await DeserializeBody<PullRequestEventPayload>(request.Content);
                //process...
            }
            else
            {
                logger.LogInformation($"Unknown event '{eventName}', ignoring");
            }

            return request.CreateResponse(HttpStatusCode.OK);
        }

        private static async Task<T> DeserializeBody<T>(HttpContent content)
        {
            string json = await content.ReadAsStringAsync();
            var serializer = new SimpleJsonSerializer();
            return serializer.Deserialize<T>(json);
        }
    }
}
