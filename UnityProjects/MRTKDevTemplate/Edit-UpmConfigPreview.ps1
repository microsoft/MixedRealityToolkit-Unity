# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Generates a .upmconfig.toml file to use with private preview NPM feeds in Unity's Package Manager.
.DESCRIPTION
    Generates a .upmconfig.toml file to use with private preview NPM feeds in Unity's Package Manager. This file is automatically written to $env:USERPROFILE/.upmconfig.toml.
.PARAMETER PAT
    The PAT generated from the feed host (like https://mixedrealitycloud.visualstudio.com/_usersSettings/tokens).
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$PAT,

    [ValidateSet('MRTK')]
    [string]$Instance = 'MRTK'
)

$base64 = [Convert]::ToBase64String([System.Text.Encoding]::UTF8.GetBytes("user:$($PAT.Trim())"))

if ($Instance -eq 'MRTK') {
    $url = "https://mixedrealitycloud.pkgs.visualstudio.com/MRTK/_packaging/MRTK/npm/registry"
}

$upmconfig = "[npmAuth.`"$url`"]
_auth = `"$base64`"
alwaysAuth = true"

$path = Join-Path $env:USERPROFILE ".upmconfig.toml"

if (-not (Test-Path $path)) {
    New-Item -Path $path
}
else {
    $upmconfig = "`n$upmconfig"
}

if ((Select-String -Path $path -Pattern $url -SimpleMatch -Quiet) -or (Select-String -Path $path -Pattern "$url/" -SimpleMatch -Quiet)) {
    $regex = "(\[npmAuth\.`"$url/?`"\]\s*_auth\s*=\s*)`"(.*)`""
    ((Get-Content -Path $path -Raw) -Replace $regex, "`$1`"$base64`"") | Set-Content -Path $path -NoNewline
    Write-Host "Patched PAT in $path"
}
else {
    Add-Content -Path $path -Value $upmconfig
    Write-Host "Embedded PAT in $path"
}
