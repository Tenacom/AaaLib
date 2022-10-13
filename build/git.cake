// Copyright (C) Tenacom and contributors. Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

#nullable enable

// ---------------------------------------------------------------------------------------------
// Git repository helpers
// ---------------------------------------------------------------------------------------------

using System;
using System.Linq;

/*
 * Summary : Gets the name of the current Git branch.
 * Params  : context - The Cake context.
 * Returns : If HEAD is on a branch, the name of the branch; otherwise, the empty string.
 */
static string GetCurrentGitBranch(this ICakeContext context) => context.Exec("git", "branch --show-current").FirstOrDefault(string.Empty);

/*
 * Summary : Attempts to get information about the remote repository.
 * Params  : context - The Cake context.
 * Returns : Remote  - The Git remote name.
 *           HostUrl - The base URL of the Git repository host.
 *           Owner   - The repository owner.
 *           Name    - The repository name.
 * Remarks : - If the githubRepository argument is given, or the GITHUB_REPOSITORY environment variable is set
 *             (as it happens in GitHub Actions,) Owner and Name are taken from there, while Remote is set
 *             to the first Git remote found whose fetch URL matches them.
 *           - If GITHUB_REPOSITORY is not available, Git remote fetch URLs are parsed for Owner and Name;
 *             remotes "upstream" and "origin" are tested, in that order, in case "origin" is a fork.
 */
static bool TryGetRepositoryInfo(this ICakeContext context, out (string Remote, string HostUrl, string Owner, string Name) result)
{
    return TryGetRepositoryInfoFromGitHubActions(out result)
        || TryGetRepositoryInfoFromGitRemote("upstream", out result)
        || TryGetRepositoryInfoFromGitRemote("origin", out result);

    bool TryGetRepositoryInfoFromGitHubActions(out (string Remote, string HostUrl, string Owner, string Name) result)
    {
        var repository = context.GetOption<string>("githubRepository", string.Empty);
        if (string.IsNullOrEmpty(repository))
        {
            result = default;
            return false;
        }

        var hostUrl = context.GetOptionOrFail<string>("githubServerUrl");
        var segments = repository.Split('/');
        foreach (var remote in context.Exec("git", "remote"))
        {
            if (TryGetRepositoryInfoFromGitRemote(remote, out result)
                && string.Equals(result.HostUrl, hostUrl, StringComparison.Ordinal)
                && string.Equals(result.Owner, segments[0], StringComparison.Ordinal)
                && string.Equals(result.Name, segments[1], StringComparison.Ordinal))
            {
                return true;
            }
        }

        result = default;
        return false;
    }

    bool TryGetRepositoryInfoFromGitRemote(string remote, out (string Remote, string HostUrl, string Owner, string Name) result)
    {
        if (context.Exec("git", "remote get-url " + remote, out var output) != 0)
        {
            result = default;
            return false;
        }

        var url = output.FirstOrDefault();
        if (string.IsNullOrEmpty(url))
        {
            result = default;
            return false;
        }

        Uri uri;
        try
        {
            uri = new Uri(url);
        }
        catch (UriFormatException)
        {
            result = default;
            return false;
        }

        var path = uri.AbsolutePath;
        path = path.EndsWith(".git", StringComparison.Ordinal)
            ? path.Substring(1, path.Length - 5)
            : path.Substring(1);

        var segments = path.Split('/');
        if (segments.Length != 2)
        {
            result = default;
            return false;
        }

        result = (remote, $"{uri.Scheme}://{uri.Host}{(uri.IsDefaultPort ? null : ":" + uri.Port.ToString())}", segments[0], segments[1]);
        return true;
    }
}

/*
 * Summary : Tells whether a tag exists in the local Git repository.
 * Params  : context - The Cake context.
 *           tag     - The tag to check for.
 * Returns : True if the tag exists; false otherwise.
 */
static bool GitTagExists(this ICakeContext context, string tag) => context.Exec("git", "tag").Any(s => string.Equals(tag, s, StringComparison.Ordinal));

/*
 * Summary : Sets Git user name and email.
 * Params  : context - The Cake context.
 *           name    - The name of the user.
 *           email   - The email address of the user.
 */
static void GitSetUserIdentity(this ICakeContext context, string name, string email)
{
    context.Information($"Setting Git user name to '{name}'...");
    _ = context.Exec(
        "git",
        new ProcessArgumentBuilder()
            .Append("config")
            .Append("user.name")
            .AppendQuoted(name));

    context.Information($"Setting Git user email to '{email}'...");
    _ = context.Exec(
        "git",
        new ProcessArgumentBuilder()
            .Append("config")
            .Append("user.email")
            .AppendQuoted(email));
}

static void GitSetGitHubTokenForRemote(this ICakeContext context, string remote, string token)
{
    var fetchUrl = GetRemoteUrlOrEmpty(false);
    Ensure(!string.IsNullOrEmpty(fetchUrl), $"Cannot get Git fetch URL for '{remote}'.");
    context.Verbose($"Git fetch URL for '{remote}' is '{fetchUrl}'.");

    var pushUrl = GetRemoteUrlOrEmpty(true);
    Ensure(!string.IsNullOrEmpty(pushUrl), $"Cannot get Git push URL for '{remote}'.");
    context.Verbose($"Git push URL for '{remote}' is '{pushUrl}'.");

    // This is how actions/checkout builds the extra header
    // (including the questionable casing of "AUTHORIZATION: basic")
    // https://github.com/actions/checkout/blob/v3.1.0/src/git-auth-helper.ts#L54-L63
    var authText = "x-access-token:" + token;
    var authBytes = Encoding.UTF8.GetBytes(authText);
    var header = "AUTHORIZATION: basic " + Convert.ToBase64String(authBytes);

    SetAuthHeaderForUrl(fetchUrl, header);
    if (!string.Equals(fetchUrl, pushUrl, StringComparison.Ordinal))
    {
        SetAuthHeaderForUrl(pushUrl, header);
    }

    string GetRemoteUrlOrEmpty(bool push)
    {
        try
        {
            return context.Exec("git", $"remote get-url {(push ? "--push " : null)}{remote}").FirstOrDefault(string.Empty);
        }
        catch (CakeException)
        {
            return string.Empty;
        }
    }

    void SetAuthHeaderForUrl(string url, string extraHeader)
    {
        context.Information($"Setting authorization header for '{url}'...");
        _ = context.Exec(
            "git",
            new ProcessArgumentBuilder()
                .Append("config")
                .Append($"http.{url}.extraHeader")
                .AppendQuoted(extraHeader));
    }
}
