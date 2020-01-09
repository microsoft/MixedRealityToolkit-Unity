<#
.SYNOPSIS
    Validates the docs to check for common patterns and usage that shouldn't be
    checked in.
.DESCRIPTION
    This currently checks:

    - That documentation doesn't contain fully resolved links (i.e. https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation)
      and are instead relative links. Relative links can be resolved and validated by docfx, and fully resolved links cannot.

    Returns 0 if there are no issues, non-zero if there are.
.EXAMPLE
    .\validatedocs.ps1 -Directory c:\path\to\MRTK\Documentation
#>
param(
    # The directory containing the docs to validate. This is the fallback if
    # ChangesFile doesn't exist or isn't valid, and as a result this is
    # always required.
    [Parameter(Mandatory=$true)]
    [string]$Directory,

    # The filename containing the list of files to scope the code validation
    # to. This is useful in pull request validation when there isn't a need
    # to check every single file in the repo for changes (i.e. only the list
    # of changed files)
    #
    # If ChangesFile doesn't exist (i.e. not specified, null, is specified but
    # the actual file doesn't exist), then this defaults to checking for everything
    # in the repo.
    [string]$ChangesFile,

    # The directory containing the repo root. Used in conjunction with ChangesFile
    [string]$RepoRoot
)

<#
.SYNOPSIS
    Checks if the given file at the given line contains a fully resolved documentation
    link. Returns true if found.
#>
function CheckDocLinks {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileContent[$LineNumber] -match "https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation") {
            Write-Host "An non-relative doc link was found in $FileName at line $LineNumber "
            Write-Host "Avoid doc links containing https://microsoft.github.io/MixedRealityToolkit-Unity/Documentation "
            Write-Host "and use relative links instead."
            $true;
        }
        $false
    }
}

function CheckDocument {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        # Each line of each script is checked by all of the validators above - this ensures that in
        # a single pass, we'll get all of the issues highlighted all at once, rather than
        # repeatedly running this script, discovering a single issue, fixing it, and then
        # re-running the script
        $issueFound = $false
        $fileContent = Get-Content $FileName
        for ($i = 0; $i -lt $fileContent.Length; $i++) {
            if (CheckDocLinks $FileName $fileContent $i) {
                $issueFound = $true
            }
        }
        $issueFound
    }
}

$containsIssue = $false

# If the file containing the list of changes was provided and actually exists,
# this validation should scope to only those changed files.
if (($ChangesFile) -and (Test-Path $Output -PathType leaf)) {
    # TODO(https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7022)
    # There may be ways to configure common modules so that paths like this aren't required
    Import-Module (Resolve-Path("$RepoRoot\scripts\ci\common.psm1"))

    Write-Host "Checking only changed files for doc issues:"
    $changedFiles = GetChangedFiles -Filename $ChangesFile -RepoRoot $RepoRoot

    foreach ($changedFile in $changedFiles) {
        if ((IsMarkdownFile -Filename $changedFile) -and (CheckDocument $changedFile)) {
            $containsIssue = $true;
        }
    }
}
else {
    Write-Host "Checking $Directory for common doc issues"

    $docFiles = Get-ChildItem $Directory *.md -Recurse | Select-Object FullName
    foreach ($docFile in $docFiles) {
        if (CheckDocument $docFile.FullName) {
            $containsIssue = $true
        }
    }
}

if ($containsIssue) {
    Write-Output "Issues found, please see above for details"
    exit 1;
}
else {
    Write-Output "No issues found"
    exit 0;
}
