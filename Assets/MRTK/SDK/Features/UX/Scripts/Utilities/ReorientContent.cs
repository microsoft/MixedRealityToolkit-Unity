using Microsoft.MixedReality.Toolkit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Utilities
{
    /// <summary>
    /// Helper script to help recenter and rocient scene content in front of the user
    /// </summary>
    public class ReorientContent : MonoBehaviour
    {
        public void AdjustContentOrientation()
        {
            MixedRealitySceneContent sceneContent = GameObject.Find("MixedRealitySceneContent").GetComponent<MixedRealitySceneContent>();
            sceneContent.ReorientContent();
        }
    }
}