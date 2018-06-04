using System;
using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.iOS
{
    public class UnityARAnchorManager 
    {


        private Dictionary<string, ARPlaneAnchorGameObject> planeAnchorMap = null;


        public UnityARAnchorManager ()
        {
#if UNITY_IOS || UNITY_EDITOR
            planeAnchorMap = new Dictionary<string,ARPlaneAnchorGameObject> ();
            UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
            UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
            UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
#endif
        }

#if UNITY_IOS || UNITY_EDITOR
        public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            Debug.Log("Add anchor in manager");
            GameObject go = UnityARUtility.CreatePlaneInScene (arPlaneAnchor);
            go.AddComponent<DontDestroyOnLoad> ();  //this is so these GOs persist across scene loads
            ARPlaneAnchorGameObject arpag = new ARPlaneAnchorGameObject ();
            arpag.planeAnchor = arPlaneAnchor;
            arpag.gameObject = go;
            planeAnchorMap.Add (arPlaneAnchor.identifier, arpag);
        }
    

        public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            Debug.Log("Remove anchor in manager");

            if (planeAnchorMap.ContainsKey (arPlaneAnchor.identifier)) {
                ARPlaneAnchorGameObject arpag = planeAnchorMap [arPlaneAnchor.identifier];
                GameObject.Destroy (arpag.gameObject);
                planeAnchorMap.Remove (arPlaneAnchor.identifier);
            }
        }

        public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            Debug.Log("Update anchor in manager");

            if (planeAnchorMap.ContainsKey (arPlaneAnchor.identifier)) {
                ARPlaneAnchorGameObject arpag = planeAnchorMap [arPlaneAnchor.identifier];
                UnityARUtility.UpdatePlaneWithAnchorTransform (arpag.gameObject, arPlaneAnchor);
                arpag.planeAnchor = arPlaneAnchor;
                planeAnchorMap [arPlaneAnchor.identifier] = arpag;
            }
        }
#endif
        public void Destroy()
        {
            foreach (ARPlaneAnchorGameObject arpag in GetCurrentPlaneAnchors()) {
                GameObject.Destroy (arpag.gameObject);
            }

            planeAnchorMap.Clear ();
        }

        public List<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors()
        {
            return planeAnchorMap.Values.ToList ();
        }
    }
}

