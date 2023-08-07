<#
.SYNOPSIS
    Runs Unity with a given project and invokes a method in that context.

.DESCRIPTION
    Runs Unity in 'batchmode' to execute the method that scripts Unity. The script will block
    until the Unity process exits, but writing the Editor.log to the Output as the process runs.

.PARAMETER ProjectPath
    - The path to the Unity project.

# Set either ImportPackage OR ExecuteMethod
.PARAMETER ImportPackage
    - The name of the .unitypackage to import.

# Set either ImportPackage OR ExecuteMethod
.PARAMETER ExecuteMethod
    - The fully quailfied name of the method to execute.

.PARAMETER BuildTarget
    - The Unity build target to default to.

.PARAMETER UnityVersion
    - The version of Unity to search for and use.

.PARAMETER VerbosityLevel
    - How detailed the output should be.

.PARAMETER AdditionalArguments
    - Addition argumennts passed to Unity via cmd line arguments.

.EXAMPLE
    To use ExecuteMethod:
    C:\PS> invoke-unity.ps1 -Project Apps/HLPancakes -BuildTarget WindowsStoreApps -ExecuteMethod Build.GeneratePlayerSolution -VerbosityLevel detailed

    To use ImportPackage:
    C:\PS> invoke-unity.ps1 -Project C:\internal.azure-spatial-anchors-samples\Unity -ImportPackage C:\Users\Yoda\Downloads\AzureSpatialAnchors.unitypackage -VerbosityLevel detailed
#>
[CmdletBinding()]
Param(
    [Parameter(Mandatory)]
    [String]$ProjectPath,

    [Parameter(Mandatory = $true, ParameterSetName = 'ExecuteMethod')]
    [String]$ExecuteMethod,

    [Parameter(Mandatory = $true, ParameterSetName = 'ImportPackage')]
    [String]$ImportPackage,

    [ValidateSet('iOS', 'Android', 'WindowsStoreApps', 'StandAlone')]
    [String]$BuildTarget = 'WindowsStoreApps',

    [Parameter(Mandatory)]
    [String]$UnityVersion,

    [Parameter(HelpMessage = 'Verbosity level: minimal, detailed')]
    [ValidateSet('minimal', 'detailed')]
    [string] $VerbosityLevel = 'minimal',

    [Parameter()]
    [String]$AdditionalArguments
     
)
function startProcess {
    param(
        [ValidateNotNullOrEmpty()][string] $Application,
        [string] $ArgumentList
    )

    Write-Verbose "Application  : $Application"
    Write-Verbose "ArgumentList : $ArgumentList"
    Start-Process -FilePath $Application -ArgumentList $ArgumentList -PassThru
}

function whileRunning {
    param([System.Diagnostics.Process]$process, $scope)

    while (-not $unityProcess.HasExited) {
        $result = &$scope
        if ($result) {
            return $result
        }
    }

    $result = &$scope
    if ($result) {
        return $result
    }
}

$ErrorActionPreference = 'Stop'
$consoleInputRedirected = $true
if (-not [Console]::IsInputRedirected) {
    $consoleInputRedirected = $false
    [Console]::TreatControlCAsInput = $true
}

$minimalVerbosityFilter = '(?:[^\:]*: (warning|error))|(?:^DisplayProgressbar\:)'
$unityApplicationName = 'Unity.exe'
$unityEditorFolderCandidates = @(
    "c:\Program Files\Unity\Hub\Editor\$UnityVersion\Editor",
    "c:\Program Files\Unity$UnityVersion\Editor\"
    )

$unityEditorFolder = $unityEditorFolderCandidates |
    Where-Object { Test-Path -Path $(Join-Path $_ $unityApplicationName ) } |
    Select-Object -First 1

if (-not $unityEditorFolder) {
    throw "Unity version $UnityVersion was not found."
}

$unityApplication = Join-Path -Resolve $unityEditorFolder $unityApplicationName
$arguments = "-quit -batchmode -buildTarget `"$BuildTarget`" -projectPath `"$ProjectPath`""

if ($ExecuteMethod) {
    $arguments += " -executeMethod `"$ExecuteMethod`""
}
elseif ($ImportPackage) {
    $arguments += " -importPackage `"$ImportPackage`""
}

if ($BuildTarget -eq 'StandAlone')
{
   $projectName = [System.IO.Path]::GetFileName($ProjectPath)
   $arguments += " -buildWindows64Player `"$ProjectPath/../output/$projectName.exe`""
}

if (-not ([string]::IsNullOrEmpty($AdditionalArguments)))
{
    $arguments += " $AdditionalArguments"
}

$unityLogFile = Join-Path $env:LocalAppData 'Unity/Editor/Editor.Log'

if (Test-Path $unityLogFile) {
    Remove-Item -Force $unityLogFile -ErrorAction SilentlyContinue
}

Write-Host "Starting Process: $unityApplication $arguments"
$unityProcess = startProcess $unityApplication $arguments
$unityLog = whileRunning $unityProcess {
    if (Test-Path $unityLogFile) {
        Get-Item $unityLogFile
    }
}

if (-not $unityLog) {
    throw "A log file was not created."
}

$offset = 0

while ($true) {
    $unityLog.Refresh()
    $logLength = $unityLog.Length

    if ($logLength -gt $offset) {
        $stream = $unityLog.Open([System.IO.FileMode]::Open, [System.IO.FileAccess]::Read, [System.IO.FileShare]::ReadWrite)
        $stream.Position = $offset
        $reader = New-Object System.IO.StreamReader($stream)
        while (-not $reader.EndOfStream) {
            $line = $reader.ReadLine()
            if (($VerbosityLevel -eq 'detailed') -or ($line -match $minimalVerbosityFilter)) {
                Write-Output $line
            }
        }

        $offset = $stream.Position
    }
    elseif ($unityProcess.HasExited) {
        break
    } else {
        if (-not $consoleInputRedirected -and [Console]::KeyAvailable) {
            $key = [Console]::ReadKey($true)
            if (($key.Modifiers -band [ConsoleModifiers]'Control') -and ($key.key -eq 'C')) {
                Write-Host 'Ctrl+C: Closing the Unity process...' -ForegroundColor Yellow
                $unityProcess.Kill
                whileRunning $unityProcess {
                    Start-Sleep -Milliseconds 100
                    Write-Host '.' -ForegroundColor Yellow -NoNewline
                }

                Write-Host ''
                break
            }
        }

        Start-Sleep -Milliseconds 500
    }
}

if (-not ($unityProcess.ExitCode -eq 0)) {
    throw "The Unity process failed with error code " + $unityProcess.ExitCode
}
