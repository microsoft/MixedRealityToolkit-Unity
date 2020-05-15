<#
.SYNOPSIS
    Validates the code and assets to check for common patterns and usage that shouldn't be
    checked in.
.DESCRIPTION
    This currently checks both CSharp files and Unity serialized assets.

    - That Boo.Lang isn't used anywhere in the code. This is an autocomplete option
      that occurs when using Lists and other collections - the right thing to do
      is to use System.Collections
    - Validates that there are no empty doccomments (e.g. ///<summary></summary>)
    - All checked in profiles should not be marked custom
    - There are no references to hardcoded paths
    - There's only a single MixedRealityWorkspace per scene
    - There aren't any references to Camera.main

    Returns 0 if there are no issues, non-zero if there are.
.EXAMPLE
    .\validatecode.ps1 -Directory c:\path\to\MRTK\Assets -ChangesFile c:\path\to\changes\file.txt -RepoRoot c:\path\to\MRTK
#>
param(
    # The directory containing the code to validate. This won't be used if ChangesFile
    # is specified, but is always required because it's the fallback if
    # ChangesFile doesn't exist or isn't valid.
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
    Checks if the given file (at the given line number) contains a reference to Boo.Lang
    Returns true if such a reference exists.
#>
function CheckBooLang {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileContent[$LineNumber] -match "^using Boo\.Lang;") {
            Write-Host "An instance of Boo.Lang was found in $FileName at line $LineNumber "
            Write-Host "Use System.Collections instead."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains an empty <returns></returns>
    or an <param ...></param> doccomment. These are typically added automatically by IDEs,
    but empty doccomments don't add value to code.
#>
function CheckEmptyDoccomment {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        $tags = @("param", "returns")
        $containsEmptyDoccomment = $false

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
                $containsEmptyDoccomment = $true
            }
        }
        $containsEmptyDoccomment
    }
}

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains a comment of the type:
    //This comment doesn't have a space between // and This.
    There should be a space between // and the comment.
#>
function CheckSpacelessComments {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        $hasIssue = $false

        # This regex looks for any non doccomment (i.e. //, not ///) where there isn't
        # a space after the //.
        # Explanation of the stuff inside the regex:
        # \s      - matches a space, to ensure that we don't capture cases like https://
        # //      - matches '//'
        # [^\s//] - matches a single character that is not a whitespace character and also
        #           not the '/' character (because doccomments like /// <summary> would
        #           otherwise get matched).
        $matcher = "\s//[^\s//]"
        if ($FileContent[$LineNumber] -match $matcher) {
            Write-Host "Comment in $FileName at line $LineNumber is missing a space after '//'"
            Write-Host $FileContent[$LineNumber]
            $hasIssue = $true
        }
        
        $hasIssue
    }
}

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains a reference to Camera.main
    Returns true if such a reference exists.
#>
function CheckMainCamera {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileName -notmatch "CameraCache.cs" -and $FileContent[$LineNumber] -match "Camera\.main") {
            Write-Host "An instance of Camera.main was found in $FileName at line $LineNumber "
            Write-Host "Use CameraCache.Main instead."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Checks if the given profile contains references to Assembly-CSharp, often indicative of invalid reference.
    Returns true if such a reference exists.
#>
function CheckAssemblyCSharp {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileName -and $FileContent[$LineNumber] -match "Assembly-CSharp") {
            Write-Host "An instance of 'Assembly-CSharp' was found in $FileName at line $LineNumber"
            Write-Host "Please update this to reference the correct assembly."
            $true
        }
        $false
    }
}

<#
.SYNOPSIS
    Checks if the given profile is marked as a custom profile.
    Returns true if such a reference exists.
#>
function CheckCustomProfile {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileName -notmatch "Examples" -and $FileContent[$LineNumber] -match "isCustomProfile: 1") {
            Write-Host "An instance of 'isCustomProfile: 1' was found in $FileName at line $LineNumber"
            Write-Host "Please update this to 'isCustomProfile: 0' instead."
            $true
        }
        $false
    }
}

