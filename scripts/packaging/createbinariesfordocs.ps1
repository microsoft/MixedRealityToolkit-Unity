# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Builds the binaries needed to generate API reference docs for Mixed Reality Toolkit.
.DESCRIPTION
    Builds the binaries needed to generate API reference docs for Mixed Reality Toolkit.
.PARAMETER OutputDirectory
    Where should we place the output?
.PARAMETER Version
    What version of the artifacts should we build?
.PARAMETER UnityDirectory
    Where is the directory of the version of Unity we use to generate the binaries?
#>
param(
    [string]$OutputDirectory,
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]$Version,
    [string]$UnityDirectory
)

Write-Verbose "Checking parameters:"
if (-not $OutputDirectory) {
    throw "-OutputDirectory is a required flag"
}
if (-not $Version) {
    throw "-Version is a required flag"
}
if (-not $UnityDirectory) {
    throw "-UnityDirectory is a required flag"
}

$unityEditor = Get-ChildItem $UnityDirectory -Filter 'Unity.exe' -Recurse | Select-Object -First 1 -ExpandProperty FullName
if (-not $unityEditor) {
    throw "Unable to find the Unity editor executable in $UnityDirectory"
}
Write-Verbose $unityEditor

function RunUnityTask {
    param([string]$taskName, [string]$methodToExecute)
    Write-Output "Starting running Unity task: $($taskName)"
    $logFile = New-Item -Path "Logs\Unity.$($taskName).$($Version).log" -ItemType File -Force

    $ProjectLocation = Resolve-Path "$(Get-Location)\..\"
    Write-Output $ProjectLocation
    $proc = Start-Process -FilePath "$unityEditor" -ArgumentList "-projectPath $ProjectLocation -batchmode -executeMethod $($methodToExecute) -logFile $($logFile.FullName) -nographics -quit" -PassThru
    $ljob = Start-Job -ScriptBlock { param($log) Get-Content "$log" -Wait } -ArgumentList $logFile.FullName

    while (-not $proc.HasExited -and $ljob.HasMoreData) {
        Receive-Job $ljob
        Start-Sleep -Milliseconds 200
    }
    Receive-Job $ljob

    Stop-Job $ljob

    Remove-Job $ljob
    Stop-Process $proc
    if ($proc.ExitCode -ge 1) {
        Write-Error "Failed to execute Unity Task '$($taskName)', see log '$($logFile)' for more information."

        if (Test-Path $logFile.FullName) {
            Write-Output '====================================================='
            Write-Output '              Unity build docs finished              '
            Write-Output '====================================================='

            Write-Output '====================================================='
            Write-Output '                Begin Unity build log                '
            Write-Output '====================================================='

            Get-Content $logFile.FullName

            Write-Output '====================================================='
            Write-Output '                 End Unity build log                 '
            Write-Output '====================================================='
        }
        else {
            Write-Error 'Unity build log missing!'
        }

        exit($proc.ExitCode)
    }
}

$OriginalPath = Get-Location
try {
    Set-Location (Split-Path $MyInvocation.MyCommand.Path)
    Set-Location "..\\..\\"
    New-Item -ItemType Directory "BinariesForDocs" -ErrorAction SilentlyContinue
    Set-Location "BinariesForDocs"

    ### Run MSBuild Generation
    RunUnityTask -taskName "MSBuildGeneration" -methodToExecute "Microsoft.MixedReality.Toolkit.MSBuild.MSBuildTools.GenerateSDKProjects"

    ### Build the needed flavor for MRTK
    Write-Output "============ Building InEditor WSA ============ "
    $logFilePath = "$(Get-Location)\Logs\Build.InEditor.WSA.$($Version).log"
    dotnet nuget add source https://api.nuget.org/v3/index.json -n nuget.org
    dotnet msbuild ..\NuGet\BuildSource.proj -target:BuildWSAEditor > $logFilePath
    if ($lastexitcode -ge 1) {
        Write-Output "Copying $logFilePath to $OutputDirectory"
        Copy-Item -Path $logFilePath -Destination "$OutputDirectory"
        Write-Error "Building InEditor WSA Failed! See log file for more information $logFilePath"
        exit($lastexitcode)
    }

    Write-Output "============ Copying the binaries ============ "
    New-Item -ItemType Directory "MRTK_$($Version)"
    New-Item -ItemType Directory "MRTK_$($Version)\dependencies"
    Remove-Item "..\MSBuild\Publish\InEditor\WSA\*.pdb"
    Move-Item "..\MSBuild\Publish\InEditor\WSA\Microsoft.MixedReality.Toolkit*" "MRTK_$($Version)\"
    Move-Item "..\MSBuild\Publish\InEditor\WSA\*.dll" "MRTK_$($Version)\dependencies\"
    Copy-Item "$($UnityDirectory)\Data\Managed\UnityEditor.dll" "MRTK_$($Version)\dependencies\"
    Copy-Item "$($UnityDirectory)\Data\Managed\UnityEngine.dll" "MRTK_$($Version)\dependencies\"
    if ($UnityDirectory.Contains("2018")) {
        Copy-Item "$($UnityDirectory)\Data\UnityExtensions\Unity\GUISystem\UnityEngine.UI.dll" "MRTK_$($Version)\dependencies\"
        Remove-Item "MRTK_$($Version)\Microsoft.MixedReality.Toolkit.Tests.PlayModeTests.*"
        Remove-Item "MRTK_$($Version)\Microsoft.MixedReality.Toolkit.Tests.EditModeTests.*"
    }
    nuget install Microsoft.Windows.MixedReality.DotNetWinRT -OutputDirectory packages
    Copy-Item "packages\Microsoft.Windows.MixedReality.DotNetWinRT*\lib\unity\net46\Microsoft.Windows.MixedReality.DotNetWinRT.dll" "MRTK_$($Version)\dependencies\"
    Copy-Item -Path "MRTK_$($Version)" -Destination "$OutputDirectory" -Recurse
}
finally {
    Set-Location $OriginalPath
}
