// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blend
{
    /// <summary>
    /// Abstract class for accessing and setting color values on a different types of objects
    /// </summary>
    public abstract class ColorObject
    {
        /// <summary>
        /// Settings for the type of color value to access in the shader
        /// </summary>
        //public enum ShaderColorTypes { Color, TintColor, RimColor, EmissiveColor, HoverColorOverride, EmissionColor }
        public const string DefaultColor = "_Color";
        protected string shaderColorProperty = DefaultColor;
        

        /// <summary>
        /// common float shader properties
        /// </summary>
        //public enum ShaderFloats { Cutoff, Smoothness, Ambient, CameraIntensity, Contrast, RimPower, RoundCornerPower, RoundCornerMargin, BorderWidth, BorderMinValue, RefractiveIndex, TransitionTime, Glossiness, Metallic, SpecularHighlights, GlossyReflections, BumpScale, Parallax }

        /// <summary>
        /// reference to the the host game object
        /// </summary>
        public GameObject GameObjectRef;

        /// <summary>
        /// is the assumed object type valid.
        /// </summary>
        public bool Valid { get { return isValid; } }
        protected bool isValid = false;

        public ColorObject(GameObject gameObject)
        {
            // constructor
            GameObjectRef = gameObject;
        }

        /// <summary>
        /// Sets the color on the object
        /// </summary>
        /// <param name="color">Color: color to set</param>
        public abstract void SetColor(Color color);

        /// <summary>
        /// gets the color on the object
        /// </summary>
        /// <returns></returns>
        public abstract Color GetColor();

        /// <summary>
        /// sets the alpha value of the color
        /// </summary>
        /// <param name="alpha"></param>
        public abstract void SetAlpha(float alpha);

        /// <summary>
        /// get the current alpha value
        /// </summary>
        /// <returns></returns>
        public abstract float GetAlpha();

        /// <summary>
        /// set a float property of a shader
        /// </summary>
        /// <param name="property"></param>
        /// <param name="value"></param>
        public abstract void SetFloat(string property, float value);

        /// <summary>
        /// get the current property of the shader
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public abstract float GetFloat(string property);

        /// <summary>
        /// sets the shader color property string using in Material.GetColor() or Material.SetColor()
        /// </summary>
        /// <param name="type">string</param>
        public void SetShaderColorType(string property)
        {
            shaderColorProperty = property;
        }
    }

    /// <summary>
    /// A version of ColorObject that accesses color properties on UI Text objects
    /// </summary>
    public class TextColorObject : ColorObject
    {
        private Text text;

        public TextColorObject(GameObject gameObject) : base(gameObject)
        {
            text = gameObject.GetComponent<Text>();
            isValid = text != null;
        }

        public override void SetColor(Color color)
        {
            if (text != null)
            {
                text.color = color;
            }
        }

        public override Color GetColor()
        {
            if (text != null)
            {
                return text.color;
            }
            return Color.black;
        }

        public override void SetAlpha(float alpha)
        {
            if (text != null)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }
        }

        public override float GetAlpha()
        {
            if (text != null)
            {
                Color color = text.color;
                return color.a;
            }
            return 0;
        }
        
        public override void SetFloat(string property, float value)
        {
            // ignore for text objects
            Debug.Log("Trying to set a shader value on a Text object - no value was set");
        }

        public override float GetFloat(string property)
        {
            // ignore for text objects
            Debug.Log("Trying to get a shader value from a Text object - this always returns 0");
            return 0;
        }
    }

    /// <summary>
    /// A version of ColorObject that accesses color properties on TextMesh objects
    /// </summary>
    public class TextMeshColorObject : ColorObject
    {
        private TextMesh textMesh;
        public TextMeshColorObject(GameObject gameObject) : base(gameObject)
        {
            textMesh = gameObject.GetComponent<TextMesh>();
            isValid = textMesh != null;
        }

        public override void SetColor(Color color)
        {
            if (textMesh != null)
            {
                textMesh.color = color;
            }
        }

        public override Color GetColor()
        {
            if (textMesh != null)
            {
                return textMesh.color;
            }
            return Color.black;
        }

        public override void SetAlpha(float alpha)
        {
            if (textMesh != null)
            {
                Color color = textMesh.color;
                color.a = alpha;
                textMesh.color = color;
            }
        }

        public override float GetAlpha()
        {
            if (textMesh != null)
            {
                Color color = textMesh.color;
                return color.a;
            }
            return 0;
        }

        public override void SetFloat(string property, float value)
        {
            // ignore for text objects
            Debug.Log("Trying to set a shader value on a Text object - no value was set");
        }

        public override float GetFloat(string property)
        {
            // ignore for text objects
            Debug.Log("Trying to get a shader value from a Text object - this always returns 0");
            return 0;
        }
    }

    /// <summary>
    /// A version of Color Object that accesses color properties on material shaders
    /// </summary>
    public class MaterialColorObject : ColorObject
    {
        private MaterialPropertyBlock materialBlock;

        public MaterialColorObject(GameObject gameObject) : base(gameObject)
        {
            materialBlock = new MaterialPropertyBlock();
            Renderer renderer = gameObject.GetComponent<Renderer>();
            renderer.GetPropertyBlock(materialBlock);
            
            Color color = Color.blue;
            if (renderer != null)
            {
                Material material = ColorAbstraction.GetValidMaterial(renderer);
                if(material != null)
                {
                    color = material.GetVector(shaderColorProperty);
                }

                materialBlock.SetColor(shaderColorProperty, color);
            }

            gameObject.GetComponent<Renderer>().SetPropertyBlock(materialBlock);
            
            isValid = true;
        }

        public override void SetColor(Color color)
        {
            GameObjectRef.GetComponent<Renderer>().GetPropertyBlock(materialBlock);
            materialBlock.SetColor(shaderColorProperty, color);
            GameObjectRef.GetComponent<Renderer>().SetPropertyBlock(materialBlock);
        }

        public override Color GetColor()
        {
            GameObjectRef.GetComponent<Renderer>().GetPropertyBlock(materialBlock);
            Color color = materialBlock.GetVector(shaderColorProperty);
            return color;
        }

        public override void SetAlpha(float alpha)
        {
            Color color = GetColor();
            color.a = alpha;
            SetColor(color);
        }

        public override float GetAlpha()
        {
            Color color = GetColor();
            return color.a;
        }

        public override void SetFloat(string property, float value)
        {
            GameObjectRef.GetComponent<Renderer>().GetPropertyBlock(materialBlock);
            materialBlock.SetFloat(property, value);
            GameObjectRef.GetComponent<Renderer>().SetPropertyBlock(materialBlock);
        }

        public override float GetFloat(string property)
        {
            GameObjectRef.GetComponent<Renderer>().GetPropertyBlock(materialBlock);
            float value = materialBlock.GetFloat(property);
            return value;
        }
    }

    /// <summary>
    /// A version of Color Object that accesses color properties of mulitple materials
    /// </summary>
    public class MaterialsColorObject : ColorObject
    {
        private Material[] materials;

        public MaterialsColorObject(GameObject gameObject) : base(gameObject)
        {
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    materials = renderer.sharedMaterials;
                }
                else
                {
                    materials = renderer.materials;
                }
#else
                materials = renderer.materials;
#endif
            }

            isValid = materials.Length > 1;
        }

        public override void SetColor(Color color)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetColor(shaderColorProperty, color);
                }
            }
        }

        public override Color GetColor()
        {
            if (materials != null)
            {
                return materials[0].GetColor(shaderColorProperty);
            }
            return Color.black;
        }

        public override void SetAlpha(float alpha)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    Color color = materials[i].GetColor(shaderColorProperty);
                    color.a = alpha;
                    materials[i].SetColor(shaderColorProperty, color);
                }
            }
        }

        public override float GetAlpha()
        {
            if (materials != null)
            {
                Color color = materials[0].GetColor(shaderColorProperty);
                return color.a;
            }
            return 0;
        }

        public override void SetFloat(string property, float value)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    materials[i].SetFloat(property, value);
                }
            }
        }

        public override float GetFloat(string property)
        {
            if (materials != null)
            {
                float value = materials[0].GetFloat(property);
                return value;
            }
            return 0;
        }

        public Material[] GetMaterials()
        {
            return materials;
        }

        public Color GetColorByMaterialName(string name)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (TrimName(materials[i].name) == name)
                    {
                        return materials[i].GetColor(shaderColorProperty);
                    }
                }
            }

            return Color.black;
        }

        public void SetColorByMaterialName(Color color, string name)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (TrimName(materials[i].name) == name)
                    {
                        materials[i].SetColor(shaderColorProperty, color);
                    }
                }
            }
        }

        public void SetAlphaByMaterialName(float alpha, string name)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (TrimName(materials[i].name) == name)
                    {
                        Color color = materials[i].GetColor(shaderColorProperty);
                        color.a = alpha;
                        materials[i].SetColor(shaderColorProperty, color);
                    }
                }
            }
        }

        public float GetAlphaByMaterialName(string name)
        {
            if (materials != null)
            {
                for (int i = 0; i < materials.Length; i++)
                {
                    if (TrimName(materials[i].name) == name)
                    {
                        Color color = materials[i].GetColor(shaderColorProperty);
                        return color.a;
                    }
                }
            }
            return 0;
        }

        private string TrimName(string name)
        {
            int SpaceIndex = name.IndexOf(" ");
            if (SpaceIndex > -1)
            {
                name = name.Substring(0, SpaceIndex);
            }

            return name;
        }
    }

    /// <summary>
    /// Abstraction layer for accessing colorObjects based on object type
    /// </summary>
    public class ColorAbstraction: ColorObject
    {
        private ColorObject colorObject;

        public ColorAbstraction(GameObject gameObject, string colorProperty = DefaultColor) : base(gameObject)
        {
            GameObjectRef = gameObject;
            colorObject = GetColorObject(gameObject);
            if (colorObject != null)
            {
                shaderColorProperty = colorProperty;
                isValid = true;
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object!");
            }
        }

        public override Color GetColor()
        {
            if (colorObject != null)
            {
                return colorObject.GetColor();
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object! Default color returned!");
            }
            return Color.black;
        }

        public override void SetColor(Color color)
        {
            if (colorObject != null)
            {
                colorObject.SetColor(color);
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object! No Colors set");
            }
        }

        public override void SetAlpha(float alpha)
        {
            if (colorObject != null)
            {
                colorObject.SetAlpha(alpha);
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object! No alpha set");
            }
        }

        public override float GetAlpha()
        {
            if (colorObject != null)
            {
                return colorObject.GetAlpha();
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object! No alpha returned!");
            }
            return 0;
        }

        public override void SetFloat(string property, float value)
        {
            if (colorObject != null)
            {
                colorObject.SetFloat(property, value);
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object! No value set");
            }
        }

        public void SetShaderFloat(string property, float value)
        {
            SetFloat(property, value);
        }

        public override float GetFloat(string property)
        {
            if (colorObject != null)
            {
                return colorObject.GetFloat(property);
            }
            else
            {
                Debug.Log("There are no Color supported components attached to this game object! No value returned!");
            }
            return 0;
        }

        public float GetShaderFloat(string property)
        {
            return GetFloat(property);
        }

        public void SetColorByMaterialName(Color color, string name)
        {
            MaterialsColorObject matColor = colorObject as MaterialsColorObject;
            if (matColor != null)
            {
                matColor.SetColorByMaterialName(color, name);
            }
            else
            {
                colorObject.SetColor(color);
            }
        }

        public Color GetColorByMaterialName(string name)
        {
            MaterialsColorObject matColor = colorObject as MaterialsColorObject;
            if (matColor != null)
            {
                return matColor.GetColorByMaterialName(name);
            }
            return colorObject.GetColor();
        }

        public void SetAlphaByMaterialName(float alpha, string name)
        {
            MaterialsColorObject matColor = colorObject as MaterialsColorObject;
            if (matColor != null)
            {
                matColor.SetAlphaByMaterialName(alpha, name);
            }
            else
            {
                colorObject.SetAlpha(alpha);
            }
        }

        public float GetAlphaByMaterialName(float alpha, string name)
        {
            MaterialsColorObject matColor = colorObject as MaterialsColorObject;
            if (matColor != null)
            {
                return matColor.GetAlphaByMaterialName(name);
            }
            else
            {
                return colorObject.GetAlpha();
            }
        }

        public Material[] GetMaterials()
        {
            MaterialsColorObject matColor = colorObject as MaterialsColorObject;
            if (matColor != null)
            {
                return matColor.GetMaterials();
            }
            return null;
        }

        public static ColorObject GetColorObject(GameObject gameObject)
        {
            TextColorObject textColor = new TextColorObject(gameObject);
            if (textColor.Valid)
            {
                return textColor;
            }

            TextMeshColorObject textMeshColor = new TextMeshColorObject(gameObject);
            if (textMeshColor.Valid)
            {
                return textMeshColor;
            }

            MaterialsColorObject materialsColor = new MaterialsColorObject(gameObject);
            if (materialsColor.Valid)
            {
                return materialsColor;
            }

            MaterialColorObject materialColor = new MaterialColorObject(gameObject);
            if (materialColor.Valid)
            {
                return materialColor;
            }

            return null;
        }

        public static TextColorObject GetTextColorObject(GameObject gameObject)
        {
            TextColorObject textColor = new TextColorObject(gameObject);
            if (textColor.Valid)
            {
                return textColor;
            }

            return null;
        }

        public static TextMeshColorObject GetTextMeshColorObject(GameObject gameObject)
        {
            TextMeshColorObject textMeshColor = new TextMeshColorObject(gameObject);
            if (textMeshColor.Valid)
            {
                return textMeshColor;
            }

            return null;
        }

        public static MaterialColorObject GetMaterialColorObject(GameObject gameObject, string shaderType = DefaultColor)
        {
            MaterialColorObject materialColor = new MaterialColorObject(gameObject);
            if (materialColor.Valid)
            {
                materialColor.SetShaderColorType(shaderType);
                return materialColor;
            }

            return null;
        }

        public static MaterialsColorObject GetMaterialsColorObject(GameObject gameObject, string shaderType = DefaultColor)
        {
            MaterialsColorObject materialsColor = new MaterialsColorObject(gameObject);
            if (materialsColor.Valid)
            {
                materialsColor.SetShaderColorType(shaderType);
                return materialsColor;
            }

            return null;
        }

        /// <summary>
        /// Creates a new MaterialPropertyBlock and copies over all the current values from the material
        /// </summary>
        /// <param name="host"></param>
        /// <param name="colorProperties"></param>
        /// <param name="floatProerties"></param>
        /// <returns></returns>
        public static MaterialPropertyBlock GetValidPropertyBlock(GameObject host, string[] colorProperties, string[] floatProerties)
        {
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();
            Renderer renderer = host.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.GetPropertyBlock(propertyBlock);

                for (int i = 0; i < colorProperties.Length; i++)
                {
                    Color color = GetMaterialColor(renderer, colorProperties[i]);
                    propertyBlock.SetColor(colorProperties[i], color);
                }

                for (int i = 0; i < floatProerties.Length; i++)
                {
                    float value = GetMaterialFloatValue(renderer, floatProerties[i]);
                    propertyBlock.SetFloat(floatProerties[i], value);
                }
            }

            host.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
            return propertyBlock;

        }

        /// <summary>
        /// Returns the current color on the material
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static Color GetMaterialColor(Renderer renderer, string property)
        {
            Color color = Color.blue;
            if (renderer != null)
            {
                Material material = GetValidMaterial(renderer);
                if (material != null)
                {
                    return material.GetVector(property);
                }
            }

            return color;
        }


        /// <summary>
        /// Returns the current float value on the material
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static float GetMaterialFloatValue(Renderer renderer, string property)
        {
            float value = -1;
            if (renderer != null)
            {
                Material material = GetValidMaterial(renderer);
                if(material != null)
                {
                    return material.GetFloat(property);
                }
            }

            return value;
        }

        public static Material GetValidMaterial(Renderer renderer)
        {
            Material material = null;

            if (renderer != null)
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    material = renderer.sharedMaterial;
                }
                else
                {
                    material = renderer.material;
                }
#else
                material = renderer.material;
#endif
            }
            return material;
        }
    }
}
