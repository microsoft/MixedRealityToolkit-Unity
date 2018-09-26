using System.Collections;
using System.Collections.Generic;
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
