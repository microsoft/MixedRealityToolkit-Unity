<#
.SYNOPSIS
    Cleans up temporary files created during the UPM packaging of the MRTK/Exampes folder.
.DESCRIPTION
    Cleans up temporary files created during the UPM packaging of the MRTK/Exampes folder.
#>

$projectRoot = Resolve-Path "$(Get-Location)\..\.." 
$examplesRoot = "$projectRoot\Assets\MRTK\Examples"

$samplesFolder = "$examplesRoot\Samples~"
if (Test-Path -Path $samplesFolder) {
    Remove-Item -Path $samplesFolder -Recurse -Force
}