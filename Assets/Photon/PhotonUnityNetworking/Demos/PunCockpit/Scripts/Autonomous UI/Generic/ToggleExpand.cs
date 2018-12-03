// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleExpand.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// UI toggle to activate GameObject.
    /// </summary>
    public class ToggleExpand : MonoBehaviour
    {
        public GameObject Content;

        public Toggle Toggle;

        bool _init;

        void OnEnable()
        {
            Content.SetActive(Toggle.isOn);

            if (!_init)
            {
                _init = true;
                Toggle.onValueChanged.AddListener(HandleToggleOnValudChanged);
            }

            HandleToggleOnValudChanged(Toggle.isOn);

        }


        void HandleToggleOnValudChanged(bool value)
        {
            Content.SetActive(value);
        }

    }
}