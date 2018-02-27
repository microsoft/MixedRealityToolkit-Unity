using System.Collections;
using System.Collections.Generic;

using UnityEngine;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

public class SceneContentScaler : MonoBehaviour
{
    public enum HMDType
    {
        Immersive,
        Transparent
    }

    [SerializeField]
    [Tooltip("Which HMD type is assumed by default and does not change scale?")]
    private HMDType default_HMD_Type = HMDType.Immersive;

    [SerializeField]
    [Tooltip("Scale this GameObject by this factor if scene is being viewed with an HMD that is NOT the type described above.")]
    private Vector3 scaleIfNotDefault = new Vector3(1.0f, 1.0f, 1.0f);


    private void Start()
    {

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        if (HolographicSettings.IsDisplayOpaque)
        {
            if (default_HMD_Type != HMDType.Transparent)
            {
                transform.localScale = scaleIfNotDefault;
            }
        }
        else
        {
            //if( HolographicSettings.IsDisplayOpaque == false)
            if (default_HMD_Type == HMDType.Immersive)
            {
                transform.localScale = scaleIfNotDefault;
            }
        }
#else   // the only case to handle if WSA not defined is...
        if (default_HMD_Type == HMDType.Transparent)
        {
            transform.localScale = scaleIfNotDefault;
        }
#endif

    }

    private void Update()
    {
    }
}
