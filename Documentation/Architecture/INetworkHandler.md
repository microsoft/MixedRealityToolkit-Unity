# INetworkHandler<T> Interface

The INetworkHandler is the interface that defines the requirements of the spatial awareness system. The interface is divided, logically into multiple sections. As new functionality is added, the appropriate settings section is to be defined.

### OnDataReceived()

| Type |
| --- |
| NetworkEventData<T> |

Is triggered by incoming data and includes eventData as a parameter. 
Data can be accessed by using `eventData.value` which will be in the type specified in the parameter.

In order to receive data of a particular type, one must implement that variant of INetworkHandler interface and use the same overload of OnDataReceived().

