<#
.SYNOPSIS
    Validates the asset files to ensure references flow along the allowed
    dependency chain (ex: Examples -> Foundation -> Standard Assets).
.DESCRIPTION
    This script checks a subset of Unity serialized assets, including

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
    #[Parameter(Mandatory=$true)]
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

$Directory = "C:\git-os\dk\mrtk\depValidationCi\Assets"

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
    "*.unity",
    "*.prefab",
    "*.asset",
    "*.mat",
    "*.anim",
    "*.controller",
    "*.playable"    
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
    Given a string, determine if it is a valud GUID
.DESCRIPTION
    Attempts to parse the provided string into a Guid. Returns true if successful, otherwise false.
    Note: This method defines an empty GUID (all 0) as being invalid.
#>
function IsValidGuid {
    [CmdletBinding()]
    param(
        [string]$guid
    )
    process {
        [System.Guid]$temp = [System.Guid]::Empty;
        [bool] $isValid = [System.Guid]::TryParse($guid, [ref]$temp)
        if ($isValid) {
            $isValid = ($temp -ne [System.Guid]::Empty)
        }

        $isValid
    }
}

<#
.SYNOPSIS
    Extracts a GUID, if present, from a string (expected format: "guid: <32-digits>").
.DESCRIPTION
    Returns the GUID identified or an empty string.
#>function ExtractGuid {
    [CmdletBinding()]
    param(
        [string]$text
    )
    process {
        [string]$tag = "guid: "
        [int] $tagLength = $tag.Length
        [int] $guidStringLength = 32

        $guid = ""
        
        if ($text.Contains($tag)) {
            [int]$idx = $line.IndexOf($tag)
            $guid = $text.Substring($idx + $tagLength, $guidStringLength)
            $isValid = IsValidGuid($guid)
            if (-not $isValid) {
                $guid = ""
            }
        }

        $guid        
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
        $guid = ""
        $fileContents = [System.IO.File]::ReadAllLines($file.FullName)
        foreach ($line in $fileContents.GetEnumerator()) {          
            $guid = ExtractGuid($line)
            if ($guid -ne "") {
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
        $guids = New-Object -TypeName System.Collections.ArrayList
        $fileContents = [System.IO.File]::ReadAllLines($file.FullName)
        foreach ($line in $fileContents.GetEnumerator()) {
            $guid = ExtractGuid($line)
            if ($guid -ne "") {
                if (-not $guids.Contains($guid)) {
                    $guids.Add($guid)
                }
            }      
        }

        $guids
    }
}

$allFiles = @{}

<#
.SYNOPSIS
    Obtain all MRTK file GUIDs.
.DESCRIPTION
    Gather the GUID for every source and asset file (from the associated .meta files). We will use this 
    data to validate the dependencies.
#>
function GatherGuids { 
    [CmdletBinding()]
    param()
    process {
        foreach ($package in $packages.GetEnumerator()) {
            $packageName = $package.name
            Write-Output "Collecting files and GUIDs from $packageName"

            foreach ($folder in $package.value.GetEnumerator()) {
                $packageFolder = Join-Path $Directory $folder
                
                $assetFiles = Get-ChildItem -Path $packageFolder -Filter "*.meta" -File -Recurse
                if (-not $assetFiles) {
                    continue
                }
                
                if (IsArray($assetFiles.GetType())){
                    foreach ($asset in $assetFiles.GetEnumerator()) {
                        $guid = ReadSingleGuid($asset)
                        $allFiles.Add($asset.FullName, $guid)
                    }
                }
                else {
                    $guid = ReadSingleGuid($assetFiles)
                    $allFiles.Add($assetFiles.FullName, $guid)
                }     
            }
        }
    }
}

# $assetDependencies = ${}

<#
.SYNOPSIS
    Obtain all MRTK asset depdendency GUIDs.
.DESCRIPTION
    Reads the target assets and collects the dependency GUIDs.
#>
function GatherDependencyGuids { 
    [CmdletBinding()]
    param()
    process {       
        foreach ($package in $packages.GetEnumerator()) {
            $packageName = $package.name
            Write-Output $packageName

            foreach ($folder in $package.value.GetEnumerator()) {
                $packageFolder = Join-Path $Directory $folder

                foreach ($ext in $assetExtensions.GetEnumerator()) {
                    Write-Output $ext

                    $assetFiles = Get-ChildItem -Path $packageFolder -Filter $ext -File -Recurse
                    if (-not $assetFiles) {
                        continue
                    }

                    if (IsArray($assetFiles.GetType())){        
                        foreach ($asset in $assetFiles.GetEnumerator()) {
                            $assetName = $asset.FullName
                            Write-Output $assetName
                            $guids = ReadGuids($asset)
                            # $assetDependencies.Add($asset, $guids)
                            # $dependencies.Add($asset, $guids)
                        }
                    }
                    else {
                        $guids = ReadGuids($assetFiles)
                        # $assetDependencies.Add($assetFiles, $guids)
                        # $dependencies.Add($assetFiles, $guids)
                    }
                }
            }
        }

        # $dependencies
    }
}

# GatherGuids
# foreach ($entry in $allFiles.GetEnumerator()){
#     $fileName = $entry.key
#     $guid = $entry.value
#     Write-Output "$fileName : $guid"
# }
# Write-Output "---"
$testGuids = ReadGuids("C:\git-os\dk\mrtk\depValidationCi\Assets\MRTK\Examples\Demos\HandTracking\Scenes\HandInteractionExamples.unity")
foreach ($item in $testGuids) {
    $isValid = IsValidGuid($item)
    Write-Output "$item : $isValid"
}
# GatherDependencyGuids
# Write-Output "---"

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