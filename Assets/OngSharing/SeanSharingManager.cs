//using HoloToolkit.Unity;
//using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;

//These below are for serializing the EventData. Keep for now.
//using System.IO;
//using System.Runtime.Serialization;
//using System.Runtime.Serialization.Formatters.Binary;
//using System.Xml.Linq;
//using System.Text;


public class SeanSharingManager : Singleton<SeanSharingManager> {

    public PhotonView photonView;
    //Standard input code:::::
    //Standard input code:::::

    //public static InputEventData IED;
    public static MixedRealityPointerEventData IED;
    public static FocusEventData FED;

    //public InputEventData IED2;
    //public static InputClickedEventData IED3;

    public string EventDataString;

    //public InputEventData IEDs;

    //https://answers.unity.com/questions/1405691/how-to-call-an-event-trigger-on-a-button-through-s.html

    public static List<GameObject> indexedObjs;

    public static bool FocusTimeout = false;

    private void Awake()
    {
        if (IED == null)
        {
            IED = new MixedRealityPointerEventData(EventSystem.current);            
        }

        if (FED == null)
        {
            FED = new FocusEventData(EventSystem.current);
        }
    }

    public void InputDown(int Object)
    {
        print("SeanSharingManager: Input Down Being Sent from GameObject: " + getSharedObjectFromID(Object).name);
        //print(getSharedObjectFromID(Object) + ":::::::::::::");
        photonView.RPC("InputDownRPC", PhotonTargets.Others, Object);
        StartCoroutine(reindex());
    }

    [PunRPC]
    public void InputDownRPC(int Object)
    {

        print("SeanSharingManager: Input Down RPC Recieved for object number: " + Object + ". Which is of name: " +getSharedObjectFromID(Object));

        if (getSharedObjectFromID(Object).GetComponent<ClickSharer>() != null)
            getSharedObjectFromID(Object).GetComponent<ClickSharer>().time();
        else
            print("SeanSharingManager: WARNING: There's an object without a click sharer: " + getSharedObjectFromID(Object).name);


        getSharedObjectFromID(Object).SendMessage("OnPointerDown", IED);

        StartCoroutine(reindex());

    }

    IEnumerator reindex()
    {
        yield return null;
        yield return null;
        yield return null;
        GetComponent<OngSharingIDgenerator>().index();
    }

    public void InputUp(int Object)
    {
      
        photonView.RPC("InputUpRPC", PhotonTargets.Others, Object);
    }

    [PunRPC]
    public void InputUpRPC(int Object)
    {
        getSharedObjectFromID(Object).GetComponent<ClickSharer>().time();
        getSharedObjectFromID(Object).SendMessage("OnPointerUp", IED);
    }

    public void InputClicked(int Object)
    {
        print("SeanSharingManager: Input Click Being Sent");
        photonView.RPC("InputClickedRPC", PhotonTargets.Others, Object);
    }

    [PunRPC]
    public void InputClickedRPC(int Object)
    {
        getSharedObjectFromID(Object).GetComponent<ClickSharer>().time();
        getSharedObjectFromID(Object).SendMessage("OnPointerClicked", IED);

        print("Input Clicked RPC Recieved for object number: " + Object + ". Which is of name: " + getSharedObjectFromID(Object));
    }

    public void FocusEnter(int Object)
    {
        print("SeanSharingManager: Focus Enter Being Sent");
        photonView.RPC("FocusEnterRPC", PhotonTargets.Others, Object);
    }

    [PunRPC]
    public void FocusEnterRPC(int Object)
    {
        print("SeanSharingManager: Focused entered RPC Recieved for object number: " + Object + ". Which is of name: " + getSharedObjectFromID(Object));

        getSharedObjectFromID(Object).GetComponent<ClickSharer>().time();
        getSharedObjectFromID(Object).SendMessage("OnFocusEnter", FED);
    }

    public void FocusExit(int Object)
    {
        print("SeanSharingManager: Focus Exit Being Sent");
        photonView.RPC("FocusExitRPC", PhotonTargets.Others, Object);
    }

    [PunRPC]
    public void FocusExitRPC(int Object)
    {
        print("SeanSharingManager: Input Exit RPC Recieved for object number: " + Object + ". Which is of name: " + getSharedObjectFromID(Object));

        getSharedObjectFromID(Object).GetComponent<ClickSharer>().time();
        getSharedObjectFromID(Object).SendMessage("OnFocusExit", FED);
    }

    GameObject getSharedObjectFromID(int ObjectID)
    {
        var objectToReturn = gameObject; //Just set the default object to be this object

        foreach (GameObject obj in indexedObjs)
        {
            if (obj.GetComponent<ClickSharer>().sharingID == ObjectID)
            {
                objectToReturn = obj;
                return obj;
            }
        }

        return objectToReturn;
    }


    //Standard input code:::::
    //Standard input code:::::
    // Use this for initialization
    void Start () {
        photonView = GetComponent<PhotonView>();


        //StartCoroutine(GetEventData());
    }

    //IEnumerator GetEventData()
    //{
    //    while (IED == null)
    //    {
    //        yield return null;
    //    }

    //    // Stream the file with a File Stream. (Note that File.Create() 'Creates' or 'Overwrites' a file.)
    //    FileStream file = File.Create(Application.persistentDataPath + "/eventdata.dat");

    //    print("Saving data file: " + Application.persistentDataPath + "/eventdata.dat");

    //    //Serialize to xml
    //    DataContractSerializer bf = new DataContractSerializer(IED.GetType());
    //    MemoryStream streamer = new MemoryStream();

    //    //Serialize the file
    //    bf.WriteObject(streamer, IED);
    //    streamer.Seek(0, SeekOrigin.Begin);

    //    //Save to disk
    //    file.Write(streamer.GetBuffer(), 0, streamer.GetBuffer().Length);

    //    // Close the file to prevent any corruptions
    //    file.Close();

    //    string result = XElement.Parse(Encoding.ASCII.GetString(streamer.GetBuffer()).Replace("\0", "")).ToString();
    //    Debug.Log("Serialized Result: " + result);

    //    EventDataString = result;

    //    print("Sucessfully saved eventdata.");

    //    print("Now loading the event data.");

    //    Deserialize(result, IED.GetType());

    //    print("IED: " + IED);
    //    IED2 = IED;
    //    print("IED2: " + IED2);

    //    IED.ToString();

        
    //}

    //public static object Deserialize(string result, Type toType)
    //{
    //    if (File.Exists(Application.persistentDataPath + "/eventdata.dat"))
    //    {
    //        using (Stream stream = new MemoryStream())
    //        {
    //            byte[] data = System.Text.Encoding.UTF8.GetBytes(result);
    //            stream.Write(data, 0, data.Length);
    //            stream.Position = 0;
    //            DataContractSerializer deserializer = new DataContractSerializer(toType);
    //            return deserializer.ReadObject(stream);
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log(string.Format("File doesn't exist at path: {0}{1}", Application.persistentDataPath, "/eventdata.dat"));
    //        return null;
    //    }

    //}

    //Update is called once per frame
    void Update () {

		
	}

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            //We own this player: send the others our data 

        }
        else
        {
            //Network player, receive data 

        }
    }
}
