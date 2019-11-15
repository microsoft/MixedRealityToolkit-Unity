<#
 # Copies files required for Unity 2019 into the project structure.
 #>

$scriptPath = Split-Path $PSCommandPath -Parent
$manifestPath = "Packages\manifest.json"
$arFoundationAsmdefPath = "Assets\MixedRealityToolkit.Staging\UnityAR\Microsoft.MixedReality.Toolkit.Providers.UnityAR.asmdef"
$projectPath = Split-Path (Split-Path $scriptPath -Parent) -Parent

Copy-Item (Join-Path $scriptPath $manifestPath) (Join-Path $projectPath $manifestPath)
Copy-Item (Join-Path $scriptPath $arFoundationAsmdefPath) (Join-Path $projectPath $arFoundationAsmdefPath)