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
.PARAMETER Revision
    The revision number for the build.  (e.g. "230508.3" or "2") 
.PARAMETER ReleasePackages
    An array of the package names that are approved for release.  If the package isn't in this array, it will get labeled -Pre

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
    Push-Location $OutputDirectory
    $parseVersion = -not $Version


    # loop through package directories, update package version, assembly version, and build version hash for updating dependencies
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
            $inlineVersion = Select-String '"version" *: *"([0-9.]+)(-?[a-zA-Z0-9.]*)' -InputObject (Get-Content -Path $_)
            $Version = $inlineVersion.Matches.Groups[1].Value
            $suffix = $inlineVersion.Matches.Groups[2].Value

        }
        
        if (!$releasePkgs.Contains( $packageName )) {
            $preview = "$PreviewTag."
            Write-Output "Version preview: $preview"
        } else {
            $preview = ""
        }

        $buildTag = "$preview$Revision"
        Write-Output "buildTag: $buildTag"


        $versionHash[$packageName]="$Version-$buildTag"

        Write-Output " Version: $Version"
        Write-Output " suffix:  $suffix"

        Write-Output "Patching package version to $Version-$buildTag"
        ((Get-Content -Path $_ -Raw) -Replace '("version": )"(?:[0-9.]+|%version%)-?[a-zA-Z0-9.]*', "`$1`"$Version-$buildTag") | Set-Content -Path $_ -NoNewline

        Write-Output "Patching assembly version to $Version$BuildNumber"
        Get-ChildItem -Path $packagePath/AssemblyInfo.cs -Recurse | ForEach-Object {
            (Get-Content -Path $_ -Raw) -Replace '\[assembly:.AssemblyVersion\(.*', "[assembly: AssemblyVersion(`"$Version.0`")]`r" | Set-Content -Path $_ -NoNewline
            Add-Content -Path $_ -Value "[assembly: AssemblyFileVersion(`"$Version$BuildNumber`")]"
            Add-Content -Path $_ -Value "[assembly: AssemblyInformationalVersion(`"$Version-$buildTag`")]"
        }


    }

    # update dependencies using the versionHash map
    Get-ChildItem -Path $ProjectRoot/*/package.json | ForEach-Object {
        $currentPackageName = Select-String -Pattern "com\.microsoft\.mrtk\.\w+" -Path $_ | Select-Object -First 1
        if (-not $currentPackageName) {
            return # this is not an MRTK package, so skip
        }

        $currentPackageName = $currentPackageName.Matches[0].Value
        $packageFriendlyName = (Select-String -Pattern "`"displayName`": `"(.+)`"" -Path $_ | Select-Object -First 1).Matches.Groups[1].Value

        $packagePath = $_.Directory
        $docFolder = "$packagePath/Documentation~"

        
        Write-Output "____________________________________________________________________________"
        Write-Output "   Package: $($currentPackageName)"

        foreach ($packageName in $versionHash.Keys) {
            if ($currentPackageName -eq $packageName) {
                continue
            }

            $searchRegex = "$($packageName).*:.*""(.*)"""
            $searchMatches = Select-String $searchRegex -InputObject (Get-Content -Path $_)
            if ($searchMatches.Matches.Groups) {
                $newVersion = $versionHash["$($packageName)"]
                Write-Output "        Patching dependency $($packageName) from $($searchMatches.Matches.Groups[1].Value) to $($newVersion)"
                (Get-Content -Path $_ -Raw) -Replace $searchRegex, "$($packageName)"": ""$($newVersion)""" | Set-Content -Path $_ -NoNewline
            }
        }

        Write-Output "____________________________________________________________________________`n"

        # build the package
        Write-Output "Packing $packageFriendlyName"
        npm pack $packagePath


        # clean up
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
