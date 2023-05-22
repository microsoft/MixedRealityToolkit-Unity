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

    [string]$OutputDirectory = "./artifacts/upm/release" 
)

$versionHash = @{}
$PackageRoot = Resolve-Path -Path $PackageRoot

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
    Write-Output "PackageSearchPath: $packageSearchPath"

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

        $inlineVersion = Select-String '^.*"version":.*"(?<sem>[0-9]\.[0-9]\.[0-9])-(?<prerelease>prerelease\.)*(?<tag>pre\.\d*)*\.*(?<build>\d{6}\.\d*)' -InputObject (Get-Content -Path $_)
        $version = $inlineVersion.Matches[0].Groups['sem'].Value
        $prerelease = $inlineVersion.Matches[0].Groups['prerelease'].Value
        $tag = $inlineVersion.Matches[0].Groups['tag'].Value
        $build = $inlineVersion.Matches[0].Groups['build'].Value

        if ($tag) {
            $tag = "-$tag"
        }

        Write-Output "Version: $version   prerelease: $prerelease   tag: $tag   build: $build"
        
        $versionHash[$packageName]=$version

        Write-Output " Version: $version"
        
        Write-Output "Patching package version to $version$tag"
        ((Get-Content -Path $_ -Raw) -Replace '("version": )"(?:[0-9.]+|%version%)-?[a-zA-Z0-9.]*', "`$1`"$version$tag") | Set-Content -Path $_ -NoNewline

        Write-Output "Patching assembly version to $version$tag"
        Get-ChildItem -Path $packagePath/AssemblyInfo.cs -Recurse | ForEach-Object {
            (Get-Content -Path $_ -Raw) -Replace '\[assembly:.AssemblyVersion\(.*', "[assembly: AssemblyVersion(`"$version.0`")]`r" | Set-Content -Path $_ -NoNewline
            ((Get-Content -Path $_ -Raw) -Replace "assembly: AssemblyFileVersion\`(\`".*\`"", "assembly: AssemblyFileVersion(`"$version$tag`"") | Set-Content -Path $_ -NoNewline
            ((Get-Content -Path $_ -Raw) -Replace "assembly: AssemblyInformationalVersion\`(\`".*\`"", "assembly: AssemblyInformationalVersion(`"$version$tag`"") | Set-Content -Path $_ -NoNewline
        }

        Write-Output "Packing $packageFriendlyName to $OutputDirectory"
        npm pack $packagePath -pack-destination $OutputDirectory

    }
}
finally {
    Write-Output "Removing temp directory $repackTempDirectory"
    Remove-Item -Force -Recurse $repackTempDirectory
}
