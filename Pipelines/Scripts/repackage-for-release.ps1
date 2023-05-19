# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Repackages release candidate tgz to  the Mixed Reality Toolkit Unity Package Manager (UPM) packages.
.DESCRIPTION
    Builds UPM packages for the Mixed Reality Toolkit.
.PARAMETER PackageRoot
    The root folder of the project.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER BuildNumber
    The fourth digit (revision) for the full version.
.PARAMETER PreviewTag
    The tag to append after the version, including the preview number (e.g. "internal.0" or "pre.100")
.PARAMETER Revision
    The revision number for the build.  (e.g. "230508.3" or "2") 
.PARAMETER ReleasePackages
    An array of the package names that are approved for release.  If the package isn't in this array, it will get labeled -pre
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$PackageRoot,

    [string]$OutputDirectory = "./artifacts/upm",

    [ValidatePattern("\d+\.\d+\.\d+")]
    [string]$Version,

    [ValidatePattern("\d+")]
    [string]$BuildNumber,

    [ValidatePattern("[A-Za-z]+\.\d+[\.\d+]*")]
    [string]$PreviewTag,
    
    [string]$Revision,

    [string]$ReleasePackages = ""    
)
$releasePkgs = $ReleasePackages.Split(",")
$versionHash = @{}
$ProjectRoot = Resolve-Path -Path $ProjectRoot


if ($BuildNumber) {
    $BuildNumber = ".$BuildNumber"
}

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}

$OutputDirectory = Resolve-Path -Path $OutputDirectory
Write-Output "OutputDirectory: $OutputDirectory"

Write-Output "Release packages: $releasePkgs"

try {
           
    $repackTempDirectory = Join-Path $OutputDirectory "tmp"
    Write-Output "Temp Directory: $repackTempDirectory"
    if (-not (Test-Path $repackTempDirectory -PathType Container)) {
        New-Item $repackTempDirectory -ItemType Directory | Out-Null
    }
    
    Get-ChildItem -Path (Join-Path $PackageRoot "*.tgz") | ForEach-Object {
        New-Item -ItemType Directory -Force (Join-Path $repackTempDirectory $_.BaseName)
        tar -xzf $_ -C (Join-Path $repackTempDirectory $_.BaseName)
    }
    $packageSearchPath = "$repackTempDirectory\*\package\package.json"

    Get-ChildItem -Path $packageSearchPath | ForEach-Object {
        $packageName = Select-String -Pattern "com\.microsoft\.mrtk\.\w+" -Path $_.FullName | Select-Object -First 1

        if (-not $packageName) {
            return # this is not an MRTK package, so skip
        }

        $packageName = $packageName.Matches[0].Value
        $packageFriendlyName = (Select-String -Pattern "`"displayName`": `"(.+)`"" -Path $_ | Select-Object -First 1).Matches.Groups[1].Value

        $packagePath = $_.Directory

        Write-Output ""
        Write-Output "====================="
        Write-Output "Creating $packageName"
        Write-Output "====================="


        $inlineVersion = Select-String '"version" *: *"([0-9.]+)(-?[a-zA-Z0-9.]*)' -InputObject (Get-Content -Path $_)
        $Version = $inlineVersion.Matches.Groups[1].Value
        $suffix = $inlineVersion.Matches.Groups[2].Value

        
        if (!$releasePkgs.Contains( $packageName )) {
            $preview = "$PreviewTag."
            Write-Output "Version preview: $preview"
        } else {
            $preview = ""
        }

        $buildTag = "$preview$Revision"
        Write-Output "buildTag: $buildTag"


        $versionHash[$packageName]=$Version

        Write-Output " Version: $Version"
        Write-Output " suffix:  $suffix"

        Write-Output "Patching package version to $Version-$buildTag"
        ((Get-Content -Path $_ -Raw) -Replace '("version": )"(?:[0-9.]+|%version%)-?[a-zA-Z0-9.]*', "`$1`"$Version-$buildTag") | Set-Content -Path $_ -NoNewline

        Write-Output "Patching assembly version to $Version$BuildNumber"
        Get-ChildItem -Path $packagePath/AssemblyInfo.cs -Recurse | ForEach-Object {
            (Get-Content -Path $_ -Raw) -Replace '\[assembly:.AssemblyVersion\(.*', "[assembly: AssemblyVersion(`"$Version.0`")]`r" | Set-Content -Path $_ -NoNewline
            ((Get-Content -Path $_ -Raw) -Replace "(assembly: AssemblyFileVersion.)`"(?:[0-9.]+)", "`$1`"$Version$BuildNumber") | Set-Content -Path $_ -NoNewline
            ((Get-Content -Path $_ -Raw) -Replace "(assembly: AssemblyInformationalVersion.)`"(?:[0-9.]+)-?[a-zA-Z0-9.]*", "`$1`"$Version-$buildTag") | Set-Content -Path $_ -NoNewline
        }

        Write-Output "Packing $packageFriendlyName"
        npm pack $packagePath -pack-destination $OutputDirectory

    }
}
finally {
        Remove-Item -Force -Recurse $repackTempDirectory
}
