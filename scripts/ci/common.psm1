# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Given the path to the list of raw git changes, returns an array of
    those changes rooted in the git root directory.
.DESCRIPTION
    For example, the raw git changes will contain lines like:

    Assets/File.cs

    This function will return a list of paths that look like (assuming
    that RepoRoot is C:\repo):

    C:\repo\Assets\File.cs
#>
function GetChangedFiles { 
    [CmdletBinding()]
    param(
        [string]$Filename,
        [string]$RepoRoot
    )
    process {
        $rawContent = Get-Content -Path $Filename
        $processedContent = @()
        foreach ($line in $rawContent) {
            $joinedPath = Join-Path -Path $RepoRoot -ChildPath $line
            $processedContent += $joinedPath
        }
        $processedContent
    }
}

<#
.SYNOPSIS
    Returns true if the given file is a markdown document and
    false otherwise. Uses the extension of the file, not the actual
    content to determine this.
#>
function IsMarkdownFile {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        [IO.Path]::GetExtension($Filename).ToLower() -eq ".md"
    }
}

<#
.SYNOPSIS
    Returns true if the given file is a csharp file and
    false otherwise. Uses the extension of the file, not the actual
    content to determine this.
#>
function IsCSharpFile {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        [IO.Path]::GetExtension($Filename).ToLower() -eq ".cs"
    }
}

<#
.SYNOPSIS
    Returns true if the given file is a .asset file and
    false otherwise. Uses the extension of the file, not the actual
    content to determine this.
#>
function IsAssetFile {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        [IO.Path]::GetExtension($Filename).ToLower() -eq ".asset"
    }
}

<#
.SYNOPSIS
    Returns true if the given file is a Unity scene and
    false otherwise. Uses the extension of the file, not the actual
    content to determine this.
#>
function IsUnityScene {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        [IO.Path]::GetExtension($Filename).ToLower() -eq ".unity"
    }
}

<#
.SYNOPSIS
    Returns true if the given file is a Unity meta file and
    false otherwise. Uses the extension of the file, not the actual
    content to determine this.
#>
function IsMetaFile {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        [IO.Path]::GetExtension($Filename).ToLower() -eq ".meta"
    }
}

<#
.SYNOPSIS
    Returns true if the given file is a Unity asmdef file and
    false otherwise. Uses the extension of the file, not the actual
    content to determine this.
#>
function IsAsmDef {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        [IO.Path]::GetExtension($Filename).ToLower() -eq ".asmdef"
    }
}