# This dictionary contains the list of all allowed hardcoded path exceptions
# You should NOT add things to this list unless you believe that your scenario should only work
# when the MRTK is placed in a specific location in the root assets folder. This is only valid
# in two locations currently:
# 1) Test code
# 2) Profile cloning code - this code needs to choose some reasonable default, preferring
#    "Assets/MixedRealityToolkit.Generated/CustomProfiles" as that default location
$HardcodedPathExceptions = @{
    "MixedRealityToolkitConfigurationProfileInspector.cs" = @(
        'var newProfile = profile.CreateAsset("Assets/MixedRealityToolkit.Generated/CustomProfiles") as MixedRealityToolkitConfigurationProfile;'
    );
    # This exception should be deleted once https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6448 is resolved
    "MRTKExamplesHub.unity" = @(
        'Path: Assets/MRTK/Examples/Experimental/ExamplesHub/Scenes/MRTKExamplesHubMainMenu.unity'
        'value: Assets/MRTK/Examples/Experimental/ExamplesHub/Scenes/MRTKExamplesHubMainMenu.unity'
    );
    # This exception should be deleted once https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6448 is resolved
    "MRTKExamplesHubMainMenu.unity" = @(
        'value: Assets/MRTK/Examples/Demos/UX/Tooltips/Scenes/TooltipExamples.unity'
        'value: Assets/MRTK/Examples/Demos/HandTracking/Scenes/HandMenuExamples.unity'
        'value: Assets/MRTK/Examples/Demos/HandTracking/Scenes/HandInteractionExamples.unity'
        'value: Assets/MRTK/Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-04-TargetPositioning.unity'
        'value: Assets/MRTK/Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-03-Navigation.unity'
        'value: Assets/MRTK/Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-02-TargetSelection.unity'
        'value: Assets/MRTK/Examples/Demos/UX/Slate/SlateExample.unity'
        'value: Assets/MRTK/Examples/Demos/UX/PressableButton/Scenes/PressableButtonExample.unity'
        'value: Assets/MRTK/Examples/Demos/StandardShader/Scenes/ClippingExamples.unity'
        'value: Assets/MRTK/Examples/Demos/UX/Slider/Scenes/SliderExample.unity'
        'value: Assets/MRTK/Examples/Demos/UX/BoundingBox/Scenes/BoundingBoxExamples.unity'
        'value: Assets/MRTK/Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-05-Visualizer.unity'
        'value: Assets/MRTK/Examples/Demos/HandTracking/Scenes/NearMenuExamples.unity'
        'value: Assets/MRTK/Examples/Demos/StandardShader/Scenes/MaterialGallery.unity'
    );
}

<#
.SYNOPSIS
    Checks if the line contains a hardcoded path i.e. "Assets/MRTK/Examples"
    Hardcoded paths are generally not allowed, except in certain cases such as:

    - Tests cases (tests are not run by consumers of the toolkit)
    - Default saved location of custom profiles (it will default to a folder in the Assets/ location)
#>
function CheckHardcodedPath {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        # Some files have total exceptions (i.e. ones that deal with profile cloning or for tests)
        if ($FileName -match "Assets.MixedRealityToolkit\.Tests" -or
            $FileName -match "MixedRealityProfileCloneWindow\.cs") {
            return $false
        }

        $results = Select-String -Pattern "Assets.MixedRealityToolkit" $FileName -AllMatches
        $containsIssue = $false
        $relativeFileName = Split-Path $FileName -leaf

        foreach ($match in $results) {
            $trimmed = $match.Line.trim()
            if (-Not $HardcodedPathExceptions.Contains($relativeFileName) -Or
                    ($HardcodedPathExceptions.Contains($relativeFileName) -And
                    -Not $HardcodedPathExceptions[$relativeFileName].Contains($trimmed))) {
                Write-Host "Hardcoded path detected in $FileName - $trimmed"
                Write-Host "Please delete usage of hardcoded path or add an exception in HardcodedPathExceptions"
                Write-Host "if this is a valid hardcoded path usage."
                $containsIssue = $true
            }
        }
        $containsIssue
    }
}

<#
.SYNOPSIS
    Checks if the file has a corresponding meta checked in.
    Returns true if the meta is missing.
