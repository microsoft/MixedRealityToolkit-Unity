# IMixedRealityNetworkingSystem Class

## Properties

| Type |
| --- |
| `HashSet<IMixedRealityNetworkDataProvider>` NetworkDataProviders |

The IMixedRealityNetworkDataProviders detected by the system.

## Methods

### RequestNetworkDataProviderSourceId()

Request a new IMixedRealityEventSource.SourceId for the IMixedRealityNetworkDataProvider.

### RaiseNetworkDataProviderDetected(IMixedRealityNetworkDataProvider networkDataProvider)

Raise that a IMixedRealityNetworkDataProvider has been detected by the system.

### RaiseNetworkDataProviderLost(IMixedRealityNetworkDataProvider networkDataProvider)

Raise that a IMixedRealityNetworkDataProvider has been lost by the system.

### SendData<T>(T data)

Send data out over the wire to whomever is listening.

### OnDataReceived<T>(T data)

Raised when data has been received from an IMixedRealityNetworkDataProvider. It's up to the IMixedRealityNetworkingSystem to forward this data to whomever needs it.
