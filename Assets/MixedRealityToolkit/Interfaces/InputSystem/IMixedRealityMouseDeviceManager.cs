using Microsoft.MixedReality.Toolkit.Input;

public interface IMixedRealityMouseDeviceManager : IMixedRealityInputDeviceManager
{
    /// <summary>
    /// Typed representation of the ConfigurationProfile property.
    /// </summary>
    MixedRealityMouseInputProfile MouseInputProfile { get; }
}
