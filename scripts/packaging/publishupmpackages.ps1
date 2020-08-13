<#
.SYNOPSIS
    Publishes the Mixed Reality Toolkit Unity Package Manager (UPM) packacges.
.DESCRIPTION
    Publishes UPM packages for the Mixed Reality Toolkit.
.PARAMETER PackageDirectory
    Where should we find the packages to upload? Defaults to ".\artifacts\upm"
.PARAMETER IsPublicRelease
    Is this a public release? If not, the packages will be published to an internal server for testing.
    The default value for this parameter is 0 (false).
#>
param(
    [string]$PackageDirectory,
    [bool]$IsPublicRelease = $False
)

if (-not $PackageDirectory) {
    throw "Missing required parameter: -PackageDirectory."
}
$PackageDirectory = Resolve-Path -Path $PackageDirectory

$startPath = "$(Get-Location)"

Write-Output "Publishing packages from: $PackageDirectory"
Write-Output "Public release: $IsPublicRelease"

# # Change to the project root directory
# Set-Location $PackageDirectory

# Create the .npmrc file
$npmrcFileName = "./.npmrc"
$registryPath = "https://pkgs.dev.azure.com/aipmr/MixedReality-Unity-Packages/_packaging/Unity-packages/npm/registry/"
if (-not $IsPublicRelease) {
    $registryPath = $env:TESTREGISTRY
}
$npmrcContents = "registry=$registryPath`n`nalways-auth=true"
Out-File -FilePath $npmrcFileName -InputObject $npmrcContents -Encoding utf8

[string]$temp = Get-Content -FilePath $npmrcFileName
Write-Output $temp

# # Authenticate to the registry
# vsts-npm-auth -config .npmrc

# # Get the list of package (.tgz) files
# $packages = Get-ChildItem -Name -Include "*.tgz"
# foreach ($package in $packages)
# {
#     Write-Output "======================="
#     Write-Output "Publishing: $package"
#     Write-Output "======================="
#     npm publish $package    
# }

Remove-Item -Path $npmrcFileName

# Return to the starting path
Set-Location $startPath
