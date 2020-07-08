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
.PARAMETER OutputTarget
    What is the target for the artifacts? To the Official server ("official"), test server ("test") or local folder ("local")?
#>
param(
    [ValidatePattern("^\d+\.\d+\.\d+")]
    [string]$NodejsVersion = "12.18.0",
    [string]$OutputDirectory = ".\artifacts\upm",
    [ValidatePattern("^\d+\.\d+\.\d+-?[a-zA-Z0-9\.]*$")] # todo - format of d.d.d[-preview.0-9.0-9]
    [string]$PackageVersion,
    [string]$OutputTarget = "local"
)

if (-not $PackageVersion) {
        throw "Unknown package version. Please specify -PackageVersion when building."
}

if ($OutputTarget -eq "local") {
    if (-not (Test-Path $OutputDirectory -PathType Container)) {
        New-Item $OutputDirectory -ItemType Directory | Out-Null
    }
    $OutputDirectory = Resolve-Path "$(Get-Location)\$OutputDirectory"
}

$projectRoot = Resolve-Path "$(Get-Location)\..\.." 
$scriptPath = "$projectRoot\scripts\packaging"

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
# These paths are projectRoot relative.
$packages = [ordered]@{
    "foundation" = "Assets\MRTK";
    # providers
    "leapmotion.legacy" = "Assets\MRTK\Providers\LeapMotion";
    "oculus" = "Assets\MRTK\Providers\Oculus\XRSDK"
    "openvr.legacy" = "Assets\MRTK\Providers\OpenVR";
    "unityar" = "Assets\MRTK\Providers\UnityAR";
    "windows" = "Assets\MRTK\Providers\Windows";
    "wmr.legacy" = "Assets\MRTK\Providers\WindowsMixedReality\XR2018";
    "wmr" = "Assets\MRTK\Providers\WindowsMixedReality\XRSDK";
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

$npmPath = "npm"

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
# 1) Copy the appropriate publishing (.npmrc) file and set the npm command
# 2) Replace the %version% token in the package.json file with the value of PackageVersion
# 3) Overwrite the package.json file
# 4) Create and publish the packages (local publishing copies the package to the OutputFolder)
# 5) Cleanup files created and/or modified

$npmrcFile = ".npmrc"
$npmrcFileFullPath = "$projectRoot\scripts\packaging\$npmrcFile.$OutputTarget"
$cmdFullPath = "$env:systemroot\system32\cmd.exe"
$npmCommand = "pack"
$updateAuth = $true
$isLocalBuild = ($OutputTarget -eq "local")

# Create and publish the packages
foreach ($entry in $packages.GetEnumerator()) {
    $packageFolder = $entry.Value
    $packagePath = "$projectRoot\$packageFolder"
    $npmrcBackup = "$npmrcFile.mrtk-bak"
  
    # Switch to the folder containing the package.json file
    Set-Location $packagePath

    # Backup any existing .npmrc file
    if (Test-Path -Path $npmrcFile ) {
        Rename-Item -Path $npmrcFile  -NewName $npmrcBackup
    }

     if (-not ($isLocalBuild)) {
        # Copy the appropriate .nmprc file
        Copy-Item -Path $npmrcFileFullPath -Destination ".\$npmrcFile" -Force

        # Set the npm command to "publish"
        $npmCommand = "publish"
    }

    # Get/update the credentials needed to access the server.
    # This only needs to happen when credentials are updated. We run this script
    # once per build to ensure the machine is ready to publish.
    if (($updateAuth -eq $true) -and (Test-Path -Path $npmrcFile )) {
        Start-Process -FilePath "$PSHOME\powershell.exe" -ArgumentList "vsts-npm-auth.ps1 -config $npmrcFile" -NoNewWindow -Wait
        $updateAuth = $false
    }

    # Apply the version number to the package json file
    $packageJsonPath = "$packagePath\package.json"
    $packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
    $packageJson = ($packageJson -replace "%version%", $PackageVersion)
    [System.IO.File]::WriteAllText($packageJsonPath, $packageJson)

    # Create and publish the package
    $packageName = $entry.Name
    $registryName = $OutputPath

    if ($isLocalBuild) {
        $registryName = $OutputDirectory
    }
    else {
        $registryName = "the $OutputTarget registry"
    }

    # The examples folder needs a custom setup step
    if ($entry.Name -eq "examples") {
        Write-Output "Preparing the examples folder"
        Start-Process -FilePath "$PSHOME\powershell.exe" -ArgumentList "$scriptPath\examplesfolderpreupm.ps1" -NoNewWindow -Wait
    }

    Write-Output "======================="
    Write-Output "Creating $scope.$product.$packageName and publishing to $registryName"
    Write-Output "======================="
    Start-Process -FilePath $cmdFullPath -ArgumentList "/c $npmPath $npmCommand" -NoNewWindow -Wait

    if ($isLocalBuild) {
        # Move package file to OutputFolder
        Move-Item -Path ".\*.tgz" $OutputDirectory -Force
    }

    # ======================
    # Cleanup the changes we have made
    # ======================

    # The examples folder needs a custom cleanup step
    if ($entry.Name -eq "examples") {
        Write-Output "Cleaning up examples folder"
        Start-Process -FilePath "$PSHOME\powershell.exe" -ArgumentList "$scriptPath\examplesfolderpostupm.ps1" -NoNewWindow -Wait
    }
    
    # Restore the package.json file
    Start-Process -FilePath "git" -ArgumentList "checkout package.json" -NoNewWindow -Wait

    # Delete the .npmrc file
    Remove-Item -ErrorAction SilentlyContinue $npmrcFile

    # Restore any backup file that we may have made
    if (Test-Path -Path "$npmrcBackup") {
         Rename-Item -Path "$npmrcBackup" -NewName $npmrcFile 
    }
}

# Return the the scripts\packaging folder
Set-Location -Path $scriptPath

if ($nodejsInstalled -eq $false) {
    # Cleanup the node.js "installation"
    Remove-Item ".\$archiveName" -Recurse -Force
    Remove-Item $archiveFile -Force
}