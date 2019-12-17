<#
.SYNOPSIS
    Given a GitHub PullRequest ID, target branch and repo root, this figures out the set of
    changed files that have occured in this PR.
.DESCRIPTION
    Generates a file containing a list of all modified files (added/modified) in
    the given pull request. The output file contains a list that is newline delimited, for
    example:

    Assets/MixedRealityToolkit.SDK/AssemblyInfo.cs
    Assets/MixedRealityToolkit.Tests/PlayModeTests/ManipulationHandlerTests.cs

    The file will not contain files that have been completely deleted

    Note that this script doesn't use the GitHub API, because doing so would require
    secret access to be publicly exposed - instead, this uses information that already
    exists on the local repo copy itself that had been copied down at Git checkout time.
.PARAMETER PullRequestId
    The ID of the pull request.
.PARAMETER Output
    The output filename containing the list of modified files.
.PARAMETER RepoRoot
    The root folder of the repo - used to detect which files no longer exist
.EXAMPLE
    .\githubchanges.ps1 -Output c:\path\to\changes\file.txt -PullRequestId 1234 -RepoRoot c:\path\to\mrtk -TargetBranch mrtk_development
#>
param(
    [string]$PullRequestId,
    [string]$TargetBranch,
    [Parameter(Mandatory=$true)]
    [string]$Output,
    [Parameter(Mandatory=$true)]
    [string]$RepoRoot
)

# The pull request ID might not be present (i.e. this is an adhoc build being spun up)
# and the target branch might not be set in which case there's nothing to validate.
if ([string]::IsNullOrEmpty($PullRequestId) -or [string]::IsNullOrEmpty($TargetBranch))
{
    Write-Host "PullRequestId and TargetBranch aren't specified, skipping."
    exit 0;
}

# If the output file already exists, blow it away.
# Each run should get a new set of changed files.
if (Test-Path $Output -PathType leaf) {
    Remove-Item $Output
}
New-Item -ItemType File -Force -Path $Output

# Create the path to the .git file in the repo root.
$gitDir = Join-Path -Path $RepoRoot -ChildPath ".git"

git --git-dir=$gitDir --work-tree=$RepoRoot  fetch --force --tags --prune --progress --no-recurse-submodules origin $TargetBranch

Write-Output "git --git-dir=$gitDir --work-tree=$RepoRoot diff --name-only pull/$PullRequestId/merge origin/$TargetBranch"

# The set of changed files is the diff between the target branch and the pull request
# branch that was checked out locally
$ChangedFiles=$(git --git-dir=$gitDir --work-tree=$RepoRoot diff --name-only pull/$PullRequestId/merge origin/$TargetBranch 2>&1)

foreach ($ChangedFile in $ChangedFiles) {
    $JoinedPath = Join-Path -Path $RepoRoot -ChildPath $ChangedFile
    # Only save the path if the file still exists - also, do not store the absolute path
    # of the file, in case this set of information is used later in the pipeline on a different
    # machine/context.
    if (Test-Path $JoinedPath -PathType leaf) {
        Add-Content -Path $Output -Value $ChangedFile
    }
}
