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

Push-Location  # save current working directory

Set-Location $PSScriptRoot
$GitRoot = (git rev-parse --show-toplevel) | Out-String
$GitRoot = ($GitRoot -replace "/", "\").Trim()

$FullProductName = "Microsoft Mixed Reality Toolkit"

$Errors = @()

foreach ($file in (Get-ChildItem -Path $GitRoot -Recurse))
{
    if ($file.Name -eq "version.txt")
    {
        $contents = Get-Content -Path $file.FullName
        $pattern = "(?<=Microsoft Mixed Reality Toolkit\s+)(\d+\.\d+\.\d+)"
        $match = [regex]::Match($contents, $pattern)
        if ($match.Success)
        {
            $FromVersion = $match.captures.groups[1].ToString()
            if ($FromVersion -eq $NewVersion)
            {
                Write-Host "$($file.FullName): version up to date ($NewVersion)"
            }
            else
            {
                Write-Host "$($file.FullName): updating version from $FromVersion to $NewVersion"
                $contents = $contents -replace $pattern, $NewVersion
                $contents | Out-File -FilePath $file.FullName -Encoding UTF8
            }
        }
        else
        {
            $errors += "$($file.FullName): pattern not found: $pattern"
        }
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
