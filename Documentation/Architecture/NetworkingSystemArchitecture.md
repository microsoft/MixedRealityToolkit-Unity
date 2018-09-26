# Mixed Reality Toolkit Networking System Architecture

The Mixed Reality Networking System is intended to abstract the specific implementation details of platforms, such as the Photon and WebRTC, that provide support for synchronizing applications and experiences and providing the data to Mixed Reality experiences.

Where possible, we are designing for common functionality (ex: identification of planar surfaces) well as access to the lowest level of available data (ex: mesh or point data).

Each interface defined will implement one or more Properties, Methods and/or Events (PMEs) that can be accessed by application code.

## Interfaces

- [INetworkHandler<T>](./INetworkHandler.md)

## Classes

- [INetworkingBase<T>](./INetworkingBase.md)
- [IPhotonNetworking<T>](./IPhotonNetworking.md)

## Enumerations

- [NetworkingType](./NetworkingType.md)

## Event Data Types

- [NetworkEventData<T>](./NetworkEventData.md)


## Example Implementation

The Mixed Reality Toolkit's default implementation of the spatial awareness system can be found in the Assets/MixedRealityToolkit/SDK/Features/SpatialAwarenessSystem folder. This implementation can be used as an example of how to build your own spatial awareness system. It is well documented and demonstrates each of the features described in this architecture specification.

```C#
using UnityEngine;

public class Test : MonoBehaviour, INetworkHandler<int>, INetworkHandler<Vector3>
{   
	void Start () {

		//one must specify the type of variable you're sending

		INetworkingBase<int>.Send(3);

		INetworkingBase<Vector3>.Send(GetInstanceID(), new Vector3(5, 20, -80), 1);
	}

	public void OnDataReceived(NetworkEventData<int> eventData)
	{
		print(eventData.value);
	}

	public void OnDataReceived(NetworkEventData<Vector3> eventData)
	{
		print(eventData.value);
	}
}
```