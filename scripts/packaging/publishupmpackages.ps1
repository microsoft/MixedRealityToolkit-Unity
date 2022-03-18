# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Publishes the Mixed Reality Toolkit Unity Package Manager (UPM) packacges.
.DESCRIPTION
    Publishes UPM packages for the Mixed Reality Toolkit.
.PARAMETER PackageDirectory
    Where should we find the packages to upload? Defaults to ".\artifacts\upm"
.PARAMETER RegistryPath
    To which registry should the packages be uploaded?
#>
param(
    [string]$PackageDirectory,
    [string]$RegistryPath
)

if (-not $PackageDirectory) {
    throw "Missing required parameter: -PackageDirectory."
}
$PackageDirectory = Resolve-Path -Path $PackageDirectory

if (-not $RegistryPath) {
    throw "Missing required parameter: -RegistryPath."
}

$startPath = "$(Get-Location)"

Write-Output "Publishing packages from: $PackageDirectory"

# Change to the project root directory
Set-Location $PackageDirectory

# Create the .npmrc file
$npmrcFileName = "./.npmrc"

$npmrcContents = "registry=$RegistryPath`n`nalways-auth=true"
Out-File -FilePath $npmrcFileName -InputObject $npmrcContents -Encoding utf8

# Authenticate to the registry
npm install -g vsts-npm-auth
vsts-npm-auth -config .npmrc

# Get the list of package (.tgz) files
$packages = Get-ChildItem -Name -Include "*.tgz"
foreach ($package in $packages)
{
    Write-Output "======================="
    Write-Output "Publishing: $package"
    Write-Output "======================="
    npm publish $package
}

Remove-Item -Path $npmrcFileName

# Return to the starting path
Set-Location $startPath
