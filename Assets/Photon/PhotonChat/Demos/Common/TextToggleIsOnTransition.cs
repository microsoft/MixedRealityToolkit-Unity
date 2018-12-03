// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextToggleIsOnTransition.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
//  Use this on Button texts to have some color transition on the text as well without corrupting button's behaviour.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using UnityEngine;  
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Photon.Chat.UtilityScripts
{

	/// <summary>
	/// Use this on toggles texts to have some color transition on the text depending on the isOn State.
	/// </summary>
	[RequireComponent(typeof(Text))]
	public class TextToggleIsOnTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {

        /// <summary>
        /// The toggle Component.
        /// </summary>
		public Toggle toggle;

		Text _text;

        /// <summary>
        /// The color of the normal on transition state.
        /// </summary>
		public Color NormalOnColor= Color.white;

        /// <summary>
        /// The color of the normal off transition state.
        /// </summary>
		public Color NormalOffColor = Color.black;

        /// <summary>
        /// The color of the hover on transition state.
        /// </summary>
		public Color HoverOnColor= Color.black;

        /// <summary>
        /// The color of the hover off transition state.
        /// </summary>
		public Color HoverOffColor = Color.black;

		bool isHover;

		public void OnEnable()
		{
			_text = GetComponent<Text>();
		
			OnValueChanged (toggle.isOn);

			toggle.onValueChanged.AddListener(OnValueChanged);

		}

		public void OnDisable()
		{
			toggle.onValueChanged.RemoveListener(OnValueChanged);
		}

		public void OnValueChanged(bool isOn)
		{
				_text.color = isOn? (isHover?HoverOnColor:HoverOnColor) : (isHover?NormalOffColor:NormalOffColor) ;
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			isHover = true;
			_text.color = toggle.isOn?HoverOnColor:HoverOffColor;
		}
		
		public void OnPointerExit(PointerEventData eventData)
		{
			isHover = false;
			_text.color = toggle.isOn?NormalOnColor:NormalOffColor;
		}

	}
}