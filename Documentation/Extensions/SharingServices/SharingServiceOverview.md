# Sharing Service Overview

This extension provides a basic way to send and receive data to / from other devices.

## Enabling the extension

To enable the extension, open your RegisteredServiceProvider profile. Click Register a new Service Provider to add a new configuration. In the Component Type field, select PhotonSharingService (or any other ISharingService implementation).

### Installing and configuring Photon

The default implementation of the sharing service uses Photon's PUN 2. Without this package installed, the service will not function. [Download the required Photon package here](https://assetstore.unity.com/packages/tools/network/pun-2-free-119922). See that package's documentation for details on how to set up and configure your Photon account.

---
## Next Steps

- [Pinging Devices](SharingServicePingingDevices.md)
- [Sending and Receiving Data](SharingServiceSendingAndReceiving.md)
- [Using Data Subscriptions](SharingServiceDataSubscriptions.md)
- [Using App Roles](SharingServiceAppRoles.md)
- [Profile Options](SharingServiceProfileOptions.md)
