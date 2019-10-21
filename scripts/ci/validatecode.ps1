<#
.SYNOPSIS
    Validates the code to check for common patterns and usage that shouldn't be
    checked in.
.DESCRIPTION
    This currently checks:

    - That Boo.Lang isn't used anywhere in the code. This is an autocomplete option
      that occurs when using Lists and other collections - the right thing to do
      is to use System.Collections

    Returns 0 if there are no issues, non-zero if there are.
.PARAMETER Directory
    The directory containing the code to validate. Only CSharp files
.EXAMPLE
    .\validatecode.ps1 -Directory c:\path\to\MRTK\Assets
#>
param(
    [Parameter(Mandatory=$true)]
    [string]$Directory
)

function CheckBooLang(
    [string]$FileName,
    [string[]]$FileContent,
    [int]$LineNumber
) {
    <#
    .SYNOPSIS
        Checks if the given file (at the given line number) contains a reference to Boo.Lang
        Returns true if such a reference exists.
    #>
    if ($FileContent[$LineNumber] -match "^using Boo\.Lang;") {
        Write-Host "An instance of Boo.Lang was found in $FileName at line $LineNumber "
        Write-Host "Use System.Collections instead."
        return $true;
    }
    return $false
}

function CheckEmptyDoccomment(
    [string]$FileName,
    [string[]]$FileContent,
    [int]$LineNumber
) {
    <#
    .SYNOPSIS
        Checks if the given file (at the given line number) contains an empty <returns></returns>
        or an <param ...></param> doccomment. These are typically added automatically by IDEs,
        but empty doccomments don't add value to code.
    #>
    $tags = @("param", "returns")
    $containsEmptyDoccomment = $false;

    foreach ($tag in $tags) {
        # This generates regexes that look like:
        # ///\s*<returns[\sa-zA-Z"=]*>\s*</returns>
        # which basically looks for an empty tag (allowing for alphanumeric param names
        # and values in the tag itself)
        $matcher = "///\s*<$tag[\sa-zA-Z0-9`"=]*>\s*</$tag>"
        if ($FileContent[$LineNumber] -match $matcher) {
            Write-Host "An empty doccomment was found in $FileName at line $LineNumber "
            Write-Host "Delete the line or add a description "
            Write-Host $FileContent[$LineNumber]
            $containsEmptyDoccomment = $true;
        }
    }
    return $containsEmptyDoccomment
}

function CheckCustomProfile(
    [string]$FileName,
    [string[]]$FileContent,
    [int]$LineNumber
) {
    <#
    .SYNOPSIS
        Checks if the given profile is marked as a custom profile.
        Returns true if such a reference exists.
    #>
    if ($FileName -notmatch "Examples" -and $FileContent[$LineNumber] -match "isCustomProfile: 1") {
        Write-Host "An instance of 'isCustomProfile: 1' was found in $FileName at line $LineNumber"
        Write-Host "Please update this to 'isCustomProfile: 0' instead."
        return $true;
    }
    return $false
}

function CheckScript(
    [string]$FileName
) {
    # Each line of each script is checked by all of the validators above - this ensures that in
    # a single pass, we'll get all of the issues highlighted all at once, rather than
    # repeatedly running this script, discovering a single issue, fixing it, and then
    # re-running the script
    $containsIssue = $false
    $fileContent = Get-Content $FileName
    for ($i = 0; $i -lt $fileContent.Length; $i++) {
        if (CheckBooLang $FileName $fileContent $i) {
            $containsIssue = $true
        }
        if (CheckEmptyDoccomment $FileName $fileContent $i) {
            $containsIssue = $true
        }
    }
    return $containsIssue
}

function CheckAsset(
    [string]$FileName
) {
    # Each line of each asset is checked by all of the validators above - this ensures that in
    # a single pass, we'll get all of the issues highlighted all at once, rather than
    # repeatedly running this script, discovering a single issue, fixing it, and then
    # re-running the script
    $containsIssue = $false
    $fileContent = Get-Content $FileName
    for ($i = 0; $i -lt $fileContent.Length; $i++) {
        if (CheckCustomProfile $FileName $fileContent $i) {
            $containsIssue = $true
        }
    }
    return $containsIssue
}

function CheckUnityScene(
    [string]$FileName
) {
    # Checks if there is more than one MixedRealityPlayspace objects in each example unity scene
    $containsIssue = $false

    $MatchesPlayspaces = Select-String MixedRealityPlayspace $FileName -AllMatches
    $NumPlayspaces = $MatchesPlayspaces.Matches.Count

    if ($NumPlayspaces -gt 1){
        Write-Host "There are multiple MixedRealityPlayspace objects in $FileName, delete the extra playspaces from the unity scene."
        $containsIssue = $true
    }
    return $containsIssue
}

Write-Output "Checking $Directory for common code issues"

$codeFiles = Get-ChildItem $Directory *.cs -Recurse | Select-Object FullName
$containsIssue = $false
foreach ($codeFile in $codeFiles) {
    if (CheckScript $codeFile.FullName) {
        $containsIssue = $true
    }
}

$codeFiles = Get-ChildItem $Directory *.asset -Recurse | Select-Object FullName
foreach ($codeFile in $codeFiles) {
    if (CheckAsset $codeFile.FullName) {
        $containsIssue = $true
    }
}

# Check all Unity scenes for extra MixedRealityPlayspace objects 
$codeFiles = Get-ChildItem $Directory *.unity -Recurse | Select-Object FullName
foreach ($codeFile in $codeFiles) {
    if (CheckUnityScene $codeFile.FullName) {
        $containsIssue = $true
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
