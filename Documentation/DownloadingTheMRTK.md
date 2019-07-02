# How to download the Mixed Reality Toolkit

![](../Documentation/Images/MRTK_Logo_Rev.png)

Below you'll find several methods for importing the MRTK into your project. For feedback on these methods, or to request an additional distribution method, please [file an issue on GitHub](https://github.com/Microsoft/MixedRealityToolkit-Unity/issues/new/choose)

## Unity Asset Packages - Easy to get started

The easiest way to get started is to download MRTK as a set of Unity Asset Packages. These can be downloaded from the [Releases](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases) page of the MRTK project. 

There are two asset packages for each release:

- **Foundation -** Contains the core MRTK as well as all of the code and materials needed to build Mixed Reality experiences.
- **Examples -** An optional package which contains example scenes and samples. This package depends on the **Foundation** package to function.  

With this approach, to update the version of MRTK your project is using:

1. Delete existing MRTK folders from your project. These folders begin with '*MixedRealityToolkit*'. This step should not be skipped as Unity doesn't overwrite existing files when importing a package.
2. Download the latest asset packages from the [Releases](https://github.com/Microsoft/MixedRealityToolkit-Unity/releases) page on GitHub.
3. Import the updated packages in Unity from **Assets -> Import Package -> Custom Package**

**IMPORTANT:** When using this approach be aware that a *copy* of MRTK will be added to your project. If you commit your project to source control, by default you'll also be committing your own *copy* of MRTK as well. This may be desirable in many projects, but just be aware that your copy will not remain "in sync" with ongoing development on GitHub. 

## Unity Virtual Package - Easy to share and update

If you want to share one copy of the MRTK across multiple projects, or if you frequently want to update to the latest code on GitHub, a Unity Virtual Package can help. Here's how:

1. Using your Git tool of choice, pull MRTK down to a local folder on your machine.
2. In Unity, open Package Manager using **Window -> Package Manager**.
3. Under the packages list, click on the '+' sign and select '*Add package from disk...*'
4. Browse to the local folder where you just pulled MRTK, then go into the *Assets* folder.
5. Double-click on **package.json**.

Unity will now treat the MRTK folder as if it were a Package Manager package.

With this approach, to update the version of MRTK your project is using simply pull latest changes. Unity should automatically detect any updates.

**IMPORTANT:** When using this approach please be aware that all Unity projects which reference this folder are effectively sharing the same copy of MRTK. It can be highly beneficial that updating one folder automatically updates all projects. However, you may also find it desirable to keep some projects on certain versions of the MRTK longer. This can be accomplished by pulling down different versions of MRTK into different folders.

Finally, be aware that Package Manager will write the *absolute* path of the virtual package into **manifest.json** for your project. This is fine for a single developer or a single machine, but paths often change across machines. It's possible to edit **manifest.json** and use relative paths rather than absolute ones.

## GitHub Submodule - Fine-grained control

If you want very fine-grained control over how MRTK is incorporated into individual projects, it's possible to setup MRTK as a Git Submodule. For step-by-step instructions please see [this guide](https://www.rageagainstthepixel.com/expert-import-mrtk/) by Stephen Hodgson.