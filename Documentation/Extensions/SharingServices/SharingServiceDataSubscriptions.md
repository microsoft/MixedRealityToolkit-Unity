# Sharing Service : Data Subscriptions

Some devices may only care about a small subset of the data being sent by other devices. Senders can sepcify which devices receive which data using device IDs, but a responsible choice can require detailed knowledge of another connected device's needs. Data subscriptions puts the decision in the device's hands.

By default a device's subscription mode is set to `SubscriptionModeEnum.All`, which means it will receive all data types. This setting can be changed to `SubscriptionModeEnum.Manual` via `ISharingService.SetLocalSubscriptionMode`:
```c#
public class IOTDevice : MonoBehaviour
{
    private const short dataTypeHoloLens = 1;
    private const short dataTypeIOT = 2;

    private void Start()
    {
        MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
        // Since we're an IOT device we only care about IOT data
        service.SetLocalSubscriptionMode(SubscriptionModeEnum.Manual, new short[] { dataTypeIOT });
    }
}
```

## Responding to subscription mode changes

If a device's subscription mode changes, it's up to your scripts to ensure that the desired data types are included in its manual subscriptions. Scripts can subscribe to the `ISharingService.OnLocalSubscriptionModeChange` event, which will be invoked every time the subscription mode is set, including on initialization:
```c#
public class ScriptUsingService : MonoBehaviour
{
    // Custom data type defined for this script
    private const short dataTypeForThisScript = 1000;
    private ISharingService service;

    private void Start()
    {
        MixedRealityServiceRegistry.TryGetService<ISharingService>(out service);
        // Subscribe to this event in case another script changes our local subscription type
        service.OnLocalSubscriptionModeChange += OnLocalSubscriptionModeChange;
    }

    private void OnLocalSubscriptionModeChanged(SubscriptionEventArgs e)
    {
        switch (e.Mode)
        {
            case SubscriptionModeEnum.Manual:
                // Ensure that our data type is included in the manual subscriptions.
                service.SetLocalSubscription(dataTypeForThisScript, true);
                break;
                
            default:
                // No need to do anything.
                break;
            }
        }
    }
}
```