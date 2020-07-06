<#
.SYNOPSIS
    Prepares the MRTK\Examples folder for UPM packaging.
.DESCRIPTION
    Prepares the MRTK\Examples folder for UPM packaging.
#>

$examplesRoot = "$(Get-Location)"

# This hashtable contains mappings of the sample categories to the folder which contains
# the code and assets.
#
# Note that capitalization below in the key itself is significant.
#
# These paths are examplesRoot relative.
$exampleFolders = [ordered]@{
    "Demos" = "Demos";
    "Experimental" = "Experimental";
    "Editor" = "Editor";
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
$samplesFolder = "$examplesRoot\Samples~"
if (-not (Test-Path -Path $samplesFolder)) {
    New-Item $samplesFolder -ItemType Directory | Out-Null
}

# Copy each example folder
foreach ($entry in $exampleFolders.GetEnumerator()) {
    $sampleGroupName = $entry.Name
    Write-Output "Copying $examplesRoot\$sampleGroupName to $samplesFolder\$sampleGroupName"
    Copy-Item -Path "$examplesRoot\$sampleGroupName" -Destination "$samplesFolder\$sampleGroupName" -Recurse -Force
    Copy-Item -Path "$examplesRoot\$sampleGroupName.meta"-Destination "$samplesFolder\$sampleGroupName.meta"
}

# Create the samples data for the project.json file
$sampleCount = $exampleFolders.keys.count
$samples = "`"samples`": ["
$i = 0
foreach ($entry in $exampleFolders.GetEnumerator()) {
    # Since we need to place appropriate separator characters between entries, we need to
    # keep track of how many items we have written.
    $i++

    $samples = $samples + "`n      {`n"
    $samples = $samples + "        `"displayName`": `"" + $entry.Name + "`"," + "`n"
    $samples = $samples + "        `"description`": `"MRTK Examples: " + $entry.Name + "`"," + "`n"
    $samples = $samples + "        `"path`": `"Samples~/" + $entry.Name + "`"" + "`n"
    $samples = $samples + "      }"
    if (-not ($i -eq $sampleCount)) {
        $samples = $samples + ","
    }
}
$samples = $samples + "`n   ]"

# Update the project.json file to specify the samples contanined in the package
$packageJsonPath = "$examplesRoot\package.json"
$packageJson = [System.IO.File]::ReadAllText($packageJsonPath)
$packageJson = ($packageJson -replace "%samples%", $samples)
[System.IO.File]::WriteAllText($packageJsonPath, $packageJson)
