<#
.SYNOPSIS
    todo
.DESCRIPTION
    todo
#>

$projectRoot = Resolve-Path "$(Get-Location)\..\.." 
$examplesRoot = "$projectRoot\Assets\MRTK\Examples"

# todo
#
$exampleFolders = @{
    "Demos" = "Demos";
    "Experimental" = "Experimental";
    "Editor" = "Editor";
}

# Ensure the required folder exists
$samplesFolder = "$examplesRoot\Samples~"
if (-not (Test-Path -Path $samplesFolder)) {
    New-Item $samplesFolder -ItemType Directory | Out-Null
}

# Copy each example folder
foreach ($entry in $exampleFolders.GetEnumerator()) {
    $sampleGroupName = $entry.Name
    Write-Output "Copying $examplesRoot\$sampleGroupName to $samplesFolder\$sampleGroupName"
    Copy-Item -Path "$examplesRoot\$sampleGroupName" -Destination "$samplesFolder\$sampleGroupName" -Recurse
    Copy-Item -Path "$examplesRoot\$sampleGroupName.meta"-Destination "$samplesFolder\$sampleGroupName.meta"
}

# Update the project.json file to specify the samples contanined in the package
# todo

