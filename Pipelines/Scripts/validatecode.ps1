# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

<#
.SYNOPSIS
    Validates the code and assets to check for common patterns and usage that shouldn't be
    checked in.
.DESCRIPTION
    This currently checks both CSharp files and Unity serialized assets.

    - That Boo.Lang isn't used anywhere in the code. This is an autocomplete option
      that occurs when using Lists and other collections - the right thing to do
      is to use System.Collections
    - Validates that there are no empty doccomments (e.g. ///<summary></summary>)
    - There are no references to hardcoded paths
    - There aren't any references to Camera.main
    - That each assembly definition has an AssemblyInfo.cs

    Returns 0 if there are no issues, non-zero if there are.
.EXAMPLE
    .\validatecode.ps1 -Directory c:\path\to\MRTK -ChangesFile c:\path\to\changes\file.txt
#>
param(
    # The directory containing the code to validate.
    # Also used in conjunction with ChangesFile to determine the repo root.
    [Parameter(Mandatory = $true)]
    [string]$Directory,

    # The filename containing the list of files to scope the code validation
    # to. This is useful in pull request validation when there isn't a need
    # to check every single file in the repo for changes (i.e. only the list
    # of changed files)
    #
    # If ChangesFile doesn't exist (i.e. not specified, null, is specified but
    # the actual file doesn't exist), then this defaults to checking for everything
    # in the repo.
    [string]$ChangesFile
)

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains a reference to Boo.Lang
    Returns true if such a reference exists.
#>
function CheckBooLang {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileContent[$LineNumber] -match "^using Boo\.Lang;") {
            Write-Host "An instance of Boo.Lang was found in $FileName at line $LineNumber "
            Write-Host "Use System.Collections instead."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains an empty <returns></returns>
    or an <param ...></param> doccomment. These are typically added automatically by IDEs,
    but empty doccomments don't add value to code.
#>
function CheckEmptyDoccomment {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        $tags = @("param", "returns")
        $containsEmptyDoccomment = $false

        foreach ($tag in $tags) {
            # This generates regexes that look like:
            # ///\s*<returns[\sa-zA-Z"=]*>\s*</returns>
            # which basically looks for an empty tag (allowing for alphanumeric param names
            # and values in the tag itself)
            $matcher = "///\s*<$tag[\sa-zA-Z0-9`"=_]*>\s*</$tag>"
            if ($FileContent[$LineNumber] -match $matcher) {
                Write-Host "An empty doccomment was found in $FileName at line $LineNumber "
                Write-Host "Delete the line or add a description "
                Write-Host $FileContent[$LineNumber]
                $containsEmptyDoccomment = $true
            }
        }
        $containsEmptyDoccomment
    }
}

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains a comment of the type:
    //This comment doesn't have a space between // and This.
    There should be a space between // and the comment.
#>
function CheckSpacelessComments {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        $hasIssue = $false

        # This regex looks for any non doccomment (i.e. //, not ///) where there isn't
        # a space after the //.
        # Explanation of the stuff inside the regex:
        # \s      - matches a space, to ensure that we don't capture cases like https://
        # //      - matches '//'
        # [^\s//] - matches a single character that is not a whitespace character and also
        #           not the '/' character (because doccomments like /// <summary> would
        #           otherwise get matched).
        $matcher = "\s//[^\s//]"
        if ($FileContent[$LineNumber] -match $matcher) {
            Write-Host "Comment in $FileName at line $LineNumber is missing a space after '//'"
            Write-Host $FileContent[$LineNumber]
            $hasIssue = $true
        }

        $hasIssue
    }
}

<#
.SYNOPSIS
    Checks if the given profile contains references to Assembly-CSharp, often indicative of invalid reference.
    Returns true if such a reference exists.
#>
function CheckAssemblyCSharp {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileName -and $FileContent[$LineNumber] -match "Assembly-CSharp") {
            Write-Host "An instance of 'Assembly-CSharp' was found in $FileName at line $LineNumber"
            Write-Host "Please update this to reference the correct assembly."
            $true
        }
        $false
    }
}

<#
.SYNOPSIS
    Checks if the given profile is marked as a custom profile.
    Returns true if such a reference exists.
#>
function CheckCustomProfile {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        if ($FileName -notmatch "Examples" -and $FileContent[$LineNumber] -match "isCustomProfile: 1") {
            Write-Host "An instance of 'isCustomProfile: 1' was found in $FileName at line $LineNumber"
            Write-Host "Please update this to 'isCustomProfile: 0' instead."
            $true
        }
        $false
    }
}

<#
.SYNOPSIS
    Checks if the file has a corresponding meta checked in.
    Returns true if the meta is missing.
