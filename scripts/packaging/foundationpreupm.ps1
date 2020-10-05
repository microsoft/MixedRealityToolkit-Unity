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
Copy-Item -Path "$PackageRoot/SceneSystemResources" -Destination "$samplesFolder/SceneSystemResources" -Recurse -Force

# Create the samples data for the package.json file
$samples = "`"samples`": ["
$samples = $samples + "`n      {`n"
$samples = $samples + "        `"displayName`": `"Scene System Resources`",`n"
$samples = $samples + "        `"description`": `"Resources used by the MRTK Scene System`",`n"
$samples = $samples + "        `"path`": `"Samples~/SceneSystemResources`"`n"
$samples = $samples + "      }"
$samples = $samples + "`n   ]"

# Update the project.json file to specify the samples contanined in the package
$packageJsonPath = "$PackageRoot/package.json"
$packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
$packageJson = ($packageJson -replace "%samples%", $samples)
[System.IO.File]::WriteAllText($packageJsonPath, $packageJson)
