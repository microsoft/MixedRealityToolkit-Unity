# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Prepares the MRTK foundation for UPM packaging.
.DESCRIPTION
    Prepares the MRTK foundation for UPM packaging.
.PARAMETER PackageRoot
    The root folder containing the foundation package contents. If not specified, the current folder is presumed.
#>
param(
    [string]$PackageRoot
)

if (-not $PackageRoot) {
    throw "Missing required parameter: -PackageRoot."
}

# Ensure the required folder exists
$samplesFolder = "$PackageRoot\Samples~"
if (-not (Test-Path -Path $samplesFolder)) {
    New-Item $samplesFolder -ItemType Directory | Out-Null
}

# Copy the scene system's default scenes to Samples~
Move-Item -Path "$PackageRoot/Services/SceneSystem/SceneSystemResources" -Destination "$samplesFolder/SceneSystemResources" -Force
Move-Item -Path "$PackageRoot/Services/SceneSystem/SceneSystemResources.meta" -Destination "$samplesFolder/SceneSystemResources.meta" -Force
