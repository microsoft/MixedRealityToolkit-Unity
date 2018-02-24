# Mixed Reality Toolkit – Multi-VR approach (High level)

## Objective

To rearchitect the current Mixed Reality Toolkit to provide:

 * An abstractable base interface that will support multiple VR platforms, predominately HL/MR/OpenVR/OpenXR
 * A Building block style front end interface that is easy for new users to construct new Mixed Reality solutions easily, whilst still providing an open framework for intensive developers to consume.

## Outline Architecture

![](External/ReadMeImages/MRTK-vNext-HLA.png)
<div style="text-align:center"> Figure 1 : Draft High-Level architecture </div>

The internal side of the MRTK, is internal / private, not something a user would need to use / access.  We can debate whether we still allow some of it to be “open” for customization.  But I feel that should be through users creating “extensions” rather than modifying base code.

The SDK side is what the users will be consuming, either through a constructed set of prefabs or adding the management scripts.  The obvious aim being to enable (as much as possible) for a consumer to drag and drop the MRTK in to their existing scene and enable behaviors / movement and interactions.

 
## High-Level Architecture breakdown

The architecture approach lends from many different sources (VRTK, OpenXR, EditorVR) to provide a structure that aligns to the aims for the Multi-VR approach proposed for the Mixed Reality Toolkit.
In all cases, the framework should follow the 80/20 rule, providing core mechanisms to deliver common use cases whilst also being open enough to allow for extension or even replacement of key components (for advanced users)

 * Device Abstraction Layer

The DAL creates a bridge layer between an external provider SDK and confirming the outputs / inputs of those devices to conform to the MRTK construct definitions.
Ideally, these interfaces should be at the script level consuming the vendor SDK direct rather than relying on vendor prefab definitions.

 * MRTK Constructs

A coordinated set of specifications to coordinate an abstracted system for multiple VR.  The specifications detail the inner workings for the toolkit and define extendable contracts for the SDK to expose to both providers (devices) and consumers (SDK).

 * MRTK Interface Layer

The interface layer provides an extensible bridge to map and coordinate inputs coming from VR devices and controllers as well as providing an abstracted mechanism to feedback to VR systems (e.g. Haptics).  It also defines preset interactions that can be composited for use by the SDK.

 * MRTK User Abstraction layer

This is the frontend of the entire SDK, providing pre-built components for the most used implementations for Mixed Reality.  This should be provided through a set of easy to use components / scripts that can be “Drag and Dropped” to build a project.
Like the Unity UI System, this does not preclude users building their own components and potentially submitting back to the project for inclusion.

 
## Key Principles

Following the feedback we’ve received both internally and through current consumers, I propose the following key principles we should follow building this new architecture:

 * The new framework needs to provide “out of the box” components to enable rapid prototyping and development.  Future internal changes should not break these components or cause them to degrade in functionality.
 * Every component / feature provided by the framework (either prefab or script) should have corresponding unit tests to validate its functionality.  Any future merge to the framework will have to validate these test before merging to ensure there is no degradation of service.
 * The new framework should provide a programmatic interface along side the SDK components, to enable advanced users to extend or replace any component within the framework.
 * Each new interface / component should be fully documented (where possible). These should detail all interactions and intended uses.  For the advanced cases, we’ll need to review how we explain how to replace components (e.g. replacing the input system with another, like InControl)
 * Working examples / demos should be created for any specific system, these should have purpose and not just be a “test”.
 * All testing components / scripts / prefabs / scenes should only be retained in the dev branch.  The master branch should only contain the “ToolKit” and working examples.  Master is for consumers only.
 * Simulator options need to be provided as another device. This will also form the template for new MR/XR/VR devices / SDKs.
 
## Fringe thoughts (dumping ground for ideas :D)

 * Defining headset gameplay, allowing for sitting / standing or free roaming play.
