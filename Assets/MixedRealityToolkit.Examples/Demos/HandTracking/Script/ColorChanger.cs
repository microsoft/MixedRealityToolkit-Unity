using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos
{
    public class ColorChanger : MonoBehaviour
    {
        public MeshRenderer rend;
        public Material[] mats;
        public int cur;

        public void Increment()
        {
            if (mats != null && mats.Length > 0)
            {
                cur = (cur + 1) % mats.Length;
                if (rend != null)
                {
                    rend.material = mats[cur];
                }
            }
        }
    }
}