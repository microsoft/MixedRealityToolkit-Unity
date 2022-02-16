# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Builds the Mixed Reality Toolkit NuGet packages.
.DESCRIPTION
    Builds NuGet packages for the Mixed Reality Toolkit.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER Version
    What version of the artifacts should we build? If unspecified, the highest
    git tag pointing to HEAD is searched. If none is found, an error is reported.
#>
param(
    [string]$OutputDirectory = ".\artifacts",
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]$Version = "0.0.0",
    [Parameter(Mandatory=$true)]
    [string]$UnityDirectory,
    [string]$VisualStudioDirectory = "C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise"
)

Write-Verbose "Reconciling Unity binary:"
$unityEditor = Get-ChildItem $UnityDirectory -Filter 'Unity.exe' -Recurse | Select-Object -First 1 -ExpandProperty FullName
if (-not $unityEditor) {
    throw "Unable to find the unity editor executable in $UnityDirectory"
}
Write-Verbose $unityEditor;

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
        exit($proc.ExitCode)
    }
}

$OriginalPath = Get-Location
try {
    Set-Location (Split-Path $MyInvocation.MyCommand.Path)
    Set-Location "..\\..\\"
    New-Item -ItemType Directory "NuGet" -ErrorAction SilentlyContinue
    Set-Location "NuGet"

    ### Run MSBuild Generation
    RunUnityTask -taskName "MSBuildGeneration" -methodToExecute "Microsoft.MixedReality.Toolkit.MSBuild.MSBuildTools.GenerateSDKProjects"

    ### Build all the needed flavors for MRTK
    Write-Output "============ Building InEditor WindowsStandalone32 ============ "
    dotnet msbuild .\BuildSource.proj -target:BuildStandaloneEditor > "Logs\Build.InEditor.WindowsStandalone32.$($Version).log"
    if ($lastexitcode -ge 1) {
        Write-Error "Building InEditor WindowsStandalone32 Failed! See log file for more information $(Get-Location)\Logs\Build.InEditor.WindowsStandalone32.$($Version).log";
        exit($lastexitcode)
    }
    Write-Output "============ Building InEditor WSA ============ "
    dotnet msbuild .\BuildSource.proj -target:BuildWSAEditor > "Logs\Build.InEditor.WSA.$($Version).log"
    if ($lastexitcode -ge 1) {
        Write-Error "Building InEditor WSA Failed! See log file for more information $(Get-Location)\Logs\Build.InEditor.WSA.$($Version).log";
        exit($lastexitcode)
    }
    Write-Output "============ Building Player WindowsStandalone32 ============ "
    dotnet msbuild .\BuildSource.proj -target:BuildStandalonePlayer > "Logs\Build.Player.WindowsStandalone32.$($Version).log"
    if ($lastexitcode -ge 1) {
        Write-Error "Building Player WindowsStandalone32 Failed! See log file for more information $(Get-Location)\Logs\Build.Player.WindowsStandalone32.$($Version).log";
        exit($lastexitcode)
    }
    Write-Output "============ Building Player Android ============ "
    dotnet msbuild .\BuildSource.proj -target:BuildAndroidPlayer > "Logs\Build.Player.Android.$($Version).log"
    if ($lastexitcode -ge 1) {
        Write-Error "Building Player Android Failed! See log file for more information $(Get-Location)\Logs\Build.Player.Android.$($Version).log";
        exit($lastexitcode)
    }
    Write-Output "============ Building Player iOS  ============ "
    dotnet msbuild .\BuildSource.proj -target:BuildIOSPlayer > "Logs\Build.Player.iOS.$($Version).log"
    if ($lastexitcode -ge 1) {
        Write-Error "Building Player iOS Failed! See log file for more information $(Get-Location)\Logs\Build.Player.iOS.$($Version).log";
        exit($lastexitcode)
    }
    Write-Output "============ Building Player WSA ============ "
    if ($VisualStudioDirectory -match "2019") {
        $VisualStudioDirectory = Join-Path $VisualStudioDirectory "MSBuild\Microsoft\WindowsXaml\v16.0"
    }
    else {
        $VisualStudioDirectory = Join-Path $VisualStudioDirectory "MSBuild\Microsoft\WindowsXaml\v15.0"
    }
    dotnet msbuild .\BuildSource.proj -target:BuildWSAPlayer /p:XamlTargetsPath=$VisualStudioDirectory > "Logs\Build.Player.WSA.$($Version).log"
    if ($lastexitcode -ge 1) {
        Write-Error "Building Player WSA Failed! See log file for more information $(Get-Location)\Logs\Build.Player.WSA.$($Version).log";
        exit($lastexitcode)
    }

    ### Run Asset retargeting:
    RunUnityTask -taskName "AssetRetargeting" -methodToExecute "Microsoft.MixedReality.Toolkit.MSBuild.AssetScriptReferenceRetargeter.RetargetAssets"
    
    ### Package NuGet
    New-Item -ItemType Directory -Force -Path $OutputDirectory
    $OutputDirectory = Resolve-Path $OutputDirectory
    $releaseNotes = "Built on local Unity"

    # Add this location to the path as NuGet.exe may be installed here
    $env:PATH = "$($env:Path);$((Get-Location).Path)"

    # Check if NuGet.exe is in the environment PATH, if not go ahead and install it to this directory
    where.exe nuget > $null 2> $null
    if ($lastexitcode -ne 0) {
        Write-Host "Could not find NuGet.exe in the path. Downloading it now from: https://dist.nuget.org/win-x86-commandline/latest/nuget.exe"
        Invoke-WebRequest -Uri "https://dist.nuget.org/win-x86-commandline/latest/nuget.exe" -OutFile ".\nuget.exe"
    }
    
    # Kick jobs for bundling up the nuget packages
    Get-ChildItem -Filter *.nuspec -Recurse | Foreach-Object {
        Write-Verbose "Starting nuget job for $($_.FullName)"
        $props = "version=$Version;releaseNotes=$releaseNotes"

        nuget pack $_.FullName -OutputDirectory $OutputDirectory -Properties $props -Exclude *.nuspec.meta
        
        # ===
        # https://github.com/microsoft/MixedRealityToolkit-Unity/issues/6276 states that not having a version on the assemblies in our NuGet packages
        # is an issue... commenting the localVersion update
        # ===

        # To make debugging the MRTK NuGet packages locally much easier automatically create new packages with version 0.0.0 and then
        # restore them to the machine NuGet feed. To test changes to the packages developers can run this script and then change their
        # project to consume version 0.0.0 and restore. Because the package is in the machine global feed it will resolve properly.
        # $localVersion = '0.0.0'
        $packageId = ([xml](Get-Content $_.FullName)).package.metadata.id
        # $finalInstallPath = [System.IO.Path]::Combine($env:UserProfile, '.nuget', 'packages', $packageId, $localVersion)
        $finalInstallPath = [System.IO.Path]::Combine($env:UserProfile, '.nuget', 'packages', $packageId, $Version)
        
        # Repack but with a hard-coded version of 0.0.0 (the -Version parameter overrides the property value for version)
        # nuget pack $_.FullName -OutputDirectory $OutputDirectory -Properties $props -Exclude *.nuspec.meta -Version $localVersion
        nuget pack $_.FullName -OutputDirectory $OutputDirectory -Properties $props -Exclude *.nuspec.meta -Version $Version
        
        # If the package is already installed to the machine global cache delete it, otherwise the next restore will no-op
        if ([System.IO.Directory]::Exists($finalInstallPath)) {
            Remove-Item -Recurse -Force $finalInstallPath
        }
        
        # Restore the package by providing the nupkg folder. After this restore the machine global cache will be populated with the package
        $restoreProjectPath = [System.IO.Path]::Combine((Split-Path $MyInvocation.MyCommand.Path), 'NuGetRestoreProject.csproj')
        # dotnet build "$restoreProjectPath" -p:RestorePackageFeed="$(convert-path $OutputDirectory)" -p:RestorePackageId=$packageId -p:RestorePackageVersion=$localVersion
        dotnet build "$restoreProjectPath" -p:RestorePackageFeed="$(convert-path $OutputDirectory)" -p:RestorePackageId=$packageId -p:RestorePackageVersion=$Version
    }
}
finally {
    Set-Location $OriginalPath
}