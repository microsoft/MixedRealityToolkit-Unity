# Developer Documentation Guide

MRTK uses [docfx](https://dotnet.github.io/docfx/index.html) to generate html documentation out of triple slash comments in code and .md files in the MRTK repository. Docfx documentation generation is automatically triggered by CI (soon) on completed PRs in the mrtk_development branch.
The current state of the developer documentation can be found on the [MRTK github.io page](https://microsoft.github.io/MixedRealityToolkit-Unity/)

Docfx supports DFM Docfx Flavored Markdown which includes GFM Github Flavored Markdown. The full documentation and feature list can be found [here](https://dotnet.github.io/docfx/tutorial/docfx.exe_user_manual.html)

Docfx is not only converting but also checking all used local links in the documentation. If a path can't be resolved it won't be converted into it's html equivalent. Therefor it's important to only use relative paths when referring to other local files.


## Building docfx locally

The docfx build files in the MRTK repo can be used to create a local version of the developer documentation in a doc/ subfolder in the root of the project. 
### Setup
* get the latest version of [docfx](https://dotnet.github.io/docfx/index.html)
* extract the files in a folder on your computer
* add the folder to your PATH in your environment variables

### Generation
* open a powershell or cmd prompt in the root of the MRTK project
* execute docfx docfx.json (optionally with the -f option to force a rebuild of doc files)
* execute docfx serve doc (optionally with -p *portnumber* if you don't want to use the 8888 default port)
* open a web browser with localhost:*portnumber*

Note that on executing the docfx command on the json build file docfx will show any broken links in the documentation as warning. 
Please make sure whenever you perform changes on any of the documentation files or API to update all links pointing to these articles or code.

## Using crefs and hrefs in /// documented code
Docfx supports crefs in /// documented code. It will translate those references to links pointing to the generated api documentation or to external documentation websites.
External xref services for resolving links to external libraries/apis can be added to the docfx.json build settings file in the property *xrefService*.

For external apis that don't provide an xref service hrefs to the documentation website can be added to the comments.

Examples:

```
/// Links to MRTK internal class SystemType
///<see cref="Microsoft.MixedReality.Toolkit.Core.Definitions.Utilities.SystemType"/>

/// Links to external API - link provided by xref service
/// <see cref="System.Collections.Generic.ICollection{Type}.Contains"/>

/// Links to Unity web API reference
/// <see href="https://docs.unity3d.com/ScriptReference/EditorGUI.PropertyField.html">EditorGUI.PropertyField</see>
```

## Linking in .md documentation files
Docfx is translating and validating all relative local links on generation, there's no special syntax required. Referring to another documentation article should always be done by referring to the corresponding .md file, never the auto generated .html file. Please note that all links to local files need to be relative to the file you're modifying.

Linking to the API documentation can be done by using [cross references](https://dotnet.github.io/docfx/tutorial/links_and_cross_references.html). Docfx automatically generated UIDs for all API docs by mangling the signature. 

Example:

This links to the [BoundarySystem API](xref:Microsoft.MixedReality.Toolkit.Core.Definitions.BoundarySystem)
as well as this short version: @Microsoft.MixedReality.Toolkit.Core.Definitions.BoundarySystem

```
This links to the [BoundarySystem API](xref:Microsoft.MixedReality.Toolkit.Core.Definitions.BoundarySystem)
as well as this short version: @Microsoft.MixedReality.Toolkit.Core.Definitions.BoundarySystem
```

## Adding new .md files to developer docs
Docfx will pick up any .md files in folders that are added as content files in the build section of the docfx.json and generate html files out of them. For new folders a corresponding entry in the build file needs to be added. 

### Navigation entries
To determine the entries of the navigation in the developer docs docfx uses toc.yml/toc.md - table of content files. 
The toc file in the root of the project defines entries in the top navigation bar whereas the toc.yml files in the subfolders of the repo define subtopics in the sidebar navigation.
toc.yml files can be used for structuring and there can be any amount of those files. For more info about defining entries for toc.yml check the [docfx documentation entry on toc](https://dotnet.github.io/docfx/tutorial/intro_toc.html).

## Resource files
There are some files like images, videos or pdfs that the documentation can refer to but are not converted by docfx. For those files there's a resource section in the docfx.json. Files in that section will only be copied over without performing any conversion on them.

Currently there's a definition for the following resource types:

<table>
<tr>
<td><i>ResourceType</i></td> <td><i>Path</i></td>
</tr>
<tr>
<td>Images</td> <td>External/ReadMeImages/</td>
</tr>
</table>




## Good practices for developers
* Use **relative paths** whenever referring to MRTK internal pages
* Use **cross references** for linking to any MRTK API page by using the **mangled UID**
* Use **crefs and hrefs** to link to internal or external documentation in **/// comments**
* Use the indicated folders in this doc for resource files
* **Run docfx locally** and check for warnings in the output whenever you modify existing APIs or update documentation pages
* Watch out for docfx **warnings on CI** after completing and merging your PR into one of the official MRTK branches

## See also
* [DocFX](https://dotnet.github.io/docfx/index.html)
* [MRTK developer documentation on github.io](https://microsoft.github.io/MixedRealityToolkit-Unity/)

