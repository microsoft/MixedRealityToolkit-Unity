<#
.SYNOPSIS
    Cleans up temporary files created during the UPM packaging of the MRTK/Exampes folder.
.DESCRIPTION
    Cleans up temporary files created during the UPM packaging of the MRTK/Exampes folder.
#>

$examplesRoot = "$(Get-Location)"
$samplesFolder = "$examplesRoot\Samples~"

if (Test-Path -Path $samplesFolder) {
    Remove-Item -Path $samplesFolder -Recurse -Force
}