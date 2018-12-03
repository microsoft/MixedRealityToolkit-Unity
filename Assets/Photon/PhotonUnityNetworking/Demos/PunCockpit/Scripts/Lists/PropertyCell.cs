// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyCell.cs" company="Exit Games GmbH">
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
    /// Generic string Property Cell.
    /// </summary>
    public class PropertyCell : MonoBehaviour
    {
        public Text PropertyText;
        public Text ValueText;

        public Image isUpdatedFlag;

        public LayoutElement LayoutElement;

        public void UpdateInfo(string value)
        {
            bool _pingUpdate = string.Equals(this.ValueText.text, value);
            this.ValueText.text = value;

			if (this!=null && this.isActiveAndEnabled && _pingUpdate)
            {
                StartCoroutine(UpdateUIPing());
            }
        }

        public void AddToList(string property, string value, bool animate = false)
        {
            this.PropertyText.text = property;
            if (animate)
            {
                UpdateInfo(value);
            }
            else
            {
                this.ValueText.text = value;
                isUpdatedFlag.gameObject.SetActive(false);
            }



            if (animate)
            {

                StartCoroutine("Add");
            }
            else
            {
                LayoutElement.minHeight = 30f;
            }
        }

        public void RemoveFromList()
        {
            StartCoroutine("Remove");
        }

        IEnumerator UpdateUIPing()
        {
            isUpdatedFlag.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);

            isUpdatedFlag.gameObject.SetActive(false);
        }

        IEnumerator Add()
        {
            LayoutElement.minHeight = 0f;

            while (LayoutElement.minHeight != 30f)
            {

                LayoutElement.minHeight = Mathf.MoveTowards(LayoutElement.minHeight, 30f, 2f);
                yield return new WaitForEndOfFrame();
            }
        }

        IEnumerator Remove()
        {
            while (LayoutElement.minHeight != 0f)
            {
                LayoutElement.minHeight = Mathf.MoveTowards(LayoutElement.minHeight, 0f, 2f);
                yield return new WaitForEndOfFrame();
            }

            Destroy(this.gameObject);
        }

    }
}