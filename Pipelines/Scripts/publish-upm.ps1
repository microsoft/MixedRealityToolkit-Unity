# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Publishes the Mixed Reality Toolkit Unity Package Manager (UPM) packages.
.DESCRIPTION
    Publishes UPM packages for the Mixed Reality Toolkit.
.PARAMETER PackageDirectory
    Where should we find the packages to upload? Defaults to "./artifacts/upm"
.PARAMETER RegistryPath
    To which registry should the packages be uploaded? Don't include this if the .npmrc is handled externally.
.PARAMETER Authenticate
    If vsts-npm-auth needs to be run to authenticate against the feed. Don't include this if authentication is handled externally.
#>
param(
    [Parameter(Mandatory = $true)]
    [string]$PackageDirectory = "./artifacts/upm",
    [string]$RegistryPath,
    [switch]$Authenticate
)

$PackageDirectory = Resolve-Path -Path $PackageDirectory

Write-Output "Publishing packages from: $PackageDirectory"

$npmrcPath = ".npmrc"

try {
    if ($RegistryPath) {
        Out-File -FilePath $npmrcPath -InputObject "registry=$RegistryPath`nalways-auth=true" -Encoding utf8
        Write-Output "Writing file to $(Resolve-Path -Path $npmrcPath)..."
    }

    if ($Authenticate.IsPresent) {
        npm install -g vsts-npm-auth --registry https://registry.npmjs.com --always-auth false
        vsts-npm-auth -config $npmrcPath
    }

    # Get the list of package (.tgz) files
    Get-ChildItem -Path $PackageDirectory -Filter "com.microsoft.mrtk.*.tgz" | ForEach-Object {
        Write-Output "======================="
        Write-Output "Publishing: $($_.Name) "
        Write-Output "======================="
        npm publish $_
    }
}
finally {
    if ($RegistryPath) {
        Write-Output "Deleting file from $(Resolve-Path -Path $npmrcPath)..."
        Remove-Item -Path $npmrcPath
    }
}
