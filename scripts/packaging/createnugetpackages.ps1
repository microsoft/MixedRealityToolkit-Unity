<#
.SYNOPSIS
    Builds the Mixed Reality Toolkit nuget packacges.
.DESCRIPTION
    Builds nuget packages for the Mixed Reality Toolkit.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER Version
    What version of the artifacts should we build? If unspecified, the highest
    git tag pointing to HEAD is searched. If none is found, an error is reported.
#>
param(
    [string]$OutputDirectory = ".\artifacts",
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]$Version,
    [string]$UnityDirectory,
    [string]$MSBuildExtensionsPath="C:\Program Files (x86)\Microsoft Visual Studio\2017\Enterprise\MSBuild"
)

Write-Verbose "Reconciling Unity binary:"
if (-not $UnityDirectory) {
    throw "-UnityDirectory is a required flag"
}

$unityEditor = Get-ChildItem $UnityDirectory -Filter 'Unity.exe' -Recurse | Select-Object -First 1 -ExpandProperty FullName
if (-not $unityEditor) {
    throw "Unable to find the unity editor executable in $UnityDirectory"
}
Write-Verbose $unityEditor;

function RunUnityTask
{
    param([string]$taskName, [string]$methodToExecute)

    $logFile = New-Item -Path "Logs\Unity.$($taskName).$($Version).log" -ItemType File -Force
    
    $ProjectLocation = Resolve-Path "$(Get-Location)\..\"
    Write-Output $ProjectLocation
    $proc = Start-Process -FilePath "$unityEditor" -ArgumentList "-projectPath $ProjectLocation -batchmode -executeMethod $($methodToExecute) -logFile $($logFile.FullName) -nographics -quit" -PassThru
    $ljob = Start-Job -ScriptBlock { param($log) Get-Content "$log" -Wait } -ArgumentList $logFile.FullName
    
    while (-not $proc.HasExited -and $ljob.HasMoreData)
    {
        Receive-Job $ljob
        Start-Sleep -Milliseconds 200
    }
    Receive-Job $ljob
    
    Stop-Job $ljob
    
    Remove-Job $ljob
    Stop-Process $proc
}

$OriginalPath = Get-Location
try
{
    Set-Location (Split-Path $MyInvocation.MyCommand.Path)
    Set-Location "..\\..\\NuGet"

    ### Run MSBuild Generation
    RunUnityTask -taskName "MSBuildGeneration" -methodToExecute "Microsoft.MixedReality.Toolkit.MSBuild.MSBuildTools.GenerateSDKProjects"

    ### Build all the needed flavors for MRTK
    ..\MSBuild\Projects\buildall.bat InEditor WindowsStandalone32 $MSBuildExtensionsPath
    ..\MSBuild\Projects\buildall.bat Player WindowsStandalone32 $MSBuildExtensionsPath
    ..\MSBuild\Projects\buildall.bat Player WSA $MSBuildExtensionsPath 


    ### Run Asset regargetting:
    RunUnityTask -taskName "AssetRetargeting" -methodToExecute "Microsoft.MixedReality.Toolkit.MSBuild.AssetScriptReferenceRetargeter.RetargetAssets"
    
    ### Package NuGet
    New-Item -ItemType Directory -Force -Path $OutputDirectory
    $OutputDirectory = Resolve-Path $OutputDirectory
    $releaseNotes = "Built on local Unity"
    
    # Kick jobs for bundling up the nuget packages
    $nugetJobs = Get-ChildItem -Filter *.nuspec -Recurse | Foreach-Object {
        Write-Verbose "Starting nuget job for $($_.FullName)"
        Start-Job { 
            param($name, $outDir, $props) 
            nuget pack $name -OutputDirectory $outDir -Properties $props -Exclude *.nuspec.meta
        } -ArgumentList $_.FullName, $OutputDirectory, "version=$Version;releaseNotes=$releaseNotes"
    }
    
    # Wait for, receive, and remove all the nuget jobs
    $nugetJobs | Receive-Job -Wait -AutoRemoveJob
}
finally
{
    Set-Location $OriginalPath
}