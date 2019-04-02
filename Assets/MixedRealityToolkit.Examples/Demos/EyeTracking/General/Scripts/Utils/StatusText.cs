// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    public class StatusText : MonoBehaviour
    {
        #region Singleton
        private static StatusText _Instance;
        public static StatusText Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = FindObjectOfType<StatusText>();
                }
                return _Instance;
            }
        }
        #endregion

        [SerializeField]
        private TextMesh status = null;

        public void Start()
        {
            StatusText.Instance.Log("...", false);
        }

        public void Log(string msg, bool add)
        {
            if (status != null)
            {
                if (add)
                {
                    status.text += msg;
                }
                else
                {
                    status.text = msg;
                }
            }
        }
    }
}
