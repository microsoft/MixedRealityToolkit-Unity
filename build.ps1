<#
.SYNOPSIS
    Builds the Mixed Reality Toolkit artifacts.
.DESCRIPTION
    Builds nuget and unity package artifacts for the Mixed Reality Toolkit.
.PARAMETER OutputDirectory
    Where should we place the output? Defaults to ".\artifacts"
.PARAMETER Version
    What version of the artifacts should we build? If unspecified, the highest
    git tag pointing to HEAD is searched. If none is found, an error is reported.
.PARAMETER UnityVersion
    What version of Unity should we use? Falls back to -ge, then to -latest.
.PARAMETER Clean
    Should we clean OutputDirectory before building?
.EXAMPLE
    .\build.ps1 -Version 1.2.3
.EXAMPLE
    .\build.ps1 -OutputDirectory .\artifacts -Version 1.2.3 -Clean
#>
param(
    [string]$OutputDirectory = ".\artifacts",
    [ValidatePattern("^\d+\.\d+\.\d+$")]
    [string]$Version,
    [ValidatePattern("^\d+\.\d+\.\d+[fpb]\d+$")]
    [string]$UnityVersion, 
    [switch]$Clean,
    [switch]$Verbose
)

Import-Module UnitySetup -MinimumVersion '4.0.97' -ErrorAction Stop
if ( $Verbose ) { $VerbosePreference = 'Continue' }

# No version is specified, try to pull one from relevant git tags
if ( -not $Version) {
    $tagVersion = git tag --points-at HEAD | 
        Foreach-Object { if ( $_ -match "^(\d+\.\d+\.\d+)$" ) { [version]$matches[1] } } | 
        Sort-Object -Descending |
        Select-Object -First 1

    if ( $tagVerson ) { $Version = $tagVersion }
}

if ( -not $Version ) { 
    throw "Could not find a valid version to build. Specify -Version when building, or tag your git commit."
}

$upi = Get-UnityProjectInstance
if ( $UnityVersion ) {
    $usi = Get-UnitySetupInstance | Select-UnitySetupInstance -Version $UnityVersion
    if ( -not $usi ) {
        throw "Could not find specified version of unity: $UnityVersion"
    }
}
else {

    $usis = Get-UnitySetupInstance

    # Try to find an equal or greater version than our project
    $usi = $usis | Where-Object { $_.Version -ge $upi.Version } | 
        Sort-Object -Property 'Version' | 
        Select-Object -First 1

    # well, just use the latest then.
    if ( -not $usi ) { $usi = $usis | Select-UnitySetupInstance -Latest }
    if ( -not $usi ) { throw 'Could not find version of Unity to build with.' }

    # warn if we're not using the exact version
    if ( $usi.Version.CompareTo( $upi.Version ) -ne 0 ) {
        Write-Warning "Could not find Unity $($upi.Version), falling back to $($usi.Version)"
    }

    $UnityVersion = $usi.Version 
}

Write-Verbose "Building version $Version of Mixed Reality Toolkit packages."

if ( $Clean ) {
    Remove-Item -ErrorAction SilentlyContinue $OutputDirectory -Recurse 
}
if (-not (Test-Path $OutputDirectory -PathType Container)) {
    New-Item $OutputDirectory -ItemType Directory | Out-Null
}

$OutputDirectory = Resolve-Path $OutputDirectory
$releaseNotes = "Built on Unity $($upi.Version)"

# Kick jobs for bundling up the nuget packages
$nugetJobs = Get-ChildItem *.nuspec -Recurse | Foreach-Object {
    Write-Verbose "Starting nuget job for $($_.FullName)"
    Start-Job { 
        param($name, $outDir, $props) 
        nuget pack $name -OutputDirectory $outDir -Properties $props -Exclude *.nuspec.meta
    } -ArgumentList $_.FullName, $OutputDirectory, "version=$Version;releaseNotes=$releaseNotes"
}

# Structure analagous unity packages for all nuget packages
$unityPackages = Get-ChildItem *.nuspec -Recurse | ForEach-Object {
    [xml]$xmlDoc = Get-Content $_.FullName
    [pscustomobject]@{ 
        "packageName" = $xmlDoc.package.metadata.id
        "directory"   = (Resolve-Path -Relative $_.DirectoryName).Trim(".\")
    }
}

# Build all the unity packages one at a time
$unityPackages | Foreach-Object {
    try {
        # Output the Unity package associated with this nuspec location
        $unityPackagePath = "$OutputDirectory\$($_.packageName).$Version.unitypackage"
        Write-Verbose "Attempting to create unity package $unityPackagePath from $($_.directory)"
    
        $sueArgs = @{
            'BatchMode'       = $true
            'AcceptAPIUpdate' = $true
            'Quit'            = $true
            'PassThru'        = $true
            'Wait'            = $true
            'ExportPackage'   = "$($_.directory)", "$unityPackagePath"
            'LogFile'         = "Build-UnityPackage-$($_.packageName).$Version.log"
            'Version'         = $UnityVersion
        }

        $process = Start-UnityEditor @sueArgs
        if (($process.ExitCode -eq 0) -and (Test-Path $unityPackagePath)) {
            Write-Verbose "Successfully created $unityPackagePath"
        }
        else { Write-Error "Failed to create $unityPackagePath" }
    }
    catch { Write-Error $_ }
}

# Wait for, receive, and remove all the nuget jobs
$nugetJobs | Receive-Job -Wait -AutoRemoveJob
