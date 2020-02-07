<#
.SYNOPSIS
    This script generates a list of changed files in a given pull request and outputs
    that list to a file.
.DESCRIPTION
    Generates a file containing a list of all modified files (added/modified) in
    the given pull request. The output file contains a list that is newline delimited, for
    example:

    Assets/MixedRealityToolkit.SDK/AssemblyInfo.cs
    Assets/MixedRealityToolkit.Tests/PlayModeTests/ManipulationHandlerTests.cs

    The file will not contain files that have been completely deleted (for example, if
    a change deleted Assets/MixedRealityToolkit.SDK/DeletedFile.cs), that line will not exist
    in the output file.

    Note that this script assumes that the local git repo doesn't already contain the
    target branch (e.g. mrtk_development). This is what happens by default on Azure DevOps
    pipeline integrations with Github pull requests.

    In particular, this will checkout (via this command:
    git fetch --force --tags --prune --progress --no-recurse-submodules origin $(System.PullRequest.TargetBranch))
.EXAMPLE
    .\githubchanges.ps1 -OutputFile c:\path\to\changes\file.txt -PullRequestId 1234 -RepoRoot c:\path\to\mrtk -TargetBranch mrtk_development
#>
param(
    # The ID of the pull request. (e.g. 1234)
    [string]$PullRequestId,

    # The target branch that the pull request will merge into (e.g. mrtk_development)
    [string]$TargetBranch,

    # The output filename (e.g. c:\path\to\output.txt)
    [Parameter(Mandatory=$true)]
    [string]$OutputFile,

    # The root folder of the repo (e.g. c:\repo)
    # This primarily used to filter out files that were deleted.
    [Parameter(Mandatory=$true)]
    [string]$RepoRoot
)

# The pull request ID might not be present (i.e. this is an adhoc build being spun up)
# and the target branch might not be set in which case there's nothing to validate.
if ([string]::IsNullOrEmpty($PullRequestId) -or [string]::IsNullOrEmpty($TargetBranch))
{
    Write-Warning "PullRequestId or TargetBranch aren't specified, skipping."
    exit 0;
}

# If the output file already exists, blow it away. Each run should get a new set of changed files.
if (Test-Path $OutputFile -PathType leaf) {
    Remove-Item $OutputFile
}
New-Item -ItemType File -Force -Path $OutputFile

# The path to the .git file is necessary when invoking the git command below, as the working
# directory may not actually be pointed toward the git path.
$gitDir = Join-Path -Path $RepoRoot -ChildPath ".git"

# Fetches the target branch so that the git diffing down below will actually be possible. git diff will list
# the set of changed files between two different commit stamps (or branches, in this case), and needs
# both branches to exist in order to make this happen.
# Uses a shallow fetch (i.e. depth=1) because only the latest commit from the target branch is
# needed to do the diff.
git --git-dir=$gitDir --work-tree=$RepoRoot  fetch --depth=1 --force --tags --prune --progress --no-recurse-submodules origin $TargetBranch

# The set of changed files is the diff between the target branch and the pull request
# branch that was checked out locally. Note that the format of the pull request branch
# (i.e. "pull/$PullRequestId/merge") is based on the format that Azure DevOps does for its
# local checkout of the pull request code.
# WARNING: This is a loose dependency on Azure DevOps git checkout mechanism - if this errors out
# we'd likely need to another fetch. Something like:
#
# git fetch origin pull/$PullRequestId/head:local_branch
# $changedFiles=$(git --git-dir=$gitDir --work-tree=$RepoRoot diff --name-only local_branch origin/$TargetBranch 2>&1)
$changedFiles=$(git --git-dir=$gitDir --work-tree=$RepoRoot diff --name-only pull/$PullRequestId/merge origin/$TargetBranch 2>&1)

foreach ($changedFile in $changedFiles) {
    $joinedPath = Join-Path -Path $RepoRoot -ChildPath $changedFile
    # Only save the path if the file still exists - also, do not store the absolute path
    # of the file, in case this set of information is used later in the pipeline on a different
    # machine/context.
    if (Test-Path $joinedPath -PathType leaf) {
        Add-Content -Path $OutputFile -Value $changedFile
    }
}
