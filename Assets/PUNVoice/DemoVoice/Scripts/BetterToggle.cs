// ----------------------------------------------------------------------------
// <copyright file="BetterToggle.cs" company="Exit Games GmbH">
// Photon Voice Demo for PUN- Copyright (C) 2016 Exit Games GmbH
// </copyright>
// <summary>
// Unity UI extension class that should be used with Unity's built-in Toggle
// to broadcast value change in a better way.
// </summary>
// <author>developer@photonengine.com</author>
// ----------------------------------------------------------------------------

namespace ExitGames.Demos.DemoPunVoice {

    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(Toggle))]
    [DisallowMultipleComponent]
    public class BetterToggle : MonoBehaviour {
        private Toggle toggle;

        public delegate void OnToggle(Toggle toggle);

        public static event OnToggle ToggleValueChanged;

        private void Start() {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(); });
        }

        public void OnToggleValueChanged() {
            if (ToggleValueChanged != null) {
                ToggleValueChanged(toggle);
            }
        }
    }
}