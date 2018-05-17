using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.iOS
{
    public class UnityARAnchorManager
    {
        private Dictionary<string, ARPlaneAnchorGameObject> planeAnchorMap;

        public UnityARAnchorManager()
        {
            planeAnchorMap = new Dictionary<string, ARPlaneAnchorGameObject>();
            UnityARSessionNativeInterface.ARAnchorAddedEvent += AddAnchor;
            UnityARSessionNativeInterface.ARAnchorUpdatedEvent += UpdateAnchor;
            UnityARSessionNativeInterface.ARAnchorRemovedEvent += RemoveAnchor;
        }

        public void AddAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            Debug.Log("Add anchor in manager");
            GameObject go = UnityARUtility.CreatePlaneInScene(arPlaneAnchor);
            Object.DontDestroyOnLoad(go);
            var anchor = new ARPlaneAnchorGameObject
            {
                planeAnchor = arPlaneAnchor,
                gameObject = go
            };

            planeAnchorMap.Add(arPlaneAnchor.identifier, anchor);
        }

        public void RemoveAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            Debug.Log("Remove anchor in manager");

            if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
            {
                ARPlaneAnchorGameObject anchor = planeAnchorMap[arPlaneAnchor.identifier];
                Object.Destroy(anchor.gameObject);
                planeAnchorMap.Remove(arPlaneAnchor.identifier);
            }
        }

        public void UpdateAnchor(ARPlaneAnchor arPlaneAnchor)
        {
            Debug.Log("Update anchor in manager");

            if (planeAnchorMap.ContainsKey(arPlaneAnchor.identifier))
            {
                ARPlaneAnchorGameObject anchor = planeAnchorMap[arPlaneAnchor.identifier];
                UnityARUtility.UpdatePlaneWithAnchorTransform(anchor.gameObject, arPlaneAnchor);
                anchor.planeAnchor = arPlaneAnchor;
                planeAnchorMap[arPlaneAnchor.identifier] = anchor;
            }
        }

        public void Destroy()
        {
            List<ARPlaneAnchorGameObject> list = GetCurrentPlaneAnchors();
            for (var i = 0; i < list.Count; i++)
            {
                Object.Destroy(list[i].gameObject);
            }

            planeAnchorMap.Clear();
        }

        public List<ARPlaneAnchorGameObject> GetCurrentPlaneAnchors()
        {
            return planeAnchorMap.Values.ToList();
        }
    }
}
