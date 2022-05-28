# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Updates Mixed Reality Toolkit version (i.e. major.minor.patch) across all known files.
#>
param(
    [Parameter(Position=0,mandatory=$true)]
    [string]$NewVersion
)


function GetEncoding($Path)
{
    [System.Byte[]]$Byte = Get-Content -AsByteStream -ReadCount 4 -TotalCount 4 -Path $Path
    if ($Byte[0] -eq 0xef -and $Byte[1] -eq 0xbb -and $Byte[2] -eq 0xbf)
    {
        return 'UTF8'
    }
    return 'ASCII'
}

function ReplaceVersionInFile($Path, $NewVersion, $Patterns, $Strict=$False)
{
    $Errors = @()
    $contents = Get-Content -Path $Path
    foreach ($pattern in $Patterns)
    {
        $match = [regex]::Match($contents, $pattern)
        if ($match.Success)
        {
            $FromVersion = $match.captures.groups[1].ToString()
            if ($FromVersion -eq $NewVersion)
            {
                Write-Host "${Path}: version up to date ($NewVersion)"
            }
            else
            {
                $Encoding = (GetEncoding $Path)
                Write-Host "${Path} ($Encoding): updating version from $FromVersion to $NewVersion"
                $contents = $contents -replace $pattern, $NewVersion
                $contents | Out-File -FilePath $Path -Encoding $Encoding
            }
        }
        ElseIf ($Strict)
        {
            $errors += "${Path}: pattern not found: $pattern"
        }
    }
    return $Errors
}

Push-Location  # save current working directory

Set-Location $PSScriptRoot
$GitRoot = (git rev-parse --show-toplevel) | Out-String
$GitRoot = ($GitRoot -replace "/", "\").Trim()

$Errors = @()

$PipelinesDir = Get-Item (Join-Path $GitRoot "pipelines")

foreach ($file in (Get-ChildItem -Path $GitRoot -Recurse))
{
    if ($file -is [System.IO.DirectoryInfo])
    {
        continue;
    }
    if ($file.Name -eq "version.txt")
    {
        $Errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns @("(?<=Microsoft Mixed Reality Toolkit\s+)(\d+\.\d+\.\d+)") -Strict $True
    }
    ElseIf ($file.Directory.FullName.StartsWith($PipelinesDir.FullName))
    {
        if (($file.Extension -eq ".yml") -or ($file.Extension -eq ".yaml"))
        {
            $Errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns @("(?<=MRTKVersion:\s+)(\d+\.\d+\.\d+)")
        }
    }
    ElseIf ($file.Name -eq "ProjectSettings.asset")
    {
        $Errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns @("(?<=bundleVersion:\s+)(\d+\.\d+\.\d+)", "(?<=metroPackageVersion:\s+)(\d+\.\d+\.\d+)(?=\.\d+)")
    }
    ElseIf ($file.Name -eq "UwpAppxBuildToolsTest.cs")
    {
        $Errors += ReplaceVersionInFile -Path $file.FullName -NewVersion $NewVersion -Patterns  @("(?<=\sVersion=')(\d+\.\d+\.\d+)(?=\.\d+\')") -Strict $True
    }
}

Pop-Location  # restore original working directory

if ($errors)
{
    Write-Host -ForegroundColor Red "Errors:"
    foreach ($line in $errors)
    {
        Write-Host  -ForegroundColor Red "    $line"
    }
    exit 1
}
else
{
    Write-Host
    Write-Host -ForegroundColor Green "Version updated to $NewVersion. Please review all changes before submitting."
    Write-Host
    exit 0
}
