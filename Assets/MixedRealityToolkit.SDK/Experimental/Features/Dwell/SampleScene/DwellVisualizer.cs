using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.Input;

namespace Microsoft.MixedReality.Toolkit.Experimental.Dwell
{
    public class DwellVisualizer : MonoBehaviour
    {
        private bool isDwelling = false;

        private DwellHandler dwellHandler;

        [SerializeField]
        private Image dwellVisualImage = null;

        [SerializeField]
        private TextMeshProUGUI dwellStatus = null;

        public void DwellStarted(IMixedRealityPointer pointer)
        {
            Debug.Log("dwellstarted with pointer " + pointer.InputSourceParent.SourceType);
            isDwelling = true;
        }

        public void DwellIntended(IMixedRealityPointer pointer)
        {
            Debug.Log("dwellintended " + pointer.InputSourceParent.SourceType);
        }

        public void DwellCanceled(IMixedRealityPointer pointer)
        {
            Debug.Log("dwellcanceled");
            isDwelling = false;
        }

        public void DwellCompleted(IMixedRealityPointer pointer)
        {
            Debug.Log("dwellCompleted");
            isDwelling = false;

            dwellVisualImage.transform.localScale = Vector3.zero;
        }

        private void Awake()
        {
            dwellHandler = this.GetComponent<DwellHandler>();
        }

        public void Update()
        {
            if (isDwelling)
            {
                float value = dwellHandler.CalculateDwellProgress();
                dwellVisualImage.transform.localScale = new Vector3(value, value, value);
            }
        }

        public void ButtonExecute()
        {
            dwellStatus.text = "Button Executed";
        }
    }
}