Has to be easy enough to enable single projects to handle both behaviors
 * Defining headset movement, is it free roam, shifting or teleporting?
 * With Teleporting specifically, is it cursor based or zone based (something we don’t have).  After watching the Rick and Morty retrospective, they struggled with this and movement space, something an SDK should be able to provide a starter experience for.
 * Adding interaction mechanics to objects in the scene easily. Is it grabbable, moveable, altered by gravity etc.
 * Helpers for 3d’ifying scene objects, 2 sided drawing or best practices
 * Examples, Examples, Examples – Ensuring our examples are bigger than current based off the latest examples scene that came from VRTK which was well received. (showed ALL interaction options within a small house)
 * Videos – a YT / FB video channel with educational snippets dedicated to MRTK 

 
## Proposed Tasks

To ensure we have a smooth transition, we need several key tasks to be completed, to ensure a smooth transition and ensure we limit (as much as possible) any future breaking changes:

1. Build the new frontend architecture, to enable both existing users and new to start building from, comprising of an Initial set interactable component prefabs / components:

 * Grabbing
 * Touching
 * Basic interaction
 * Teleporting

2. A single example scene to demonstrate the use of the new components

3. Short video demonstration of example

Followed up with further front end components.
This will enable users to start using the new style approach for building new solutions.

Either in parallel (or following), we should focus on building the new underlying Multi-vr framework and stitching components together.

1. Define the underlying interfaces for the Multi-vr approach which abstracts the work Stephen has started in the Input system
2. In a feature branch, align the existing input and interaction systems to adopt the new interfaces
3. Update the current MR implementation to the new interfaces

It will be key to further understand any gaps we currently have in the toolkit to align to this new approach.

Once ready, the above prefabs/components will be updated to use the new underlying framework with little to no impact on their designed scenes.
 
# Reference Material

## Notable components in VRTK

**Pointer** (VRTK Example Basic Pointer)(Pointer with Area Collision)

- Line type (Straight, Bezier)(VRTK Example Bezier Pointer)
- Tip type (teleport target, sphere, dot, none…)
- Color
- Thickness
- Pointer Interaction, Menu Selection with Pointer (VRTK Example)
- Grab, Move, Rotate, Scale with Pointer (Shell behavior)

**Teleporting** (VRTK Example)

- Pointer style
- Transition type
- Transition fade speed
- Exclude teleport location (VRTK Example)

**Object Touching / Grabbing** (VRTK Example)

- Object touched event
- Highlight / Outline on touched
- InteractableObject: Object grabbable, Highlight on touch
- Throwing 
- Grab and Trigger (VRTK Example)
- Grab and Trigger Multiple (VRTK Example)
- Grab rotation snapping (VRTK Example)
- Grab attach mechanics: Fixed Joint / Spring Joint / Track Object (VRTK Example)
- Grab Force hold object (VRTK Example)
- Child on Grab: Bow and Arrow (VRTK Example)
- Controller Ghost or Physical (ghost passes through objects while)

**Menu System** (available in MRDL)

- Radial menu
- Grid menu
- Touchpad Axis Control (VRTK Example)
- Radial touchpad menu (VRTK Example)

**Tooltips** (available in MRDL) 

- Tip
- Connector
- Spawner
- Manager (tutorials, spaces)

**Headset collision fading** (VRTK Example) (James)

# Open XR references

TBC

The new MRTK approach would span across the Application and SDK/Device layers, seeing how OpenXR is a Device to the new architecture.

![](External/ReadMeImages/OpenXR-HLA.jpg)
<div style="text-align:center">Figure 2 : OpenXR architecture</div>

# Expected Experience reference (starter for 10)

 * **Interaction**

- Highlighting
- Pressing (e.g. buttons and a way to execute actions like UI Buttons)
- Activation (similar to pressing but an on/off state)
- Holding
- Raw Input (exposing individual inputs in a managed way)
- Speech

 * **Manipulation**

- Grabbing
- Transforming
- Alteration (e.g. scaling, animation)

 * **Experiences**

- Controller menus / actions
- Climbing / Moving with Controllers
- Pointers and Cursors (linked to each controller input)
- Doors and Interactions

 * **Motion**

- Free Teleportation
- Restricted Teleportation
- Locomotion
- Twitch movement

 * **Extended Experiences**

- FPS
- Bow and Arrow
- Common use cases