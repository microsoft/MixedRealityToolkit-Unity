using HoloToolkit.Unity.Preview.SpectatorView;
using UnityEngine.Networking;

/// <summary>
/// Example NetworkManager implementation. It simply starts tge server if the HoloLens is running the app.
/// This component represents the NetworkManager of an entire UNET multilens application.
/// </summary>
public class SpectatorViewNetworkManager : NetworkManager
{
    /// <summary>
    /// Is the device a host or a client? (HoloLens or mobile?)
    /// </summary>
    private bool isHost;

    // Use this for initialization
    private void Start()
    {
        isHost = FindObjectOfType<PlatformSwitcher>().TargetPlatform == PlatformSwitcher.Platform.Hololens;
        if (isHost)
        {
            StartHost();
        }
    }
}
