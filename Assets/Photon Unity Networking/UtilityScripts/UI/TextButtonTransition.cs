// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TextButtonTransition.cs" company="Exit Games GmbH">
// </copyright>
// <summary>
//  Use this on Button texts to have some color transition on the text as well without corrupting button's behaviour.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------


using UnityEngine;  
using System.Collections;  
using UnityEngine.EventSystems;  
using UnityEngine.UI;

namespace ExitGames.UtilityScripts
{

	/// <summary>
	/// Use this on Button texts to have some color transition on the text as well without corrupting button's behaviour.
	/// </summary>
	[RequireComponent(typeof(Text))]
	public class TextButtonTransition : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {
		
		Text _text;

		public Color NormalColor= Color.white;
		public Color HoverColor = Color.black;

		public void Awake()
		{
			_text = GetComponent<Text>();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			_text.color = HoverColor;
		}
		
		public void OnPointerExit(PointerEventData eventData)
		{
			_text.color = NormalColor; 
		}
	}
}