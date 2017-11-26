using HoloToolkit.Unity.Boundary;
using UnityEngine;

#if UNITY_2017_2_OR_NEWER
using UnityEngine.XR;
#else
using UnityEngine.VR;
#endif

public class SceneContentAdjuster : MonoBehaviour
{
    private Vector3 lastFloorHeight;
    private float floorHeightOffset = 1f;

    private void Awake()
    {
#if UNITY_2017_2_OR_NEWER
        if (Application.isEditor && XRDevice.isPresent)
        {
            lastFloorHeight.y = floorHeightOffset;
            transform.position = lastFloorHeight;
        }
#else
        if (VRDevice.isPresent)
        {
            Destroy(this);
        }
#endif
    }

    private void Update()
    {
#if UNITY_2017_2_OR_NEWER
        if (!Application.isEditor && XRDevice.isPresent)
        {
            floorHeightOffset = BoundaryManager.Instance.CurrentFloorHeightOffset;

            if (lastFloorHeight.y != floorHeightOffset)
            {
                lastFloorHeight.y = floorHeightOffset;
                transform.position = lastFloorHeight;
            }
        }
#endif
    }
}
