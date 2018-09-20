# Feature Contribution Process

Adding features to the Mixed Reality Toolkit is split up into a few iteration steps, so maintainers can have time to review and and ensure the process goes smoothly.

# Feature Requirements

Features must have the following:

* Assembly Definitions for code outside of the `MixedRealityToolkit/_Core` folder.
    * This ensures features are self-contained and have no dependencies to other features.
* Be defined using an interface found in `MixedRealityToolkit/_Core/definitions/<FeatureName>System`.
* A feature's concrete manager implementation should inherit directly from `BaseManager` or `MixedRealityEventManager` if they will raise events.
* A feature's concrete manager implementation should setup and verify scene is ready for that system to use in `Initialize`.
* A feature's concrete manager should also clean up after themselves removing anything created in the scene in `Destroy`.
* Features should have a configuration profile that derives from a scriptable object.
    * If the feature is proposed to be a core feature of the Mixed Reality Toolkit, the profile must be accessible in the Main Configuration Profile.
* Be registered with the Mixed Reality Manager.
    * If the feature is a core feature, this should be hard coded into the `MixedRealityManager` and added to the `MixedRealityConfigurationProfile`.
        * This includes being able to specify a concrete implementation via dropdown using `SystemType`.
        * A default configuration profile located in `MixedRealityToolkit-SDK/Profiles` and be assigned in the default configuration profile for the Mixed Reality Manager
    * If this feature is **not** a core feature, then it must be registered using the component configuration profile and implement `IMixedRealityComponent`.
* Have a default implementation located in `MixedRealityToolkit-SDK/Features/<FeatureName>`

# Process

1. [Open a Proposal and related Tasks](#proposal)
2. [Submit an Architecture Draft or Outline](#architecture-draft)
3. [Submit final Architecture documentation](#architecture-documentation)
4. [Implement Core feature interfaces and event datum (if applicable)](#core-implementation)
5. [Implement SDK components](#sdk-implementation)
6. [Implement Examples](#example-implementation)

## Proposal

 Start by opening a proposal describing the feature or the problem you want to solve. Describe the approach and how it fits into the version of the Mixed Reality Toolkit you're targeting. This will enable everyone have a discussion about the proposal and, hopefully, identify some potential pitfalls before any work is started.

 New Proposals will be reviewed and discussed during our weekly ship room meetings. If a proposal is accepted, then supplemental tasks will be created and assigned.

## Architecture Draft

The first task will generally be to draft the initial architecture document for the feature or work to be done. This document should typically be one or two pages long and include a high level overview of the feature and how it will relate to other parts of the Mixed Reality Toolkit.

* The draft must be easy to consume with key areas highlighted.
* The draft must include a list of the proposed core interfaces, configuration profiles, and event datum.
* The draft must include a simple graphic of the proposed architecture.

>TODO: Add link to architecture draft template

Once the draft is completed, a pull requests can be made against the newly created feature branch for review by maintainers.

## Architecture Documentation

Once the draft architectural document is accepted and merged into the feature branch, additional pull requests can be made to expand and flesh out the final document.

>TODO: Add link to architecture template

Once the architecture document is approved, then the first code submissions can be made.

## Core Implementation

Now work can be done to implement the interfaces, configuration profiles, and event data that will be part of the the Mixed Reality Toolkit's core framework.

>If needed, the architectural document can be updated to align with any changes to the implementation.

## SDK Implementation

Once the core interfaces and events are finished, then work on the SDK components can start.

## Example Implementation

Once the SDK components are finished, then any examples can be updated and added.
