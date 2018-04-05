# Mixed Reality Toolkit - Internal - Managers

This folder contains all the MRTK Internal Managers and supporting functions

## Manager Definitions

The list of definitions for Internal MRTK Managers are listed below.

### MixedRealityManager

The MixedRealityManager is the core manager responsible for maintaining all active components in a Mixed Reality project.
It is also responsible for selecting and maintaining the active SDK / Device currently in operation in the project depending on the platform and available hardware.

All other managers depend on the MixedRealityManager for coordinating calls between components as well as the active profile for the Mixed Reality Toolkit

> [See the MixedRealityManager Guide for more info](/External/Documentation/MixedRealityManagerConstruction.md)