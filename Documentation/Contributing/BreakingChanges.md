# Breaking changes

Consumers of the MRTK depend on having a stable release-to-release API surface, so that they can take updates to the MRTK without having large breaking changes each time.

This page describes our current policy regarding breaking changes in the MRTK, along with some longer term goals around how we can better manage the tradeoff between keeping breaking changes low and being able to make the right long term technical changes to the code.

## What is a breaking change?

A change is a breaking change if it satisfies any of the conditions in the [List A](#list-a) AND satisfies all of the conditions in [list B](#list-b)

### List A

- The addition, removal, or update of any member or function of any interface (or removal/rename of the entire interface).
- The removal, update (changing type/definition, making private or internal) of any protected or public member or function of class. (or removal/rename of the entire class).
- The change in the order of events fired by a class.
- The rename of any private SerializedField (without a corresponding FormerlySerializedAs tag) or public property on a ScriptableObject (especially changes to profiles).
- Changing the type of a field on a ScriptableObject (especially changes to profiles).
- Updates to the namespace or asmdefs of any class or interface.
- Removal of any prefab or removal of a script on the top level object of a prefab.

### List B

- The asset in question is in the foundation package (i.e. it's in one of the following folders):

  - MRTK/Core
  - MRTK/Providers/
  - MRTK/Services/
  - MRTK/SDK/
  - MRTK/Extensions

- The asset in question does not belong to the experimental namespace.

> [!IMPORTANT]
> Any asset that sits in the examples package (i.e. part of the MRTK/Examples/ folder) is subject to change at any time, as assets there are designed to be copied and viewed by consumers as 'reference implementations' but are not part of the core set of APIs and assets. Assets in the experimental namespace (or more generally, features labelled as experimental) are ones that get published before all due diligence has been done (i.e. tests, UX iteration, documentation) and is published early to get feedback sooner.  However, because they don't have tests and documentation, and because we likely haven't nailed down all of the interactions and designs, we publish them in a state where the public should assume that they can and will change (i.e. be modified, completely removed, etc).
>
> See [Experimental features](../Contributing/ExperimentalFeatures.md) for more information.

As the surface area for breaking changes is very large, it's important to note that having an absolute
rule that says "no breaking changes" would be impossible - there may be issues that can only be fixed in
a sane way by having a breaking change. To put another way, the only way we could really have "no breaking changes"
is to have no changes at all.

Our standing policy is to avoid making breaking changes if possible, and only do so if the change would
accrue significant customer or framework long term value.

## What to do about breaking changes

If it is possible to accomplish something without a breaking change and without compromising the long term structure and viability of the feature, don't do the breaking change. If there is no other way, the current policy is to evaluate each individual breaking change, to understand if the benefit from taking the change outweighs the cost to the consumer of absorbing the change. Debate about what is worth doing and what isn't will generally take place on the PR or issue discussion itself.

What can happen here falls into several buckets:

### The breaking change adds value but could be written in a way that isn't breaking

For example, [this PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4882) added a new feature that was initially written in a way that was breaking - it modified an existing interface - but was then rewritten where the feature was broken out as its own interface. This is generally the best possible outcome. Do not try to force a change into a non-breaking form if doing so would compromise the long term viability or structure of the feature.

### The breaking change adds sufficient value to the customer that it's worth doing

Document what the breaking changes are and provide the best possible mitigation (i.e. prescriptive steps on how to migrate, or better yet tooling that will automatically migrate for the customer). Each release may contain a small amount of changes that are breaking - these should always be documented in docs as was done in [this PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4858). If there already is a 2.x.x→2.x+1.x+1 migration guide, then add instructions or tooling to that doc. If it doesn't exist, create it.

### The breaking change adds value but the customer pain would be too high

Not all types of breaking changes are created equal - some are significantly more painful that others, based on our experience and based on customer experiences. For example, changes to interfaces may be
painful, but if the breaking change is one in which a customer is unlikely to have extended/implemented in the past (the diagnostic visualization system, for example), then the actual cost is probably low to nothing. However, if the change is the type of a field on a ScriptableObject (for example, on one of the core profiles of the MRTK), this is likely to cause massive customer pain. Customers have already cloned the default profile, merging/updating profiles can be extremely hard to do manually (i.e. via a text editor during merge time), and re-copying the default profile and reconfiguring everything by hand is extremely likely to lead to hard to debug regressions.

These changes we have to put back onto the shelf until a branch exists that will allow significantly breaking changes (along with significant value that will give customers a reason to upgrade). Such a branch doesn't currently exist. In our future iteration planning meetings, we will review the set of changes/issues that were 'too breaking' to see if we reached a critical mass to make it reasonable to pursue a set of changes all at once. Note that it's dangerous to spin up a "everything is allowed" branch without due diligence being done because of the limited engineering resources we have, and the fact that we'd have to split testing and validation across those two. There needs to be a clear purpose and well-communicated start and end date of such a branch when it exists.

## Long term management of breaking changes

In the long term, we should seek to reduce the scope of what is a breaking change by increasing the set of conditions in [List B](#list-b). Going forward the set of things in [List A](#list-a) will always technically be breaking for the set of files and assets that we deem to be in the "public API surface." The way that we can get a little more freedom for iteration (i.e. changing up the internal implementation
details, allowing for easier refactoring and sharing of code between multiple classes, etc) is to be more explicit about which portions of the code are official surface, rather than implementation detail.

One thing we've already done is introduce the concept of an "experimental" feature (it belongs in the experimental namespace, it may not have tests/documentation, and is publicly proclaimed to exist but may be removed and updated without warning). This has given has freedom to add new features sooner to get earlier feedback, but not be immediately tied to its API surface (because we may not have fully thought out the API surface).

### Other examples of things that could help in the future

- Usage of the [internal keyword](https://docs.microsoft.com/dotnet/csharp/language-reference/keywords/internal).
  This would allow for us to have shared code within our own assemblies (for reducing code duplication) without making things public to external consumers.
- Creation of an "internal" namespace (i.e. Microsoft.MixedReality.Toolkit.Internal.Utilities),
  where we publicly document that anything contained within that internal namespace is subject to change at anytime and could be removed, etc. This is similar to how C++ header libraries will make use of ::internal namespaces to hide their implementation details.
