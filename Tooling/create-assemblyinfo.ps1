# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

$gitRoot = ((git -C $PSScriptRoot rev-parse --show-toplevel) | Out-String).Trim()

Get-ChildItem -Path (Join-Path $gitRoot * package.json) | ForEach-Object {
    $packageName = Select-String -Pattern "com\.microsoft\.mrtk\.\w+" -Path $_ | Select-Object -First 1

    if (-not $packageName) {
        return # this is not an MRTK package, so skip
    }

    $packageName = $packageName.Matches[0].Value

    $asmdefs = Get-ChildItem $_.Directory *.asmdef -Recurse | Select-Object FullName
    foreach ($asmdef in $asmdefs) {
        # The AssemblyInfo.cs file will be added as a sibling of the .asmdef location,
        # so we need to trim off the filename.
        $folder = Split-Path -Path $asmdef.FullName
        $filename = Join-Path -Path $folder -ChildPath "AssemblyInfo.cs"

        if (-not (Test-Path -Path $filename)) {
            # Parse the assembly name for embedding into the AssemblyInfo file
            $assemblyName = Select-String -Pattern "Microsoft\.MixedReality\.Toolkit\.(\w+(?:.\w+)*)" -Path $asmdef.FullName | Select-Object -First 1

            if (-not $assemblyName) {
                return # assembly isn't MRTK-branded, so skip
            }

            $assemblyName = $assemblyName.Matches[0].Value
            $assemblyName = $assemblyName.Split(".") | Select-Object -Skip 3 | ForEach-Object { $_ -csplit '(?=[A-Z][a-z])' -ne '' -join ' ' } # Skip the first three entries, which are Microsoft, MixedReality, and Toolkit

            if ($assemblyName[-1] -eq "Editor") {
                $assemblyName[-1] = "- Editor"
            }

            # Note that this is left-indent adjusted so that the file output
            # ends up looking reasonable.
            $copyright =
            @"
// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.
"@

            $content =
            @"

using System.Reflection;

[assembly: AssemblyProduct("Microsoft® Mixed Reality Toolkit $assemblyName")]
[assembly: AssemblyCopyright("Copyright © Microsoft Corporation")]

// The AssemblyVersion attribute is checked-in and is recommended not to be changed often.
// https://docs.microsoft.com/troubleshoot/visualstudio/general/assembly-version-assembly-file-version
// AssemblyFileVersion and AssemblyInformationalVersion are added by pack-upm.ps1 to match the current MRTK build version.
[assembly: AssemblyVersion("3.0.0.0")]
"@

            Set-Content -Path $filename -Value $copyright
            Add-Content -Path $filename -Value $content
            Write-Host "Added AssemblyInfo.cs for $assemblyName at $filename"
        }
    }
}
