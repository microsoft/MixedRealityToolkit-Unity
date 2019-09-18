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


function ReplaceVersionInFile($FullName, $NewVersion, $Patterns)
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
        else
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

foreach ($file in (Get-ChildItem -Path $GitRoot -Recurse))
{
    if ($file.Name -eq "version.txt")
    {
        $Errors += ReplaceVersionInFile -FullName $file.FullName -NewVersion $NewVersion -Patterns @("(?<=Microsoft Mixed Reality Toolkit\s+)(\d+\.\d+\.\d+)")
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
