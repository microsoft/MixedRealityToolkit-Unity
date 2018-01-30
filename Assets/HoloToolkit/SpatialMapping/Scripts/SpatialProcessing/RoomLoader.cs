using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA.Persistence;
using System.Collections;
using System.Collections.Generic;

public class RoomLoader : MonoBehaviour
{

    public GameObject surfaceObject;            // prefab for surface mesh objects
    public string fileName;                     // name of file used to store mesh
    public string anchorStoreName;              // name of world anchor for room

    List<Mesh> roomMeshes;                      // list of meshes saved from spatial mapping
    WorldAnchorStore anchorStore;               // store of world anchors

    // Use this for initialization
    void Start()
    {
        // get instance of WorldAnchorStore
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    // Update is called once per frame
    void Update()
    {
    }

    void AnchorStoreReady(WorldAnchorStore store)
    {
        // save instance
        anchorStore = store;

        // load room meshes
        roomMeshes = MeshSaver.Load(fileName) as List<Mesh>;

        foreach (Mesh surface in roomMeshes)
        {
            GameObject obj = Instantiate(surfaceObject) as GameObject;
            obj.GetComponent<MeshFilter>().mesh = surface;
            obj.GetComponent<MeshCollider>().sharedMesh = surface;

            if (!anchorStore.Load(surface.name, obj))
                Debug.Log("WorldAnchor load failed...");

            Debug.Log("Mesh " + surface.name + " Position: " + obj.transform.position + "\n--- Rotation: " + obj.transform.localRotation + "\n--- Scale: " + obj.transform.localScale);
        }

        GameObject.Find("GameManager").GetComponent<GameManager>().RoomLoaded();
    }
}