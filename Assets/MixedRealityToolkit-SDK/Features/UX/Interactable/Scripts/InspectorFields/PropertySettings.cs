// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    [System.Serializable]
    public struct PropertySetting
    {
        public InspectorField.FieldTypes Type;
        public string Label;
        public string Name;
        public string Tooltip;
        public object Value;
        public int IntValue { get {  return (int)Value; } set { Value = value; } }
        public string StringValue { get { return (string)Value; } set { Value = value; } }
        public float FloatValue { get { return (float)Value; } set { Value = value; } }
        public bool BoolValue { get { return (bool)Value; } set { Value = value; } }
        public GameObject GameObjectValue { get { return (GameObject)Value; } set { Value = value; } }
        public ScriptableObject ScriptableObjectValue { get { return (ScriptableObject)Value; } set { Value = value; } }
        public UnityEngine.Object ObjectValue { get { return (UnityEngine.Object)Value; } set { Value = value; } }
        public Material MaterialValue { get { return (Material)Value; } set { Value = value; } }
        public Texture TextureValue { get { return (Texture)Value; } set { Value = value; } }
        public Color ColorValue { get { return (Color)Value; } set { Value = value; } }
        public Vector2 Vector2Value { get { return (Vector2)Value; } set { Value = value; } }
        public Vector3 Vector3Value { get { return (Vector3)Value; } set { Value = value; } }
        public Vector4 Vector4Value { get { return (Vector4)Value; } set { Value = value; } }
        public AnimationCurve CurveValue { get { return (AnimationCurve)Value; } set { Value = value; } }
        public AudioClip AudioClipValue { get { return (AudioClip)Value; } set { Value = value; } }
        public Quaternion QuaternionValue { get { return (Quaternion)Value; } set { Value = value; } }
        public UnityEvent EventValue { get { return (UnityEvent)Value; } set { Value = value; } }
        public string[] Options;
    }
}
