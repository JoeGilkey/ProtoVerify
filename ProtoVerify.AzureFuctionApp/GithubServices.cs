using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Azure.WebJobs;

using ProtoVerify.AzureFuctionApp.Services;

namespace ProtoVerify.AzureFuctionApp
{
    public class GithubServices
    {
        private GithubServices(IConfigurationRoot configuration, IOptions<GithubSettings> options, GithubConnectionCache githubConnectionCache, PullRequestHandler pullRequestHandler, GithubPayloadValidator payloadValidator)
        {
            Configuration = configuration;
            Options = options;
            GithubConnectionCache = githubConnectionCache;
            PullRequestHandler = pullRequestHandler;
            PayloadValidator = payloadValidator;
        }

        public IConfigurationRoot Configuration { get; }
        public IOptions<GithubSettings> Options { get; }
        public GithubConnectionCache GithubConnectionCache { get; }
        public PullRequestHandler PullRequestHandler { get; }
        public GithubPayloadValidator PayloadValidator { get; }

        private static GithubServices _services = null;
        public static GithubServices GetServicesFromContext(ExecutionContext context)
        {
            if (_services != null) return _services;

            var configuration = new ConfigurationBuilder()
                .SetBasePath(context.FunctionAppDirectory)
                .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var options = Microsoft.Extensions.Options.Options.Create(configuration.GetSection("Github").Get<GithubSettings>());

            var githubConnectionCache = new GithubConnectionCache(new GithubAppTokenService(options));

            var pullRequestHandler = new PullRequestHandler(
                new PullRequestInfoProvider(),
                new RepositorySettingsProvider(),
                new WorkInProgressPullRequestPolicy(),
                new CommitStatusWriter(options));

            var payloadValidator = new GithubPayloadValidator(options);

            return _services = new GithubServices(configuration, options, githubConnectionCache, pullRequestHandler, payloadValidator);
        }
    }
}
