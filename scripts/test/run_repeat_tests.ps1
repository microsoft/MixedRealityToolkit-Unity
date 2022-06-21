# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
 # Runs the playmode tests in batch mode, repeatedly.
 #>
[CmdletBinding()]
param (
    # Path to your Unity project
    [Parameter(Position = 0)]
    [ValidateScript({ Test-Path $_ -PathType Container })]
    [string]
    $projectPath = "$PSScriptRoot/../../",
    # Folder that will contain test results output and logs
    [string]
    [ValidateScript({ Test-Path $_ -PathType Container })]
    $outFolder = "$PSScriptRoot/out/",
    # Path to your Unity Executable
    [ValidateScript({ [System.IO.File]::Exists($_) -and $_.EndsWith(".exe") })]
    [string]
    $unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.26f1\Editor\Unity.exe",
    # Optional test filter
    [Parameter(Mandatory = $false)]
    [string]
    $editorTestsFilter = "",
    # Number of times to repeat runs
    [Parameter(Mandatory = $true)]
    [int]
    $Times
)

$passThruParams = $PsBoundParameters
$passThruParams.Remove("Times")
for ($i = 0; $i -lt $Times; $i++) {
    $user_i = $i + 1
    Write-Host -ForegroundColor Cyan "Run $user_i of $Times"
    Invoke-Expression $PSScriptRoot\run_playmode_tests.ps1 @passThruParams
}