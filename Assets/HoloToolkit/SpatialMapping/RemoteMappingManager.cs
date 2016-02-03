using UnityEngine;
using UnityEngine.WSA.Speech;
using System.Collections.Generic;

namespace HoloToolkit
{
    [RequireComponent(typeof(RemoteMeshTarget))]
    public class RemoteMappingManager : Singleton<RemoteMappingManager>
    { 
        /// <summary>
        /// Recieves meshes collected over the network.
        /// </summary>
        private RemoteMeshTarget remoteMeshTarget;

        /// <summary>
        /// Used for voice commands.
        /// </summary>
        private KeywordRecognizer keywordRecognizer;

        // Use this for initialization.
        private void Start()
        {
            // Setup a keyword recognizer to allow the user to send meshes over the network.
            List<string> Keywords = new List<string>();
            Keywords.Add("send meshes");

            keywordRecognizer = new KeywordRecognizer(Keywords.ToArray());
            keywordRecognizer.OnPhraseRecognized += KeywordRecognizer_OnPhraseRecognized;
            keywordRecognizer.Start();

#if UNITY_EDITOR
            remoteMeshTarget = GetComponent<RemoteMeshTarget>();

            if (remoteMeshTarget != null && SpatialMappingManager.Instance.Source == null)
            {
                // Use the network-based mapping source to recieve meshes in the Unity editor.
                SpatialMappingManager.Instance.SetSpatialMappingSource(remoteMeshTarget);
            }
#endif
        }

        // Called every frame by the Unity engine.
        private void Update()
        {
#if UNITY_EDITOR
            // N - To use the 'network' sourced mesh.  
            if (Input.GetKeyUp(KeyCode.N))
            {
                SpatialMappingManager.Instance.SetSpatialMappingSource(remoteMeshTarget);
            }
#endif
        }

        /// <summary>
        /// Called by keywordRecognizer when a phrase we registered for is heard.
        /// </summary>
        /// <param name="args">Information about the recognition event.</param>
        private void KeywordRecognizer_OnPhraseRecognized(PhraseRecognizedEventArgs args)
        {
            switch (args.text.ToLower())
            {
                case "send meshes":
                    SendMeshes();
                    break;
            }
        }

        /// <summary>
        /// Sends the spatial mapping surfaces from the HoloLens to a remote system running the Unity editor.
        /// </summary>
        private void SendMeshes()
        {
#if !UNITY_EDITOR
            List<MeshFilter> MeshFilters = SpatialMappingManager.Instance.GetMeshFilters();
            for (int index = 0; index < MeshFilters.Count; index++)
            {
                List<Mesh> meshesToSend = new List<Mesh>();
                MeshFilter filter = MeshFilters[index];
                Mesh source = filter.sharedMesh;
                Mesh clone = new Mesh();
                Vector3[] verts = (Vector3[])source.vertices.Clone();
            
                for(int vertIndex=0; vertIndex < verts.Length; vertIndex++)
                {
                    verts[vertIndex] = filter.transform.TransformPoint(verts[vertIndex]);
                }

                clone.SetVertices(verts, verts.Length); 
                clone.SetTriangles(source.triangles, 0);
                meshesToSend.Add(clone);
                byte[] serialized = SimpleMeshSerializer.Serialize(meshesToSend);
                RemoteMeshSource.Instance.SendData(serialized);
            }
#endif
        }
    }
}