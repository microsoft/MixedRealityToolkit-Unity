<#
.SYNOPSIS
    Builds the Mixed Reality Toolkit Unity Package Manager (UPM) packacges.
.DESCRIPTION
    Builds UPM packages for the Mixed Reality Toolkit.
.PARAMETER NodejsVersion
    The desired version of node.js to use when packaging. If not specified, a known good version will be used.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER PackageVersion
    What version of the artifacts should we build? If unspecified, the highest
    git tag pointing to HEAD is searched. If none is found, an error is reported.
.PARAMETER ProjectRoot
    The root folder of the project.
#>
param(
    [ValidatePattern("^\d+\.\d+\.\d+")]
    [string]$NodejsVersion = "12.18.0",
    [string]$OutputDirectory = ".\artifacts\upm",
    [ValidatePattern("^\d+\.\d+\.\d+-?[a-zA-Z0-9\.]*$")] # todo - format of d.d.d[-preview.0-9.0-9]
    [string]$PackageVersion,
    [string]$ProjectRoot
)

if (-not $PackageVersion) {
        throw "Unknown package version. Please specify -PackageVersion when building."
}

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}
$OutputDirectory = Resolve-Path "$(Get-Location)\$OutputDirectory"

if (-not $ProjectRoot) {
    # ProjectRoot was not specified, presume the current location is Root\scripts\packaging
    $ProjectRoot = Resolve-Path "$(Get-Location)\..\.." 
}
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
    # providers
    "leapmotion.legacy" = "Assets\MRTK\Providers\LeapMotion";
    "oculus.xrplugin" = "Assets\MRTK\Providers\Oculus\XRSDK"
    "openvr.legacy" = "Assets\MRTK\Providers\OpenVR";
    "unityar" = "Assets\MRTK\Providers\UnityAR";
    "windows" = "Assets\MRTK\Providers\Windows";
    "wmr" = "Assets\MRTK\Providers\WindowsMixedReality\XR2018";
    "wmr.xrplugin" = "Assets\MRTK\Providers\WindowsMixedReality\XRSDK";
    "wmr.shared" = "Assets\MRTK\Providers\WindowsMixedReality\Shared";
    "xrplugin" = "Assets\MRTK\Providers\XRSDK";
    # extensions
    "handphysicsservice" = "Assets\MRTK\Extensions\HandPhysicsService";
    "losttrackingservice" = "Assets\MRTK\Extensions\LostTrackingService";
    "scenetransitionservice" = "Assets\MRTK\Extensions\SceneTransitionService";
    # other packages
    "tools" = "Assets\MRTK\Tools";
    "testutilties" = "Assets\MRTK\Tests\TestUtilities";
    "examples" = "Assets\MRTK\Examples";
}

# Ensure we can call npm.cmd to package and publish
[boolean]$nodejsInstalled = $false
try {
    node.exe -v > $null 2> $null
    $nodejsInstalled = $true
}
catch {}

if ($nodejsInstalled -eq $false)
{
    # Acquire and unpack node.js
    $archiveName = "node-v$NodejsVersion-win-x64"
    $archiveFile = ".\$archiveName.zip"
    $downloadUri = "https://nodejs.org/dist/v$NodejsVersion/$archiveFile"
    Write-Output "Downloading node.js v$NodejsVersion"
    Invoke-WebRequest -Uri $downloadUri -OutFile $archiveFile
    Write-Output "Extracting $archiveFile"
    Expand-Archive -Path $archiveFile -DestinationPath ".\" -Force

    $npmPath = "$scriptPath\$archiveName\$npmPath"
}

# Beginning of the upm packaging script main section
# The overall structure of this script is:
#
# 1) Replace the %version% token in the package.json file with the value of PackageVersion
# 2) Overwrite the package.json file
# 3) Create and the packages and copy to the OutputFolder
# 4) Cleanup files created and/or modified

$cmdFullPath = "$env:systemroot\system32\cmd.exe"

# Create and publish the packages
foreach ($entry in $packages.GetEnumerator()) {
    $packageFolder = $entry.Value
    $packagePath = "$ProjectRoot\$packageFolder"
  
    # Switch to the folder containing the package.json file
    Set-Location $packagePath

    # Apply the version number to the package json file
    $packageJsonPath = "$packagePath\package.json"
    $packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
    $packageJson = ($packageJson -replace "%version%", $PackageVersion)
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
    npm pack

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

# Return the the scripts\packaging folder
Set-Location -Path $scriptPath

if ($nodejsInstalled -eq $false) {
    # Cleanup the node.js "installation"
    Remove-Item ".\$archiveName" -Recurse -Force
    Remove-Item $archiveFile -Force
}
