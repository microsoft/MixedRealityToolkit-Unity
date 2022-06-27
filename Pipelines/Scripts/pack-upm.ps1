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
.PARAMETER BuildNumber
    The fourth digit (revision) for the full version.
.PARAMETER PreviewTag
    The tag to append after the version, including the preview number (e.g. "internal.0" or "pre.100")
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$ProjectRoot,

    [string]$OutputDirectory = "./artifacts/upm",

    [ValidatePattern("\d+\.\d+\.\d+")]
    [string]$Version,

    [ValidatePattern("\d+")]
    [string]$BuildNumber,

    [ValidatePattern("[A-Za-z]+\.\d+[\.\d+]*")]
    [string]$PreviewTag
)

$ProjectRoot = Resolve-Path -Path $ProjectRoot

if ($PreviewTag) {
    $PreviewTag = "-$PreviewTag"
    Write-Output "Version preview: $PreviewTag"
}

if ($BuildNumber) {
    $BuildNumber = ".$BuildNumber"
}

if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}

$OutputDirectory = Resolve-Path -Path $OutputDirectory
Write-Output "OutputDirectory: $OutputDirectory"

try {
    Push-Location $OutputDirectory

    $parseVersion = -not $Version

    Get-ChildItem -Path $ProjectRoot/*/package.json | ForEach-Object {
        $packageName = Select-String -Pattern "com\.microsoft\.mrtk\.\w+" -Path $_ | Select-Object -First 1

        if (-not $packageName) {
            return # this is not an MRTK package, so skip
        }

        $packageName = $packageName.Matches[0].Value
        $packageFriendlyName = (Select-String -Pattern "`"displayName`": `"(.+)`"" -Path $_ | Select-Object -First 1).Matches.Groups[1].Value

        $packagePath = $_.Directory
        $docFolder = "$packagePath/Documentation~"

        Write-Output ""
        Write-Output "====================="
        Write-Output "Creating $packageName"
        Write-Output "====================="

        Write-Output "Copying Documentation~ to $packageFriendlyName"

        if (Test-Path -Path $docFolder) {
            Copy-Item -Path "$ProjectRoot/Pipelines/UPM/Documentation~/*" -Destination $docFolder -Recurse
        }
        else {
            Copy-Item -Path "$ProjectRoot/Pipelines/UPM/Documentation~" -Destination $docFolder -Recurse
        }

        if ($parseVersion) {
            $inlineVersion = Select-String '"version" *: *"([0-9.]+)-?[a-zA-Z0-9.]*' -InputObject (Get-Content -Path $_)
            $Version = $inlineVersion.Matches.Groups[1].Value
        }

        Write-Output "Patching package version to $Version$PreviewTag"
        ((Get-Content -Path $_ -Raw) -Replace '("version": |"com\.microsoft\.mrtk\.\w+" *: *)"(?:[0-9.]+|%version%)-?[a-zA-Z0-9.]*', "`$1`"$Version$PreviewTag") | Set-Content -Path $_ -NoNewline

        Write-Output "Patching assembly version to $Version$BuildNumber"
        Get-ChildItem -Path $packagePath/AssemblyInfo.cs -Recurse | ForEach-Object {
            Add-Content -Path $_ -Value "[assembly: AssemblyFileVersion(`"$Version$BuildNumber`")]"
            Add-Content -Path $_ -Value "[assembly: AssemblyInformationalVersion(`"$Version$PreviewTag`")]"
        }

        Write-Output "Packing $packageFriendlyName"
        npm pack $packagePath

        if (Test-Path -Path $docFolder) {
            Write-Output "Cleaning up Documentation~ from $packageFriendlyName"
            # A documentation folder was created. Remove it.
            Remove-Item -Path $docFolder -Recurse -Force
            # But restore anything that's checked-in.
            if (git ls-files $docFolder) {
                git -C $packagePath checkout $docFolder
            }
        }

        git -C $packagePath checkout $_
        Get-ChildItem -Path $packagePath/AssemblyInfo.cs -Recurse | ForEach-Object {
            git -C $packagePath checkout $_
        }
    }
}
finally {
    Pop-Location
}
