# Sharing Service : Sending and Receiving Data

## Sending

Once the service has connected, data can be sent via `ISharingService.SendData(SendDataArgs args)`.

By default data will be sent to all connected devices, including the device that sent it.

```c#
private void Update()
{
    // Retrieve the service
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
    // Wait for service to connect
    if (service.Status != ConnectStatus.Connected)
    {   
        return;
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.S))
    {
        // Send the data
        service.SendData(new SendDataArgs()
        {
            Data = new byte[] { 100 }
        });
    }
}
```

## Receiving

To receive data, subscribe to the `ISharingService.OnReceiveData` event.

By default data will be received by all connected devices.

```c#
private void Start()
{
    // Retrieve the service
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);
    // Subscribe to data received event
    service.OnReceiveData += OnReceiveData;
}

private void OnReceiveData(DataEventArgs e)
{
    // Output: 100
    Debug.Log(e.Data[0]);
}
```
___
## Delivery options

SendDataArgs has several options to control how and to whom data is sent.
- [Type](SharingServiceSendingAndReceiving.md#type)
- [TargetMode](SharingServiceSendingAndReceiving.md#targetmode)
- [DeliveryMode](SharingServiceSendingAndReceiving.md#deliverymode)

## Type

Sending and receiving raw bytes isn't very useful. In most cases apps will send and receive serialized data.

Use `SendDataArgs.Type` to help receivers identify the data's type and handle it appropriately.

**Important:** Data types are not moderated by the service so it's up to your app to keep them straight. Collisions between data types will cause data errors.
```c#
// Define our data types
private const short dataTypeFloat = 1;
private const short dataTypeInt = 2;

private void Update()
{
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);

    if (UnityEngine.Input.GetKeyDown(KeyCode.F))
    {
        // Send a float
        service.SendData(new SendDataArgs()
        {
            Type = dataTypeFloat,
            Data = BitConverter.GetBytes(1.25f)
        });
    }

    if (UnityEngine.Input.GetKeyDown(KeyCode.I))
    {
        // Send an integer
        service.SendData(new SendDataArgs()
        {
            Type = dataTypeInt,
            Data = BitConverter.GetBytes(500)
        });
    }
}

private void OnReceiveData(DataEventArgs e)
{
    switch (e.Type)
    {
        case dataTypeFloat:
            float f = BitConverter.ToSingle(e.Data, 0);
            Debug.Log(f); // Output: 1.25
            break;

        case dataTypeInt:
            int i = BitConverter.ToInt32(e.Data, 0);
            Debug.Log(i); // Output: 500
            break;
    }
}
```
Complex types can be serialized using a tool or library like C#'s BinaryConverter or JsonDotNet. Serialization methods have varying limitations and platform support so experimentation may be neccessary to determine which is best for your project.

Here's an example using Unity's JsonUtility.
```c#
// Complex data
[Serializable]
private struct ComplexTypeA
{
    public int Value;
}

// Define our data type
private const short dataTypeComplexTypeA = 1;

private void Update()
{
    MixedRealityServiceRegistry.TryGetService<ISharingService>(out ISharingService service);

    if (UnityEngine.Input.GetKeyDown(KeyCode.S))
    {
        // Send complex data
        service.SendData(new SendDataArgs()
        {
            Type = dataTypeComplexTypeA,
            Data = Serialize<ComplexTypeA>(new ComplexData() { Value = 200 });
        });
    }
}

private void OnReceiveData(DataEventArgs e)
{
    switch (e.Type)
    {
        case dataTypeComplexTypeA:
            ComplexTypeA data = Deserialize<ComplexTypeA>(e.Data);
            Debug.Log(data.Value); // Output: 200
            break;
    }
}

private byte[] Serialize<T>(T value)
{
    string valueAsJson = JsonUtility.ToJson(value);
    return Encoding.ASCII.GetBytes(valueAsJson);
}

private T Deserialize<T>(byte[] bytes)
{
    string dataAsJson = Encoding.ASCII.GetString(bytes);
    T data = JsonUtility.FromJson<T>(dataAsJson);
    return data;
}
```

Keep in mind that data type is not intrinsically tied to the literal type being serialized. For instance, data type could be used purely to route data to subsystems, after which the first byte in the data array could be used to identify the data's type.

## TargetMode

`SendDataArgs.TargetMode` lets you specify which devices will receive sent data. Currently there are no options that let you override [data subscriptions.](SharingServiceDataSubscriptions.md)

TargetMode | Description | Overrides Subscriptions
--- | --- | ---
**Default** | All devices including the sender will receive data. | No
**SkipSender** | All devices except sender will receive the data. | No
**Manual** | All devices in `SendDataArgs.Targets` will receive data. | No

## DeliveryMode

`SendDataArgs.DeliveryMode` lets you specify how important data is and whether it should arrive in the order sent. These options mirror the delivery options available in Photon.

DeliveryMode | Guaranteed Arrival | Arrives In Sequence
--- | --- | ---
**Unreliable** (Default) | No | Yes
**Reliable** | Yes | Yes
**UnreliableUnsequenced** | No | No
**ReliableUnsequenced** | Yes | No