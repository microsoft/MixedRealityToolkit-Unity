//using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Microsoft.MixedReality.Toolkit.InputSystem.Sources;

public class OngSharingIDgenerator : MonoBehaviour
{

    public int sharingID; //each gameobject should have a unique ID    
    public static bool existingIDs; //This toggle assumes you already have ID scripts on shared objects, and will just update the IDs, rather than create new ID scripts for every object
    public bool isMaster;

    // Use this for initialization
    void Start() {

        //If this is the master script - then get all gameobjects and put a copy of this script on all other game objects
        //to set a unique but consistent set of IDs accross devices.

        sharingID = -1;

        if (isMaster)
        {
            //InputManager.Instance.AddGlobalListener(gameObject);
            index();
        }
        else
            print("OngSharingIDGenerator not on a master object. It's on this object: " + gameObject.name);

        
    }

    public void index()
    {        
        existingIDs = true;
        print("Indexing. This is only being called once!");
        StartCoroutine(AddSharingScriptsToAllObjectsInThisParent());
    }

    //Adds sharing IDs to all objects in this parent. For sharing, please place all objects as a child of this parent.
    IEnumerator AddSharingScriptsToAllObjectsInThisParent()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        MonoBehaviour[] g;

        yield return new WaitForSeconds(3f);

            g = gameObject.GetComponentsInChildren<MonoBehaviour>(true);
            
            foreach (MonoBehaviour child in g)
            {
                if (child != null && child.gameObject.name != "InputManager" && child.gameObject.name != "DefaultCursor" && child.gameObject.name != "XboxControllerInputSource" && child.gameObject.layer != 2)
                {
                    objectsInScene.Add(child.gameObject);
                }
            }
        
            
        foreach (GameObject go in objectsInScene)
        {


            if (go.name != gameObject.name) //Make sure not to put a duplicate copy of the script on this object
            {
                if (go.hideFlags != HideFlags.None)
                    continue;

                //Add gameobject to a list of all gameobjects - in case we need to iterate over them in the future

                if (go.GetComponent<ClickSharer>() == null) //If this script already exists on an object, don't add another one.
                {

                    if (go.GetComponent<MonoBehaviour>() != null) //If object contains script of any sort
                    {
                        sharingID++;
                        go.AddComponent<ClickSharer>(); //Add this script to the game object. It's only purpose for other objects will be to hold a unique ID.
                        go.GetComponent<ClickSharer>().sharingID = sharingID; //Increment the sharingID and set the newly created script's ID to the incremented ID 
                        go.GetComponent<ClickSharer>().objectName = go.name; //Hard coding the name, so we can later compare in the even this object was cloned elsewhere.
                    }
                }
            }
        }

        yield return null;
        SeanSharingManager.indexedObjs = objectsInScene;
        Debug.Log("Added Sharing ID's to all " + objectsInScene.Count + " Objects.");
        Debug.Log("SharingID went up to " + sharingID);


    }

    //This is for ALL Objects. Doesn't work very well yet, as root objects differ between when running on device vs running in Unity.
    IEnumerator AddSharingScriptsToAllObjects()
    {
        List<GameObject> objectsInScene = new List<GameObject>();

        MonoBehaviour[] g;
        foreach (GameObject rootObjects in UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects())
        {
            g = rootObjects.GetComponentsInChildren<MonoBehaviour>(true);
            //print(g.Length + "::" + rootObjects);
            foreach (MonoBehaviour child in g)
            {
                if (child != null && child.gameObject.name != "InputManager" && child.gameObject.name != "DefaultCursor" && child.gameObject.name != "XboxControllerInputSource" && child.gameObject.layer != 2)
                {
                    objectsInScene.Add(child.gameObject);
                }
            }
        }

        GameObject DontDestroyObj = new GameObject("DontDestroyObj");
        DontDestroyOnLoad(DontDestroyObj);

        foreach (GameObject rootObjects in DontDestroyObj.scene.GetRootGameObjects())
        {
            g = rootObjects.GetComponentsInChildren<MonoBehaviour>(true);
            //print(g.Length + "::" + rootObjects);
            foreach (MonoBehaviour child in g)
            {
                if (child != null && child.gameObject.name != "InputManager" && child.gameObject.name != "DefaultCursor" && child.gameObject.name != "XboxControllerInputSource" && child.gameObject.layer != 2)
                {
                    objectsInScene.Add(child.gameObject);
                }
            }            
        }
        Destroy(DontDestroyObj);

        foreach (GameObject go in objectsInScene)
        {
            

            if (go.name != gameObject.name) //Make sure not to put a duplicate copy of the script on this object
            {
                if (go.hideFlags != HideFlags.None)
                    continue;

                //Add gameobject to a list of all gameobjects - in case we need to iterate over them in the future

                if (go.GetComponent<ClickSharer>() == null) //If this script already exists on an object, don't add another one.
                {

                    if (go.GetComponent<MonoBehaviour>() != null) //If object contains script of any sort
                    {
                        sharingID++;
                        go.AddComponent<ClickSharer>(); //Add this script to the game object. It's only purpose for other objects will be to hold a unique ID.
                        go.GetComponent<ClickSharer>().sharingID = sharingID; //Increment the sharingID and set the newly created script's ID to the incremented ID 
                        go.GetComponent<ClickSharer>().objectName = go.name; //Hard coding the name, so we can later compare in the even this object was cloned elsewhere.
                    }
                }
            }            
        }

        yield return null;
        SeanSharingManager.indexedObjs = objectsInScene;
        Debug.Log("Added Sharing ID's to all " + objectsInScene.Count + " Objects.");
        Debug.Log("SharingID went up to " + sharingID);


    }

    
}