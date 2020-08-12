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
    [string]$PackageDirectory = ".\artifacts\upm",
    [bool]$IsPublicRelease = $False
)

$startPath = "$(Get-Location)"
Write-Output $PackageDirectory
# $PackageDirectory = Resolve-Path -Path $PackageDirectory

Write-Output "Publishing packages from: $PackageDirectory"
Write-Output "Public release: $IsPublicRelease"
[bool] $envSet = $False
if ($env:TESTREGISTRY) {
    $envSet = $True
}
Write-Output "Environment set: $envSet"

# # Change to the project root directory
# Set-Location $PackageDirectory

# # Create the .npmrc file
# $registryPath = "https://pkgs.dev.azure.com/aipmr/MixedReality-Unity-Packages/_packaging/Unity-packages/npm/registry/"
# if (-not $IsPublicRelease) {
#     $registryPath = "$env:TESTREGISTRY"
# }
# Write-Output $registryPath
# $npmrcContents = "registry=$registryPath`n`nalways-auth=true"

# Out-File -FilePath "./.npmrc" -InputObject $npmrcContents -Encoding utf8

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

# Remove-Item -Path "./.npmrc"

# Return to the starting path
Set-Location $startPath
