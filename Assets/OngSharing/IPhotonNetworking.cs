using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class IPhotonNetworking<T> : Singleton<IPhotonNetworking<T>> {


    int Identity;
    TypeCode typeCode;
    T dataStream;
    NetworkEventData<T> NED;

    public void Send(int ID, T data, float reliability)
    {
        PhotonView photonView = GetComponent<PhotonView>();
        typeCode = Type.GetTypeCode(data.GetType());
        Identity = ID;
        dataStream = data;        
    }

    public static GameObject getObjectById(int id)
    {
        Dictionary<int, GameObject> m_instanceMap = new Dictionary<int, GameObject>();
        //record instance map

        m_instanceMap.Clear();
        List<GameObject> gos = new List<GameObject>();
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (gos.Contains(go))
            {
                continue;
            }
            gos.Add(go);
            m_instanceMap[go.GetInstanceID()] = go;
        }

        if (m_instanceMap.ContainsKey(id))
        {
            return m_instanceMap[id];
        }
        else
        {
            return null;
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(Identity);
            stream.SendNext(typeCode);
            stream.SendNext(dataStream);
        }
        else
        {
            Identity = (int)stream.ReceiveNext();
            typeCode = (TypeCode)stream.ReceiveNext();
            dataStream = (T)stream.ReceiveNext();

            //if (NED == null)
            //{
            //    NED = new NetworkEventData<T>(EventSystem.current);
            //}
            //NED.value = dataStream;

            NED.Initialize(dataStream);

            getObjectById(Identity).SendMessage("OnDataReceived", NED);

            //ExecuteEvents.Execute<T>(NED, );
            //public static bool Execute(GameObject target, EventSystems.BaseEventData eventData, EventFunction<T> functor); 
        }
    }
}
