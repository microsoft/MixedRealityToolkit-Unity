<img src="../../Documentation/Images/MRTK_Logo_Rev.png">

# Documentation guidelines

This document outlines the documentation guidelines and standards for the Mixed Reality Toolkit (MRTK). It's purpose is to get you started quickly by giving an introduction about the technical aspects that you need to know, to point out common pitfalls and to describe the writing style that you should try to follow.

The page itself is supposed to serve as an example, therefore it uses the intended style and the most common markup features of the documentation.

Herein you will find some general style advice as well as the standards for the following forms of the MRTK documentation:

- [Source](#source-documentation)
- [How-to](#how-to-documentation)
- [Design](#design-documentation)
- [Performance notes](#performance-notes)
- [Breaking changes](#breaking-changes)

---

## Functionality and markup

This section describes frequently needed features. To see how they work, look at the source code of the page.

1. Numbered lists
   1. Nested numbered lists with at least 3 leading blank spaces
   1. The actual number in code is irrelevant; parsing will take care of setting the correct item number.
* Bullet point lists
   * Nested bullet point lists
* Text in **bold** with \*\*double asterisk\*\*
* _italic_ *text* with \_underscore\_ or \*single asterisk\*
* Text `highlighted as code` within a sentence \`using backquotes\`
* Links to docs pages [MRTK documentation guidelines](DocumentationGuide.md)
* Links to [anchors within a page](#style); anchors are formed by replacing spaces with dashes, and converting to lowercase

For code samples we use the blocks with three backticks \`\`\` and specify *csharp* as the language for syntax highlighting:

``` csharp
int SampleFunction(int i)
{
   return i + 42;
}
```

When mentioning code within a sentence `use a single backtick`.

### TODOs

Avoid using TODOS in docs, as over time these TODOs (like code TODOs) tend to accumulate
and information about how they should be updated and why gets lost.

If it is absolutely necessary to add a TODO, follow these steps:

1. File a new issue on Github describing the context behind the TODO, and provide enough
   background that another contributor would be able to understand and then address the
   TODO.
2. Reference the issue URL in the todo in the docs.

\<\!-- TODO(https://github.com/microsoft/MixedRealityToolkit-Unity/issues/ISSUE_NUMBER_HERE): A brief blurb on the issue --\>

### Highlighted sections

To highlight specific points to the reader, use *> [!NOTE]* , *> [!WARNING]* , and *> [!IMPORTANT]* to produce the following styles. It is recommended to use notes for general points and warning/important points only for special relevant cases.

> [!NOTE]
> Example of a note

> [!WARNING]
> Example of a warning

> [!IMPORTANT]
> Example of an important comment

## Page layout

### Introduction

The part right after the main chapter title should serve as a short introduction what the chapter is about. Do not make this too long, instead add sub headlines. These allow to link to sections and can be saved as bookmarks.

### Main body

Use two-level and three-level headlines to structure the rest.

**Mini Sections**

Use a bold line of text for blocks that should stand out. We might replace this by four-level headlines at some point.

### 'See also' section

Most pages should end with a chapter called *See also*. This chapter is simply a bullet pointed list of links to pages related to this topic. These links may also appear within the page text where appropriate, but this is not required. Similarly, the page text may contain links to pages that are not related to the main topic, these should not be included in the *See also* list. See [this page's ''See also'' chapter](#see-also) as an example for the choice of links.

## Table of Contents (TOC)

Toc files are used for generating the navigation bars in the MRTK github.io documentation.
Whenever you're adding a new file to the documentation make sure that there's an entry for that file in one of the toc.yml files of the documentation folder. Only articles listed in the toc files will show up in the navigation of the developer docs. 
There can be a toc file for every subfolder in the documentation folder which can be linked into any existing toc file to add it as a subsection to the corresponding part of the navigation.

## Style

### Writing style

General rule of thumb: Try to **sound professional**. That usually means to avoid a 'conversational tone'. Also try to avoid hyperbole and sensationalism.

1. Don't try to be (overly) funny.
2. Never write 'I'
3. Avoid 'we'. This can usually be rephrased easily, sometimes you can use 'MRTK' instead. Example: "we support this feature" -> "MRTK supports this feature" or "the following features are supported ...".
4. Similarly, try to avoid 'you'. Example: "With this simple change your shader becomes configurable!" -> "Shaders can be made configurable with little effort."
5. Do not use 'sloppy phrases'.
6. Avoid sounding overly excited, we do not need to sell anything.
7. Similarly, avoid being overly dramatic. Exclamation marks are rarely needed.

### Capitalization

* Use **Sentence case for headlines**. Ie. capitalize the first letter and names, but nothing else.
* Use regular English for everything else. That means **do not capitalize arbitrary words**, even if they hold a special meaning in that context. Prefer *italic text*, if you really want to highlight certain words, [see below](#emphasis-and-highlighting).
* When a link is embedded in a sentence (which is the preferred method), the standard chapter name always uses capital letters, thus breaking the rule of no arbitrary capitalization inside text. Therefore use a custom link name to fix the capitalization. As an example, here is a link to the [bounding box](../README_BoundingBox.md) documentation.
* Do capitalize names, such as *Unity*.
* Do NOT capitalize "editor" when writing *Unity editor*.

### Emphasis and highlighting

There are two ways to emphasize or highlight words, making them bold or making them italic. The effect of bold text is that **bold text sticks out** and therefore can easily be noticed while skimming a piece of text or even just scrolling over a page. Bold is great to highlight phrases that people should remember. However, **use bold text rarely**, because it is generally distracting.

Often one wants to either 'group' something that belongs logically together or highlight a specific term, because it has a special meaning. Such things do not need to stand out of the overall text. Use italic text as a *lightweight method* to highlight something.

Similarly, when a filename, a path or a menu-entry is mentioned in text, prefer to make it italic to logically group it, without being distracting.

In general, try to **avoid unnecessary text highlighting**. Special terms can be highlighted once to make the reader aware, do not repeat such highlighting throughout the text, when it serves no purpose anymore and only distracts.

### Mentioning menu entries

When mentioning a menu entry that a user should click, the current convention is:

*Project > Files > Create > Leaf*

### Links

Insert as many useful links to other pages as possible, but each link only once. Assume a reader clicks on every link in your page, and think about how annoying it would be, if the same page opens 20 times.

Prefer links embedded in a sentence:

* BAD: Guidelines are useful. See [this chapter](DocumentationGuide.md) for details.
* GOOD: [Guidelines](DocumentationGuide.md) are useful.

Avoid external links, they can become outdated or contain copyrighted content.

When you add a link, consider whether it should also be listed in the [See also](#see-also) section. Similarly, check whether a link to your new page should be added to the linked-to page.

### Images / screenshots

**Use screenshots sparingly.** Maintaining images in documentation is a lot of work, small UI changes can make a lot of screenshots outdated. The following rules will reduce maintenance effort:

1. Do not use screenshots for things that can be described in text. Especially, **never screenshot a property grid** for the sole purpose of showing property names and values.
2. Do not include things in a screenshot that are irrelevant to what is shown. For instance, when a rendering effect is shown, make a screenshot of the viewport, but exclude any UI around it. When you have to show some UI, try to move windows around such that only that important part is in the image.
3. When you do screenshot UI, only show the important parts. For example, when talking about buttons in a toolbar, you can make a small image that shows the important toolbar buttons, but exclude everything around it.
4. Only use images that are easy to reproduce. That means do not paint markers or highlights into screenshots. First, there are no consistent rules how these should look, anyway. Second, reproducing such a screenshot is additional effort. Instead, describe the important parts in text. There are exceptions to this rule, but they are rare.
5. Obviously, it is much more effort to recreate an animated GIF. If you make one, expect to be responsible to recreate it for the rest of your life, or expect people to throw it out, if they don't want to spend that time.
6. Keep the number of images in an article low. Often a good method is to make one overall screenshot of some tool, that shows everything, and then describe the rest in text. This makes it easy to replace the screenshot when necessary.

Some other aspects:

* Default image width is 500 pixels, as this displays well on most monitors. Try not to deviate too much from it. 800 pixels width should be the maximum.
* Use PNGs for screenshots of UI.
* Use PNGs or JPGs for 3D viewport screenshots. Prefer quality over compression ratio.

### List of component properties

When documenting a list of properties, use bold text to highlight the property name, then line breaks and regular text to describe them. Do not use sub-chapters or bullet point lists.

Also, don't forget to finish all sentences with a period.

## When you are finished with a page

1. Make sure you followed the guidelines in this document.
1. Browse the document structure and see if your new document could be mentioned under the [See also](#see-also) section of other pages.
1. If available, have someone with knowledge of the topic proof-read the page for technical correctness.
1. Have someone proof-read your page for style and formatting. This can be someone unfamiliar with the topic, which is also a good idea to get feedback about how understandable the documentation is.

## Source documentation
API documentation will be generated automatically from the MRTK source files. To facilitate this, source files are required to contain the following: 

- [Class, struct, enum summary blocks](#class-struct-enum-summary-blocks)
- [Property, method, event summary blocks](#property-method-event-summary-blocks)
- [Feature introduction version and dependencies](#feature-introduction-version-and-dependencies)
- [Serialized fields](#serialized-fields)
- [Enumeration values](#enumeration-values)

In addition to the above, the code should be well commented to allow for maintenance, bug fixes and ease of customization. 

### Class, struct, enum summary blocks
If a class, struct or enum is being added to the MRTK, it's purpose must be described. This is to take the form of a summary block above the class.

```
    /// <summary>
    /// AudioOccluder implements IAudioInfluencer to provide an occlusion effect.
    /// </summary>
```

If there are any class level dependencies, they should be documented in a remarks block, immediately below the summary.

```
    /// <remarks>
    /// Ensure that all sound emitting objects have an attached AudioInfluencerController. 
    /// Failing to do so will result in the desired effect not being applied to the sound.
    /// </remarks>
```

Pull Requests submitted without summaries for classes, strutures or enums will not be approved.

### Property, method, event summary blocks
Properties, methods and events (PMEs) as well as fields are to be documented with summary blocks, regardless of code visibility (public, private, protected and internal). The documentation generation tool is responsible for filtering out and publishing only the public and protected features.

NOTE: A summary block is **not** required for Unity methods (ex: Awake, Start, Update).

PME documentation is **required** for a pull request to be approved.

As part of a PME summary block, the meaning and purpose of parameters and returned data is required.

```
        /// <summary>
        /// Sets the cached native cutoff frequency of the attached low pass filter.
        /// </summary>
        /// <param name="frequency">The new low pass filter cutoff frequency.</param>
        /// <returns>The new cutoff frequency value.</returns>

```

### Feature introduction version and dependencies
As part of the API summary documentation, information regarding the MRTK version in which the feature was introduced and any dependencies should be documented in a remarks block.

Dependencies should include extension and/or platform dependencies.

```
    /// <remarks>
    /// Introduced in MRTK version: 2018.06.0
    /// Minimum Unity version: 2018.0.0f1
    /// Minimum Operating System: Windows 10.0.11111.0
    /// Requires installation of: ImaginarySDK v2.1
    /// </remarks>
```

### Serialized fields
It is a good practice to use Unity's tooltip attribute to provide runtime documentation for a script's fields in the inspector.

So that configuration options are included in the API documentation, scripts are required to include *at least* the tooltip contents in a summary block.

```
        /// <summary>
        /// The quality level of the simulated audio source (ex: AM radio).
        /// </summary>
        [Tooltip("The quality level of the simulated audio source.")]
```

### Enumeration values
When defining and enumeration, code must also document the meaning of the enum values using a summary block. Remarks blocks can optionally be used to provide additional details to enhance understanding.

```
        /// <summary>
        /// Full range of human hearing.
        /// </summary>
        /// <remarks>
        /// The frequency range used is a bit wider than that of human
        /// hearing. It closely resembles the range used for audio CDs.
        /// </remarks>
```


## How-to documentation

Many users of the Mixed Reality Toolkit may not need to use the API documentation. These users will take advantage of our pre-made, reusable prefabs and scripts to create their experiences.

Each feature area will contain one or more markdown (.md) files that describe at a fairly high level, what is provided. Depending on the size and/or complexity of a given feature area, there may be a need for additional files, up to one per feature provided.

When a feature is added (or the usage is changed), overview documentation must be provided.

As part of this documentation, how-to sections, including illustrations, should be provided to assist customers new to a feature or concept in getting started.


## Design documentation

Mixed Reality provides an opportunity to create entirely new worlds. Part of this is likely to involve the creation of custom assets for use with the MRTK. To make this as friction free as possible for customers, components should provide design documentation describing any formatting or other requirements for art assets.

Some examples where design documentation can be helpful:
- Cursor models
- Spatial mapping visualizations
- Sound effect files

This type of documentation is **strongly** recommended, and **may** be requested as part of a pull request review. 

This may or may not be different from the design recommendation on the [MS Developer site](https://docs.microsoft.com/en-us/windows/mixed-reality/design)


## Performance notes

Some important features come at a performance cost. Often this code will very depending how they are configured.

For example:
```
When using the spatial mapping component, the performance impact will increase with the level of detail requested. It is recommended to use the least detail possible for your experience.
```

Performance notes are recommended for CPU and/or GPU heavy components and **may** be requested as part of a pull request review. Any applicable performance notes are to be included in API **and** overiew documentation. 


## Breaking changes

Breaking changes documentation is to consist of a top level [file](BreakingChanges.md) which links to each feature area's individual BreakingChanges.md.

The feature area BreakingChanges.md files are to contain the list of all known breaking changes for a given release **as well as** the history of breaking changes from past releases.

For example:
```
Spatial sound breaking changes

2018.07.2
* Spatialization of the imaginary effect is now required.
* Management of randomized AudioClip files requires an entropy value in the manager node.

2018.07.1
No known breaking changes

2018.07.0
...
```

The information contained within the feature level BreakingChanges.md files will be aggregated to the release notes for each new MRTK release.

Any breaking changes that are part of a change **must** be documented as part of a Pull Request.


## See also

* [Documentation portal generation guide](DevDocGuide.md)
