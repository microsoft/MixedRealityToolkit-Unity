# Mixed Reality Toolkit - Internal - Interfaces

This folder contains all the Interface definitions for MRTK operation

## Interface Definitions

The list of definitions for Internal MRTK Interfaces are listed below.

### IMixedRealityManager

The IManager interface ensures that all Manager components comply with the requirements of all Mixed Reality Toolkit managers.

### IMixedRealityBoundarySystem

The IMixedRealityBoundarySystem interface is the base interface to identify a component as an Boundary System Manager.  This is used to register an Boundary system in the Mixed Reality Toolkit, any replacement Boundary Systems should inherit from this interface for registration.

### IMixedRealityInputSystem

The IMixedRealityBoundarySystem interface is the base interface to identify a component as an Input System Manager.  This is used to register an Input system in the Mixed Reality Toolkit, any replacement Input Systems should inherit from this interface for registration.