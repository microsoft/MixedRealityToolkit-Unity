# Known Issues

This page covers common issues that one might encounter when using the MRTK and their corresponding
workarounds. Note that this page is not exhaustive of all issues - the
full set of [known issues is tracked on Github](https://github.com/microsoft/MixedRealityToolkit-Unity/issues).

## Long Paths

When building on Windows, there is a MAX_PATH limit of 255 characters. Unity is affected by these
limits and may fail to build a binary if its resolved output path is longer than 255 characters.

This can manifest as CS0006 errors in Visual Studio that look like:

```
CS0006: Metadata file 'C:\path\to\longer\file\that\is\longer\than\255\characters\mrtk.long.binary.name.dll'
could not be found.
```

This can be worked around by moving your Unity project folder closer to the root of the drive:

```
C:\src\project
```

See [this issue](https://github.com/microsoft/MixedRealityToolkit-Unity/issues/5469) for more background information.
