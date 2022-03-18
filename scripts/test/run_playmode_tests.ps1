# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
 # Runs the playmode tests in batch mode. This is the same technique as 
 # used by the MRTK PR validation when you do "/azp run mrtk_pr" on github.
 #>
param(
    # Path to your Unity project
    [Parameter(Position=0)]
    [ValidateScript({Test-Path $_ -PathType Container})]
    [string]
    $projectPath = "$PSScriptRoot/../../",
    # Folder that will contain test results output and logs
    [string]
    [ValidateScript({Test-Path $_ -PathType Container})]
    $outFolder = "$PSScriptRoot/out/",
    # Path to your Unity Executable
    [ValidateScript({[System.IO.File]::Exists($_) -and $_.EndsWith(".exe") })]
    [string]
    $unityExePath = "C:\Program Files\Unity\Hub\Editor\2018.4.26f1\Editor\Unity.exe",
    # Optional test filter
    [Parameter(Mandatory=$false)]
    [string]
    $editorTestsFilter = ""
)
$dateStr = Get-Date -format "yyyy_MM_dd-HHmmss"
if (-not (Test-Path $outFolder))
{
    New-Item -ItemType Directory $outFolder
}
$logPath = "$outFolder\playmode_tests_log-$dateStr.log"
$testResultPath = "$outFolder\playmode_tests_result-$dateStr.xml"

$timer = [System.Diagnostics.Stopwatch]::StartNew()
Write-Host "Starting test run"
Write-Host "Writing test output to $logPath...`n"

$unityArgs = @(
    "-runTests",
    "-testPlatform playmode"
    "-batchmode",
    "-editorTestsResultFile $testResultPath",
    "-logFile $logPath",
    "-projectPath $projectPath",
    "-editorTestsFilter $editorTestsFilter"
    )
Write-Host "Running command:"
Write-Host $unityExePath ($unityArgs -Join " ")
$handle = Start-Process -FilePath $unityExePath -PassThru -ArgumentList $unityArgs

Start-Process powershell -ArgumentList @(
    "-command", 
    "Get-Content $logPath -Wait")

Write-Host "`nOpening new window to view test output..."
Write-Host "Results will be printed here when the test completes"
while (-not $handle.HasExited)
{
    Start-Sleep 3
}

Write-Host "`nTest completed! Results written to $testResultPath"
Write-Host -ForegroundColor Cyan "`nTest results:" 
Write-Host -ForegroundColor Cyan "Tests took: $($timer.Elapsed)"

[xml]$cn = Get-Content $testResultPath
$cnx = $cn["test-run"]
Write-Host -ForegroundColor Cyan "passed: $($cnx.passed) failed: $($cnx.failed)"
if ($cnx.failed -gt 0)
{
    Write-Host ""
    Write-Host -ForegroundColor Cyan "Failed tests:"
    $testcases = $cnx.GetElementsByTagName("test-case")
    foreach ($item in $testcases) {
        if($item.result -ne "Passed")
        {
            Write-Host "$($item.classname)::$($item.name)"
        }
    }
}
