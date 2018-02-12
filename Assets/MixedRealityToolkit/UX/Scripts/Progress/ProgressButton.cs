using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using MixedRealityToolkit.InputModule;
using MixedRealityToolkit.Examples.UX;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif


namespace MixedRealityToolkit.UX.Buttons
{
    public class ProgressButton : MonoBehaviour
    {
        void OnEnable()
        {
            GetComponent<Button>().OnButtonClicked += OnButtonClicked;
        }

        private void OnButtonClicked(GameObject obj)
        {
            string na = this.gameObject.name;
            ProgressExamples examples = Object.FindObjectOfType<ProgressExamples>();
            examples.LaunchProgress(this.gameObject);
        }
    }
}
