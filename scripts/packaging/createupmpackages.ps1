<#
.SYNOPSIS
    Builds the Mixed Reality Toolkit Unity Package Manager (UPM) packacges.
.DESCRIPTION
    Builds UPM packages for the Mixed Reality Toolkit.
.PARAMETER ProjectRoot
    The root folder of the project.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER Version
    What version of the artifacts should we build?
.PARAMETER BuildNumber
    The build number to append to the version. Note: This value is required when the ExcludeBuildNumber parameter is omitted.
.PARAMETER ExcludeBuildNumber
    Indicates that the build number should be excluded from the generated artifacts. If this parameter is specified, the version
    of the artifacts will be formatted as "<$Version>", if omitted, the artifact version will be "<$Version>-preview.<BuildNumber>".
#>
param(
    [string]$ProjectRoot,
    [string]$OutputDirectory = "./artifacts/upm",
    [ValidatePattern("^\d+\.\d+\.\d+-?[a-zA-Z0-9\.]*$")]
    [string]$Version,
    [ValidatePattern("^\d+?[\.\d+]*$")]
    [string]$BuildNumber,
    [Parameter(Mandatory=$false)]
    [Switch]$ExcludeBuildNumber
)

[string]$startPath = $(Get-Location)

if (-not $ProjectRoot) {
    throw "Missing required parameter: -ProjectRoot."
}
$ProjectRoot = Resolve-Path -Path $ProjectRoot

if (-not $Version) {
    throw "Missing required parameter: -Version."
}

if ((-not $BuildNumber) -and (-not $ExcludeBuildNumber)) {
    throw "Missing required parameter: -BuildNumber. This parameter is required when -ExcludeBuildNumber is not specified."
}
if (-not $ExcludeBuildNumber) {
    $Version = "$Version-preview.$BuildNumber"
}
Write-Output "Package version: $Version"

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}
$OutputDirectory = Resolve-Path -Path $OutputDirectory
Write-Output "OutputDirectory: $OutputDirectory"

$scriptPath = "$ProjectRoot/scripts/packaging"

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
    "foundation" = "Assets/MRTK";
    "standardassets" = "Assets/MRTK/StandardAssets";
    # extensions
    "extensions" = "Assets/MRTK/Extensions";
    # tools
    "tools" = "Assets/MRTK/Tools";
    # tests
    "testutilties" = "Assets/MRTK/Tests/TestUtilities";
    # examples
    "examples" = "Assets/MRTK/Examples";
}

# Beginning of the upm packaging script main section
# The overall structure of this script is:
#
# 1) Replace the %version% token in the package.json file with the value of Version
# 2) Overwrite the package.json file
# 3) Create and the packages and copy to the OutputFolder
# 4) Cleanup files created and/or modified

# Create and publish the packages
foreach ($entry in $packages.GetEnumerator()) {
    $packageFolder = $entry.Value
    $packagePath = Resolve-Path -Path "$ProjectRoot/$packageFolder"
  
    # Switch to the folder containing the package.json file
    Set-Location $packagePath

    # The package manifest files what we use are actually templates,
    # rename the files so that npm can consume them.
    Rename-Item -Path "$packagePath/packagetemplate.json" -NewName "$packagePath/package.json"
    Rename-Item -Path "$packagePath/packagetemplate.json.meta" -NewName "$packagePath/package.json.meta"

    # Apply the version number to the package json file
    $packageJsonPath = "$packagePath/package.json"
    $packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
    $packageJson = ($packageJson -replace "%version%", $Version)
    [System.IO.File]::WriteAllText($packageJsonPath, $packageJson)

    # Create and publish the package
    $packageName = $entry.Name

    $samplesFolder = "$packagePath/Samples~"

    if ($packageName -eq "examples") {
        # The examples folder is a collection of sample projects. In order to perform the necessary
        # preparaton, without overly complicating this script, we will use a helper script to prepare
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
        if (($PackageName -ne "foundation") -and ($PackageName -ne "foundation.xr2018") -and (Test-Path -Path $exampleFolder)) {
            # Ensure the required samples exists
            if (-not (Test-Path -Path $samplesFolder)) {
                New-Item $samplesFolder -ItemType Directory | Out-Null
            }

            # Copy the examples
            Write-Output "Copying $exampleFolder to $samplesFolder"
            Copy-Item -Path $exampleFolder -Destination $samplesFolder -Recurse -Force
        }
    }

    Write-Output "======================="
    Write-Output "Creating $scope.$product.$packageName"
    Write-Output "======================="
    npm pack

    # Move package file to OutputFolder
    Move-Item -Path "./*.tgz" $OutputDirectory -Force

    # ======================
    # Cleanup the changes we have made
    # ======================
    Write-Output "Cleaning up temporary changes"

    if (Test-Path -Path $samplesFolder) {
        # A samples folder was created. Remove it.
        Remove-Item -Path $samplesFolder -Recurse -Force
    }
    
    # Delete the renamed package.json.* files
    Remove-Item -Path "$packagePath/package.json"
    Remove-Item -Path "$packagePath/package.json.meta"

    # Restore the original template files
    Start-Process -FilePath "git" -ArgumentList "checkout packagetemplate.*" -NoNewWindow -Wait
}

# Return to the starting path
Set-Location $startPath
