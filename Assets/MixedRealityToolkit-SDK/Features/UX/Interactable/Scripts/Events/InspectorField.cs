using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace HoloToolkit.Unity
{
    public class InspectorField : Attribute
    {
        public enum FieldTypes { Float, Int, String, Bool, Color, DropdownInt, DropdownString, GameObject, ScriptableObject, Object, Material, Texture, Vector2, Vector3, Vector4, Curve, Quaternion, AudioClip }

        public FieldTypes Type { get; set; }
        public string Label { get; set; }
        public string Tooltip { get; set; }
        public string[] Options { get; set; }
    }
}
