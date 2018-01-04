// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;
using UnityEngine.UI;

namespace MixedRealityToolkit.Examples.Prototyping
{
    /// <summary>
    /// CycleStatusDisplay shows the current index and array length (i/l) of a CycleBase component
    /// Works with TextMesh or UI Text
    /// </summary>
    public class CycleStatusDisplay : MonoBehaviour
    {
        [Tooltip("A GameObject containing a component that implements ICycle")]
        public GameObject CycleHost;

        private ICycle mCycleHost;
        private TextMesh mTextMesh;
        private Text mText;

        // Use this for initialization
        void Awake()
        {
            if(CycleHost == null)
            {
                CycleHost = this.gameObject;
                Debug.Log("CycleHost was set to self by default");
            }

            mTextMesh = GetComponent<TextMesh>();
            mText = GetComponent<Text>();

            if (mTextMesh == null && mText == null)
            {
               Debug.LogError("There are no Text Components on this <GameObject:" + this.gameObject.name + ">");
            }

            mCycleHost = CycleHost.GetComponent<ICycle>();
        }

        /// <summary>
        /// Update the status of the ICycle component
        /// </summary>
        void Update()
        {
            if (mTextMesh != null && mCycleHost != null)
            {
                mTextMesh.text = (mCycleHost.Index + 1) + "/" + (mCycleHost.GetLastIndex() + 1);
            }

            if (mText != null && mCycleHost != null)
            {
                mText.text = (mCycleHost.Index + 1) + "/" + (mCycleHost.GetLastIndex() + 1);
            }
        }
    }

}
