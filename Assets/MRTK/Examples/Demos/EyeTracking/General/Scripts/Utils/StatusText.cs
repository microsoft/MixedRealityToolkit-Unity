// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    [System.Obsolete("This component is no longer supported", true)]
    [AddComponentMenu("Scripts/MRTK/Obsolete/StatusText")]
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

        private void Awake()
        {
            Debug.LogError(this.GetType().Name + " is deprecated");
        }

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
