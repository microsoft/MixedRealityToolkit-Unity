using UnityEngine.EventSystems;

public class NetworkEventData<T> : BaseEventData
{
    public uint SourceId { get; private set; }

    public T value;

    //public <T> value;

    /// <summary>
    /// Used to initialize/reset the event and populate the data.
    /// </summary>
    /// <param name="transferData"></param>
    public void Initialize(T transferData)
    {
        Reset();
        value = transferData;
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventSystem">Typically will be <see cref="EventSystem.current"/></param>
    public NetworkEventData(EventSystem eventSystem) : base(eventSystem) { }
}
