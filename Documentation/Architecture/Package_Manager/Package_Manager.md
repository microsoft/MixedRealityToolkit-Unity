# Package Manager

The package manager is intended to highlight what is available from both Microsoft and the community for Mixed Reality Toolkit (MRTK). It will suplement Unity's built in package management that is available by highlighting [vetted](#package-vetting) resources for developers. The packages themselves will be NuGet packages that are installed to be consumed in Unity. This provides us with a well developed technology and publishing platform for the ecosystem. 

## Package Vetting

Package vetting for showing up in the package manager UI should be done in multiple ways. Naming is subject to change, but the below outlines the general split on how packages will be presented. 

The main goal is that all packages are contained within the Mixed Reality Toolkit Repo, or some separate repo mantained by the same community. This brings clarity to users about where issues can be filed, where to find more information about specific packages, and how they can get their packages added. This will explicitly not display any packages that are not contained within these constraints; while they may be good packages that work well with Mixed Reality Toolkit, they are not mantained within the community and will not be promoted in the same way. Instead, it will be up to the package owners to promote it's use in another way. We may find other promotion techniques if we as a community believe a package provides value but is not part of our ecosystem. 

## Toolkit Packages

Toolkit packages are promoted and mantained with some promise of stability and support (TBD). These will be packages like Azure integration that we believe work well with Mixed Reality Toolkit and explicitly provide features that the MRTK group is promoting. These packages will also have a strict process for submitting code changes.

## Extension Packages

These packages extend the provided functionality from MRTK as provided by the community. The process for code changes will be much less stringent. However, we still want to celebrate community contributions and will surface packages like this in the package manager UI. We belive these add value to MRTK, but are not mantained by the toolkit contributors directly.

## Prerelease Packages

Packages will be marked as prerelease that are considered in an alpha or beta state, and not committed to being a reliable release. This could be features that have never shipped but are being evaluated, as well as updates to packages that are available for alpha or beta testing. 