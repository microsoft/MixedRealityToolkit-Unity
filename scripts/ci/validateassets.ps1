# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

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
    [Parameter(Mandatory = $true)]
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

# This table defines MRTK package layout.
#
# The keys represent the package name (com.microsoft.mixedreality.toolkit.<key>). The values are the collection
# of folders included in the package.
#
# The folders are relative to the RepoRoot.
$packages = [ordered]@{
    "StandardAssets" = @(
        "MRTK/StandardAssets"
    );
    "Foundation"     = @(
        "MRTK/Core",
        "MRTK/Providers",
        "MRTK/SDK",
        "MRTK/Services"
    );
    "Extensions"     = @(
        "MRTK/Extensions"
    );
    "Tools"          = @(
        "MRTK/Tools"
    );
    "TestUtilities"  = @(
        "MRTK/Tests/TestUtilities"
    );
    "Examples"       = @(
        "MRTK/Examples"
    );
}

# This table contains the collection of allowed package dependencies.
$allowedPackageDependencies = [ordered]@{
    "StandardAssets" = @(
        "StandardAssets"
    );
    "Foundation"     = @(
        "StandardAssets",
        "Foundation"
    );
    "Extensions"     = @(
        "StandardAssets",
        "Foundation",
        "Extensions"
    );
    "Tools"          = @(
        "StandardAssets",
        "Foundation",
        "Tools"
    );
    "TestUtilities"  = @(
        "StandardAssets",
        "Foundation",
        "TestUtilities"
    );
    "Examples"       = @(
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
    Creates a System.IO.FileInfo for the file the associated with the specified .meta file.
.DESCRIPTION
    Creates a FileInfo object.
    Ex: For "ActionScript.cs.meta" return a FileInfo object for "ActionScript.cs".
#>
function GetFileFromMeta {
    [CmdletBinding()]
    param(
        $metaFile
    )
    process {
        $assetPath = $asset.FullName
        [int]$idx = $assetPath.IndexOf(".meta")
        $assetPath = $assetPath.Substring(0, $idx)

        $file = New-Object -TypeName System.IO.FileInfo -ArgumentList $assetPath

        $file
    }
}

<#
.SYNOPSIS
    Given a string, determine if it is a valid GUID
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
#>
function ExtractGuid {
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
        $file
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
        $file
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
        $guidTable = @{}

        foreach ($package in $packages.GetEnumerator()) {
            $packageName = $package.name

            Write-Host "Collecting file GUIDs from $packageName"

            foreach ($folder in $package.value.GetEnumerator()) {
                $packageFolder = Join-Path $Directory $folder

                $assetFiles = Get-ChildItem -Path $packageFolder -Filter "*.meta" -File -Recurse
                if (-not $assetFiles) {
                    continue
                }

                if (IsArray($assetFiles.GetType())) {
                    foreach ($asset in $assetFiles.GetEnumerator()) {
                        $guid = ReadSingleGuid($asset)
                        $file = GetFileFromMeta($asset)
                        $guidTable.Add($guid, $file)
                    }
                }
                else {
                    $guid = ReadSingleGuid($assetFiles)
                    $file = GetFileFromMeta($asseFiles)
                    $guidTable.Add($guid, $file)
                }
            }
        }

        $guidTable
    }
}

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
        $guidTable = @{}

        foreach ($package in $packages.GetEnumerator()) {
            $packageName = $package.name
            Write-Host "Collecting dependency GUIDs from $packageName"

            foreach ($folder in $package.value.GetEnumerator()) {
                $packageFolder = Join-Path $Directory $folder

                foreach ($ext in $assetExtensions.GetEnumerator()) {
                    $assetFiles = Get-ChildItem -Path $packageFolder -Filter $ext -File -Recurse
                    if (-not $assetFiles) {
                        continue
                    }

                    if (IsArray($assetFiles.GetType())) {
                        foreach ($asset in $assetFiles.GetEnumerator()) {
                            $guids = ReadGuids($asset)
                            $guidTable.Add($asset, $guids)
                        }
                    }
                    else {
                        $guids = ReadGuids($assetFiles)
                        $guidTable.Add($assetFiles, $guids)
                    }
                }
            }
        }

        $guidTable
    }
}

<#
.SYNOPSIS
    Determines the name of the package containing the specified file.
.DESCRIPTION
    Compares the name of the file with the folders included in each package. Returns the package name or the empty string.
#>
function GetPackageName {
    [CmdletBinding()]
    param(
        $file
    )
    process {
        [string]$packageName = ""

        foreach ($item in $packages.GetEnumerator()) {
            $name = $item.key
            $folders = $item.value

            $fileName = $file.FullName
            $filename = $fileName.Replace('\', '/')

            foreach ($folder in $folders.GetEnumerator()) {
                [int]$index = $fileName.IndexOf($folder)
                if ($index -ge 0) {
                    $packageName = $name
                    break
                }
            }
        }

        Write-Host "$packageName : $fileName"

        $packageName
    }
}

# ####################
# Start of main script
# ####################
$fileGuids = GatherFileGuids

[string]$newEventSystemGuid = "76c392e42b5098c458856cdf6ecaaaa1"

[int]$errorCount = 0;

# todo - https://github.com/microsoft/MixedRealityToolkit-Unity/issues/8944 - support scoped validation here (GatherScopedDependencyGuids)
$dependencyGuids = GatherDependencyGuids
foreach ($item in $dependencyGuids.GetEnumerator()) {
    $file = $item.key
    $dependencies = $item.value

    Write-Host "Validating $file dependencies..."
    [int]$numInvalid = 0

    $filePackageName = GetPackageName($file)
    Write-Host "$file is in $filePackageName"
    if ($null -eq $filePackageName) {
        Write-Host "SCRIPT BUG: Could not determine the package containing $file - is the script missing a package?"
        $errorCount++
        continue
    }

    if ($null -ne $dependencies) {
        foreach ($guid in $dependencies.GetEnumerator()) {
            $dependencyFile = $fileGuids[$guid]
            if ($null -eq $dependencyFile) {
                if ($guid -eq $newEventSystemGuid) {
                    Write-Host "`nERROR: A package-based event system is included in $file. For Unity 2018 compatibility, please replace this with the built-in event system."
                    $numInvalid++
                }
                continue;
            }
            Write-Host " @ $dependencyFile"
            if ($null -ne $dependencyFile) {
                [string]$dependentPackageName = GetPackageName($dependencyFile)

                [bool]$isValid = $false

                $allowed = $allowedPackageDependencies[$filePackageName]
                if ($null -ne $allowed) {
                    if ($allowed.Contains($dependentPackageName)) {
                        $isValid = $true
                    }
                    else {
                        Write-Host "`nERROR: $file is NOT allowed to depend upon files in the $dependentPackageName package."
                    }
                }

                if (-not $isValid) {
                    $numInvalid++
                }
            }
        }
    }

    $errorCount = $errorCount + $numInvalid

    if ($numInvalid -ne 0) {
        $fileName = $file.FullName
        Write-Host "ERROR: $fileName contains $numInvalid dependency errors.`n"
    }
}

Write-Host "`nFound $errorCount total dependency issues."

exit $errorCount
