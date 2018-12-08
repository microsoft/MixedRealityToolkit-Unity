# IMixedRealityNetworkingHandler<T> Interface

### OnDataReceived()

| Type |
| --- |
| `BaseNetworkingEventData<T>` eventData |

Is triggered by incoming data and includes eventData as a parameter. 
Data can be accessed by using `eventData.value` which will be in the type specified in the parameter.

In order to receive data of a particular type, one must implement that variant of INetworkHandler interface and use the same overload of `OnDataReceived()` and the handler must be registered with the `IMixedRealityNetworkSystem`.

## Example usage:
```C#

public class NetworkHandler : MonoBehaviour,
    IMixedRealityNetworkingHandler<float>
    IMixedRealityNetworkingHandler<Vector3>
{
    public void OnDataReceived(BaseNetworkingEventData<float> eventData)
    {
        Debug.Log($"Received {eventData.Data.ToString()}");
    }

    public void OnDataReceived(BaseNetworkingEventData<Vector3> eventData)
    {
        Debug.Log($"Received {eventData.Data.ToString()}");
    }
}

```