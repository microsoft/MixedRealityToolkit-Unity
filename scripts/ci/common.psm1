<#
.SYNOPSIS
    Given a path to the list of Github changes, gets a list of
#>
function GetChangedFiles { 
    param(
        [string]$Filename,
        [string]$RepoRoot
    )
    $RawContent = Get-Content -Path $Filename
    [System.Collections.ArrayList]$ProcessedContent = @()
    foreach ($Filename in $RawContent) {
        $joinedPath = Join-Path -Path $RepoRoot -ChildPath $Filename
        $ProcessedContent.Add($joinedPath)
    }
    return $ProcessedContent
}

<#
.SYNOPSIS
    Returns true if the given file is a markdown document and
    false otherwise
#>
function IsMarkdown { param([string]$Filename)
    $extension = [IO.Path]::GetExtension($Filename).ToLower()
    return $extension -eq ".md"
}

<#
.SYNOPSIS
    Returns true if the given file is a csharp file and
    false otherwise
#>
function IsCSharp { param([string]$Filename)
    $extension = [IO.Path]::GetExtension($Filename).ToLower()
    return $extension -eq ".cs"
}

<#
.SYNOPSIS
    Returns true if the given file is a .asset file and
    false otherwise
#>
function IsAsset { param([string]$Filename)
    $extension = [IO.Path]::GetExtension($Filename).ToLower()
    return $extension -eq ".asset"
}

<#
.SYNOPSIS
    Returns true if the given file is a Unity scene and
    false otherwise
#>
function IsUnityScene { param([string]$Filename)
    $extension = [IO.Path]::GetExtension($Filename).ToLower()
    return $extension -eq ".unity"
}

<#
.SYNOPSIS
    Returns true if the given file is a Unity meta file and
    false otherwise
#>
function IsMetaFile { param([string]$Filename)
    $extension = [IO.Path]::GetExtension($Filename).ToLower()
    return $extension -eq ".meta"
}