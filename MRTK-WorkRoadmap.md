# Work Roadmap for early vNext development 

Here are some of the goals and tasks we're pursuing during the Alpha/Beta and early release stages of MRTK vNext. Please create github issues for specific tasks, but if there's a general area that needs planning and development, feel free to add it here.
 
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

To ensure we have a smooth transition, we need several key tasks to be completed and ensure we limit (as much as possible) any future breaking changes:

1. Build the new front-end architecture, enabling both new and existing users to start building from. Comprising of an initial set of interactable prefabs / components:

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
