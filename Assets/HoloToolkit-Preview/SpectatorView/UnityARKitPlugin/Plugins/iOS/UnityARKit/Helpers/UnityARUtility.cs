using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.iOS
{
    public class UnityARUtility
    {
        private MeshCollider meshCollider; //declared to avoid code stripping of class
        private MeshFilter meshFilter; //declared to avoid code stripping of class
#if UNITY_IOS || UNITY_EDITOR
        private static GameObject planePrefab = null;
#endif

        public static void InitializePlanePrefab(GameObject go)
        {
#if UNITY_IOS || UNITY_EDITOR
            planePrefab = go;
#endif
        }

#if UNITY_IOS || UNITY_EDITOR
        public static GameObject CreatePlaneInScene(ARPlaneAnchor arPlaneAnchor)
        {
            GameObject plane;
            if (planePrefab != null) {
                plane = GameObject.Instantiate(planePrefab);
            } else {
                plane = new GameObject (); //put in a blank gameObject to get at least a transform to manipulate
            }

            plane.name = arPlaneAnchor.identifier;

            return UpdatePlaneWithAnchorTransform(plane, arPlaneAnchor);

        }

        public static GameObject UpdatePlaneWithAnchorTransform(GameObject plane, ARPlaneAnchor arPlaneAnchor)
        {
            
            //do coordinate conversion from ARKit to Unity
            plane.transform.position = UnityARMatrixOps.GetPosition (arPlaneAnchor.transform);
            plane.transform.rotation = UnityARMatrixOps.GetRotation (arPlaneAnchor.transform);

            MeshFilter mf = plane.GetComponentInChildren<MeshFilter> ();

            if (mf != null) {
                //since our plane mesh is actually 10mx10m in the world, we scale it here by 0.1f
                mf.gameObject.transform.localScale = new Vector3(arPlaneAnchor.extent.x * 0.1f ,arPlaneAnchor.extent.y * 0.1f ,arPlaneAnchor.extent.z * 0.1f );

                //convert our center position to unity coords
                mf.gameObject.transform.localPosition = new Vector3(arPlaneAnchor.center.x,arPlaneAnchor.center.y, -arPlaneAnchor.center.z);
            }

            return plane;
        }
#endif

    }
}

