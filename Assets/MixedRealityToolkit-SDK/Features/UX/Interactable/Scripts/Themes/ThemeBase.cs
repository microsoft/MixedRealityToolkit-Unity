using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.Toolkit.SDK.UX
{
    public enum ThemePropertyValueTypes { Float, Int, Color, ShaderFloat, shaderRange, Vector2, Vector3, Vector4, Quaternion, Texture, Material, AudioClip, Animaiton, GameObject, String, Bool }

    [System.Serializable]
    public class ThemePropertyValue
    {
        public string Name;
        public string String;
        public bool Bool;
        public int Int;
        public float Float;
        public Texture Texture;
        public Material Material;
        public GameObject GameObject;
        public Vector2 Vector2;
        public Vector3 Vector3;
        public Vector4 Vector4;
        public Color Color;
        public Quaternion Quaternion;
        public AudioClip AudioClip;
        public Animation Animation;
    }

    public enum ShaderPropertyType { Color, Float, Range, TexEnv, Vector, None }

    [System.Serializable]
    public struct ShaderProperties
    {
        public string Name;
        public ShaderPropertyType Type;
        public Vector2 Range;
    }

    public struct ShaderInfo
    {
        public ShaderProperties[] ShaderOptions;
        public string Name;
    }

    [System.Serializable]
    public class ThemeProperty
    {
        public string Name;
        public ThemePropertyValueTypes Type;
        public List<ThemePropertyValue> Values;
        public ThemePropertyValue StartValue;
        public int PropId;
        public List<ShaderProperties> ShaderOptions;
        public List<string> ShaderOptionNames;
        public ThemePropertyValue Default;
        public string ShaderName;

        public string GetShaderPropId()
        {
            if (ShaderOptionNames.Count > PropId)
            {
                return ShaderOptionNames[PropId];
            }
            
            return "_Color";
        }
    }

    [System.Serializable]
    public class EaseSettings
    {
        public enum BasicEaseCurves { Linear, EaseIn, EaseOut, EaseInOut }
        public bool EaseValues = false;
        public AnimationCurve Curve = AnimationCurve.Linear(0, 1, 1, 1);
        public float LerpTime = 0.5f;
        private float timer = 0.5f;

        public EaseSettings()
        {
            Stop();
        }

        public void OnUpdate()
        {
            if (timer < LerpTime)
            {
                timer = Mathf.Min(timer + Time.deltaTime, LerpTime);
            }
        }

        public void Start()
        {
            timer = 0;
            if (!EaseValues)
            {
                timer = LerpTime;
            }
        }

        public bool IsPlaying()
        {
            return timer < LerpTime;
        }

        public void Stop()
        {
            timer = LerpTime;
        }

        public float GetLinear()
        {
            return timer / LerpTime;
        }

        public float GetCurved()
        {
            return IsLinear() ? GetLinear() : Curve.Evaluate(GetLinear());
        }

        protected bool IsLinear()
        {
            if (Curve.keys.Length > 1)
            {
                return (Curve.keys[0].value == 1 && Curve.keys[1].value == 1);
            }

            return false;
        }

        public void SetCurve(BasicEaseCurves curve)
        {
            AnimationCurve animation = AnimationCurve.Linear(0, 1, 1, 1);
            switch (curve)
            {
                case BasicEaseCurves.EaseIn:
                    animation = new AnimationCurve(new Keyframe(0, 0), new Keyframe(1, 1, 2.5f, 0));
                    break;
                case BasicEaseCurves.EaseOut:
                    animation = new AnimationCurve(new Keyframe(0, 0, 0, 2.5f), new Keyframe(1, 1));
                    break;
                case BasicEaseCurves.EaseInOut:
                    animation = AnimationCurve.EaseInOut(0, 0, 1, 1);
                    break;
                default:
                    break;
            }

            Curve = animation;
        }
    }
    
    public abstract class ThemeBase
    {
        public Type[] Types;
        public string Name = "Base Theme";
        public List<ThemeProperty> ThemeProperties = new List<ThemeProperty>();
        public List<ThemePropertyValue> CustomSettings = new List<ThemePropertyValue>();
        public GameObject Host;
        public EaseSettings Ease;
        public bool Loaded;
        private bool hasFirstState = false;

        private int lastState = -1;

        //! find a way to set the default values of the properties, like scale should be Vector3.one
        // these should be custom, per theme

        public abstract void SetValue(ThemeProperty property, int index, float percentage);

        public abstract ThemePropertyValue GetProperty(ThemeProperty property);

        public virtual void Init(GameObject host, ThemePropertySettings settings)
        {
            Host = host;

            for (int i = 0; i < settings.Properties.Count; i++)
            {
                ThemeProperty prop = ThemeProperties[i];
                prop.ShaderOptionNames = settings.Properties[i].ShaderOptionNames;
                prop.ShaderOptions = settings.Properties[i].ShaderOptions;
                prop.PropId = settings.Properties[i].PropId;
                prop.Values = settings.Properties[i].Values;
                
                ThemeProperties[i] = prop;
            }

            Ease = CopyEase(settings.Easing);
            Ease.Stop();

            Loaded = true;

        }

        protected float LerpFloat(float s, float e, float t)
        {
            return (e - s) * t + s;
        }

        protected int LerpInt(int s, int e, float t)
        {
            return Mathf.RoundToInt((e - s) * t) + s;
        }

        protected EaseSettings CopyEase(EaseSettings ease)
        {
            EaseSettings newEase = new EaseSettings();
            newEase.Curve = ease.Curve;
            newEase.EaseValues = ease.EaseValues;
            newEase.LerpTime = ease.LerpTime;

            return newEase;
        }

        public virtual void OnUpdate(int state, bool force = false)
        {
            if(state != lastState || force)
            {
                for (int i = 0; i < ThemeProperties.Count; i++)
                {
                    ThemeProperty current = ThemeProperties[i];
                    current.StartValue = GetProperty(current);
                    if (hasFirstState)
                    {
                        Ease.Start();
                        SetValue(current, state, Ease.GetCurved());
                    }
                    else
                    {
                        SetValue(current, state, 1);
                        if(i >= ThemeProperties.Count - 1)
                        {
                            hasFirstState = true;
                        }
                    }
                    ThemeProperties[i] = current;
                }

                lastState = state;
            }
            else if(Ease.EaseValues && Ease.IsPlaying())
            {
                Ease.OnUpdate();
                for (int i = 0; i < ThemeProperties.Count; i++)
                {
                    ThemeProperty current = ThemeProperties[i];
                    SetValue(current, state, Ease.GetCurved());
                }
            }

            lastState = state;
        }

        public static MaterialPropertyBlock GetMaterialPropertyBlock(GameObject gameObject, ShaderProperties[] props)
        {
            MaterialPropertyBlock materialBlock = GetPropertyBlock(gameObject);
            Renderer renderer = gameObject.GetComponent<Renderer>();

            float value;
            if (renderer != null)
            {
                Material material = GetValidMaterial(renderer);
                if (material != null)
                { 
                    for (int i = 0; i < props.Length; i++)
                    {
                        ShaderProperties prop = props[i];
                        switch (props[i].Type)
                        {
                            case ShaderPropertyType.Color:
                                Color color = material.GetVector(prop.Name);
                                materialBlock.SetColor(prop.Name, color);
                                break;
                            case ShaderPropertyType.Float:
                                value = material.GetFloat(prop.Name);
                                materialBlock.SetFloat(prop.Name, value);
                                break;
                            case ShaderPropertyType.Range:
                                value = material.GetFloat(prop.Name);
                                materialBlock.SetFloat(prop.Name, value);
                                break;
                            default:
                                break;
                        }
                    }
                }
                gameObject.GetComponent<Renderer>().SetPropertyBlock(materialBlock);
            }

            return materialBlock;
        }

        public static MaterialPropertyBlock GetPropertyBlock(GameObject gameObject)
        {
            MaterialPropertyBlock materialBlock = new MaterialPropertyBlock();
            Renderer renderer = gameObject.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.GetPropertyBlock(materialBlock);
            }
            return materialBlock;
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
