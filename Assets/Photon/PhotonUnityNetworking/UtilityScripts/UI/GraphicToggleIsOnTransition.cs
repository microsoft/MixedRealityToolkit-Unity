// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ImageToggleIsOnTransition.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
//  Use this on Toggle graphics to have some color transition as well without corrupting toggle's behaviour.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>
    /// Use this on toggles texts to have some color transition on the text depending on the isOn State.
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    public class GraphicToggleIsOnTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Toggle toggle;

        private Graphic _graphic;

        public Color NormalOnColor = Color.white;
        public Color NormalOffColor = Color.black;
        public Color HoverOnColor = Color.black;
        public Color HoverOffColor = Color.black;

        private bool isHover;

        public void OnPointerEnter(PointerEventData eventData)
        {
            this.isHover = true;
            this._graphic.color = this.toggle.isOn ? this.HoverOnColor : this.HoverOffColor;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            this.isHover = false;
            this._graphic.color = this.toggle.isOn ? this.NormalOnColor : this.NormalOffColor;
        }

        public void OnEnable()
        {
            this._graphic = this.GetComponent<Graphic>();

            this.OnValueChanged(this.toggle.isOn);

            this.toggle.onValueChanged.AddListener(this.OnValueChanged);
        }

        public void OnDisable()
        {
            this.toggle.onValueChanged.RemoveListener(this.OnValueChanged);
        }

        public void OnValueChanged(bool isOn)
        {
            this._graphic.color = isOn ? (this.isHover ? this.HoverOnColor : this.HoverOnColor) : (this.isHover ? this.NormalOffColor : this.NormalOffColor);
        }
    }
}