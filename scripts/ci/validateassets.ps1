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
        "MRTK/StandardAssets"
    );
    "Foundation" =      @(
        "MRTK/Core",
        "MRTK/Providers",
        "MRTK/SDK",
        "MRTK/Services"
    );
    "Extensions" =      @(
        "MRTK/Extensions"
    );       
    "Tools" =           @(
        "MRTK/Tools"
    );     
    "TestUtilities" =   @(
        "MRTK/Tests/TestUtilities"
    );     
    "Examples" =      @(
        "MRTK/Examples"
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

<#
.SYNOPSIS
    Given the type of an object, return wether or not it is an array.
.DESCRIPTION
    Powershell functions, such as Get-ChildItem can return either an array
    of items or a single item (not an array of size 1). In order to properly
    enumerate the results, use this function to determine if the object type is
    an array.
#>
function IsArray { 
    [CmdletBinding()]
    param(
        $objectType
    )
    process {
        $objectType -eq [Object[]]
    }
}

<#
.SYNOPSIS
    Reads a single GUID contained within a file.
.DESCRIPTION
    This function returns the first GUID, when formatted as "guid: <32-digits>", found in the file.
#>
function ReadSingleGuid { 
    [CmdletBinding()]
    param(
        [System.IO.FileInfo]$file
    )
    process {
        [string]$tag = "guid: "
        [int] $tagLength = $tag.Length
        [int] $guidStringLength = 32
        
        $guid = ""
        $fileContents = [System.IO.File]::ReadAllLines($file.FullName)
        foreach ($line in $fileContents.GetEnumerator()) {          
            [int]$idx = $line.IndexOf($tag)
            $guid = ""
            if ($idx -ge 0) {
                $guid = $line.Substring($idx + $tagLength, $guidStringLength)
                break
            }
        }
        $guid
    }
}

<#
.SYNOPSIS
    Reads all GUIDs contained within a file.
.DESCRIPTION
    This method returns all of the GUIDs, when formatted as "guid: <32-digits>", contained within the file.
#>
function ReadGuids { 
    [CmdletBinding()]
    param(
        [System.IO.FileInfo]$file
    )
    process {
        [string]$tag = "guid: "
        [int] $tagLength = $tag.Length
        [int] $guidStringLength = 32
        
        $guids = New-Object System.Collections.ArrayList
        $fileContents = [System.IO.File]::ReadAllLines($file.FullName)
        foreach ($line in $fileContents.GetEnumerator()) {
           
            [int]$idx = $line.IndexOf($tag)
            if ($idx -ge 0) {
                $guid = $line.Substring($idx + $tagLength, $guidStringLength)
                $guids.Add($guid)
            }

        }
        $guids
    }
}

# Gather the GUID for every source and asset file (from the associated .meta files). We will use this 
# data to validate the dependencies.
# todo
foreach ($package in $packages.GetEnumerator()) {
    $packageName = $package.name
    Write-Output $packageName

    foreach ($folder in $package.value.GetEnumerator()) {
        $packageFolder = Join-Path $Directory $folder
        
        $assetFiles = Get-ChildItem -Path $packageFolder -Filter "*.meta" -File -Recurse
        if (-not $assetFiles) {
            continue
        }

        if (IsArray($assetFiles.GetType())){
            Write-Output "Array"

            foreach ($asset in $assetFiles.GetEnumerator()) {
                $assetName = $asset.Name
                $guid = ReadSingleGuid($asset)
                Write-Output "$assetName : $guid"
            }
        }
        else {
            $guid = ReadSingleGuid($assetFiles)
            $assetName = $asset.Name
            Write-Output "$assetName : $guid"
        }     
    }
}

# Open each of the target assets and check each GUID to ensure the dependencies are in an approved package.
# todo: the following walks the packages and enumerates each of the target asset types
foreach ($package in $packages.GetEnumerator()) {
    $packageName = $package.name
    Write-Output $packageName

    foreach ($folder in $package.value.GetEnumerator()) {
        $packageFolder = Join-Path $Directory $folder

        foreach ($ext in $assetExtensions.GetEnumerator()) {
            Write-Output $ext

            $assetFiles = Get-ChildItem -Path $packageFolder -Filter "*.$ext" -File -Recurse
            if (-not $assetFiles) {
                continue
            }

            if (IsArray($assetFiles.GetType())){
                #$assetName = $asset.Name
                $guids = ReadGuids($asset)
                #Write-Output $guids
            }
            else {
                #$assetName = $assetFiles.Name
                $guids = ReadGuids($assetFiles)
                #Write-Output $guids
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