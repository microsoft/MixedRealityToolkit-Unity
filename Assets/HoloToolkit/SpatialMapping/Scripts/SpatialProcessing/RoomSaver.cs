using HoloToolkit.Unity;
using UnityEngine;
using System.Collections.Generic;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine.XR.WSA.Persistence;
using UnityEngine.XR.WSA;

namespace HoloToolkit.Unity.SpatialMapping
{
    public class RoomSaver : MonoBehaviour
    {

        public string fileName;             // name of file to store meshes
        public string anchorStoreName;      // name of world anchor to store for room

        private List<MeshFilter> roomMeshFilters;
        private WorldAnchorStore anchorStore;
        private int meshCount = 0;

        // Use this for initialization
        private void Start()
        {
            WorldAnchorStore.GetAsync(AnchorStoreReady);
        }

        private void AnchorStoreReady(WorldAnchorStore store)
        {
            anchorStore = store;
        }

        public bool SaveRoom()
        {
            // if the anchor store is not ready then we cannot save the room mesh
            if (anchorStore == null)
            {
                return false;
            }

            // delete old relevant anchors
            string[] anchorIds = anchorStore.GetAllIds();
            for (int i = 0; i < anchorIds.Length; i++)
            {
                if (anchorIds[i].Contains(anchorStoreName))
                {
                    anchorStore.Delete(anchorIds[i]);
                }
            }

            // Old anchors deleted...

            // get all mesh filters used for spatial mapping meshes
            roomMeshFilters = SpatialUnderstanding.Instance.UnderstandingCustomMesh.GetMeshFilters() as List<MeshFilter>;

            // Mesh filters fetched...

            // create new list of room meshes for serialization
            List<Mesh> roomMeshes = new List<Mesh>();

            // cycle through all room mesh filters
            foreach (MeshFilter filter in roomMeshFilters)
            {
                // increase count of meshes in room
                meshCount++;

                // make mesh name = anchor name + mesh count
                string meshName = anchorStoreName + meshCount.ToString();
                filter.mesh.name = meshName;

                // add mesh to room meshes for serialization
                roomMeshes.Add(filter.mesh);

                // save world anchor
                WorldAnchor attachingAnchor = filter.gameObject.GetComponent<WorldAnchor>();
                if (attachingAnchor == null)
                {
                    attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
                }
                else
                {
                    // Deleting existing anchor...
                    Destroy(attachingAnchor);
                    // Creating new anchor...
                    attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
                }
                if (attachingAnchor.isLocated)
                {
                    if (!anchorStore.Save(meshName, attachingAnchor))
                        Debug.Log("" + meshName + ": Anchor save failed...");
                }
                else
                {
                    attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
                }
            }

            // serialize and save meshes
            MeshSaver.Save(fileName, roomMeshes);
            return true;
        }

        private void AttachingAnchor_OnTrackingChanged(WorldAnchor self, bool located)
        {
            if (located)
            {
                string meshName = self.gameObject.GetComponent<MeshFilter>().mesh.name;
                if (!anchorStore.Save(meshName, self))
                    Debug.Log("" + meshName + ": Anchor save failed...");

                self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
            }
        }
    }
}