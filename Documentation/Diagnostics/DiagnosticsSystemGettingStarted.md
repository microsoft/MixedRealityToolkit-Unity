# Diagnostic system

The Mixed Reality Toolkit Diagnostic System provides diagnostic tools that run within the application to enable analysis of application issues.

The first release of the Diagnostic System contains the [Visual Profiler](UsingVisualProfiler.md) to allow for analyzing performance issues while using the application.

## Getting started

> [!IMPORTANT]
> It is **_highly_** recommended that the Diagnostic System be enabled throughout the entire product development cycle and disabled as the last change prior to building and releasing the final version.

There are two key steps to start using the Diagnostic System.

1. [Enable](#enable-diagnostics) the Diagnostic System
2. [Configure](#configure-diagnostic-options) diagnostic options

### Enable diagnostics

The diagnostics system is managed by the MixedRealityToolkit object (or another [service registrar](xref:Microsoft.MixedReality.Toolkit.IMixedRealityServiceRegistrar) component).

The following steps presume use of the MixedRealityToolkit object. Steps required for other service registrars may be different.

1. Select the MixedRealityToolkit object in the scene hierarchy.

    ![MRTK Configured Scene Hierarchy](../../Documentation/Images/MRTK_ConfiguredHierarchy.png)

1. Navigate the Inspector panel to the Diagnostics System section and check Enable
1. Select the Diagnostics System implementation

    ![Select the Diagnostics System Implementation](../../Documentation/Images/Diagnostics/DiagnosticsSelectSystemType.png)

> [!NOTE]
> Users of the default profile, `DefaultMixedRealityToolkitConfigurationProfile` (Assets/MRTK/SDK/Profiles), will have the diagnostics system pre-configured to use the [`MixedRealityDiagnosticsSystem`](xref:Microsoft.MixedReality.Toolkit.Diagnostics.MixedRealityDiagnosticsSystem) object.

### Configure diagnostic options

The diagnostics system uses a configuration profile to specify which components are to be displayed and to configure their settings. Please see [Configuring the Diagnostics System](ConfiguringDiagnostics.md) for more information pertaining to the available component settings.

> [!IMPORTANT]
> While it is possible to use Unity's Play Mode while developing applications without requiring the build and deploy steps, it is important to evaluate the diagnostics system results using a compiled application running on the target hardware and platform.
>
> Performance diagnostics, such as the [Visual Profiler](UsingVisualProfiler.md), may not accurately reflect actual application performance when run from within the editor.

## See also

- [Diagnostics API documentation](xref:Microsoft.MixedReality.Toolkit.Diagnostics)
- [Configuring the Diagnostics System](ConfiguringDiagnostics.md)
- [Using the Visual Profiler](UsingVisualProfiler.md)
