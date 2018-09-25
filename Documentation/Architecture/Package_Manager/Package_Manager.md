# Package Manager

The package manager is intended to highlight what is available from both Microsoft and the community for Mixed Reality Toolkit. It will suplement Unity's built in package management that is available by highlighting [vetted](#package-vetting) resources for developers.

## Package Vetting

Package vetting for showing up in the package manager UI should be done in multiple ways. Naming is subject to change, but the below outlines the general split on how packages will be presented. 

The main goal is that all packages are contained within the Mixed Reality Toolkit Repo, or some separate repo mantained by the same community. This brings clarity to users about where issues can be filed, where to find more information about specific packages, and how they can get their packages added. This will explicitly not display any packages that are not contained within these constraints; while they may be good packages that work well with Mixed Reality Toolkit, they are not mantained within the community and will not be promoted in the same way. Instead, it will be up to the package owners to promote it's use in another way. We may find other promotion techniques if we as a community believe a package provides value but is not part of our ecosystem. 

### Release Packages

Release packages are promoted and mantained with some promise of stability and support (TBD). These will be packages like Azure integration that we believe work well with Mixed Reality Toolkit and explicitly provide features that Microsoft is promoting. These packages will also have a strict process for submitting code changes.

### Preview Packages

These packages contain less promise to quality and maintainance, but still exist within the ecosystem. The process for code changes will be much less stringent. However, we still want to celebrate community contributions and will surface packages like this in the package manager UI. 

# Interfaces

No interfaces will be added to the core of Mixed Reality Toolkit. Everything in the package manager will not be customizable by users. All modifications to a project will go through the [Unity Package Manager](https://docs.unity3d.com/Packages/com.unity.package-manager-ui@1.8/manual/index.html#PackManRegistry)

# Classes

# Enumerations

# Event Data Types

# Configuration Profile

None