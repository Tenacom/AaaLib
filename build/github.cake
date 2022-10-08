// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#addin nuget:?package=Octokit&version=3.0.1

#nullable enable

// ---------------------------------------------------------------------------------------------
// GitHub API helpers
// ---------------------------------------------------------------------------------------------

using System.Threading.Tasks;
using Octokit;

/*
 * Summary : Asynchronously creates a new draft release on the GitHub repository.
 * Params  : data - Build configuration data.
 * Returns : A Task, representing the ongoing operation, whose value will be the ID of the newly created release.
 */
async Task<int> CreateDraftReleaseAsync(BuildData data)
{
    var tag = data.Version;
    Information($"Generating release notes for {tag}...");
    var client = CreateGitHubClient();
    var releaseNotesRequest = new GenerateReleaseNotesRequest(tag)
    {
        TargetCommitish = data.Branch,
    };

    var generateNotesResponse = await client.Repository.Release.GenerateReleaseNotes(data.RepositoryOwner, data.RepositoryName, releaseNotesRequest).ConfigureAwait(false);
    var body = $"We also have a [human-curated changelog]({data.RepositoryHostUrl}/{data.RepositoryOwner}/{data.RepositoryName}/blob/{tag}/CHANGELOG.md).\n\n---\n\n"
             + generateNotesResponse.Body;

    Information($"Creating a draft release for {tag}...");
    var newRelease = new NewRelease(tag)
    {
        Name = tag,
        Body = body,
        TargetCommitish = data.Branch,
        Prerelease = data.IsPrerelease,
        Draft = true,
    };

    var createReleaseResponse = await client.Repository.Release.Create(data.RepositoryOwner, data.RepositoryName, newRelease).ConfigureAwait(false);
    return createReleaseResponse.Id;
}

/*
 * Summary : Asynchronously publishes a draft release on the GitHub repository.
 * Params  : data - Build configuration data.
 *           id   - The ID of the release.
 * Returns : A Task that represents the ongoing operation.
 */
Task PublishReleaseAsync(BuildData data, int id)
{
    // The version could have changed, for example if we updated the changelog, thus altering the Git height.
    var tag = data.Version;
    Information($"Publishing the previously created release as {tag}...");
    var update = new ReleaseUpdate
    {
        TagName = tag,
        Name = tag,
        Draft = false,
    };

    var client = CreateGitHubClient();
    return client.Repository.Release.Edit(data.RepositoryOwner, data.RepositoryName, id, update);
}

/*
 * Summary : Asynchronously deletes a release and, optionally, the corresponding tag on the GitHub repository.
 * Params  : data    - Build configuration data.
 *           id      - The ID of the release.
 *           tagName - The tag name, or null to not delete a tag.
 * Returns : A Task that represents the ongoing operation.
 */
async Task DeleteReleaseAsync(BuildData data, int id, string? tagName)
{
    Information("Deleting the previously created release...");
    var client = CreateGitHubClient();
    await client.Repository.Release.Delete(data.RepositoryOwner, data.RepositoryName, id).ConfigureAwait(false);
    if (tagName != null)
    {
        var reference = "refs/tags/" + tagName;
        Information($"Deleting {reference} in GitHub repository...");
        await client.Git.Reference.Delete(data.RepositoryOwner, data.RepositoryName, reference).ConfigureAwait(false);
    }
}

GitHubClient CreateGitHubClient()
{
    var client = new GitHubClient(new ProductHeaderValue("Buildvana"));
    client.Credentials = new Credentials(GetOptionOrFail<string>("githubToken"));
    return client;
}
