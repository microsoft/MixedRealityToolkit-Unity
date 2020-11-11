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
    # todo - uncomment
    # [Parameter(Mandatory=$true)]
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

# todo - remove
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
$allowePackageDependencies = [ordered]@{
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
                    $guids.Add($guid) | Out-Null
                }
            }      
        }

        $guids
    }
}

$fileGuids = @{}

<#
.SYNOPSIS
    Obtain all MRTK file GUIDs.
.DESCRIPTION
    Gather the GUID for every source and asset file (from the associated .meta files). We will use this 
    data to validate the dependencies.
#>
function GatherFileGuids { 
    [CmdletBinding()]
    param()
    process {
        foreach ($package in $packages.GetEnumerator()) {
            $packageName = $package.name

            Write-Output "Collecting file GUIDs from $packageName"

            foreach ($folder in $package.value.GetEnumerator()) {
                $packageFolder = Join-Path $Directory $folder
                
                $assetFiles = Get-ChildItem -Path $packageFolder -Filter "*.meta" -File -Recurse
                if (-not $assetFiles) {
                    continue
                }
                
                if (IsArray($assetFiles.GetType())){
                    foreach ($asset in $assetFiles.GetEnumerator()) {
                        $type = $asset.GetType()
                        $guid = ReadSingleGuid($asset)
                        $fileGuids.Add($guid, $asset)
                    }
                }
                else {
                    $guid = ReadSingleGuid($assetFiles)
                    $fileGuids.Add($guid, $assetFiles)
                }     
            }
        }
    }
}

$dependencyGuids = @{}

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
            Write-Output "Collecting dependency GUIDs from $packageName"
            
            foreach ($folder in $package.value.GetEnumerator()) {
                $packageFolder = Join-Path $Directory $folder

                foreach ($ext in $assetExtensions.GetEnumerator()) {
                    $assetFiles = Get-ChildItem -Path $packageFolder -Filter $ext -File -Recurse
                    if (-not $assetFiles) {
                        continue
                    }

                    if (IsArray($assetFiles.GetType())){        
                        foreach ($asset in $assetFiles.GetEnumerator()) {
                            $guids = ReadGuids($asset)
                            $dependencyGuids.Add($asset, $guids)
                        }
                    }
                    else {
                        $guids = ReadGuids($assetFiles)
                        $dependencyGuids.Add($assetFiles, $guids)
                    }
                }
            }
        }
    }
}

<#
.SYNOPSIS
    Determines the name of the package containing the specified file.
.DESCRIPTION
    Compares the name of the file with the substrings included in each package. Returns the package name or the empty string.
#>
function GetPackageName { 
    [CmdletBinding()]
    param(
        [System.IO.FileInfo]$file
    )
    process {
        [string]$packageName = ""

        foreach($item in $packages.GetEnumerator()) {
            $name = $item.key
            $folders = $item.value

            $fileName = $file.FullName
            $filename = $fileName.Replace('\', '/')

            foreach($folder in $folders.GetEnumerator()) {
                [int]$index = $fileName.IndexOf($folder)
                if ($index -ge 0) {
                    $packageName = $name
                    break
                }
            }
        }

        $packageName
    }
}

[int]$errorCount = 0
<#
.SYNOPSIS
    Validates the dependencies for the provided asset.
.DESCRIPTION
    Checks the paths of each dependency file with against the allow list. Returns true if valid, false otherwise.
#>
function ValidateDependencies { 
    [CmdletBinding()]
    param(
        [System.IO.FileInfo]$file,
        [System.Array]$dependencies
    )
    process {
        [bool]$isValid = $false;

        if ($null -ne $dependencies) {
            foreach ($guid in $dependencies.GetEnumerator()) {
                $dependencyFile = $fileGuids[$guid]
                if ($null -ne $dependencyFile) {
                    $dependencyFile = $dependencyFile.Replace('\', '/')
                    # todo - check against the allow list
                    # increment error count
                }
            }
        }

        $isValid
    }
}

$errors = New-Object -TypeName System.Collections.ArrayList

GatherFileGuids
GatherDependencyGuids
foreach ($item in $dependencyGuids.GetEnumerator()) {
    $file = $item.key
    $dependencies = $item.value
    $packageName = GetPackageName($file)
    $fileName = $file.FullName
    Write-Output "[$packageName] $fileName"
    # Write-Output "Validating $file dependencies..."
    # $isValid = ValidateDependencies($file, $dependencies)
    # if (-not $isValid) {
    #     $errors.Add($file) | Out-Null
    # }
}

Write-Output "Found $errorCount asset dependency issues."

if ($errorCount -ne 0) {
    [int]$fileCount = $errors.Count 
    Write-Output "$fileCount files with one or more incorrect dependencies:"
    foreach ($item in $errors.GetEnumerator()) {
        # todo
    }    
}

# # If the file containing the list of changes was provided and actually exists,
# # this validation should scope to only those changed files.
# if ($ChangesFile -and (Test-Path $ChangesFile -PathType leaf)) {
#     # TODO(https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7022)
#     # There may be ways to configure common modules so that paths like this aren't required
#     Import-Module -Force (Resolve-Path("$RepoRoot\scripts\ci\common.psm1"))

#     Write-Host "Checking only changed files for asset issues: $ChangesFile"
#     # todo
#     # $changedFiles = GetChangedFiles -Filename $ChangesFile -RepoRoot $RepoRoot
#     # ForEach ($changedFile in $changedFiles) {
#     #     Write-Host "Checking file: $changedFile"
#     #     if (((IsCSharpFile -Filename $changedFile) -and (CheckScript $changedFile)) -or
#     #         ((IsAssetFile -Filename $changedFile) -and (CheckAsset $changedFile)) -or
#     #         ((IsUnityScene -Filename $changedFile) -and (CheckUnityScene $changedFile)) -or
#     #         ((IsMetaFile -Filename $changedFile) -and (CheckForActualFile $changedFile)) -or
#     #         ((IsAsmDef -Filename $changedFile) -and (CheckAsmDef $changedFile))) {
#     #         $containsIssue = $true;
#     #     }
#     #}
# }
# else {
#     Write-Host "Checking $Directory for common asset issues"
#     # todo
# }

exit $errorCount