// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LayoutElementMatchSize.cs" company="Exit Games GmbH">
//   Part of: Pun Cockpit Demo
// </copyright>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.UI;

namespace Photon.Pun.Demo.Cockpit
{
    /// <summary>
    /// Force a LayoutElement to march a RectTransform sizeDelta. Useful for complex child content 
    /// </summary>
    public class LayoutElementMatchSize : MonoBehaviour
    {

        public LayoutElement layoutElement;
        public RectTransform Target;


        public bool MatchHeight = true;
        public bool MatchWidth;


        void Update()
        {

            if (MatchHeight)
            {
                if (layoutElement.minHeight != Target.sizeDelta.y)
                {
                    layoutElement.minHeight = Target.sizeDelta.y;
                }
            }

        }
    }
}