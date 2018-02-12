using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using MixedRealityToolkit.InputModule;

#if UNITY_WSA || UNITY_STANDALONE_WIN
using UnityEngine.Windows.Speech;
#endif


namespace MixedRealityToolkit.Examples.UX
{
    public class ProgressButton : Interactive
    {
        public override void OnInputClicked(MixedRealityToolkit.InputModule.EventData.InputClickedEventData eventData)
        {
            string na = this.gameObject.name;
            ProgressExamples examples = Object.FindObjectOfType<ProgressExamples>();
            examples.LaunchProgress(this.gameObject);
        }

    }
}
