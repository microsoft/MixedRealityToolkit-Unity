using HoloToolkit.Unity;
using UnityEngine;
using UnityEngine.VR.WSA;
using UnityEngine.VR.WSA.Persistence;
using System.Collections.Generic;
using HoloToolkit.Unity.SpatialMapping;

public class RoomSaver : MonoBehaviour
{

    public string fileName;             // name of file to store meshes
    public string anchorStoreName;      // name of world anchor to store for room

    List<MeshFilter> roomMeshFilters;
    WorldAnchorStore anchorStore;
    int meshCount = 0;

    // Use this for initialization
    void Start()
    {
        WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    void AnchorStoreReady(WorldAnchorStore store)
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

        Debug.Log("Old anchors deleted...");

        // get all mesh filters used for spatial mapping meshes
        roomMeshFilters = SpatialUnderstanding.Instance.UnderstandingCustomMesh.GetMeshFilters() as List<MeshFilter>;

        Debug.Log("Mesh filters fetched...");

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

            Debug.Log("Mesh " + filter.mesh.name + ": " + filter.transform.position + "\n--- rotation " + filter.transform.localRotation + "\n--- scale: " + filter.transform.localScale);
            // add mesh to room meshes for serialization
            roomMeshes.Add(filter.mesh);

            // save world anchor
            WorldAnchor attachingAnchor = filter.gameObject.GetComponent<WorldAnchor>();
            if (attachingAnchor == null)
            {
                attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
                Debug.Log("" + filter.mesh.name + ": Using new anchor...");
            }
            else
            {
                Debug.Log("" + filter.mesh.name + ": Deleting existing anchor...");
                DestroyImmediate(attachingAnchor);
                Debug.Log("" + filter.mesh.name + ": Creating new anchor...");
                attachingAnchor = filter.gameObject.AddComponent<WorldAnchor>();
            }
            if (attachingAnchor.isLocated)
            {
                if (!anchorStore.Save(meshName, attachingAnchor))
                    Debug.Log("" + meshName + ": Anchor save failed...");
                else
                    Debug.Log("" + meshName + ": Anchor SAVED...");
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
            else
                Debug.Log("" + meshName + ": Anchor SAVED...");

            self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
        }
    }
}