# Microsoft Mixed Reality Toolkit 2.5.0 release notes

- [What's new](#whats-new)
- [Breaking changes](#breaking-changes)
- [Updating guidance](Updating.md#upgrading-to-a-new-version-of-mrtk)
- [Known issues](#known-issues)

### What's new

**Enable MSBuild for Unity removed from the configuration dialog**

To prevent the MRTK configuration dialog from repeatedly displaying when `Enable MSBuild for Unity` is unchecked, it has been moved to the `Mixed Reality Toolkit' menu as shown in the following image.

![MSBuild for Unity menu items](Images/ConfigurationDialog/MSB4UMenuItems.png)

This change also adds the ability to remove MSBulid for Unity.

There is a confirmation dialog that will be displayed when selecting `Use MSBuild for Unity dependency resolution`.

![MSBuild for Unity confirmation](Images/ConfigurationDialog/EnableMSB4UPrompt.png)

### Breaking changes

**Rest / Device Portal API**

The `UseSSL` static property has been moved from `Rest` to `DevicePortal`.

If you did this previously...

```csharp
Rest.UseSSL = true
```

Do this now...

```csharp
DevicePortal.UseSSL = true
```

### Known issues
