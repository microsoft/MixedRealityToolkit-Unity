// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RoomListView.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;

using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Property listener base.
    /// </summary>
    public class PropertyListenerBase : MonoBehaviour
    {
        public Graphic UpdateIndicator;

        private YieldInstruction fadeInstruction = new YieldInstruction();

        float Duration = 1f;
        public void OnValueChanged()
        {
            StartCoroutine(FadeOut(UpdateIndicator));
        }

        IEnumerator FadeOut(Graphic image)
        {
            float elapsedTime = 0.0f;
            Color c = image.color;
            while (elapsedTime < Duration)
            {
                yield return fadeInstruction;
                elapsedTime += Time.deltaTime;
                c.a = 1.0f - Mathf.Clamp01(elapsedTime / Duration);
                image.color = c;
            }
        }
    }
}