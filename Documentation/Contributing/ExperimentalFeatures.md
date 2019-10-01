# Experimental Features
Some features the MRTK team works on appear to have a lot of initial value even if we haven’t fully fleshed out the details. For these types of features, we want the community to get a chance to see them early. Because they are early in the cycle, we label them as experimental to indicate that they are still evolving, and subject to change over time.

### What to expect from an experimental feature
If a component is marked experimental you can expect the following:
- An example scene demonstrating usage, located under `MixedRealityToolkit.Examples\Experimental` subfolder
- Experimental features may not have docs.
- They probably don't have tests.
- Experimental features are subject to change. 


# Experimental feature guidelines
### Experimental code should live in a separate folder
 Make sure to mirror the same folder structure of MRTK. [You can use the following PR as an example](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532). 


In that PR the experimental code went into:
`MRTK.SDK/Experimental/Features/Utilities/Solvers/HandConstraintPalmUp.cs`

The example scene went into:
`MRTK.Examples/Experimental/HandTracking/Scenes/HandBasedMenuExample.unity`

> [!NOTE]
> We considered not having a single Experimental root folder and instead putting Experimental under say `MRTK.Examples/HandTracking/Scenes/Experimental/HandBasedMenuExample.unity`. We decided to go with folders at the base to make the experimental features easier to discover.

### Experimental Code should be in a special namespace
Ensure that the experimental code lives in an experimental namespace that matches the non-experimental location. For example, 
if your component is part of solvers at `Microsoft.MixedReality.Toolkit.Utilities.Solvers`, its namespace should be `Microsoft.MixedReality.Toolkit.Experimental.Utilities.Solvers`.

See [this PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532) for an example. 

### Minimize impact to MRTK code
While your MRTK change might get your experiment to work, it could impact other people in ways you do not expect.
Any regressions you make to the MRTK core code would result in your pull request getting reverted. 

Aim to have zero impact on MRTK core code. 

If you do have impact on MRTK code, write tests to ensure that your changes do not regress in the future.

### Using you experimental feature should not impact people's ability to use core controls
Most people use core UX components like the button, ManipulationHandler and Interactable very frequently. They will likely not use your experimental feature if it prevents them from using buttons. 

Using your component should not break buttons, ManipulationHandler, BoundingBox, or interactable.

For example, in [this ScrollableObjectCollection PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/6001), adding a ScrollableObjectCollection caused people to not be able to use the HoloLens button prefabs. Even though this was not caused by a bug in the PR (but rather exposed an existing bug), it prevented the PR from getting checked in.

### Provide and example scene that demonstrates how to use the feature
People need to see how to use your feature, and how to test it.

Provide an example under MRTK.Examples/Experimental/YOUR_FEATURE

### In the pull request, provide screenshots of videos of the feature
Images and videos often make it easier for others to understand your contribution.

If adding features that contain UX, add an image / gif of the feature you are changing. Here is a good example: https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532

Another suggestion is to have a gif of Before and After, for example in this pull request: https://github.com/microsoft/MixedRealityToolkit-Unity/pull/5896


### Minimize user visible flaws in experimental features
Others will not use the experimental feature if it does not work, it will not graduate to a feature.

Test your example scene on your target platform, make sure it works as expected.

Make sure your feature also works in editor, so people can rapidly iterate and see your feature even if they don’t have the target platform.


## Graduating experimental code into MRTK code  
If a feature ends up seeing quite a lot of use, then we should graduate it into core MRTK code. To do this, the feature should have tests, documentation, and an example scene. 

When you are ready to graduate the feature MRTK, create an issue to check in your PR against. The PR should include all the things needed to make this a core feature: tests, documentation, and an example scene showing usage. 

Also, don’t forget to update the namespaces to remove the “Experimental” subspace.
