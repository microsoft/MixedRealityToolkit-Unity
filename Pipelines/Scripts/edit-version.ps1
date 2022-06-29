# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Updates Mixed Reality Toolkit version (i.e. major.minor.patch) and preview (i.e. pre.#) across all known files.
#>
param(
    [Parameter()]
    [string]$NewVersion,

    [Parameter()]
    [string]$NewPreview
)

function GetEncoding($Path) {
    [System.Byte[]]$byte = Get-Content -AsByteStream -ReadCount 4 -TotalCount 4 -Path $Path
    if ($byte[0] -eq 0xef -and $byte[1] -eq 0xbb -and $byte[2] -eq 0xbf) {
        return 'UTF8'
    }
    return 'ASCII'
}

function ReplaceVersionInFile($Path, $NewVersion, $Patterns, $Strict = $False) {
    $errors = @()
    $contents = Get-Content -Path $Path
    foreach ($pattern in $Patterns) {
        $match = [regex]::Match($contents, $pattern)
        if ($match.Success) {
            $FromVersion = $match.captures.groups[1].ToString()
            if ($FromVersion -eq $NewVersion) {
                Write-Host "${Path}: version up to date ($NewVersion)"
            }
            else {
                $Encoding = (GetEncoding $Path)
                Write-Host "${Path} ($Encoding): updating version from $FromVersion to $NewVersion"
                $contents = $contents -replace $pattern, $NewVersion
                $contents | Out-File -FilePath $Path -Encoding $Encoding
            }
        }
        elseif ($Strict) {
            $errors += "${Path}: pattern not found: $pattern"
        }
    }
    return $errors
}

$errors = @()

$gitRoot = ((git -C $PSScriptRoot rev-parse --show-toplevel) | Out-String).Trim()
$pipelinesDir = Get-Item (Join-Path $gitRoot "Pipelines")

foreach ($file in (Get-ChildItem -Path $gitRoot -Recurse)) {
    if ($file -is [System.IO.DirectoryInfo]) {
        continue;
    }

    if ($NewVersion) {
        if ($file.Name -eq "version.txt") {
            $errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns @("(?<=Microsoft Mixed Reality Toolkit\s+)(\d+\.\d+\.\d+)") -Strict $True
        }
        elseif ($file.Directory.FullName.StartsWith($PipelinesDir.FullName)) {
            if (($file.Extension -eq ".yml") -or ($file.Extension -eq ".yaml")) {
                $errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns @("(?<=MRTKVersion:\s+)(\d+\.\d+\.\d+)")
            }
        }
        elseif ($file.Name -eq "ProjectSettings.asset") {
            $errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns @("(?<=bundleVersion:\s+)(\d+\.\d+\.\d+)", "(?<=metroPackageVersion:\s+)(\d+\.\d+\.\d+)(?=\.\d+)")
        }
    }

    if ($NewPreview) {
        if ($file.Directory.FullName.StartsWith($PipelinesDir.FullName)) {
            if (($file.Extension -eq ".yml") -or ($file.Extension -eq ".yaml")) {
                $errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewPreview -Patterns @("(?<=MRTKReleaseTag:\s+')([\w.]+)")
            }
        }
        elseif ($file.Name -eq "ProjectSettings.asset") {
            $errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewPreview -Patterns @("(?<=bundleVersion:\s+\d+\.\d+\.\d+-)(\w+.\d+)")
        }
    }
}

if ($errors) {
    Write-Host -ForegroundColor Red "Errors:"
    foreach ($line in $errors) {
        Write-Host  -ForegroundColor Red "    $line"
    }
    exit 1
}
else {
    Write-Host
    if ($NewVersion) {
        Write-Host -ForegroundColor Green "Version updated to $NewVersion. Please review all changes before submitting."
    }
    if ($NewPreview) {
        Write-Host -ForegroundColor Green "Preview updated to $NewPreview. Please review all changes before submitting."
    }
    Write-Host
    exit 0
}
