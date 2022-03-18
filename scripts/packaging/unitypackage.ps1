# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Builds .unitypackage artifacts for the Mixed Reality Toolkit
.DESCRIPTION
    This script builds the following set of .unitypackages:

    - Foundation
    
    This contains the MixedRealityToolkit, MixedRealityToolkit.SDK,
    MixedRealityToolkit.Providers, and MixedRealityToolkit.Services
    content.

    - Examples

    This contains all of the content under MRTK/Examples

    Note that these packages are intended to mirror the NuGet packages
    described in Assets/MixedReality.Toolkit.Foundation.nuspec and
    Assets/MRTK/Examples/MixedReality.Toolkit.Examples.nuspec.

    Defaults to assuming that the current working directory of the script is in the root
    directory of the repo.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER RepoDirectory
    The root location of the repo. Defaults to "." which assumes that the script
    is running in the root folder of the repo.
.PARAMETER PackageVersion
    The version number of the package that is generated.
    
    If unspecified, the highest git tag pointing to HEAD is searched.
    If none is found, an error is reported.
.PARAMETER LogDirectory
    The location where Unity logs will be stored. Defaults to the current
    working directory.
.PARAMETER UnityDirectory
    The Unity install directory that will be used to build packages.
.PARAMETER Clean
    If true, the OutputDirectory will be recursively deleted prior to package
    generation.
.PARAMETER Verbose
    If true, verbose messages will be displayed.
.EXAMPLE
    .\unitypackage.ps1 -Version 2.0.0

    This will generate two packages that look like:
    artifacts\Microsoft.MixedReality.Toolkit.Unity.Foundation.2.0.0.unitypackage
    artifacts\Microsoft.MixedReality.Toolkit.Unity.Examples.2.0.0.unitypackage
.EXAMPLE
    .\build.ps1 -OutputDirectory .\out -Version 2.0.1 -Clean

    This will generate two packages that look like:
    out\Microsoft.MixedReality.Toolkit.Unity.Foundation.2.0.1.unitypackage
    out\Microsoft.MixedReality.Toolkit.Unity.Examples.2.0.1.unitypackage
#>
param(
    [string]$OutputDirectory = ".\artifacts",
    [string]$RepoDirectory = ".",
    [string]$LogDirectory,
    [string]$UnityDirectory,
    [ValidatePattern("^\d+\.\d+\.\d+-?[a-zA-Z0-9\.]*$")]
    [string]$PackageVersion,
    [switch]$Clean,
    [switch]$Verbose
)

if ( $Verbose ) { $VerbosePreference = 'Continue' }

# This hashtable contains mapping of the packages (by name) to the set
# of top level folders that should be included in that package.
# The keys of this hashtable will contribute to the final naming of the package
# (for example, in Microsoft.MixedReality.Toolkit.Unity.Foundation, the Foundation
# section comes from the key below).
#
# Note that capitalization below in the key itself is significant. Capitalization
# in the values is not significant.
#
# These paths are project-root relative.
$packages = @{
    "Foundation" = @(
        "Assets\MRTK\Core",
        "Assets\MRTK\Providers",
        "Assets\MRTK\SDK",
        "Assets\MRTK\Services",
        "Assets\MRTK\StandardAssets"
    );
    "Extensions" = @(
        "Assets\MRTK\Extensions"
    );
    "Examples" = @(
        "Assets\MRTK\Examples"
    );
    "Tools" = @(
        "Assets\MRTK\Tools"
    );
    "TestUtilities" = @(
        "Assets\MRTK\Tests\TestUtilities"
    );
}

function GetPackageVersion() {
    <#
    .SYNOPSIS
        Gets the most recent tag associated with the HEAD of the repo.
    #>
    return git tag --points-at HEAD | 
        Foreach-Object { if ( $_ -match "^(\d+\.\d+\.\d+)$" ) { [version]$matches[1] } } | 
        Sort-Object -Descending |
        Select-Object -First 1
}

function CleanPackageManifest() {
    <#
    .SYNOPSIS
        Ensures that the package manifest does not contain packages that trigger adding dependencies on
        optional features by default.
    #>

    $fileName = $RepoDirectory + "\Packages\manifest.json"
    (Get-Content $fileName) | ForEach-Object {
        if ($_ -notmatch ("arfoundation|arsubsystems|xr.management|legacyinputhelpers")) {
            $line = $_
        }
        else {
            $line = ""
        }
    $line } | Set-Content $fileName  
}

