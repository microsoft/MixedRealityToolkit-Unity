# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Validates a build log to check for the presence of non-build related warnings
    that may negatively affect the quality of the build.
.DESCRIPTION
    This script intends to prevent repeats of past issues that have come up
    (for example, reused GUIDs) which don't get caught in the build itself
    because they are not build errors.

    Returns 0 if there are no errors, non-zero if there are.
.EXAMPLE
    .\validatebuildlog.ps1 -LogFile c:\path\to\log.txt
#>
param(
    # The log file to validate
    [string]$LogFile
)

<#
.SYNOPSIS
    Checks if the given log file has a conflict guid at the specified line number.
    Outputs to console if such an error exists.
    Returns true if there is a duplicate guid.
#>
function CheckDuplicateGuids {
    [CmdletBinding()]
    param(
        [string[]]$LogFileContent,
        [int]$LineNumber
    )
    process {
        if ($LogFileContent[$LineNumber] -match "GUID \[[a-g0-9]{32}?\] for asset '.*' conflicts with") {
            for ($i = $LineNumber; $i -lt $LogFileContent.Length; $i++) {
                if ($LogFileContent[$i] -eq "Assigning a new guid.") {
                    Write-Host "Found duplicated GUID, Unity will non-deterministically use one of them, please manually "
                    Write-Host "regenerate the intended one by deleting the .meta file and re-opening the Unity editor locally."

                    # Found the end of the guid conflict message - output to the console
                    # all lines between these two locations (including the assigning a new guid message
                    # in case it falls on the same line)
                    for ($j = $LineNumber; $j -le $i; $j++) {
                        Write-Host $LogFileContent[$j]
                    }
                    $true
                }
            }
        }
        $false
    }
}

if (-not $LogFile) {
    throw "-LogFile is a required flag"
}

Write-Output "Checking $LogFile for build validation errors"

$logFileContent = Get-Content $LogFile

# Each line of the log is checked for issues - the intent is to catch ALL issues in the
# log file, instead of breaking on the first issue. The idea is to give the reader
# a full spew of all issues at once, instead of requiring multiple runs to uncover all issues
# As a result, each check below is NOT short-circuited (i.e. $containsError is ORed at the end)
$containsError = $false
for ($i = 0; $i -lt $logFileContent.Length; $i++) {
    if (CheckDuplicateGuids $logFileContent $i) {
        $containsError = $true
    }
}

if ($containsError) {
    Write-Output "Validation errors found, please see above for details"
    exit 1;
}
else {
    Write-Output "No validation errors found"
    exit 0;
}
