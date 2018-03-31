<img src="External/ReadMeImages/MRTK_Logo_Rev.png">

# Documentation Guidelines

This document outlines the documentation guidelines and standards for the Mixed Reality Toolkit (MRTK). Herein you will find the standards for the following formsfs of the MRTK documentation:

- API / Source
- Conceptual / How-To
- Design
- Performance Notes
- Breaking Changes

---

## API / Source Documentation
API documentation will be generated automatically from the MRTK source files. To facilitate this, source files are required to contain the following: 

- Class, Struct, Enum Summary Blocks
- Property, Method, Event (PME) Summary Blocks

In addition to the above, the code should be well commented to allow for maintenance, bug fixes and ease of customization. 

### Class, Struct, Enum Summary Blocks
If a class, struct or enum is being added to the MRTK, it's purpose must be described. This is to take the form of a summary block above the class.

```
    /// <summary>
    /// AudioOccluder implements IAudioInfluencer to provide an occlusion effect.
    /// </summary><< >>
```

If there are any class level dependencies, they should be documented in a remarks block, immediately below the summary.

```
    /// <remarks>
    /// Ensure that all sound emitting objects have an attached AudioInfluencerController. 
    /// Failing to do so will result in the desired effect not being applied to the sound.
    /// </remarks>
```

Pull Requests submitted without summaries for classes, strutures or enums will not be approved.

### Property, Method, Event (PME) Summary Blocks
As with classes, structures and enums, PMEs (and fields) are to be documented with summary blocks, regardless of code visibility (public, private, protected and internal). The documentation generation tool is responsible for filtering out and publishing only the public and protected features.

NOTE: A summary block is **not** required for Unity methods (ex: Awake, Start, Update).

PME documentation is **required** for a Pull Request to be approved.

As part of a PME summary block, the meaning and purpose of parameters and returned data is required.

```
        /// <summary>
        /// Sets the cached native cutoff frequency of the attached low pass filter.
        /// </summary>
        /// <param name="frequency">The new low pass filter cutoff frequency.</param>
        /// <returns>The new cutoff frequency value.</remarks>

```

#### Feature Introduction Version and Dependencies
As part of the PME summary documentation, information regarding the MRTK version in which the feature was introduced and any dependencies should be documented in a remarks block.

Dependencies should include extension and/or platform dependencies.

```
    /// <remarks>
    /// Introduced in MRTK version: 2018.06.0 
    /// Requires installation of: ImaginarySDK v2.1
    /// Minimum Operating System: Windows 10.0.11111.0
    /// </remarks>
```

#### Serialized Fields Visible in the Unity Inspector
It is a good practice to use Unity's Tooltip attribute to provide runtime documentation for a script's fields in the Inspector.

So that configuration options are included in the API documentation, scripts are required to include *at least* the tooltip contents in a summary block.

```
        /// <summary>
        /// The quality level of the simulated audio source (ex: AM radio).
        /// </summary>
        [Tooltip("The quality level of the simulated audio source.")]
```

#### Enumeration Values
When defining and enumeration, code must also document the meaning of the enum values using a summary block. Remarks blocks can optionally be used to provide additional details to enhance understanding.

---

## Conceptual Documentation

Many users of the Mixed Reality Toolkit may not need to use the API documentation. These users will take advantage of our pre-made, reusable prefabs and scripts to create their experiences.

Each feature area will contain one or more markdown (.md) files that describe at a fairly high level, what is provided. Depending on the size and/or complexity of a given feature area, there may be a need for additional files, up to one per feature provided.

When a feature is added (or the usage is changed), overview documentation must be provided.

As part of this documentation, how-to sections, including illustrations, should be provided to assist customers new to a featur or concept in getting started.

---

## Design Documentation

Mixed Reality provides an opportunity to create entirely new worlds. Part of this is likely to involve the creation of custom assets for use with the MRTK. To make this as friction free as possible for customers, components should provide design documentation describing any formatting or other requirements for art assets.

Some examples where design documentation can be helpful:
- Cursor models
- Spatial Mapping visualizations
- Sound effect files

This type of documentation is **strongly** recommended, and may be requested as part of a Pull Request review. 

---

## Breaking Changes

Breaking changes documentation is to consist of a top level [file](/BreakingChanges.md) which links to each feature area's individual BreakingChanges.md.

The feature area BreakingChanges.md files are to contain the list of all known breaking changes for a given release **as well as** the history of breaking changes from past releases.

For example:
fs```
Spatial Sound Breaking Changes

2018.07.2
* Spatialization of the imaginary effect is now required.
* Management of randomized AudioClip files requires an entopy value in the manager node.

2018.07.1
No known breaking changes

2018.07.0
...
```

The information contained within the feature level BreakingChanges.md files will be aggregated to the release notes for each new MRTK release.

Breaking changes **must** be documented as part of a Pull Request.

