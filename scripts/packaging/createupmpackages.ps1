# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Builds the Mixed Reality Toolkit Unity Package Manager (UPM) packages.
.DESCRIPTION
    Builds UPM packages for the Mixed Reality Toolkit.
.PARAMETER ProjectRoot
    The root folder of the project.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER Version
    What version of the artifacts should we build?
.PARAMETER PreviewNumber
    The preview number to append to the version. Note: Exclude this parameter to create non-preview packages.
.PARAMETER Repack
    Add this switch if ProjectRoot represents a folder with existing tarballs to be patched and repacked.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectRoot,

    [string]$OutputDirectory = "./artifacts/upm",

    [Parameter(Mandatory = $true)]
    [ValidatePattern("^\d+\.\d+\.\d+-?[a-zA-Z0-9\.]*$")]
    [string]$Version,

    [ValidatePattern("^\d+?[\.\d+]*$")]
    [string]$PreviewNumber,

    [switch]$Repack
)

$ProjectRoot = Resolve-Path -Path $ProjectRoot

if ($PreviewNumber) {
    $Version = "$Version-preview.$PreviewNumber"
}

Write-Output "Package version: $Version"

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}
$OutputDirectory = Resolve-Path -Path $OutputDirectory
Write-Output "OutputDirectory: $OutputDirectory"

$scope = "com.microsoft.mixedreality"
$product = "toolkit"

# This hashtable contains mappings of the packages (by name) to the folder which contains
# the package contents.
#
# The keys of this hashtable will be combined with the scope and product to create the
# final name of the package (for example, com.microsoft.mixedreality.toolkit.foundation
# will use the packaging file contained in the folder associated with the foundation key).
#
# Note that capitalization below in the key itself is significant. Capitalization
# in the values is not significant.
#
# These paths are ProjectRoot relative.
$packages = [ordered]@{
    "foundation"     = "Assets/MRTK";
    "standardassets" = "Assets/MRTK/StandardAssets";
    "extensions"     = "Assets/MRTK/Extensions";
    "tools"          = "Assets/MRTK/Tools";
    "testutilities"  = "Assets/MRTK/Tests/TestUtilities";
    "examples"       = "Assets/MRTK/Examples";
}

# Beginning of the upm packaging script main section
# The overall structure of this script is:
#
# * Ensure necessary documentation files (license, changelog, etc.) are copied to the appropriate location
# * Replace the %version% token in the package.json file with the value of Version
# * Overwrite the package.json file
# * Create and the packages and copy to the OutputFolder
# * Cleanup files created and/or modified

