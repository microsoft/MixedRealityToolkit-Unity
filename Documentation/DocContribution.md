---
title: MRTK Docs - What you need to know
description: 
author: 
ms.author: 
ms.date:
keywords: 
---

# MRTK Docs - What you need to know

This page is for everyone who writes MRTK documentation. It's purpose is to get you started quickly by giving an introduction about the technical aspects that you need to know, to point out common pitfalls and to describe the writing style that you should try to follow.

The page itself is supposed to serve as an example, therefore it uses the intended style and the most common markup features of the documentation.

There is a general [markdown guidance](https://docs.microsoft.com/en-us/vsts/collaborate/markdown-guidance) for VSTS.

## Functionality and markup

This page describes frequently needed features. To see how they work, look at the source code of the page.

1. Numbered lists
   1. Nested numbered lists with at least 3 leading blank spaces
   1. The actual number in code is irrelevant; parsing will take care of setting the correct item number.
* Bullet point lists
   * Nested bullet point lists
* Text in **bold** with \*\*double asterisk\*\*
* _italic_ *text* with \_underscore\_ or \*single asterisk\*
* Text `highlighted as code` within a sentence \`using backquotes\`
* Links to docs pages [MRTK Docs Contribution](DocContribution.md)
* Links to [anchors within a page](#style); anchors are formed by replacing spaces with dashes, and converting to lowercase

For code samples we use the blocks with three backticks \`\`\` and specify *csharp* as the language for syntax highlighting:

``` cpp
int SampleFunction(int i)
{
   return i + 42;
}
```

When mentioning code within a sentence `use a single backtick`.

### TODO comments

To mark areas in the documentation that need to be revisited later again, please use the following comment style, to make it easy to find those places again:

\<\!-- TODO: add more details --\>

Also consider adding pages with TODOs to the [documentation TODO list](DocsToDo.md).


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

1. Don't try to be funny. Seriously.
2. Never write 'I'
3. Avoid 'we'. This can usually be rephrased easily, sometimes you can use 'MRTK' instead. Example: "we support this feature" -> "MRTK supports this feature" or "the following features are supported ...".
4. Similarly, try to avoid 'you'. Example: "With this simple change your shader becomes configurable!" -> "Shaders can be made configurable with little effort."
5. Do not use 'sloppy phrases'.
6. Avoid sounding overly excited, we do not need to sell anything.
7. Similarly, avoid being overly dramatic. Exclamation marks are rarely needed.

### Capitalization

* Use **Sentence case for headlines**. Ie. capitalize the first letter and names, but nothing else.
* Use regular English for everything else. That means **do not capitalize arbitrary words**, even if they hold a special meaning in that context. Prefer *italic text*, if you really want to highlight certain words, [see below](#emphasis-and-highlighting).
* When a link is embedded in a sentence (which is the preferred method), the standard chapter name always uses capital letters, thus breaking the rule of no arbitrary capitalization inside text. Therefore use a custom link name to fix the capitalization. As an example, here is a link to the [bounding box](README_BoundingBox.md) documentation.
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

* BAD: Guidelines are useful. See [this chapter](DocContribution.md) for details.
* GOOD: [Guidelines](DocContribution.md) are useful.

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

First off, it is ok for a 'finished' page to contain [TODO comments](#todo-comments). However, once you stop working on a page, please do the following:

1. Make sure you followed the guidelines in this document.
1. Browse the document structure and see if your new document could be mentioned under the [See also](#see-also) section of other pages.
1. If available, have someone with knowledge of the topic proof-read the page for technical correctness.
1. Have someone proof-read your page for style and formatting. This can be someone unfamiliar with the topic, which is also a good idea to get feedback about how understandable the documentation is.

## See also

* [Docfx Documentation Guide](DevDocGuide.md)