# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
 # Recursively finds a .json files in the current working directory
 # and pretty-formats them.
 #>

function PrettyFormatJson([string]$FileName) {
    <#
    .SYNOPSIS
        Pretty formats the given json file inplace
    #>
    if (-not $FileName.ToLower().EndsWith(".json")) {
        Write-Error "$FileName is not a json file"
        return
    }

    # Arbitrarily assume that the JSON object depth can go to 100
    # which is the max supported depth for ConvertTo-Json
    Get-Content -Path $FileName |
        ConvertFrom-Json |
        ConvertTo-Json -Depth 100 |
        Set-Content -Path $FileName

    Write-Verbose "Pretty-formatted $FileName in-place"
}

$currentDirectory = Get-Location
$files = Get-ChildItem -Path $currentDirectory -File -Recurse -Filter "*.json"

foreach ($file in $files) {
    PrettyFormatJson($file.FullName);
}