# Create and publish the packages
foreach ($entry in $packages.GetEnumerator()) {
    # Create and publish the package
    $packageName = $entry.Name

    if ($Repack) {
        $packageTarball = "$scope.$product.$packageName"
        $tarballPath = Get-ChildItem -Path $ProjectRoot -Filter $packageTarball* -Name | Select-Object -First 1
        tar -xzf (Join-Path $ProjectRoot $tarballPath)
        $packagePath = Resolve-Path -Path "package"
    }
    else {
        $packagePath = Resolve-Path -Path "$ProjectRoot/$($entry.Value)"
    }

    if (-not $Repack) {
        # The package manifest files what we use are actually templates,
        # rename the files so that npm can consume them.
        Rename-Item -Path "$packagePath/packagetemplate.json" -NewName "$packagePath/package.json"
        Rename-Item -Path "$packagePath/packagetemplate.json.meta" -NewName "$packagePath/package.json.meta"

        $docFolder = "$packagePath/Documentation~"

        # Copy files used by UPM to display license, change log, etc.
        Copy-Item -Path "$ProjectRoot/LICENSE.md" -Destination "$packagePath"
        Copy-Item -Path "$ProjectRoot/UPM/UnityMetaFiles/LICENSE.md.meta.$packageName" -Destination "$packagePath/LICENSE.md.meta"
        Copy-Item -Path "$ProjectRoot/NOTICE.md" -Destination "$packagePath"
        Copy-Item -Path "$ProjectRoot/UPM/UnityMetaFiles/NOTICE.md.meta.$packageName" -Destination "$packagePath/NOTICE.md.meta"
        Copy-Item -Path "$ProjectRoot/CHANGELOG.md" -Destination "$packagePath"
        Copy-Item -Path "$ProjectRoot/UPM/UnityMetaFiles/CHANGELOG.md.meta.$packageName" -Destination "$packagePath/CHANGELOG.md.meta"
        Copy-Item -Path "$ProjectRoot/UPM/Documentation~" -Destination "$docFolder" -Recurse
        Copy-Item -Path "$ProjectRoot/Authors.md" -Destination "$docFolder"

        $scriptPath = "$ProjectRoot/scripts/packaging"
        $samplesFolder = "$packagePath/Samples~"

        if ($packageName -eq "foundation") {
            # The foundation package contains files that are required to be copied into the Assets folder to be used.
            # In order to perform the necessary preparation, without overly complicating this script, we will use a
            # helper script to prepare the folder.
            Start-Process -FilePath "$PSHOME/powershell.exe" -ArgumentList "$scriptPath/foundationpreupm.ps1 -PackageRoot $packagePath" -NoNewWindow -Wait
        }
        elseif ($packageName -eq "standardassets") {
            # The standard assets package contains shaders that need to be imported into the Assets folder so that they
            # can be modified if the render pipeline is changed. To avoid duplicate resources (in library and assets)
            # we rename the Shaders folder to Shaders~, which makes it hidden to the Unity Editor.
            Rename-Item -Path "$packagePath/Shaders" -NewName "$packagePath/Shaders~"
            Remove-Item -Path "$packagePath/Shaders.meta"
        }
        elseif ($packageName -eq "examples") {
            # The examples folder is a collection of sample projects. In order to perform the necessary
            # preparation, without overly complicating this script, we will use a helper script to prepare
            # the folder.
            Start-Process -FilePath "$PSHOME/powershell.exe" -ArgumentList "$scriptPath/examplesfolderpreupm.ps1 -PackageRoot $packagePath" -NoNewWindow -Wait
        }
        elseif ($packageName -eq "extensions") {
            # The extensions folder contains one or more folders that provide their own examples. In order
            # to perform the necessary preparation, without overly complicating this script, we will use a
            # helper script to prepare the folder.
            Start-Process -FilePath "$PSHOME/powershell.exe" -ArgumentList "$scriptPath/extensionsfolderpreupm.ps1 -PackageRoot $packagePath" -NoNewWindow -Wait
        }
        else {
            # Some other folders have localized examples that need to be prepared. Intentionally skip the foundation as those samples
            $exampleFolder = "$packagePath/Examples"
            if (($PackageName -ne "foundation") -and (Test-Path -Path $exampleFolder)) {
                # Ensure the required samples exists
                if (-not (Test-Path -Path $samplesFolder)) {
                    New-Item $samplesFolder -ItemType Directory | Out-Null
                }

                # Copy the examples
                Write-Output "Copying $exampleFolder to $samplesFolder"
                Copy-Item -Path $exampleFolder -Destination $samplesFolder -Recurse -Force
            }
        }
    }

    # Apply the version number to the package json file
    $packageJsonPath = "$packagePath/package.json"
    ((Get-Content -Path $packageJsonPath -Raw) -Replace '("version": |"com\.microsoft\.mixedreality\.toolkit\.\w+": )"([0-9.]+-?[a-zA-Z0-9.]*|%version%)', "`$1`"$Version") | Set-Content -Path $packageJsonPath -NoNewline

    Write-Output "======================="
    Write-Output "Creating $scope.$product.$packageName"
    Write-Output "======================="
    npm pack $packagePath

    # Move package file to OutputFolder
    Move-Item -Path "./$scope.$product.$packageName-$Version.tgz" $OutputDirectory -Force

    # ======================
    # Cleanup the changes we have made
    # ======================
    Write-Output "Cleaning up temporary changes"

    if ($Repack) {
        # Clean up the unpacked tarball folder
        if (Test-Path -Path $packagePath) {
            Remove-Item -Path $packagePath -Recurse -Force
        }
    }
    else {
        if (Test-Path -Path $samplesFolder) {
            # A samples folder was created. Remove it.
            Remove-Item -Path $samplesFolder -Recurse -Force
        }

        if ($packageName -eq "foundation") {
            # The foundation package MOVES some content around. This restores the moved files.
            Start-Process -FilePath "git" -ArgumentList "checkout */Services/SceneSystem/SceneSystemResources*" -NoNewWindow -Wait
        }
        elseif ($packageName -eq "standardassets") {
            # The standard assets package RENAMES and DELETES some content. This restores the original files.
            Rename-Item -Path "$packagePath/Shaders~" -NewName "$packagePath/Shaders"
            Start-Process -FilePath "git" -ArgumentList "checkout */Shaders.meta" -NoNewWindow -Wait
        }

        # Delete the files copied in previously
        Remove-Item -Path "$packagePath/LICENSE.md*"
        Remove-Item -Path "$packagePath/NOTICE.md*"
        Remove-Item -Path "$packagePath/CHANGELOG.md*"
        if (Test-Path -Path $docFolder) {
            # A documentation folder was created. Remove it.
            Remove-Item -Path $docFolder -Recurse -Force
        }

        # Delete the renamed package.json.* files
        Remove-Item -Path "$packagePath/package.json"
        Remove-Item -Path "$packagePath/package.json.meta"

        # Restore original files
        Start-Process -FilePath "git" -ArgumentList "checkout */packagetemplate.*" -NoNewWindow -Wait
    }
}
