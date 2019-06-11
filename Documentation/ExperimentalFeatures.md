# Experimental Features
Some features the MRTK team works on appear to have a lot of initial value even if we haven’t fully fleshed out the details. For these types of features, we want the community to get a chance to see them early. Because they are early in the cycle, we label them as experimental to indicate that they are still evolving, and subject to change over time.

**Experimental UI/UX features are new as of the upcoming MRTK RC2 release.** You will see these features published under `MixedRealityToolkit.SDK\Experimental` and `MixedRealityToolkit.Examples\Experimental` folders. These features are in mrtk_development as of commit [8e7aba9](https://github.com/microsoft/MixedRealityToolkit-Unity/commit/8e7aba9157687421c437b694beb6760c3270d765) and will be in the RC2 release.

### What to expect from an experimental feature
If a component is marked experimental you can expect the following:
- An example scene demonstrating usage, located under `MixedRealityToolkit.Examples\Experimental` subfolder
- Experimental features may not have docs.
- They probably don't have tests.
- Experimental features are subject to change. 


# How to contribute an Experimental feature
### Experimental code should live in a separate folder
 Make sure to mirror the same folder structure of MRTK. [You can use the following PR as an example](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532). 


In that PR the experimental code went into:
`MRTK.SDK/Experimental/Features/Utilities/Solvers/HandConstraintPalmUp.cs`

The example scene went into:
`MRTK.Examples/Experimental/HandTracking/Scenes/HandBasedMenuExample.unity`

> We considered not having a single Experimental root folder and instead putting Experimental under say `MRTK.Examples/HandTracking/Scenes/Experimental/HandBasedMenuExample.unity`. We decided to go with folders at the base to make the experimental features easier to discover.

### Experimental Code should be in a special namespace
Ensure that the experimental code lives in an experimental namespace that matches the non-experimental location. For example, 
if your component is part of solvers at `Microsoft.MixedReality.Toolkit.Utilities.Solvers`, its namespace should be `Microsoft.MixedReality.Toolkit.Experimental.Utilities.Solvers`.

See [this PR](https://github.com/microsoft/MixedRealityToolkit-Unity/pull/4532) for an example. 

### Graduating experimental code into MRTK code  
If a feature ends up seeing quite a lot of use, then we should graduate it into core MRTK code. To do this, the feature should have tests, documentation, and an example scene. 

When you are ready to graduate the feature MRTK, create an issue to check in your PR against. The PR should include all the things needed to make this a core feature: tests, documentation, and an example scene showing usage. 

Also, don’t forget to update the namespaces to remove the “Experimental” subspace.


