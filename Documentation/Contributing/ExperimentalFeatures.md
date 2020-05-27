# Experimental features

Some features the MRTK team works on appear to have a lot of initial value even if we haven’t fully fleshed out the details. For these types of features, we want the community to get a chance to see them early. Because they are early in the cycle, we label them as experimental to indicate that they are still evolving, and subject to change over time.

## What to expect from an experimental feature

If a component is marked experimental you can expect the following:

- An example scene demonstrating usage, located under `MRTK/Examples/Experimental` sub-folder
- Experimental features may not have docs.
- They probably don't have tests.
- Experimental features are subject to change.

## Experimental feature guidelines

### Experimental code should live in a separate folder

Experimental code should go into a top-level experimental folder followed by the experimental feature name. For example, if trying to contribute a new feature FooBar, put code in the following:

- Example scenes, scripts go into `MRTK/Examples/Experimental/FooBar/`
- Component scripts, prefabs go into `MRTK/SDK/Experimental/FooBar/`
- Component inspectors go into `MRTK/SDK/Inspectors/Experimental/FooBar`

When using sub-folders under the experimental feature name, try to mirror the same folder structure of MRTK.

For example, solvers would go under
`MRTK/SDK/Experimental/FooBar/Features/Utilities/Solvers/FooBarSolver.cs`

Keep scenes in a scene folder near the top: `MRTK/Examples/Experimental/FooBar/Scenes/FooBarExample.unity`

> [!NOTE]
> We considered not having a single Experimental root folder and instead putting Experimental under say `MRTK/Examples/HandTracking/Scenes/Experimental/HandBasedMenuExample.unity`. We decided to go with folders at the base to make the experimental features easier to discover.

### Experimental code should be in a special namespace

Ensure that the experimental code lives in an experimental namespace that matches the non-experimental location. For example,
if your component is part of solvers at `Microsoft.MixedReality.Toolkit.Utilities.Solvers`, its namespace should be `Microsoft.MixedReality.Toolkit.Experimental.Utilities.Solvers`.

See [this PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532) for an example.

### Experimental features should have an [Experimental] attribute

Add an `[Experimental]` attribute above one of your fields to have a small dialog appear in the component editor that mentions your feature is experimental and subject to significant changes.

### Menus for experimental features should go under "Experimental" sub-menu

Ensure that experimental features are under "experimental" sub-menus when adding commands to menus in the editor. Here are a few examples:

Adding a top-level menu command:

```c#
[MenuItem("Mixed Reality Toolkit/Experimental/MyCommand")]
public static void MyCommand()
```

Adding a component menu:

```c#
[AddComponentMenu("MRTK/Experimental/MyCommand")]
```

## Documentation

Follow these steps to add documentation for your experimental feature:

1. Any documentation for an experimental feature should go in a `README.md` file in the experimental folder. For example, [`MRTK/SDK/Experimental/ScrollingObjectCollection/README.md`](../../Assets/MRTK/SDK/Experimental/ScrollingObjectCollection/README.md).

1. Under *Feature Overviews* Add a link in the *Experimental* section at [`Documentation/toc.yml`](../toc.yml).

### Minimize impact to MRTK code

While your MRTK change might get your experiment to work, it could impact other people in ways you do not expect.
Any regressions you make to the MRTK core code would result in your pull request getting reverted.

Aim to have zero changes in folders other than experimental folders. Here is a list of folders that can have experimental changes:

- MRTK/SDK/Experimental
- MRTK/SDK/Inspectors/Experimental
- MRTK/Examples/Experimental

Changes outside of these folders should be treated very carefully. If your experimental feature must include changes to MRTK core code, consider splitting out MRTK changes into a separate pull request that includes tests and documentation.

### Using your experimental feature should not impact people's ability to use core controls

Most people use core UX components like the button, ManipulationHandler and Interactable very frequently. They will likely not use your experimental feature if it prevents them from using buttons.

Using your component should not break buttons, ManipulationHandler, BoundingBox, or interactable.

For example, in [this ScrollableObjectCollection PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/6001), adding a ScrollableObjectCollection caused people to not be able to use the HoloLens button prefabs. Even though this was not caused by a bug in the PR (but rather exposed an existing bug), it prevented the PR from getting checked in.

### Provide an example scene that demonstrates how to use the feature

People need to see how to use your feature, and how to test it.

Provide an example under MRTK/Examples/Experimental/YOUR_FEATURE

### Minimize user visible flaws in experimental features

Others will not use the experimental feature if it does not work, it will not graduate to a feature.

Test your example scene on your target platform, make sure it works as expected. Make sure your feature also works in editor, so people can rapidly iterate and see your feature even if they don’t have the target platform.

## Graduating experimental code into MRTK code

If a feature ends up seeing quite a lot of use, then we should graduate it into core MRTK code. To do this, the feature should have tests, documentation, and an example scene.

When you are ready to graduate the feature MRTK, create an issue to check in your PR against. The PR should include all the things needed to make this a core feature: tests, documentation, and an example scene showing usage.

Also, don’t forget to update the namespaces to remove the “Experimental” subspace.
