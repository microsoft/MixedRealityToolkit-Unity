# Feature Contribution Process

Adding features to the Mixed Reality Toolkit (MRTK) is split up into a few iteration steps, so maintainers can have time to review and and ensure the process goes smoothly. Please be sure to review the list of [feature requirements](#new-feature-requirements) before you get started.

# Process
The following process has been drafted to ensure all new work complies to the updated standards and architecture defined for the MRTK, this has been defined as:

1. [Open a new Proposal and related Tasks](#new-proposal)
2. [Submit an Architecture Draft or Outline](#architecture-draft)
3. [Review and finalize the Architecture documentation](#architecture-documentation)
4. [Submit a PR implementing the Core feature interfaces and event datum (if applicable)](#core-implementation)
5. [Submit a PR Implementing any required SDK components](#sdk-implementation)
6. [Submit a PR Implementing feature demos or full scale Examples](#example-implementation)

## New Proposal

 Start by opening a new Proposal or Task describing the feature or the problem you want to solve. Describe the approach and how it fits into the version of the Mixed Reality Toolkit you're targeting. This will enable everyone have a discussion about the proposal and, hopefully, identify some potential pitfalls before any work is started.

 New Proposals will be reviewed and discussed during our weekly ship room meetings and if a proposal is accepted, supplemental tasks will then be created and assigned.

## Architecture Draft

The first task once the initial proposal has been accepted, will be to draft the initial architecture document for the feature or work to be done. This document should typically be one or two pages long and include a high level overview of the feature and how it will relate to other parts of the Mixed Reality Toolkit.

* The draft must be easy to consume with key areas highlighted.
* The draft must include a list of the proposed core interfaces, configuration profiles, and event datum.
* The draft must include a simple graphic of the proposed architecture.

Ensure that the architecture of the feature complies with the [New Feature Requirements](#new-feature-requirements) set out by the Core MRTK architecture.

>TODO: Add link to architecture draft template

Once the draft is completed, this can be appended to the Proposal / Task issue on GitHub for final public review.

## Architecture Documentation

Once the draft architecture is accepted, additional pull requests can be made to submit the final full architecture documents to the repository.

>TODO: Add link to the full architecture template

Once the architecture document is approved, only then can the first code submissions can be made.

Development can begin in your own private branch and complete as normal, however, the PR's submitted back to the core MRTK project should be submitted in stages to ensure the review and approval is as smooth as it can be (and ensure core changes do not impact other features)

## Core Implementation

The initial work that should be submitted, is to implement:

* Definitions
* Interfaces
* Configuration profiles
* Event data

> If needed, the architectural document can be updated to align with any changes to the implementation.

Please ensure that all existing Unit Tests and any new tests are all passing prior to submission.

## SDK Implementation

Once the core interfaces and events are merged in to development, work can then be submitted for the SDK components.  Adding the concrete implementation of the feature and testing against the supported platforms and unit tests.

## Example Implementation

Once the SDK components are merged, then any demo scenes or updates to the example scenes can be submitted.

* Demos are for specific feature highlighting and demonstration
* Examples are full working scene learning examples

# New Feature Requirements

Most feature implementations can be broken down into 3 main parts:

1. [The Feature Manager](#manager-implementation-requirements)
2. [The Event Data](#event-data-implementation-requirements) (Optional)
3. [The Feature Handler](#handler-implementation-requirements) (Optional)

## Manager Implementation Requirements

* Assembly Definitions for code outside of the `MixedRealityToolkit/_Core` folder.
    * This ensures features are self-contained and have no dependencies to other features.
    * This only applies to `MixedRealityToolkit` folder.
* Be defined using an interface found in `MixedRealityToolkit/_Core/definitions/<FeatureName>System`.
* A feature's concrete manager implementation should inherit directly from `BaseManager` or `MixedRealityEventManager` if they will raise events.
* A feature's concrete manager implementation should setup and verify scene is ready for that system to use in `Initialize`.
* A feature's concrete manager should also clean up after themselves removing anything created in the scene in `Destroy`.
* Be registered with the Mixed Reality Manager.
    * If the feature is a core feature, this should be hard coded into the `MixedRealityManager` and added to the `MixedRealityConfigurationProfile`.
        * This includes being able to specify a concrete implementation via dropdown using `SystemType`.
        * Features should have a configuration profile that derives from a scriptable object.
        * A default configuration profile located in `MixedRealityToolkit-SDK/Profiles` and be assigned in the default configuration profile for the Mixed Reality Manager
    * If this feature is **not** a core feature, then it must be registered using the component configuration profile and implement `IMixedRealityComponent`.
* Have a default implementation located in `MixedRealityToolkit-SDK/Features/<FeatureName>`
* Events that can be raised with the system should be defined in the interface, with all the required parameters for initializing the event data.

## Event Data Implementation Requirements
The Event Data defines exactly what data the handler is expected to receive from the event.

* All Event Datum for the feature should be defined in `MixedRealityToolkit/_Core/EventDatum/<FeatureName>`.
* All new Event Data classes should inherit from `GenericBaseEventData`

## Handler Implementation Requirements

The Handler Interface defines each event a component should be listening for and the types of data passed. End users will implement the interface to execute logic based on the event data received.

* Handler interfaces should be defined in `MixedRealityToolkit/_Core/Interfaces/<FeatureName>System/Handlers`.
* Handler interfaces should inherit from `UnityEngine.EventSystems.IEventSystemHandler`
* Opt-in by default. To receive events from the system, the handler will need to register itself with the system to receive those events.
