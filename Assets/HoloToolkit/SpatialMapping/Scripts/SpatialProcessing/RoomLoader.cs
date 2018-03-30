using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.XR.WSA.Persistence;
using System.Collections;
using System.Collections.Generic;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class RoomLoader : MonoBehaviour
    {

        public GameObject surfaceObject;            // prefab for surface mesh objects
        public string fileName;                     // name of file used to store mesh
        public string anchorStoreName;              // name of world anchor for room

        private List<Mesh> roomMeshes;                      // list of meshes saved from spatial mapping
        private WorldAnchorStore anchorStore;               // store of world anchors

        // Use this for initialization
        private void Start()
        {
            // get instance of WorldAnchorStore
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

        private void AnchorStoreReady(WorldAnchorStore store)
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

            }

            GameObject.Find("GameManager").GetComponent<GameManager>().RoomLoaded();
        }
    }
}