<#
.SYNOPSIS
    Validates the asset files to ensure references flow along the allowed
    dependency chain (ex: Examples -> Foundation -> Standard Assets).
.DESCRIPTION
    This currently checks a subset of Unity serialized assets:

    - Scenes (.unity)
    - Prefabs (.prefab)
    - Scriptable Assets (.asset)
    - Materials (.mat)
    - Animations (.anim)
    - Controllers (.controller) 
    - Playables (.playable)

    Returns 0 if there are no issues, non-zero if there are.
.EXAMPLE
    .\validateassets.ps1 -Directory c:\path\to\MRTK\Assets -ChangesFile c:\path\to\changes\file.txt -RepoRoot c:\path\to\MRTK
#>
param(
    # The directory containing the assets to validate. This won't be used if ChangesFile
    # is specified, but is always required because it's the fallback if
    # ChangesFile doesn't exist or isn't valid.
    [Parameter(Mandatory=$true)]
    [string]$Directory,

    # The filename containing the list of files to scope the asset validation
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

$Directory = Resolve-Path -Path $Directory

# This table defines MRTK pacakge layout.
#
# The keys represent the package name (com.microsoft.mixedreality.toolkit.<key>). The values are the collection
# of folders included in the package.
#
# The folders are relevant to the RepoRoot.
$packages = [ordered]@{
    "StandardAssets" =  @(
        "Assets/MRTK/StandardAssets"
    );
    "Foundation" =      @(
        "Assets/MRTK/Core",
        "Assets/MRTK/Providers",
        "Assets/MRTK/SDK",
        "Assets/MRTK/Services"
    );
    "Extensions" =      @(
        "Assets/MRTK/Extensions"
    );       
    "Tools" =           @(
        "Assets/MRTK/Tools"
    );     
    "TestUtilities" =   @(
        "Assets/MRTK/Tests/TestUtilities"
    );     
    "Examples" =      @(
        "Assets/MRTK/Examples"
    );     
}

# This table contains the collection of allowed package dependencies.
$packageDependencies = [ordered]@{
    "StandardAssets" =  @(
        "StandardAssets"
    );
    "Foundation" =      @(
        "StandardAssets",
        "Foundation"
    );
    "Extensions" =      @(
        "StandardAssets",
        "Foundation",
        "Extensions"
    );       
    "Tools" =           @(
        "StandardAssets",
        "Foundation",
        "Tools"
    );    
    "TestUtilities" =   @(
        "StandardAssets",
        "Foundation",
        "TestUtilities"
    );      
    "Examples" =      @(
        "StandardAssets",
        "Foundation",
        "Extensions",
        "Examples"
    );      
}

# This list contains the extensions of the asset files that will be validated.
$assetExtensions = @(
    "unity",
    "prefab",
    "asset",
    "mat",
    "anim",
    "controller",
    "playable"    
)

# Gather the GUID for every source and asset file (from the associated .meta files). We will use this 
# data to validate the dependencies.
# todo
foreach ($package in $packages.GetEnumerator()) {
    $packageName = $package.name
    Write-Output $packageName

    foreach ($folder in $package.value.GetEnumerator()) {
        Write-Output $folder
        
        $assetFiles = Get-ChildItem -Path $folder -Filter "*.meta" -File -Recurse
        if (-not $assetFiles) {
            continue
        }

        $objectType = $assetFiles.GetType()
        if ($objectType -eq [System.IO.FileInfo]) {
            $assetName = $assetFiles.FullName
            Write-Output $assetName

            # todo
        }
        elseif ($objectType -eq [Object[]]) {
            foreach ($asset in $assetFiles.GetEnumerator()) {
                $assetName = $asset.FullName
                Write-Output $assetName

                # todo
            }
        }        
    }
}

# Open each of the target assets and check each GUID to  ensure the dependencies are in an approved package.
# todo: the following walks the packages and enumerates each of the target asset types
foreach ($package in $packages.GetEnumerator()) {
    $packageName = $package.name
    Write-Output $packageName

    foreach ($folder in $package.value.GetEnumerator()) {
        Write-Output $folder

        foreach ($ext in $assetExtensions.GetEnumerator()) {
            Write-Output $ext

            $assetFiles = Get-ChildItem -Path $folder -Filter "*.$ext" -File -Recurse
            if (-not $assetFiles) {
                continue
            }

            $objectType = $assetFiles.GetType()
            if ($objectType -eq [System.IO.FileInfo]) {
                $assetName = $assetFiles.FullName
                Write-Output $assetName

                # todo
            }
            elseif ($objectType -eq [Object[]]) {
                foreach ($asset in $assetFiles.GetEnumerator()) {
                    $assetName = $asset.FullName
                    Write-Output $assetName

                    # todo
                }
            }
        }
    }
}

[int]$errorCount = 0

# If the file containing the list of changes was provided and actually exists,
# this validation should scope to only those changed files.
if ($ChangesFile -and (Test-Path $ChangesFile -PathType leaf)) {
    # TODO(https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7022)
    # There may be ways to configure common modules so that paths like this aren't required
    Import-Module -Force (Resolve-Path("$RepoRoot\scripts\ci\common.psm1"))

    Write-Host "Checking only changed files for asset issues: $ChangesFile"
    # todo
    # $changedFiles = GetChangedFiles -Filename $ChangesFile -RepoRoot $RepoRoot
    # ForEach ($changedFile in $changedFiles) {
    #     Write-Host "Checking file: $changedFile"
    #     if (((IsCSharpFile -Filename $changedFile) -and (CheckScript $changedFile)) -or
    #         ((IsAssetFile -Filename $changedFile) -and (CheckAsset $changedFile)) -or
    #         ((IsUnityScene -Filename $changedFile) -and (CheckUnityScene $changedFile)) -or
    #         ((IsMetaFile -Filename $changedFile) -and (CheckForActualFile $changedFile)) -or
    #         ((IsAsmDef -Filename $changedFile) -and (CheckAsmDef $changedFile))) {
    #         $containsIssue = $true;
    #     }
    #}
}
else {
    Write-Host "Checking $Directory for common asset issues"
    # todo
}

Write-Output "Found $errorCount asset dependency issues."
exit $errorCount