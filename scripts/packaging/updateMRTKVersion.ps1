<#
.SYNOPSIS
    Updates Mixed Reality Toolkit version (i.e. major.minor.patch) across all known files.
#>
param(
    [Parameter(Position=0,mandatory=$true)]
    [string]$NewVersion
#     [ValidatePattern("^\d+\.\d+\.\d+$")]
#     [string]$Version,
)


function ReplaceVersionInFile($FullName, $NewVersion, $Patterns, $Strict=$False)
{
    $Errors = @()
    $contents = Get-Content -Path $FullName
    foreach ($pattern in $Patterns)
    {
        $match = [regex]::Match($contents, $pattern)
        if ($match.Success)
        {
            $FromVersion = $match.captures.groups[1].ToString()
            if ($FromVersion -eq $NewVersion)
            {
                Write-Host "${FullName}: version up to date ($NewVersion)"
            }
            else
            {
                Write-Host "${FullName}: updating version from $FromVersion to $NewVersion"
                $contents = $contents -replace $pattern, $NewVersion
                $contents | Out-File -FilePath $file.FullName -Encoding UTF8
            }
        }
        ElseIf ($Strict)
        {
            $errors += "$($file.FullName): pattern not found: $pattern"
        }
    }
    return $Errors
}

Push-Location  # save current working directory

Set-Location $PSScriptRoot
$GitRoot = (git rev-parse --show-toplevel) | Out-String
$GitRoot = ($GitRoot -replace "/", "\").Trim()

$FullProductName = "Microsoft Mixed Reality Toolkit"

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
        $Errors += ReplaceVersionInFile -FullName $file.FullName -NewVersion $NewVersion -Patterns @("(?<=Microsoft Mixed Reality Toolkit\s+)(\d+\.\d+\.\d+)") -Strict $True
    }
    ElseIf ($file.Directory.FullName.StartsWith($PipelinesDir.FullName))
    {
        if (($file.Extension -eq ".yml") -or ($file.Extension -eq ".yaml"))
        {
            $Errors += ReplaceVersionInFile -FullName $file.FullName -NewVersion $NewVersion -Patterns @("(?<=MRTKVersion:\s+)(\d+\.\d+\.\d+)")
        }
    }
    ElseIf ($file.Name -eq "ProjectSettings.asset")
    {
        $Errors += ReplaceVersionInFile -FullName $file.FullName -NewVersion $NewVersion -Patterns @("(?<=bundleVersion:\s+)(\d+\.\d+\.\d+)", "(?<=metroPackageVersion:\s+)(\d+\.\d+\.\d+)(?=\.\d+)")
    }
    ElseIf ($file.Name -eq "UwpAppxBuildToolsTest.cs")
    {
        $Errors += ReplaceVersionInFile -FullName $file.FullName -NewVersion $NewVersion -Patterns  @("(?<=\sVersion=')(\d+\.\d+\.\d+)(?=\.\d+\')") -Strict $True
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
