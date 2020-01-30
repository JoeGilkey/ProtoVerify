using System;
using System.Threading.Tasks;

using Octokit;

namespace ProtoVerify.AzureFuctionApp.Services
{
    public interface IGithubConnectionCache
    {
        Task<IConnection> GetConnectionAsync(long installationId);
    }

    public interface IGithubAppTokenService
    {
        Task<string> GetTokenForApplicationAsync();
        Task<string> GetTokenForInstallationAsync(long installationId);
    }

    public interface ICommitStatusWriter
    {
        Task WriteCommitStatusAsync(PullRequestContext context, CommitState state, string description);
    }

    public interface IGithubPayloadValidator
    {
        bool IsPayloadSignatureValid(byte[] bytes, string receivedSignature);
    }

    internal interface IPullRequestHandler
    {
        Task HandleWebhookEventAsync(PullRequestContext context);
    }

    public interface IPullRequestInfoProvider
    {
        Task<PullRequestInfo> GetPullRequestInfoAsync(PullRequestContext context);
    }

    public interface IPullRequestPolicy
    {
        (CommitState state, string description) GetStatus(PullRequestContext context);
    }

    public interface IRepositorySettingsProvider
    {
        Task<RepositorySettings> GetRepositorySettingsAsync(PullRequestContext context);
    }
}
