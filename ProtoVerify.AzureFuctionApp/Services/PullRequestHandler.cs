﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace ProtoVerify.AzureFuctionApp.Services
{
    public class PullRequestHandler : IPullRequestHandler
    {
        private readonly IPullRequestInfoProvider _prInfoProvider;
        private readonly IRepositorySettingsProvider _repositorySettingsProvider;
        private readonly IPullRequestPolicy _pullRequestPolicy;
        private readonly ICommitStatusWriter _statusWriter;

        public PullRequestHandler(IPullRequestInfoProvider prInfoProvider,
            IRepositorySettingsProvider repositorySettingsProvider,
            IPullRequestPolicy pullRequestPolicy,
            ICommitStatusWriter statusWriter)
        {
            _prInfoProvider = prInfoProvider;
            _pullRequestPolicy = pullRequestPolicy;
            _statusWriter = statusWriter;
            _repositorySettingsProvider = repositorySettingsProvider;
        }

        public async Task HandleWebhookEventAsync(PullRequestContext context)
        {
            context.Logger.LogDebug("Getting details for pull request #{PullRequestNumber}...", context.Payload.Number);
            context.PullRequestInfo = await _prInfoProvider.GetPullRequestInfoAsync(context);

            context.Logger.LogDebug("Getting repository settings for pull request #{PullRequestNumber}", context.Payload.Number);
            context.RepositorySettings = await _repositorySettingsProvider.GetRepositorySettingsAsync(context);

            context.Logger.LogDebug("Evaluating status for pull request #{PullRequestNumber}...", context.Payload.Number);
            var (state, description) = _pullRequestPolicy.GetStatus(context);
            context.Logger.LogInformation("Status for pull request #{PullRequestNumber} is '{PullRequestState}' ({PullRequestDescription})", context.Payload.Number, state, description);

            context.Logger.LogDebug("Writing commit status for pull request #{PullRequestNumber}...", context.Payload.Number);
            await _statusWriter.WriteCommitStatusAsync(context, state, description);
        }
    }
}