#>
function CheckForMetaFile {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        if (-not (Test-Path ($FileName + ".meta"))) {
            Write-Warning "Meta file missing for $FileName. Please be sure to check it in alongside this file."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Checks if the file has a corresponding meta checked in.
    Returns true if the meta is missing.
#>
function CheckForActualFile {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        # Remove .meta from path
        if (-not (Test-Path $FileName.Substring(0, $FileName.LastIndexOf('.')))) {
            Write-Warning "Actual file missing for meta file $FileName. Please be sure to check it in or remove this meta."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Checks if all assembly definitions have a corresponding AssemblyInfo.cs checked in.
    Returns true if the AssemblyInfo is missing.
#>
function CheckForAssemblyInfo {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        if (-not (Test-Path (Join-Path -Path (Split-Path $FileName) -ChildPath "AssemblyInfo.cs"))) {
            Write-Warning "AssemblyInfo.cs missing for $FileName. Please be sure to check it in alongside this asmdef."
            $true;
        }
        $false;
    }
}

function CheckScript {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
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
            if (CheckMainCamera $FileName $fileContent $i) {
                $containsIssue = $true
            }
            if (CheckSpacelessComments $FileName $fileContent $i) {
                $containsIssue = $true
            }
        }

        if (CheckHardcodedPath $FileName) {
            $containsIssue = $true
        }

        if (CheckForMetaFile $FileName) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

function CheckAsset {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
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
            if (CheckAssemblyCSharp $FileName $fileContent $i) {
                $containsIssue = $true
            }
        }

        # Filter out the ProjectSettings .asset files, which don't have a meta file and don't need one.
        if ((-not $FileName.Contains("\ProjectSettings\")) -and (CheckForMetaFile $FileName)) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

function CheckUnityScene {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        $containsIssue = $false

        # Checks if there is more than one MixedRealityPlayspace objects in each example unity scene
        $MatchesPlayspaces = Select-String MixedRealityPlayspace $FileName -AllMatches
        $NumPlayspaces = $MatchesPlayspaces.Matches.Count

        if ($NumPlayspaces -gt 1){
            Write-Warning "There are multiple MixedRealityPlayspace objects in $FileName, delete the extra playspaces from the unity scene."
            $containsIssue = $true
        }

        if (CheckHardcodedPath $FileName) {
            $containsIssue = $true
        }

        if (CheckForMetaFile $FileName) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

function CheckAssemblyDefinition {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        $containsIssue = $false

        if (CheckForAssemblyInfo $FileName) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

# If the file containing the list of changes was provided and actually exists,
# this validation should scope to only those changed files.
if ($ChangesFile -and (Test-Path $ChangesFile -PathType leaf)) {
    # TODO(https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7022)
    # There may be ways to configure common modules so that paths like this aren't required
    Import-Module -Force (Resolve-Path("$RepoRoot\scripts\ci\common.psm1"))

    Write-Host "Checking only changed files for code issues: $ChangesFile"
    $changedFiles = GetChangedFiles -Filename $ChangesFile -RepoRoot $RepoRoot
    ForEach ($changedFile in $changedFiles) {
        Write-Host "Checking file: $changedFile"
        if (((IsCSharpFile -Filename $changedFile) -and (CheckScript $changedFile)) -or
            ((IsAssetFile -Filename $changedFile) -and (CheckAsset $changedFile)) -or
            ((IsUnityScene -Filename $changedFile) -and (CheckUnityScene $changedFile)) -or
            ((IsMetaFile -Filename $changedFile) -and (CheckForActualFile $changedFile)) -or 
            ((IsAsmDef -Filename $changedFile) -and (CheckAssemblyDefinition $changedFile))) {
            $containsIssue = $true;
        }
    }
}
else {
    Write-Host "Checking $Directory for common code issues"

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
    
    $metas = Get-ChildItem $Directory *.meta -File -Recurse | Select-Object FullName
    foreach ($meta in $metas) {
        if (CheckForActualFile $meta.FullName) {
            $containsIssue = $true
        }
    }

    $asmdefs = Get-ChildItem $Directory *.asmdef -File -Recurse | Select-Object FullName
    foreach ($asmdef in $asmdefs) {
        if (CheckAssemblyDefinition $asmdef.FullName) {
            $containsIssue = $true
        }
    }
}

$folders = Get-ChildItem $Directory -Directory -Recurse | Select-Object FullName
foreach ($folder in $folders) {
    if (CheckForMetaFile $folder.FullName) {
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
