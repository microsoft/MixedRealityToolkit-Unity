<#
.SYNOPSIS
    Given a GitHub PullRequest ID, this generates a file containing a list of files that were changed
    in that pull request.
.DESCRIPTION
    Generates a file containing a list of all modified files (added/modified) in
    the given pull request. The output file contains a list that is newline delimited, for
    example:

    Assets/MixedRealityToolkit.SDK/AssemblyInfo.cs
    Assets/MixedRealityToolkit.Tests/PlayModeTests/ManipulationHandlerTests.cs

    The file will not contain files that have been completely deleted

    Note that this script also uses two environment variables: GITHUB_USERNAME and GITHUB_TOKEN
    to avoid passing explicit secrets through the command line (and thus avoiding showing up
    in logs)
.PARAMETER Output
    The output filename containing the list of modified files.
.PARAMETER RepoRoot
    The root folder of the repo - used to detect which files no longer exist
.EXAMPLE
    .\githubchanges.ps1 -Username username -Token token -Output c:\path\to\changes\file.txt -PullRequestId 1234 -RepoRoot c:\path\to
#>
param(
    [Parameter(Mandatory=$true)]
    [string]$Output,
    [Parameter(Mandatory=$true)]
    [string]$PullRequestId,
    [Parameter(Mandatory=$true)]
    [string]$RepoRoot
)

# $Username and $Token are stored in environment variables, per the security guidance here:
# https://docs.microsoft.com/en-us/azure/devops/pipelines/process/variables?view=azure-devops&tabs=yaml%2Cbatch#secret-variables
$Token = $env:GITHUB_TOKEN
$Username = $env:GITHUB_USERNAME

# This is just for testing that the env setting is correct
Write-Output $env:TEST_VALUE

# Both $Username and $Token are actually required inputs to this script, but may not
# be set for all of the pipelines that we run - for the cases where it's not set (i.e.
# intentionally not set), this script should fail gracefully and produce no output.
if ([string]::IsNullOrEmpty($Username) -or [string]::IsNullOrEmpty($Token))
{
    Write-Host "Username and Token are not both present, skipping."
    exit 0;
}

# If the output file already exists, blow it away.
# Each run should get a new set of changed files.
if (Test-Path $Output -PathType leaf) {
    Remove-Item $Output
}

# For the time being, we use the Github API directly instead of using an API layer
# like https://github.com/microsoft/PowerShellForGitHub or PyGithub, which could 
# greatly simplify this story. This may pull in some other module/installs which need
# to be validated against the pipeline infrastructure.
# This section uses basic auth (against the GitHub https target) using the instructions here:
# https://developer.github.com/v3/auth/
$BasicAuthenticationBase64 = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes(($Username + ":" + $Token)))
$Headers = @{ Authorization = "Basic $BasicAuthenticationBase64" }

# GitHub doesn't accept TLS version 1.0 which is the default on some versions of powershell (i.e. those
# on some host pool machines). Explicitly switch to version 1.2.
[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12

# The output of this API call is documented here:
# https://developer.github.com/v3/pulls/#list-pull-requests-files
$Uri = "https://api.github.com/repos/microsoft/MixedRealityToolkit-Unity/pulls/$PullRequestId/files"
$Response = Invoke-WebRequest -Uri $Uri -Headers $Headers -UseBasicParsing

# Content comes back as a raw JSON string, which must be converted into object form.
$ParsedResponse = ConvertFrom-Json $Response.content

foreach ($FileInfo in $ParsedResponse) {
    $JoinedPath = Join-Path -Path $RepoRoot -ChildPath $FileInfo.filename
    # Only save the path if the file still exists - also, do not store the absolute path
    # of the file, in case this set of information is used later in the pipeline on a different
    # machine/context.
    if (Test-Path $JoinedPath -PathType leaf) {
        Add-Content -Path $Output -Value $FileInfo.filename
    }
}