#>
function CheckForMetaFile {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        if (-not $FileName.EndsWith("~") -and -not (Test-Path ($FileName + ".meta"))) {
            Write-Warning "Meta file missing for $FileName. Please be sure to check it in alongside this file."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Checks if the meta file has a corresponding file checked in.
    Returns true if the file is missing.
#>
function CheckForActualFile {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        # Remove .meta from path
        if (-not (Test-Path $FileName.Substring(0, $FileName.LastIndexOf('.')))) {
            Write-Warning "Actual file missing for meta file $FileName. Please be sure to check it in or remove this meta."
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Returns true if the given line is a namespace declaration
#>
function IsNamespace {
    [CmdletBinding()]
    param(
        [string]$Line
    )
    process {
        if (($Line -match "^namespace\sMicrosoft\.MixedReality\.Toolkit") -or
            ($Line -match "^namespace\sMicrosoft\.Windows\.MixedReality")) {
            $true;
        }
        $false;
    }
}

<#
.SYNOPSIS
    Given a full filename path, this returns the MRTK project relative path
    of the file and normalizes the separators to /.
    For example, given D:\src\MixedRealityToolkit-Unity\Assets\MRTK\Services\DiagnosticsSystem\File.cs,
    this would return Assets/MRTK/Services/DiagnosticsSystem/File.cs.
    Note that this function assumes the Assets/MRTK prefix for all of the MRTK code,
    and if this ever changes this function would need to be updated to accommodate that.
#>
function GetProjectRelativePath {
    [CmdletBinding()]
    param(
        [string]$Filename
    )
    process {
        $normalizedFileName = $FileName.Replace("\", "/")
        $substringLength = $Directory.EndsWith("/") ? $Directory.Length : $Directory.Length + 1
        $assetFileName = $normalizedFileName.SubString($substringLength)
        $assetFileName
    }
}

# This set contains all of the currently allowed InitializeOnLoad handlers in MRTK.
# InitializeOnLoad handlers have a fairly dangerous impact on the inner loop speed of anyone
# using the MRTK, as they add milliseconds of time after each compile and prior to entering play mode.
# While individual handlers may not be that significant, the sum total of time across all handlers
# (which run serially) causes noticeable delays in responsiveness in the Unity editor.
$InitializeOnLoadExceptions = [System.Collections.Generic.HashSet[String]]@(
    "com.microsoft.mrtk.core/Editor/MRTKSettings.cs",
    "com.microsoft.mrtk.core/Editor/EditorProjectUtilities.cs",
    "com.microsoft.mrtk.core/Editor/MRTKProjectValidation.cs",
    "com.microsoft.mrtk.input/Editor/InputValidation.cs",
    "com.microsoft.mrtk.windowsspeech/Editor/WindowsSpeechValidation.cs"
)

<#
.SYNOPSIS
    Checks if the given file (at the given line number) contains a non-exempt
    InitializeOnLoad handler.
#>
function CheckInitializeOnLoad {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        $hasIssue = $false
        # This checks that the InitializeOnLoad string is both present and also not within
        # a // comment block (cases that are inside a comment block are perfectly okay since
        # the obviously do not have any actual effect)
        # "^\s*//" -> will match a case where the line begins with any amount of whitespace
        # followed by the two // characters.
        if (($FileContent[$LineNumber] -match "\[InitializeOnLoad") -and
                ($FileContent[$LineNumber] -notmatch "^\s*//")) {
            $assetFileName = GetProjectRelativePath($FileName)
            if (-Not $InitializeOnLoadExceptions.Contains($assetFileName)) {
                Write-Warning @"
A new InitializeOnLoad handler was introduced in: $assetFileName. An exception may be added
to `$InitializeOnLoadExceptions after discussion with the rest of the team.
"@
                $hasIssue = $true

                Write-Host "`"$assetFileName`","
            }
        }
        $hasIssue
    }
}

# The set of exempt files that are allowed to use Assembly.GetTypes()
# Note that this is used in a rough regex to check for any references to ".GetTypes()"
# which is generally good enough catch those incorrect use cases.
$AssemblyTypesExceptions = [System.Collections.Generic.HashSet[String]]@(
    "com.microsoft.mrtk.core/Utilities/Extensions/AssemblyExtensions.cs"
)

<#
.SYNOPSIS
    Checks that we don't have any references to Assembly.GetTypes(), which throws an exception for types
    that aren't loadable. Instead, callers should use the Assembly extensions GetLoadableTypes(), which wraps
    Assembly.GetTypes(), catches any unloadable types exceptions, and returns the actually loadable types.
    Note that this is mostly a heuristic to avoid having additional Assembly.GetTypes() calls (it doesn't do
    actual static analysis, just rough text analysis)
#>
function CheckAssemblyTypes {
    [CmdletBinding()]
    param(
        [string]$FileName,
        [string[]]$FileContent,
        [int]$LineNumber
    )
    process {
        $hasIssue = $false

        if ($FileContent[$LineNumber] -match "\.GetTypes\(\)") {
            $assetFileName = GetProjectRelativePath($FileName)
            if (-Not $AssemblyTypesExceptions.Contains($assetFileName)) {
                Write-Host "$FileName at line $LineNumber has a possible usage of Assembly.GetTypes()"
                Write-Host $FileContent[$LineNumber]
                Write-Host "If this is using Assembly.GetTypes(), switch to Assembly.GetLoadableTypes() instead or add to AssemblyTypesExceptions"
                $hasIssue = $true
            }
        }
        $hasIssue
    }
}

function CheckScript {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        # Each line of each script is checked by all of the validators above - this ensures that in
        # a single pass, we'll get all of the issues highlighted all at once, rather than
        # repeatedly running this script, discovering a single issue, fixing it, and then
        # re-running the script
        $containsIssue = $false
        $containsNamespaceDeclaration = $false;
        $fileContent = Get-Content $FileName
        for ($i = 0; $i -lt $fileContent.Length; $i++) {
            if (CheckBooLang $FileName $fileContent $i) {
                $containsIssue = $true
            }
            if (CheckEmptyDoccomment $FileName $fileContent $i) {
                $containsIssue = $true
            }
            if (CheckSpacelessComments $FileName $fileContent $i) {
                $containsIssue = $true
            }
            if (CheckInitializeOnLoad $FileName $fileContent $i) {
                $containsIssue = $true
            }
            if (CheckAssemblyTypes $FileName $fileContent $i) {
                $containsIssue = $true
            }
            $containsNamespaceDeclaration = $containsNamespaceDeclaration -or (IsNamespace $fileContent[$i])
        }

        # Only validate that there is a namespace declaration if it's not an AssemblyInfo.cs file.
        # These do not contain namespace declarations.
        if ((-not $containsNamespaceDeclaration) -and ($FileName -notmatch "AssemblyInfo.cs$")) {
            Write-Warning "$FileName is missing a namespace declaration (i.e. missing namespace Microsoft.MixedReality.Toolkit.*)"
            $containsIssue = $true;
        }

        if (CheckForMetaFile $FileName) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

function CheckAsset {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        # Each line of each asset is checked by all of the validators above - this ensures that in
        # a single pass, we'll get all of the issues highlighted all at once, rather than
        # repeatedly running this script, discovering a single issue, fixing it, and then
        # re-running the script
        $containsIssue = $false
        $fileContent = Get-Content $FileName
        for ($i = 0; $i -lt $fileContent.Length; $i++) {
            if (CheckCustomProfile $FileName $fileContent $i) {
                $containsIssue = $true
            }
            if (CheckAssemblyCSharp $FileName $fileContent $i) {
                $containsIssue = $true
            }
        }

        # Filter out the ProjectSettings .asset files, which don't have a meta file and don't need one.
        if ((-not $FileName.Contains("\ProjectSettings\")) -and (-not $FileName.Contains("/ProjectSettings/")) -and (CheckForMetaFile $FileName)) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

function CheckUnityScene {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        $containsIssue = $false

        # Checks if there is more than one MixedRealityPlayspace objects in each example unity scene
        $MatchesPlayspaces = Select-String MixedRealityPlayspace $FileName -AllMatches
        $NumPlayspaces = $MatchesPlayspaces.Matches.Count

        if ($NumPlayspaces -gt 1) {
            Write-Warning "There are multiple MixedRealityPlayspace objects in $FileName, delete the extra playspaces from the unity scene."
            $containsIssue = $true
        }

        if (CheckForMetaFile $FileName) {
            $containsIssue = $true
        }

        $containsIssue
    }
}

# This set contains all of the currently allowed asmdefs in the MRTK
# If you're reading this, it's probably because you've added a new asmdef
# and you're seeing a CI build/PR validation failure. This is because we've
# added a roadblock to force discussion of new asmdef creation to ensure that we're
# not creating a lot of tiny ones.
# There's non-trivial overhead to the addition of each asmdef (i.e. each asmdef will
# create build overhead associated with all of the stuff that happens before the actual code
# inside gets compiled.)
# In certain cases (especially lighting up a new platform/provider) this will be a necessary
# addition, but in others it may make more sense to put group the code with another existing
# binary that has a lot of overlap.
# Either way, this is an explicit speed bump being added to force discussion at future times.
$AsmDefExceptions = [System.Collections.Generic.HashSet[String]]@(

)

function CheckAsmDef {
    [CmdletBinding()]
    param(
        [string]$FileName
    )
    process {
        $containsIssue = $false
        #         $assetFileName = GetProjectRelativePath($FileName)
        #         if (-Not $AsmDefExceptions.Contains($assetFileName)) {
        #             Write-Warning @"
        # New Assembly Definition asset was added but is not on the allowed list: $assetFileName. An exception can be added to `$AsmDefExceptions
        # after a discussion with the rest of the team determining if the asmdef is necessary.
        # "@
        #             $containsIssue = $true
        #         }
        if ($FileName -match "com.microsoft.mrtk" -and -not (Test-Path (Join-Path (Split-Path $FileName) "AssemblyInfo.cs"))) {
            Write-Warning @"
New Assembly Definition asset was added but a corresponding AssemblyInfo.cs wasn't committed.
Please run create-assemblyinfo.ps1 from the Tooling folder in the repo, open the project in Unity to generate a meta, and commit the resulting files.
"@
            $containsIssue = $true
        }
        $containsIssue
    }
}

$containsIssue = $false
$Directory = Resolve-Path $Directory

# If the file containing the list of changes was provided and actually exists,
# this validation should scope to only those changed files.
if ($ChangesFile -and (Test-Path $ChangesFile -PathType leaf)) {
    # TODO(https://github.com/microsoft/MixedRealityToolkit-Unity/issues/7022)
    # There may be ways to configure common modules so that paths like this aren't required
    Import-Module -Force (Resolve-Path("$Directory\Pipelines\Scripts\common.psm1"))

    Write-Host "Checking only changed files for code issues: $ChangesFile"
    $changedFiles = GetChangedFiles -Filename $ChangesFile -RepoRoot $Directory
    ForEach ($changedFile in $changedFiles) {
        Write-Host "Checking file: $changedFile"
        if (((IsCSharpFile -Filename $changedFile) -and (CheckScript $changedFile)) -or
            ((IsAssetFile -Filename $changedFile) -and (CheckAsset $changedFile)) -or
            ((IsUnityScene -Filename $changedFile) -and (CheckUnityScene $changedFile)) -or
            ((IsAsmDef -Filename $changedFile) -and (CheckAsmDef $changedFile)) -or
            ((IsMetaFile -Filename $changedFile) -and (CheckForActualFile $changedFile))) {
            $containsIssue = $true;
        }
    }
}
else {
    $Paths = "$Directory/*/package.json", "$Directory/UnityProjects/*/Assets/"
    # Gets all package.json files one layer down, to determine which folders represent MRTK packages
    # Also checks TestProjects
    Get-ChildItem -Path $Paths | ForEach-Object {
        $package = $_.Directory
        if (-not $package) {
            $package = $_
        }

        Write-Host "Checking $package for common code issues"

        $codeFiles = Get-ChildItem $package *.cs -Recurse | Select-Object FullName
        foreach ($codeFile in $codeFiles) {
            if (CheckScript $codeFile.FullName) {
                $containsIssue = $true
            }
        }

        $codeFiles = Get-ChildItem $package *.asset -Recurse | Select-Object FullName
        foreach ($codeFile in $codeFiles) {
            if (CheckAsset $codeFile.FullName) {
                $containsIssue = $true
            }
        }

        # Check all Unity scenes for extra MixedRealityPlayspace objects
        $codeFiles = Get-ChildItem $package *.unity -Recurse | Select-Object FullName
        foreach ($codeFile in $codeFiles) {
            if (CheckUnityScene $codeFile.FullName) {
                $containsIssue = $true
            }
        }

        $metas = Get-ChildItem $package *.meta -File -Recurse | Select-Object FullName
        foreach ($meta in $metas) {
            if (CheckForActualFile $meta.FullName) {
                $containsIssue = $true
            }
        }

        $folders = Get-ChildItem $package -Directory -Recurse | Select-Object FullName
        foreach ($folder in $folders) {
            if (CheckForMetaFile $folder.FullName) {
                $containsIssue = $true
            }
        }

        $asmdefs = Get-ChildItem $package *.asmdef -File -Recurse | Select-Object FullName
        foreach ($asmdef in $asmdefs) {
            if (CheckAsmDef $asmdef.FullName) {
                $containsIssue = $true
            }
        }
    }
}

if ($containsIssue) {
    Write-Output "Issues found, please see above for details"
    exit 1;
}
else {
    Write-Output "No issues found"
    exit 0;
}
