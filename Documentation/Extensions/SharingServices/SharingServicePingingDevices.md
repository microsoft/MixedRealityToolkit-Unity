# Sharing Service : Pinging Devices

Once your app is running and the service is connected you can test your connection via `ISharingService.PingDevice(short deviceID)`:
```c#
public class PingScript : MonoBehaviour
{
    private void Start()
    {
        MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
        // Subscribe to pinged event
        service.OnLocalDevicePinged += OnLocalDevicePinged;
    }

    private void Update()
    {
        MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
        // Wait for service to connect
        if (service.Status != ConnectStatus.Connected)
        {
            return;
        }

        if (UnityEngine.Input.GetKeyDown(KeyCode.P))
        {
            foreach (short deviceID in service.ConnectedDevices)
            {   // Skip our own device
                if (deviceID != service.LocalDeviceID)
                {
                    service.PingDevice(deviceID);
                }
            }
        }
    }

    private void OnLocalDevicePinged()
    {   // Play sound, etc.
        Debug.Log("Pinged!");
    }
}
```
If [service inspectors](../..\MixedRealityConfigurationGuide.md#service-inspectors) are enabled you can also ping connected devices from within the Unity editor:

![In-Editor Ping Test](../../Images/SharingSystem/InEditorPingTest.gif)
