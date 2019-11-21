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
.PARAMETER Directory
    The directory containing the code to validate. 
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
    return $containsEmptyDoccomment
}

function CheckMainCamera(
    [string]$FileName,
    [string[]]$FileContent,
    [int]$LineNumber
) {
    <#
    .SYNOPSIS
        Checks if the given file (at the given line number) contains a reference to Camera.main
        Returns true if such a reference exists.
    #>
    if ($FileName -notmatch "CameraCache.cs" -and $FileContent[$LineNumber] -match "Camera\.main") {
        Write-Host "An instance of Camera.main was found in $FileName at line $LineNumber "
        Write-Host "Use CameraCache.Main instead."
        return $true;
    }

    return $false;
}

function CheckAssemblyCSharp(
    [string]$FileName,
    [string[]]$FileContent,
    [int]$LineNumber
) {
    <#
    .SYNOPSIS
        Checks if the given profile contains references to Assembly-CSharp, often indicative of invalid reference.
        Returns true if such a reference exists.
    #>
    if ($FileName -and $FileContent[$LineNumber] -match "Assembly-CSharp") {
        Write-Host "An instance of 'Assembly-CSharp' was found in $FileName at line $LineNumber"
        Write-Host "Please update this to reference the correct assembly."
        return $true
    }
    return $false
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
        return $true
    }
    return $false
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
        'Path: Assets/MixedRealityToolkit.Examples/Experimental/ExamplesHub/Scenes/MRTKExamplesHubMainMenu.unity'
        'value: Assets/MixedRealityToolkit.Examples/Experimental/ExamplesHub/Scenes/MRTKExamplesHubMainMenu.unity'
    );
    # This exception should be deleted once https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6448 is resolved
    "MRTKExamplesHubMainMenu.unity" = @(
        'value: Assets/MixedRealityToolkit.Examples/Demos/UX/Tooltips/Scenes/TooltipExamples.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/HandTracking/Scenes/HandMenuExamples.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/HandTracking/Scenes/HandInteractionExamples.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-04-TargetPositioning.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-03-Navigation.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-02-TargetSelection.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/UX/Slate/SlateExample.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/UX/PressableButton/Scenes/PressableButtonExample.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Scenes/ClippingExamples.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/UX/Slider/Scenes/SliderExample.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/UX/BoundingBox/Scenes/BoundingBoxExamples.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/EyeTracking/Scenes/EyeTrackingDemo-05-Visualizer.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/HandTracking/Scenes/NearMenuExamples.unity'
        'value: Assets/MixedRealityToolkit.Examples/Demos/StandardShader/Scenes/MaterialGallery.unity'
    );
}

function CheckHardcodedPath(
    [string]$FileName
) {
    <#
    .SYNOPSIS
        Checks if the line contains a hardcoded path i.e. "Assets/MixedRealityToolkit.Examples"
        Hardcoded paths are generally not allowed, except in certain cases such as:

        - Tests cases (tests are not run by consumers of the toolkit)
        - Default saved location of custom profiles (it will default to a folder in the Assets/ location)
    #>

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
                     Write-Host $trimmed
            Write-Host "Hardcoded path detected in $FileName - $trimmed"
            Write-Host "Please delete usage of hardcoded path or add an exception in HardcodedPathExceptions"
            Write-Host "if this is a valid hardcoded path usage."
            $containsIssue = $true
        }
    }
    return $containsIssue
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
        if (CheckMainCamera $FileName $fileContent $i) {
            $containsIssue = $true
        }
    }

    if (CheckHardcodedPath $FileName) {
        $containsIssue = $true
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
        if (CheckAssemblyCSharp $FileName $fileContent $i) {
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

    if (CheckHardcodedPath $FileName) {
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
