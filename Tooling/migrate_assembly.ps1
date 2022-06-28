# Copyright (c) Microsoft Corporation.
# Licensed under the MIT License.

# Replaces SerializedReferences when a class has moved to a new assembly.
# If improperly configured, this can lock up the Unity editor and cause a massive OOM crash.

param (
    [string]$path = '.',
    [string]$old_ns = "Microsoft.MixedReality.Toolkit.CoreUX",
    [string]$new_ns = "Microsoft.MixedReality.Toolkit.UX",
    [string]$old_asm = "Microsoft.MixedReality.Toolkit.CoreUX",
    [string]$new_asm = "Microsoft.MixedReality.Toolkit.UXCore",
    [string]$old_event_asm = "Microsoft.MixedReality.Toolkit.CoreUX",
    [string]$new_event_asm = "Microsoft.MixedReality.Toolkit.UXComponents"
)

$title = "Warning! This is a destructive (but idempotent) search-replace operation that, if improperly configured, can cause your Unity editor to hang, consume infinite memory, and then crash on load.
         `n`nThis will replace all SerializedReferences to the assembly `n$old_asm `nto `n$new_asm.
         `n`nThis will replace all SerializedReferences to the namespace `n$old_ns `nto `n$new_ns.
         `n`nThis will replace all UnityEvent calls to the assembly `n$old_event_asm `nto `n$new_event_asm."
$question = "Are you sure you know what you're doing?"
$choices = '&Yes, I do!', '&No, take me back to safety.'

$decision = $Host.UI.PromptForChoice($title, $question, $choices, 1)
if ($decision -eq 0) {
    Write-Host 'Replacing!'
}
else {
    Write-Host 'Cancelled, no files altered.'
    Exit
}

# This replaces the assembly name when the serialized reference is *initially defined* in a top-level prefab or scene file.
# The serialization format is different for when a serialized reference is initially specified vs when it is overridden
# in a child.
$serref_find = "ns: $old_ns, asm: $old_asm"
$serref_replace = "ns: $new_ns, asm: $new_asm"

# This replaces the implicit assembly name in prefab and scene *overrides*.
# The suffix space is important. Only serializedreferences will serialize their assembly name
# as the first element in the "value: " line.
$override_find = "value: $old_asm $old_ns."
$override_replace = "value: $new_asm $new_ns."

# This replaces the specific type when referenced in an m_PersistentCalls array.
$event_ns_find = "value: $old_ns."
$event_ns_replace = "value: $new_ns."

# This replaces the assembly when referenced in an m_PersistentCalls array.
$event_asm_find = ", $old_event_asm"
$event_asm_replace = ", $new_event_asm"

$count = 0

$configFiles = Get-ChildItem -Path $path -include *.prefab, *.unity, *.asset -exclude *Settings* -recurse
foreach ($file in $configFiles) {
    (Get-Content $file.PSPath) |
    Foreach-Object { $_ -replace $serref_find, $serref_replace } |
    Foreach-Object { $_ -replace $override_find, $override_replace } |
    Foreach-Object { $_ -replace $event_ns_find, $event_ns_replace } |
    Foreach-Object { $_ -replace $event_asm_find, $event_asm_replace } |
    Set-Content $file.PSPath

    ((Get-Content $file.PSPath) -join "`n") + "`n" | Set-Content -NoNewline $file
    $count++
    Write-Host "." -NoNewline
}

# Renormalize line endings, if git is installed.
try { git add --renormalize $path } catch {}

Write-Host "`nChecked $count files.
            `nYou should probably use the --renormalize option when git add-ing any changes to ensure you aren't committing CRLFs from this operation."