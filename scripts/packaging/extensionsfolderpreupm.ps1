# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Prepares the MRTK\Extensions folder for UPM packaging.
.DESCRIPTION
    Prepares the MRTK\Extensions folder for UPM packaging.
.PARAMETER PackageRoot
    The root folder containing the extensions package contents. If not specified, the current folder is presumed.
#>
param(
    [string]$PackageRoot
)

if (-not $PackageRoot) {
    throw "Missing required parameter: -PackageRoot."
}

# This hashtable contains the folders containing the code and assets for the extensions
# that have example folders that need processing.
#
# While key captialization is not significant for finding the folders, it is used in the sample
# description that will be written to the package manifest.
# 
# These paths are PackageRoot relative.
$exampleFolders = [ordered]@{
    "HandPhysicsService" = "HandPhysicsService/Examples";
}

# Beginning of the upm packaging script main section
# The overall structure of this script is:
#
# 1) Create the required Samples~ folder
# 2) Recursively copy the contents of the example folders into Samples~
# 3) Prepare the samples entry for the package.json file
# 4) Replace the %samples% token with the prepared data
# 5) Overwrite the package.json file

# Ensure the required folder exists
$samplesFolder = "$PackageRoot\Samples~"
if (-not (Test-Path -Path $samplesFolder)) {
    New-Item $samplesFolder -ItemType Directory | Out-Null
}

# Copy each example folder
foreach ($entry in $exampleFolders.GetEnumerator()) {
    $sampleGroupName = $entry.Name
    $sampleFolder = $entry.Value
    Write-Output "Copying $PackageRoot/$sampleFolder to $samplesFolder/$sampleGroupName"
    Copy-Item -Path "$PackageRoot/$sampleFolder" -Destination "$samplesFolder/$sampleGroupName" -Recurse -Force
    Copy-Item -Path "$PackageRoot/$sampleFolder.meta"-Destination "$samplesFolder/$sampleGroupName.meta"
}

# Create the samples data for the package.json file
$sampleCount = 0
$samples = "`"samples`": ["
foreach ($entry in $exampleFolders.GetEnumerator()) {
    # Since we need to place appropriate separator characters between entries, we need to
    # keep track of how many folders we have processed
    $sampleCount++

    $folderName = $entry.Name

    $displayName = "$folderName"
    $description = "MRTK Examples: $folderName"
    $path = "Samples~/$folderName"

    $samples = $samples + "`n      {`n"
    $samples = $samples + "        `"displayName`": `"" + $displayName + "`",`n"
    $samples = $samples + "        `"description`": `"" + $description + "`",`n"
    $samples = $samples + "        `"path`": `"" + $path + "`"`n"
    $samples = $samples + "      }"
    if (-not ($sampleCount -eq $exampleFolders.keys.count)) {
            $samples = $samples + ","
    }
}
$samples = $samples + "`n   ]"

# Update the project.json file to specify the samples contanined in the package
$packageJsonPath = "$PackageRoot/package.json"
$packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
$packageJson = ($packageJson -replace "%samples%", $samples)
[System.IO.File]::WriteAllText($packageJsonPath, $packageJson)
