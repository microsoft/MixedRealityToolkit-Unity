using UnityEngine.EventSystems;

public interface INetworkHandler<T> : IEventSystemHandler
{
    /// <param name="eventData"></param>
    void OnDataReceived(NetworkEventData<T> eventData);

    //Not necissarily possible
    //void OnDataSendComplete();	

    //Not necissarily possible
    //void OnRecievedCancelled();

    //Not necissarily possible
    //void OnSendCancelled();	
}
