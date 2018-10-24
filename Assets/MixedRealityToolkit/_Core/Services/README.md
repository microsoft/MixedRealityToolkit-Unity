# Mixed Reality Toolkit - Internal - Services

## MixedRealityOrchestrator

The `MixedRealityOrchestrator` is the core system responsible for maintaining all active services in a Mixed Reality project.
It is also responsible for selecting and maintaining the active SDK / Device currently in operation in the project depending on the platform and available hardware.

All other services depend on the `MixedRealityOrchestrator` for coordinating calls between all the registered services for the Mixed Reality Toolkit

> [See the Services Guide for more info](/External/Documentation/MixedRealityServices.md)