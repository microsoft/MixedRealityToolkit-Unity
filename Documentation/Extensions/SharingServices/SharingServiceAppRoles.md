# Sharing Service : App Roles

## When to use app roles

App roles can help scripts distribute work nonuniformly among connected devices. They can also help implement a *weak* form of authority where a single device acts as a conduit for all data being processed and distributed.

For instance, if a shared app consists of multiple IOT devices connected to a single PC, giving every client the same workload would be inefficient. Giving the PC a role analagous to a dedicated server would be preferable.

**Important note:** An app's role is an arbitrary distrinction from the perspective of the service. All connected devices are pure client-side and there is no true concept of authority. Branching execution should not be relied on for security or as an anti-cheating measure.

## Default vs explicit app roles

By default a device's app role is chosen by the service. In the Photon implementation of the service, `AppRole.Host` corresponds to a player's *Master Client* property, while `AppHost.Client` corresponds to a standard client. If a device assigned the `AppRole.Host` role is disconnected, another device will be assigned that role. (Other implementations may assign or re-assign these roles differenly.)

`AppRole.Server` must be explicitly requested on connect. If the server device disconnects, that role will not be re-assigned to another device. This is useful for dedicated-server analogues like the PC / IOT example discussed earlier.

AppRole | Photon Equivalent | Service assigns automatically | Can change when device disconnects | More than one permitted
--- | --- | --- | --- | ---
**Client** | Client | Yes | Yes | Yes
**Host** | Master Client | Yes | Yes | No
**Server** | (None) | No | No | No

## Requesting app roles on connect

The default app role can be specified in your service's [config profile](SharingServiceProfileOptions.md). However this requires different devices to use different profiles, which can be cumbersome. We recommend disabling auto-connect in your config profile and connecting via `ISharingService.ConnectCustom(ConnectConfig config)` instead.

```c#
// Requesting role based on device type
private void Start()
{
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
    if (SystemInfo.deviceType == DeviceType.Desktop)
    {   // If we're a desktop PC, request the server role
        service.ConnectCustom(new ConnectConfig()
        {
            RequestedRole = AppRoleEnum.Server,
        });
    }
    else
    {   // Otherwise connect normally.
        // The device will be assigned Client or Host by default.
        service.Connect();
    }
}
```

## Responding to app role changes

Apps can listen for their app role being set by using the `ISharingService.OnAppRoleSelected` event. This event will be invoked once on successful connect, then once each time the app's role changes.

```c#
private void Start()
{
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
    // Subscribe to event so we know when app role is initially selected / changed
    service.OnAppRoleSelected += OnAppRoleSelected;
}

private void OnAppRoleSelected(ConnectEventArgs e)
{
    // All devices do this setup
    SetUpSharedClientData();

    switch (e.AppRole)
    {
        case AppRoleEnum.Client:
            // No additional work necessary
            break;
            
        default: // AppHostEnum.Host or AppHostEnum.Server
            // Do additional host-only setup
            SetUpSpecialWork();
            break;
        }
    }
}

private void Update()
{
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
    if (service.Status != ConnectStatus.Connected)
    {
        return;
    }

    // All devices do this work
    DoSharedClientWork();

    switch (e.AppRole)
    {
        case AppRoleEnum.Client:
            // No additional work necessary
            break;

        default: // AppHostEnum.Host or AppHostEnum.Server
            // This device will do additional work
            DoSpecialWork();
            break;
    }
}
```