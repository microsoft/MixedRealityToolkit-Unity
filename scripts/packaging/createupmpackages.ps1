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
    The build number to append to the version. Note: This value is only used if the IsPreview parameter is set to true.
.PARAMETER IsOfficial
    Are we creating official packages? If not, the version of the artifiacts will be formatted as "<$Version>-preview.<BuildNumber>".
#>
param(
    [string]$ProjectRoot,
    [string]$OutputDirectory = ".\artifacts\upm",
    [ValidatePattern("^\d+\.\d+\.\d+-?[a-zA-Z0-9\.]*$")]
    [string]$Version,
    [ValidatePattern("^\d+?[\.\d+]*$")]
    [string]$BuildNumber,
    [bool]$IsOfficial = $False
)

$startPath = "$(Get-Location)"

if (-not $ProjectRoot) {
    throw "Unknown project root path. Please specify -ProjectRoot when building."
}
$ProjectRoot = Resolve-Path -Path $ProjectRoot
Write-Output "Project root: $ProjectRoot"

if (-not $Version) {
    throw "Unknown package version. Please specify -Version when building."
}

if (-not $IsOfficial) {
    if (-not $BuildNumber) {
        throw "Unknown build number. Please specify -BuildNumber when IsOfficial is false."
    }
    $Version = "$Version-preview.$BuildNumber"
}
Write-Output "Package version: $Version"

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}
$OutputDirectory = Resolve-Path -Path $OutputDirectory
Write-Output "OutputDirectory: $OutputDirectory"

$scriptPath = "$ProjectRoot\scripts\packaging"

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
    "foundation" = "Assets\MRTK";
    "foundation.xr2018" = "Assets"; 
    # providers
    "unityar" = "Assets\MRTK\Providers\UnityAR";
    # extensions
    "extensions" = "Assets\MRTK\Extensions";
    # tools
    "tools" = "Assets\MRTK\Tools";
    # tests
    "testutilties" = "Assets\MRTK\Tests\TestUtilities";
    # examples
    "examples" = "Assets\MRTK\Examples";
}

$npmCommand = "npm"

# Beginning of the upm packaging script main section
# The overall structure of this script is:
#
# 1) Replace the %version% token in the package.json file with the value of Version
# 2) Overwrite the package.json file
# 3) Create and the packages and copy to the OutputFolder
# 4) Cleanup files created and/or modified

$cmdFullPath = "$env:systemroot\system32\cmd.exe"

# Create and publish the packages
foreach ($entry in $packages.GetEnumerator()) {
    $packageFolder = $entry.Value
    $packagePath = Resolve-Path -Path "$ProjectRoot\$packageFolder"
  
    # Switch to the folder containing the package.json file
    Set-Location $packagePath

    # Apply the version number to the package json file
    $packageJsonPath = "$packagePath\package.json"
    $packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
    $packageJson = ($packageJson -replace "%version%", $Version)
    [System.IO.File]::WriteAllText($packageJsonPath, $packageJson)

    # Create and publish the package
    $packageName = $entry.Name
    $registryName = $OutputPath

    $samplesFolder = "$packagePath\Samples~"
     
    if ($packageName -eq "examples") {
        # The examples folder is a collection of sample projects. In order to perform the necessary
        # preparaton, without overly complicating this script, we will use a helper script to prepare
        # the folder.
        Start-Process -FilePath "$PSHOME\powershell.exe" -ArgumentList "$scriptPath\examplesfolderpreupm.ps1 -PackageRoot $packagePath" -NoNewWindow -Wait
    }
    elseif ($packageName -eq "extensions") {
        # The extensions folder contains one or more folders that provide their own examples. In order
        # to perform the necessary preparation, without overly complicating this script, we will use a
        # helper script to prepare the folder.
        Start-Process -FilePath "$PSHOME\powershell.exe" -ArgumentList "$scriptPath\extensionsfolderpreupm.ps1 -PackageRoot $packagePath" -NoNewWindow -Wait
    }
    # todo: similar script for extensions
    else {
        # Some other folders have localized examples that need to be prepared. Intentionally skip the foundation as those samples
        # are packaged in the examples package.
        $exampleFolder = "$packagePath\Examples"
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

    Write-Output "======================="
    Write-Output "Creating $scope.$product.$packageName"
    Write-Output "======================="
    Start-Process -FilePath $cmdFullPath -ArgumentList "/c $npmCommand pack" -NoNewWindow -Wait

    # Move package file to OutputFolder
    Move-Item -Path ".\*.tgz" $OutputDirectory -Force

    # ======================
    # Cleanup the changes we have made
    # ======================
    Write-Output "Cleaning up temporary changes"

    if (Test-Path -Path $samplesFolder) {
        # A samples folder was created. Remove it.
        Remove-Item -Path $samplesFolder -Recurse -Force
    }
    
    # Restore the package.json file
    Start-Process -FilePath "git" -ArgumentList "checkout package.json" -NoNewWindow -Wait
}

# Return to the starting path
Set-Location $startPath
