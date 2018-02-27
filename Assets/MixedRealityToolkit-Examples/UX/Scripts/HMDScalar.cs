using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_WSA && UNITY_2017_2_OR_NEWER
using UnityEngine.XR.WSA;
#endif

public class HMDScalar : MonoBehaviour {


    [SerializeField]
    [Tooltip("Scale this GameObject by this factor is scene is being viewed with an HMD that is immersive.")];
    private 
    // Use this for initialization
    void Start () {

#if UNITY_WSA && UNITY_2017_2_OR_NEWER
        // Optimize the content for immersive headset
        if (HolographicSettings.IsDisplayOpaque)
        {
            transform.localScale = IHMDScalar;
        }
#endif

    }

    // Update is called once per frame
    void Update () {
		
	}
}
