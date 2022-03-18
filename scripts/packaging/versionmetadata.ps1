# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Adds version metadata to the MRTK prior to building and packaging.
.DESCRIPTION
    This script represents a step in the build process where certain version
    metadata is added to specific locations in the code, such that the ultimately
    generated binaries and .unitypackages are correctly marked with the right
    metadata.

    In particular, this script will add:
    
    - Version.txt (to specific directories within the MRTK Assets/ folder)
      This is primarily useful for .unitypackage distributions for consumers to be
      able to quickly understand the version they are using.
    - AssemblyInfo.cs (as a sibling in all folders that contain an asmdef file)
      This is primarily useful for binary distribution (so that the generated DLLs
      will be appropriately marked with version numbers and company/origin
      information)
      If AssemblyInfo.cs already exists, this will instead just add the version
      information to the file.

    This will also validate that some project specific versioning to ensure that
    it matches the passed in version (i.e. the version values in
    ProjectSettings/ProjectSettings.asset, which are ultimately not consumed by
    consumers, but should stay in sync for consistency's sake)

    A general note on how to update the version values:
    There are three locations to update:
    pipelines/config/settings.yml (MRTKVersion)
    ProjectSettings/ProjectSettings.asset (bundleVersion, metroPackageVersion)

    They should have the same value (except metroPackageVersion which will contain
    one more trailing .0 value)
.EXAMPLE
    .\versionmetadata.ps1 -Directory c:\path\to\mrtkroot -Version 2.5.0
#>
param(
    # The root directory of the MRTK (i.e. the folder that contains the Assets/
    # and ProjectSettings/ folder).
    [Parameter(Mandatory=$true)]
    [string]$Directory,

    # The Version with three version digits (i.e. 2.1.0).
    [Parameter(Mandatory=$true)]
    [string]$Version
)

<#
.SYNOPSIS
    Adds Version.txt files to specific sub-folders within the Assets/ folder.
#>
function AddVersionTxt {
    [CmdletBinding()]
    param(
        [string]$Directory,
        [string]$Version
    )
    process {
        $locations = @(
            "Assets/MRTK/Core",
            "Assets/MRTK/Examples",
            "Assets/MRTK/Extensions",
            "Assets/MRTK/Providers",
            "Assets/MRTK/SDK",
            "Assets/MRTK/Services",
            "Assets/MRTK/StandardAssets",
            "Assets/MRTK/Tests",
            "Assets/MRTK/Tools"
        )

        $content = "Microsoft Mixed Reality Toolkit $Version"
        foreach ($location in $locations) {
            $filename = Join-Path -Path $location -ChildPath "Version.txt"
            Set-Content -Path $filename -Value $content
            Write-Host "Added Version.txt at $filename"
        }
    }
}

<#
.SYNOPSIS
    Adds AssemblyInfo.cs files to all locations within the Assets/ folder that
    have an .asmdef file.

    If AssemblyInfo.cs already exists, this will instead just add the version
    information to the file.

    Returns true if an issue is found.
#>
function AddAssemblyInfo {
    [CmdletBinding()]
    param(
        [string]$Directory,
        [string]$Version
    )
    process {
        $containsIssue = $false
        $assets = Join-Path -Path $Directory -ChildPath "Assets"
        $mrtkFolder = Join-Path -Path $assets -ChildPath "MRTK"
        $asmdefs = Get-ChildItem $assets *.asmdef -Recurse | Select-Object FullName
        foreach ($asmdef in $asmdefs) {
            # The AssemblyInfo.cs file will be added as a sibling of the .asmdef location,
            # so we need to trim off the filename.
            $folder = Split-Path -Path $asmdef.FullName
            $filename = Join-Path -Path $folder -ChildPath "AssemblyInfo.cs"

            # We also need to parse out the MRTK subfolder (i.e. Core, SDK, Examples)
            # which goes into the AssemblyProduct metadata.
            $projectFolder = $filename.Replace($mrtkFolder, "")
            $split = $projectFolder.Split([IO.Path]::DirectorySeparatorChar)

            # Since $projectFolder has the form "/SDK/Path/Thing"
            # we use index 1 to get the value "SDK"
            # Also note that there's a space ahead of the value which is intentional
            # to ensure that resolved product looks like:
            # "Microsoft® Mixed Reality Toolkit SDK" when in a non-Core folder
            # and "Microsoft® Mixed Reality Toolkit" when in the Core folder.
            $project = " " + $split[1]
            if ($project -eq " Core") {
                $project = ""
            }

            # Note that this is left-indent adjusted so that the file output
            # ends up looking reasonable.
            $copyright = 
@"
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
"@

            $content =
@"
[assembly: System.Reflection.AssemblyVersion("$Version.0")]
[assembly: System.Reflection.AssemblyFileVersion("$Version.0")]

[assembly: System.Reflection.AssemblyProduct("Microsoft® Mixed Reality Toolkit$project")]
[assembly: System.Reflection.AssemblyCopyright("Copyright © Microsoft Corporation")]
"@

            # If the AssemblyInfo already exists, then we should check that it doesn't already contain
            # a version - this would be bad as we'd doubly-define the version
            if (Test-Path -Path $filename) {
                if (Get-Content $filename | Select-String "AssemblyVersion") {
                    $containsIssue = $true
                    Write-Host "$filename should not have an AssemblyVersion declaration, this is added at CI time"
                }
                else {
                    Add-Content -Path $filename -Value $content
                    Write-Host "Updated AssemblyInfo.cs at $filename"
                }
            } else {
                Set-Content -Path $filename -Value $copyright
                Add-Content -Path $filename -Value $content
                Write-Host "Added AssemblyInfo.cs at $filename"
            }
        }
        $containsIssue
    }
}

<#
.SYNOPSIS
    Checks that the ProjectSettings file has correct matching version information.

    Returns true if an issue is found.
#>
function CheckProjectSettings {
    [CmdletBinding()]
    param(
        [string]$Directory,
        [string]$Version
    )
    process {
        $projectSettings = Join-Path -Path $Directory -ChildPath "ProjectSettings/ProjectSettings.asset"
        $containsIssue = $false
        $fileContent = Get-Content $ProjectSettings
        foreach ($line in $fileContent) {
            if (($line -match " bundleVersion") -or ($line -match " metroPackageVersion")) {
                $expected = "$Version"
                $split = $line.Split(":")
                $version = $split[1].Trim()

                # For the metroPackageVersion (which is used to generate the MRTK project
                # AppX versions, they need to have an additional .0 tacked onto the end)
                if ($line -match " metroPackageVersion") {
                    $expected = "$expected.0"
                }

                if ($expected -ne $version) {
                    $containsIssue = $true
                    Write-Host "Mismatched version on line: $line. Expected version: $expected"
                }
            }
        }
        $containsIssue
    }
}

AddVersionTxt $Directory $Version
if ((AddAssemblyInfo $Directory $Version) -Or (CheckProjectSettings $Directory $Version)) {
    Write-Output "Issues found, please see above for details"
    exit 1;
}
else {
    Write-Output "No issues found"
    exit 0;
}
