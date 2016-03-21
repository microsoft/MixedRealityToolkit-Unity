// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Windows.Speech;

namespace HoloToolkit.Unity
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

        /// <summary>
        /// Collection of supported keywords and their associated actions.
        /// </summary>
        private Dictionary<string, System.Action> keywordCollection;

        // Use this for initialization.
        private void Start()
        {
            // Create our keyword collection.
            keywordCollection = new Dictionary<string, System.Action>();
            keywordCollection.Add("send meshes", () => SendMeshes());

            // Tell the KeywordRecognizer about our keywords.
            keywordRecognizer = new KeywordRecognizer(keywordCollection.Keys.ToArray());

            // Register a callback for the KeywordRecognizer and start recognizing.
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
            System.Action keywordAction;

            if(keywordCollection.TryGetValue(args.text, out keywordAction))
            {
                keywordAction.Invoke();
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
                List<Vector3> verts = new List<Vector3>();
                verts.AddRange(source.vertices);
            
                for(int vertIndex=0; vertIndex < verts.Count; vertIndex++)
                {
                    verts[vertIndex] = filter.transform.TransformPoint(verts[vertIndex]);
                }

                clone.SetVertices(verts); 
                clone.SetTriangles(source.triangles, 0);
                meshesToSend.Add(clone);
                byte[] serialized = SimpleMeshSerializer.Serialize(meshesToSend);
                RemoteMeshSource.Instance.SendData(serialized);
            }
#endif
        }
    }
}