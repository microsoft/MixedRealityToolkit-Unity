<#
.SYNOPSIS
    Adds version metadata to the MRTK prior to building and packagine.
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

    This will also validate that some project specific versioning to ensure that
    it matches the passed in version (i.e. the version values in
    ProjectSettings/ProjectSettings.asset, which are ultimately not consumed by
    consumers, but should stay in sync for consistency's sake)
.EXAMPLE
    .\versiondata.ps1 -Directory c:\path\to\mrtkroot -Version 2.5.0
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
            "Assets/MRTK/Tests",
            "Assets/MRTK/Tools"
        )

        $content = "Microsoft Mixed Reality Toolkit $Version"
        foreach ($location in $locations) {
            $filename = Join-Path -Path $location -ChildPath "Version.txt"
            Set-Content -Path $filename -Value $content
            Write-Output "Added Version.txt at $filename"
        }
    }
}

<#
.SYNOPSIS
    Adds AssemblyInfo.cs files to all locations within the Assets/ folder that
    have an .asmdef file.
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
            "Assets/MRTK/Tests",
            "Assets/MRTK/Tools"
        )

        $content = "Microsoft Mixed Reality Toolkit $Version"
        foreach ($location in $locations) {
            $filename = Join-Path -Path $location -ChildPath "Version.txt"
            Set-Content -Path $filename -Value $content
            Write-Output "Added Version.txt at $filename"
        }
    }
}


<#
.SYNOPSIS
    Adds AssemblyInfo.cs files to all locations within the Assets/ folder that
    have an .asmdef file.
#>
function AddVersionTxt {
    [CmdletBinding()]
    param(
        [string]$Directory,
        [string]$Version
    )
    process {
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

            # Since $projectFolder has the form "/Core/Path/Thing"
            # we use index 1 to get the value "Core"
            $project = $split[1]

            # Note that this is left-indent adjusted so that the file output
            # ends up looking reasonable.
            $content = 
@"
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Reflection;

[assembly: AssemblyVersion("$Version.0")]
[assembly: AssemblyFileVersion("$Version.0")]

[assembly: AssemblyProduct("Microsoft® Mixed Reality Toolkit $project")]
[assembly: AssemblyCopyright("Copyright © Microsoft Corporation")]
"@
        
            Write-Output "Added AssemblyInfo.cs at $filename"
        }
    }
}


AddVersionTxt $Directory $Version
AddVersionTxt $Directory $Version
