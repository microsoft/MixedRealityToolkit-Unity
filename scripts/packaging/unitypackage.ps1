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

    This contains all of the content under MixedRealityToolkit.Examples

    Note that these packages are intended to mirror the NuGet packages
    described in Assets/MixedReality.Toolkit.Foundation.nuspec and
    Assets/MixedRealityToolkit.Examples/MixedReality.Toolkit.Examples.nuspec.

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
.PARAMETER UnityVersion
    What version of Unity should we use? Falls back to -ge, then to -latest.
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
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]$PackageVersion,
    [ValidatePattern("^\d+\.\d+\.\d+[fpb]\d+$")]
    [string]$UnityVersion, 
    [switch]$Clean,
    [switch]$Verbose
)

if ( $Verbose ) { $VerbosePreference = 'Continue' }

Import-Module UnitySetup -MinimumVersion '4.0.97' -ErrorAction Stop

# This hashtable contains mapping of the packages (by name) to the set
# of top level folders that should be included in that package.
# The keys of this hashtable will contribute to the final naming of the package
# (for example, in Microsoft.MixedReality.Toolkit.Unity.Foundation, the Foundation
# section comes from the key below).
#
# Note that capatilization below in the key itself is significant. Capitalization
# in the values is not significant.
#
# These paths are project-root relative.
$packages = @{
    "Foundation" = @(
        "Assets\MixedRealityToolkit",
        "Assets\MixedRealityToolkit.Providers",
        "Assets\MixedRealityToolkit.SDK",
        "Assets\MixedRealityToolkit.Services"
    );
    "Examples" = @(
        "Assets\MixedRealityToolkit.Examples"
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

function GetFallbackUnityVersion() {
    <#
    .SYNOPSIS
        Gets the first version of Unity that is greater than or equal to the Unity
        version specified in the project.
    .DESCRIPTION
        This will fallback to the latest Unity version if a greater than or equal
        to version is not found.
    #>
    $upi = Get-UnityProjectInstance
    $usis = Get-UnitySetupInstance

    # Try to find an equal or greater version than our project
    $usi = $usis | Where-Object { $_.Version -ge $upi.Version } |
        Sort-Object -Property 'Version' | 
        Select-Object -First 1

    # If no version is found that is greater than or equal to our project
    # fall back to the latest.
    if (-not $usi) {
        $usi = $usis | Select-UnitySetupInstance -Latest
    }
    if (-not $usi) {
        throw 'Could not find version of Unity to build with.'
    }

    # Warn if we're not using the exact version
    if ($usi.Version.CompareTo($upi.Version) -ne 0) {
        Write-Warning "Could not find Unity $($upi.Version), falling back to $($usi.Version)"
    }

    return $usi.Version 
}

function ValidateUnityVersion([String] $Version) {
    <#
    .SYNOPSIS
        Validates that the specified version of Unity exists.
    #>
    $usi = Get-UnitySetupInstance | Select-UnitySetupInstance -Version $Version
    if ( -not $usi ) {
        throw "Could not find specified version of unity: $Version"
    }
}

function ValidatePackageFolders(
    [String] $PackageName,
    [String[]] $Folders) {
    <#
    .SYNOPSIS
        Validates that all of the folders specified exist on disk.
    #>
    foreach ($folder in $Folders) {
        if (-not (Test-Path $folder -PathType Container)) {
            throw "Package $PackageName is missing folder $folder"
        }
    }
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

Write-Verbose "Reconciling Unity version:"
if (-not $UnityVersion) {
    $UnityVersion = GetFallbackUnityVersion
}
ValidateUnityVersion($UnityVersion)
Write-Verbose "Using Unity version $UnityVersion"

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

foreach ($entry in $packages.GetEnumerator()) {
    $packageName = $entry.Name;
    $folders = $entry.Value

    $logFileName = "$LogDirectory\Build-UnityPackage-$packageName.$PackageVersion.log"
    $unityPackagePath = "$OutputDirectory\Microsoft.MixedReality.Toolkit.Unity.${packageName}.$PackageVersion.unitypackage";
    
    # The exportPackages flag expects the last value in the array
    # to be the final output destination.
    $exportPackages = $folders + $unityPackagePath

    Write-Verbose "Generating .unitypackage: $unityPackagePath"
    Write-Verbose "Log location: $logFileName"
    
    try {
        $unityArgs = @{
            'BatchMode'       = $true
            'AcceptAPIUpdate' = $true
            'Quit'            = $true
            'PassThru'        = $true
            'Wait'            = $true
            'ExportPackage'   = $exportPackages
            'LogFile'         = $logFileName
            'Version'         = $UnityVersion
        }

        $process = Start-UnityEditor @unityArgs
        if (($process.ExitCode -eq 0) -and (Test-Path $unityPackagePath)) {
            Write-Verbose "Successfully created $unityPackagePath"
        }
        else { Write-Error "Failed to create $unityPackagePath" }
    }
    catch { Write-Error $_ }
}
