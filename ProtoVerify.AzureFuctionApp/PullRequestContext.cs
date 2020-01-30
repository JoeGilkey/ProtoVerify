using System;
using Microsoft.Extensions.Logging;

using Octokit;

namespace ProtoVerify.AzureFuctionApp
{
    public class PullRequestContext
    {
        public PullRequestContext(PullRequestEventPayload payload, IConnection githubConnection, ILogger logger)
        {
            Payload = payload;
            GithubConnection = githubConnection;
            Logger = logger;
        }

        public PullRequestEventPayload Payload { get; }
        public IConnection GithubConnection { get; }
        public ILogger Logger { get; }
        public PullRequestInfo PullRequestInfo { get; set; }
        public RepositorySettings RepositorySettings { get; set; }
    }
}