# Beginning of the .unitypackage script main section
# The overall structure of this script looks like:
#
# 1) Checking that PackageVersion and UnityVersion values are present and valid.
# 2) Ensures that the output directory and log directory exists.
# 3) Uses the Unity editor's ExportPackages functionality (using the -exportPackages)
#    to build the .unitypackage files.

Write-Verbose "Mixed Reality Toolkit .unitypackage generation beginning"

Write-Verbose "Reconciling package version:"
if (-not $PackageVersion) {
    $PackageVersion = GetPackageVersion

    # If we still can't find a package version, error out.
    if ( -not $PackageVersion ) {
        throw "Could not find a valid version to build. Specify -PackageVersion "
            + "when building, or tag your git commit."
    }
}
Write-Verbose "Using $PackageVersion"

Write-Verbose "Reconciling Unity binary:"
if (-not $UnityDirectory) {
    throw "-UnityDirectory is a required flag"
}

$unityEditor = Get-ChildItem $UnityDirectory -Filter 'Unity.exe' -Recurse | Select-Object -First 1 -ExpandProperty FullName
if (-not $unityEditor) {
    throw "Unable to find the Unity editor executable in $UnityDirectory"
}
Write-Verbose $unityEditor;

if ($Clean) {
    Write-Verbose "Recursively deleting output directory: $OutputDirectory"
    Remove-Item -ErrorAction SilentlyContinue $OutputDirectory -Recurse 
}

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}

if (-not $LogDirectory) {
    $LogDirectory = "."
} else {
    New-Item $LogDirectory -ItemType Directory | Out-Null
}

$OutputDirectory = Resolve-Path $OutputDirectory
$LogDirectory = Resolve-Path $LogDirectory
$RepoDirectory = Resolve-Path $RepoDirectory

Write-Verbose "Cleaning package manifest (removing AR and XR references)"
CleanPackageManifest

foreach ($entry in $packages.GetEnumerator()) {
    $packageName = $entry.Name;
    $folders = $entry.Value

    $logFileName = "$LogDirectory\Build-UnityPackage-$packageName.$PackageVersion.log"
    $unityPackagePath = "$OutputDirectory\Microsoft.MixedReality.Toolkit.Unity.${packageName}.$PackageVersion.unitypackage";
    
    # The exportPackages flag expects the last value in the array
    # to be the final output destination.
    $exportPackages = $folders + $unityPackagePath
    $exportPackages = $exportPackages -join " "

    Write-Verbose "Generating .unitypackage: $unityPackagePath"
    Write-Verbose "Log location: $logFileName"

    # Assumes that unity package building has failed, unless we
    # succeed down below after running the Unity packaging step.
    $exitCode = 1

    try {
        $unityArgs = "-BatchMode -Quit -Wait " +
            "-projectPath $RepoDirectory " +
            "-exportPackage $exportPackages " +
            "-logFile $logFileName"

        # Starts the Unity process, and the waits (and shows output from the editor in the console
        # while the process is still running.)
        $proc = Start-Process -FilePath "$unityEditor" -ArgumentList "$unityArgs" -PassThru
        $ljob = Start-Job -ScriptBlock { param($log) Get-Content "$log" -Wait } -ArgumentList $logFileName

        while (-not $proc.HasExited -and $ljob.HasMoreData)
        {
            Receive-Job $ljob
            Start-Sleep -Milliseconds 200
        }
        Receive-Job $ljob
        Stop-Job $ljob
        Remove-Job $ljob

        Stop-Process $proc

        $exitCode = $proc.ExitCode
        if (($proc.ExitCode -eq 0) -and (Test-Path $unityPackagePath)) {
            Write-Verbose "Successfully created $unityPackagePath"
        }
        else {
            # It's possible that $exitCode could have been set to a zero value
            # despite the package not being there - in that case this should still return
            # failure (i.e. a non-zero exit code)
            $exitCode = 1
            Write-Error "Failed to create $unityPackagePath"
        }
    }
    catch { Write-Error $_ }

    if ($exitCode -ne 0) {
        exit $exitCode
    }
}