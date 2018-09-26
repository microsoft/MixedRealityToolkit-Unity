using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class NetworkEventData : BaseEventData
{
    public uint SourceId { get; private set; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="eventSystem">Typically will be <see cref="EventSystem.current"/></param>
    protected NetworkEventData(EventSystem eventSystem) : base(eventSystem) { }
}
