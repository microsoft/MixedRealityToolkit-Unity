<#
.SYNOPSIS
    Publishes the Mixed Reality Toolkit Unity Package Manager (UPM) packacges.
.DESCRIPTION
    Publishes UPM packages for the Mixed Reality Toolkit.
.PARAMETER PackageDirectory
    Where should we find the packages to upload? Defaults to ".\artifacts"
.PARAMETER ProjectRoot
    The root folder of the project.
#>
param(
    [string]$PackageDirectory = ".\artifacts\upm",
    [string]$ProjectRoot
)

$startPath = "$(Get-Location)"

if (-not $ProjectRoot) {
    # ProjectRoot was not specified, presume the current location is Root\scripts\packaging
    $ProjectRoot = Resolve-Path "$startPath\..\.." 
}
$ProjectRoot = Resolve-Path -Path $ProjectRoot
Write-Output "Project root: $ProjectRoot"

$PackageDirectory = Resolve-Path -Path $PackageDirectory
Write-Output "Package directory: $PackageDirectory"

$npmCommand = "npm"

$cmdFullPath = "$env:systemroot\system32\cmd.exe"

# Change to the package directory
Set-Location $PackageDirectory

## todo - create the .npmrc file

# Get the list of package (.tgz) files
$packages = Get-ChildItem -Name -Include "*.tgz"
foreach ($package in $packages)
{
    Write-Output "======================="
    Write-Output "Publishing: $package"
    Write-Output "======================="
    Start-Process -FilePath $cmdFullPath -ArgumentList "/c $npmCommand publish $package" -NoNewWindow -Wait    
}

# Return to the starting path
Set-Location $startPath
