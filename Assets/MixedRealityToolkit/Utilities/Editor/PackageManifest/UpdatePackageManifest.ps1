# Copyright (c) Microsoft Corporation. All rights reserved.
# Licensed under the MIT License. See LICENSE in the project root for license information.


#[System.Reflection.Assembly]::LoadWithPartialName("System.Runtime.Serialization")

# object model
[System.Runtime.Serialization.DataContract()]
class PackageRegistry
{
    [System.Runtime.Serialization.DataMember(Name = "name")]
    [string] $Name;

    [System.Runtime.Serialization.DataMember(Name = "url")]
    [string] $Url;

    [System.Runtime.Serialization.DataMember(Name = "scopes")]
    [System.Collections.Generic.List[string]] $Scopes;
}

[System.Runtime.Serialization.DataContract()]
class PackageManifest
{
    [System.Runtime.Serialization.DataMember(Name = "registry")]
    [string] $RegistryName;

    [System.Runtime.Serialization.DataMember(Name = "scopedRegistries")]
    [System.Collections.Generic.List[PackageRegistry]] $Registries;

    [System.Runtime.Serialization.DataMember(Name = "dependencies")]
    [System.Collections.Generic.Dictionary[string, string]] $Packages;
}

# return the path to the manifest.json file
function GetPackageManifestFile()
{
    $scriptPath = Split-Path $PSCommandPath -Parent

    # this script is to reside in a sub folder under assets
    [string[]] $pathComponents = $scriptPath.Split("\")
    [int] $assetsIndex = $pathComponents.IndexOf("Assets")
    if ($assetsIndex -eq -1)
    {
        $assetsIndex = $pathComponents.IndexOf("assets")
    }
    if ($assetsIndex -eq -1)
    {
        # Faied to find project root
        return ""
    }

    # todo: error handling here
    [string] $projectRoot = ""
    for ([int] $i = 0; $i -lt $assetsIndex; $i++)
    {
        $projectRoot = ($projectRoot + $pathComponents[$i] + "\")
    }

    return ($projectRoot + "Packages\manifest.json")
}

# script start

$packageManifestFile = (GetPackageManifestFile)
if ($packageManifestFile -eq "")
{
    Write-Host
    Write-Error "Unable to locate the package manifest file."
    Write-Host
    exit
}

Write-Host
Write-Host "Updating" (GetPackageManifestFile)
Write-Host

# create and set serializer settings
[System.Runtime.Serialization.Json.DataContractJsonSerializerSettings] $serializerSettings = New-Object System.Runtime.Serialization.Json.DataContractJsonSerializerSettings
$serializerSettings.UseSimpleDictionaryFormat = 1

# create the serializer
[System.Type] $serializedObjectType = [System.Type]([PackageManifest])
[System.Runtime.Serialization.Json.DataContractJsonSerializer] $serializer = New-Object System.Runtime.Serialization.Json.DataContractJsonSerializer($serializedObjectType, $serializerSettings)

if ($null -eq $serializer)
{
    Write-Host
    Write-Error "Failed to create the manifest serializer."
    Write-Host
    exit
}

# read the manifest
[System.IO.FileStream] $fileStream = New-Object System.IO.FileStream($packageManifestFile, 3, 1) # Open, Read
if ($null -eq $fileStream)
{
    Write-Host
    Write-Error "Failed to open the manifest file."
    Write-Host
    exit
}
[PackageManifest] $manifest = $serializer.ReadObject($fileStream)
if ($null -eq $manifest)
{
    Write-Host
    Write-Error "Failed to read the manifest file."
    Write-Host
    exit
}
$fileStream.Close()

# ensure all collections exist
if ($null -eq $manifest.Registries)
{
    $manifest.Registries = New-Object System.Collections.Generic.List[PackageRegistry]
}
if ($null -eq $manifest.Packages)
{
    $manifest.Packages = New-Object System.Collections.Generic.Dictionary[string, string]
}

# TESTTEST
Write-Host "Registries "
foreach ($registry in $manifest.Registries)
{
    Write-Host "  " $registry.Name
    Write-Host "  " $registry.Url;
    Write-Host "  " $registry.Scopes

}
Write-Host "Packages "
Write-Host $manifest.Packages

# check for the msbuild scoped registry
[string] $msBuildRegistryUrl = "https://pkgs.dev.azure.com/UnityDeveloperTools/MSBuildForUnity/_packaging/UnityDeveloperTools/npm/registry"
[bool] $msBuildRegistryFound = $false

foreach($registry in $manifest.Registries)
{
    if ($registry.Url -eq $msBuildRegistryUrl)
    {
        $msBuildRegistryFound = $true
        break
    }
}

# add the msbuild registry, if needed
if (-not $msBuildRegistryFound)
{
    Write-Host "Adding MSBuild for Unity registry"
    
    [PackageRegistry] $msBuildRegistry = New-Object PackageRegistry
    $msBuildRegistry.Name = "MS Build for Unity"
    $msBuildRegistry.Url = $msBuildRegistryUrl
    $msBuildRegistry.Scopes = New-Object System.Collections.Generic.List[string]
    $msBuildRegistry.Scopes.Add("com.microsoft")

    $manifest.Registries.Add($msBuildRegistry)
}

# check for the msbuild package
[string] $msBuildPackageName = "com.microsoft.msbuildforunity"
[string] $msBuildPackageVersion = "0.8.0"
[bool] $msBuildPackageFound = $false

foreach ($package in $manifest.Packages)
{
    if ($package.Name -eq $msBuildPackageName)
    {
        $msBuildPackageFound = $true;
    }
}

# add the msbuild package, if needed
if (-not $msBuildPackageFound)
{
    Write-Host "Adding MSBuild for Unity package"
    
    $manifest.Packages.Add($msBuildPackageName, $msBuildPackageVersion)
}

# TESTTEST
Write-Host "Registries "
foreach ($registry in $manifest.Registries)
{
    Write-Host "  " $registry.Name
    Write-Host "  " $registry.Url;
    Write-Host "  " $registry.Scopes

}
Write-Host "Packages "
Write-Host $manifest.Packages

# # write the manifest, if changes were made
# if ((-not $msBuildRegistryFound) -or (-not $msBuildPackageFound))
# {
#     Write-Host "Saving modified " $packageManifestFile\

#     [System.IO.FileStream] $fileStream = New-Object System.IO.FileStream($packageManifestFile, 4, 2) # OpenOrCreate, Write
#     if ($null -eq $fileStream)
#     {
#         Write-Host
#         Write-Error "Failed to open the manifest file."
#         Write-Host
#         exit
#     }
#     $serializer.WriteObject([System.IO.FileStream] $fileStream, $manifest)
#     $fileStream.Flush()
#     $fileStream.Close()    
# }
