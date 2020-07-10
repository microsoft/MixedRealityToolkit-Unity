<#
.SYNOPSIS
    Cleans up temporary files created during the UPM packaging of the MRTK/Examples folder.
.DESCRIPTION
    Cleans up temporary files created during the UPM packaging of the MRTK/Examples folder.
#>

$examplesRoot = "$(Get-Location)"
$samplesFolder = "$examplesRoot\Samples~"

if (Test-Path -Path $samplesFolder) {
    Remove-Item -Path $samplesFolder -Recurse -Force
}
