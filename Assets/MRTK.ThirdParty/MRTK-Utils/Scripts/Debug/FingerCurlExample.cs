//------------------------------------------------------------------------------ -
//MRTK - Quest
//https ://github.com/provencher/MRTK-Quest
//Contribution by: Chaitanya Shah (https://github.com/chetu3319)
//------------------------------------------------------------------------------ -
//
//MIT License
//
//Copyright(c) 2020 Chaitanya Shah
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files(the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and / or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions :
//
//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.
//------------------------------------------------------------------------------ -

using UnityEngine;
using Microsoft.MixedReality.Toolkit.Utilities;

namespace prvncher.MixedReality.Toolkit.Utils
{
    public class FingerCurlExample : MonoBehaviour
    {
        [SerializeField]
        private Handedness handedness = Handedness.None;

        [SerializeField]
        private GameObject indexFinger = null;

        [SerializeField]
        private GameObject middleFinger = null;

        [SerializeField]
        private GameObject ringFinger = null;

        [SerializeField]
        private GameObject pinkyFinger = null;

        [SerializeField]
        private GameObject thumbFinger = null;

        void Update()
        {
            if (indexFinger != null)
            {
                var indexCurl = HandPoseUtils.IndexFingerCurl(handedness);
                indexFinger.transform.localScale = new Vector3(1.0f, 1.0f - indexCurl, 1.0f);
            }

            if (middleFinger != null)
            {
                var middleCurl = HandPoseUtils.MiddleFingerCurl(handedness);
                middleFinger.transform.localScale = new Vector3(1.0f, 1.0f - middleCurl, 1.0f);
            }

            if (ringFinger != null)
            {
                var ringCurl = HandPoseUtils.RingFingerCurl(handedness);
                ringFinger.transform.localScale = new Vector3(1.0f, 1.0f - ringCurl, 1.0f);
            }

            if (pinkyFinger != null)
            {
                var pinkyCurl = HandPoseUtils.PinkyFingerCurl(handedness);
                pinkyFinger.transform.localScale = new Vector3(1.0f, 1.0f - pinkyCurl, 1.0f);
            }

            if (thumbFinger != null)
            {
                var thumbCurl = HandPoseUtils.ThumbFingerCurl(handedness);
                thumbFinger.transform.localScale = new Vector3(1.0f, 1.0f - thumbCurl, 1.0f);
            }
        }

    }
}
