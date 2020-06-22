// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Input;
using UnityEngine;
using TMPro;
using Microsoft.MixedReality.Toolkit.UI;
using System.Linq;

namespace Microsoft.MixedReality.Toolkit.Experimental.ColorPicker
{
    /// <summary>
    /// Example script to demonstrate how to summon the color picker control at runtime.
    /// </summary>
    public class ColorPickerSummon : MonoBehaviour
    {
        public GameObject ColorPickerGameObject;
        private ColorPicker Picker;
        public void Summon(GameObject anchor)
        {
            ColorPickerGameObject.SetActive(true);
            ColorPickerGameObject.transform.position = anchor.transform.position;
            ColorPickerGameObject.transform.rotation = anchor.transform.rotation;
            Picker = ColorPickerGameObject.GetComponent<ColorPicker>();
            Picker.TargetObjectMesh = GameObject.Find(anchor.name + "/TargetObject (Mesh)").GetComponent<MeshRenderer>();
            Picker.TargetObjectSprite = GameObject.Find(anchor.name + "/TargetObject (Sprite)").GetComponent<SpriteRenderer>();
            Picker.ExtractColorFromMaterial(Picker.TargetObjectMesh);
        }
    }
}
