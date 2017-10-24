using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

// This is a set of custom property drawers + custom attributs as well as a base custom editor class
// They can be used to automatically generate inspectors with custom formatting and input validation
// The purpose of the custom editors is to help us clarify and simplify the functionality of complex & interdependent MRTK components
// The purpose of generating them in this manner is to ensure that all validation logic is visible in the target class and NOT in a separate editor class
// Everything is included in one file so that developers may easily include it with any individual scripts they wish to copy to their projects

namespace MRDL
{
    #region custom attributes

    // Base class for identifying members with special behavior
    public abstract class EditorAttribute : Attribute { }

    // Base class for custom drawing without property drawers - prevents the MRDLEditor from drawing a default property, supplies an alternative
    public abstract class DrawOverrideAttribute : EditorAttribute
    {
#if UNITY_EDITOR
        public abstract void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property);
        public abstract void DrawEditor(UnityEngine.Object target, PropertyInfo prop);
#endif

        protected string SplitCamelCase(string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;

            char[] a = str.ToCharArray();
            a[0] = char.ToUpper(a[0]);

            return Regex.Replace(
                Regex.Replace(
                    new string(a),
                    @"(\P{Ll})(\P{Ll}\p{Ll})",
                    "$1 $2"
                ),
                @"(\p{Ll})(\P{Ll})",
                "$1 $2"
            );
        }
    }

    // Base class for show / hide - shows or hides fields & properties in the editor based on the value of a member in the target object
    public abstract class ShowIfAttribute : EditorAttribute
    {
        public string MemberName { get; protected set; }
        public bool ShowIfConditionMet { get; protected set; }

#if UNITY_EDITOR
        public abstract bool ShouldShow(object target);

        protected static object GetMemberValue(object target, string memberName)
        {
            if (target == null)
                throw new NullReferenceException("Target cannot be null.");

            if (string.IsNullOrEmpty(memberName))
                throw new NullReferenceException("MemberName cannot be null.");

            Type targetType = target.GetType();

            MemberInfo[] members = targetType.GetMember(memberName);
            if (members.Length == 0)
                throw new MissingMemberException("Couldn't find member '" + memberName + "'");

            object memberValue;

            switch (members[0].MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo fieldInfo = targetType.GetField(memberName);
                    memberValue = fieldInfo.GetValue(target);
                    break;

                case MemberTypes.Property:
                    PropertyInfo propertyInfo = targetType.GetProperty(memberName);
                    memberValue = propertyInfo.GetValue(target, null);
                    break;

                default:
                    throw new MissingMemberException("Member '" + memberName + "' must be a field or property");
            }
            return memberValue;
        }

        protected static bool IsNullable(object target, string memberName)
        {
            if (target == null)
                throw new NullReferenceException("Target cannot be null.");

            if (string.IsNullOrEmpty(memberName))
                throw new NullReferenceException("MemberName cannot be null.");

            Type targetType = target.GetType();

            MemberInfo[] members = targetType.GetMember(memberName);
            if (members.Length == 0)
                throw new MissingMemberException("Couldn't find member '" + memberName + "'");

            Type memberType = members[0].DeclaringType;

            if (!memberType.IsValueType)
                return true;

            if (Nullable.GetUnderlyingType(memberType) != null)
                return true;

            return false;
        }
#endif
    }

    // Provides a clickable link to documentation in the inspector header
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DocLinkAttribute : Attribute
    {

        public string DocURL { get; private set; }
        public string Description { get; private set; }

        public DocLinkAttribute(string docURL, string description = null)
        {
            DocURL = docURL;
            Description = description;
        }
    }

    // Provides a clickable link to a tuturoial in the inspector header
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TutorialAttribute : Attribute
    {

        public string TutorialURL { get; private set; }
        public string Description { get; private set; }

        public TutorialAttribute(string tutorialURL, string description = null)
        {
            TutorialURL = tutorialURL;
            Description = description;
        }
    }

    // Indicates which components this class ought to be used with (though are not strictly required)
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UseWithAttribute : Attribute
    {
        public Type[] UseWithTypes { get; private set; }

        // IL2CPP doesn't support attributes with object arguments that are array types
        public UseWithAttribute(Type useWithType1, Type useWithType2 = null, Type useWithType3 = null, Type useWithType4 = null, Type useWithType5 = null)
        {
            List<Type> types = new List<Type>() { useWithType1 };

            if (useWithType2 != null)
                types.Add(useWithType2);

            if (useWithType3 != null)
                types.Add(useWithType3);

            if (useWithType4 != null)
                types.Add(useWithType4);

            if (useWithType5 != null)
                types.Add(useWithType5);

            UseWithTypes = types.ToArray();
        }
    }

    // Displays a text property as a textarea
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TextAreaProp : DrawOverrideAttribute
    {
        public int FontSize { get; private set; }

        public TextAreaProp(int fontSize = -1)
        {
            FontSize = fontSize;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            string propValue = (string)prop.GetValue(target, null);
            EditorGUILayout.LabelField(SplitCamelCase(prop.Name), EditorStyles.miniBoldLabel);
            GUIStyle textAreaStyle = EditorStyles.textArea;
            if (FontSize > 0)
            {
                textAreaStyle.fontSize = FontSize;
            }
            propValue = EditorGUILayout.TextArea(propValue, textAreaStyle);
            prop.SetValue(target, propValue, null);
        }
#endif

    }

    // Displays an int or float property as a range
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RangePropAttribute : DrawOverrideAttribute
    {

        public enum TypeEnum
        {
            Float,
            Int,
        }

        public float MinFloat { get; private set; }
        public float MaxFloat { get; private set; }
        public int MinInt { get; private set; }
        public int MaxInt { get; private set; }
        public TypeEnum Type { get; private set; }

        public RangePropAttribute(float min, float max)
        {
            MinFloat = min;
            MaxFloat = max;
            Type = TypeEnum.Float;
        }

        public RangePropAttribute(int min, int max)
        {
            MinInt = min;
            MaxInt = max;
            Type = TypeEnum.Int;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            // (safe since this is property-only attribute)
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            if (prop.PropertyType == typeof(int))
            {
                int propIntValue = (int)prop.GetValue(target, null);
                propIntValue = EditorGUILayout.IntSlider(SplitCamelCase(prop.Name), propIntValue, MinInt, MaxInt);
                prop.SetValue(target, propIntValue, null);
            }
            else if (prop.PropertyType == typeof(float))
            {
                float propFloatValue = (float)prop.GetValue(target, null);
                propFloatValue = EditorGUILayout.Slider(SplitCamelCase(prop.Name), propFloatValue, MinFloat, MaxFloat);
                prop.SetValue(target, propFloatValue, null);
            }
        }
#endif

    }

    // Displays a prop as editable in the inspector
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EditablePropAttribute : DrawOverrideAttribute
    {

        public string CustomLabel { get; private set; }

        public EditablePropAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            // (safe since this is property-only attribute)
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            switch (prop.PropertyType.Name)
            {
                case "Boolean":
                    bool boolValue = (bool)prop.GetValue(target, null);
                    boolValue = EditorGUILayout.Toggle(SplitCamelCase(prop.Name), boolValue);
                    prop.SetValue(target, boolValue, null);
                    break;

                default:
                    throw new NotImplementedException("No drawer for type " + prop.PropertyType.Name);
            }
        }
#endif

    }
    
    // Shows / hides based on bool value of named member
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ShowIfBoolValueAttribute : ShowIfAttribute
    {

        public ShowIfBoolValueAttribute(string boolMemberName, bool showIfConditionMet = true)
        {
            MemberName = boolMemberName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target)
        {
            bool conditionMet = (bool)GetMemberValue(target, MemberName);
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif
    }

    // Hides a field in an MRDL inspector
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideInMRDLInspector : ShowIfAttribute
    {
        public HideInMRDLInspector() { }

        public override bool ShouldShow(object target)
        {
            return false;
        }
    }

    // Shows / hides based on enum value of a named member
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfEnumValueAttribute : ShowIfAttribute
    {
        public int[] ShowValues { get; private set; }

        // IL2CPP doesn't support attributes with object arguments that are array types
        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue, bool showIfConditionMet = true)
        {
            if (!enumValue.GetType().IsEnum)
                throw new Exception("Value must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue1, object enumValue2, bool showIfConditionMet = true)
        {
            if (!enumValue1.GetType().IsEnum || !enumValue2.GetType().IsEnum)
                throw new Exception("Values must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue1), Convert.ToInt32(enumValue2) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue1, object enumValue2, object enumValue3, bool showIfConditionMet = true)
        {
            if (!enumValue1.GetType().IsEnum || !enumValue2.GetType().IsEnum || !enumValue3.GetType().IsEnum)
                throw new Exception("Values must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue1), Convert.ToInt32(enumValue2), Convert.ToInt32(enumValue3) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        public ShowIfEnumValueAttribute(string enumVariableName, object enumValue1, object enumValue2, object enumValue3, object enumValue4, bool showIfConditionMet = true)
        {
            if (!enumValue1.GetType().IsEnum || !enumValue2.GetType().IsEnum || !enumValue3.GetType().IsEnum || !enumValue4.GetType().IsEnum)
                throw new Exception("Values must be of type Enum");

            ShowValues = new int[] { Convert.ToInt32(enumValue1), Convert.ToInt32(enumValue2), Convert.ToInt32(enumValue3), Convert.ToInt32(enumValue4) };
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }

        /*public ShowIfEnumValueAttribute(string enumVariableName, object enumValues, bool showIfConditionMet = true) {
            if (enumValues.GetType().IsArray) {
                System.Array valuesArray = enumValues as System.Array;
                if (valuesArray == null || valuesArray.Length == 0)
                    throw new NullReferenceException("enumValues cannot be null or empty.");

                ShowValues = new int[valuesArray.Length];
                for (int i = 0; i < ShowValues.Length; i++) {
                    object enumValue = valuesArray.GetValue(i);
                    if (!enumValue.GetType().IsEnum)
                        throw new Exception("Values must be of type Enum");
                    ShowValues[i] = Convert.ToInt32(enumValue);
                }
            } else {
                if (!enumValues.GetType().IsEnum)
                    throw new Exception("Value must be of type Enum");

                ShowValues = new int[] { Convert.ToInt32(enumValues) };
            }
            MemberName = enumVariableName;
            ShowIfConditionMet = showIfConditionMet;
        }*/

#if UNITY_EDITOR
        public override bool ShouldShow(object target)
        {
            bool conditionMet = false;
            int memberValue = Convert.ToInt32(GetMemberValue(target, MemberName));
            for (int i = 0; i < ShowValues.Length; i++)
            {
                if (ShowValues[i] == memberValue)
                {
                    conditionMet = true;
                    break;
                }
            }
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif

        private static object GetAsUnderlyingType(Enum enval)
        {
            Type entype = enval.GetType();
            Type undertype = Enum.GetUnderlyingType(entype);
            return Convert.ChangeType(enval, undertype);
        }
    }

    // Shows / hides based on whether named member is null
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfNullAttribute : ShowIfAttribute
    {
        public ShowIfNullAttribute(string nullableMemberName, bool showIfConditionMet = false)
        {
            MemberName = nullableMemberName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target)
        {
            bool isNullable = true;
            if (target != null)
                isNullable = IsNullable(target, MemberName);

            if (!isNullable)
                throw new InvalidCastException("Member " + MemberName + " is not nullable.");

            UnityEngine.Object memberValue = (UnityEngine.Object)GetMemberValue(target, MemberName);
            bool conditionMet = memberValue == null;
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif
    }

    // Displays a drop-down menu of Component objects that are limited to the target object
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DropDownComponentAttribute : DrawOverrideAttribute
    {
        public bool AutoFill { get; private set; }
        public bool ShowComponentNames { get; private set; }
        public string CustomLabel { get; private set; }

        public DropDownComponentAttribute(bool showComponentNames = false, bool autoFill = false, string customLabel = null)
        {
            ShowComponentNames = showComponentNames;
            CustomLabel = customLabel;
            AutoFill = autoFill;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Transform transform = (target as Component).transform;

            Component componentValue = field.GetValue(target) as Component;

            Type targetType = field.FieldType;
            if (targetType == typeof(MonoBehaviour))
                targetType = typeof(Component);

            if (componentValue == null && AutoFill)
            {
                componentValue = transform.GetComponentInChildren(targetType) as Component;
            }

            componentValue = DropDownComponentField(
                SplitCamelCase(field.Name),
                componentValue,
                targetType,
                transform,
                ShowComponentNames);
            field.SetValue(target, componentValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Transform transform = (target as Component).transform;

            Component componentValue = prop.GetValue(target, null) as Component;

            Type targetType = prop.PropertyType;
            if (targetType == typeof(MonoBehaviour))
                targetType = typeof(Component);

            if (componentValue == null && AutoFill)
            {
                componentValue = transform.GetComponentInChildren(targetType) as Component;
            }

            componentValue = DropDownComponentField(
                SplitCamelCase(prop.Name),
                componentValue,
                targetType,
                transform,
                ShowComponentNames);
            prop.SetValue(target, componentValue, null);
        }

        private static Component DropDownComponentField(string label, Component obj, Type componentType, Transform transform, bool showComponentName = false)
        {
            Component[] optionObjects = transform.GetComponentsInChildren(componentType, true);
            int selectedIndex = 0;
            string[] options = new string[optionObjects.Length + 1];
            options[0] = "(None)";
            for (int i = 0; i < optionObjects.Length; i++)
            {
                if (showComponentName)
                {
                    options[i + 1] = optionObjects[i].GetType().Name + " (" + optionObjects[i].name + ")";
                }
                else
                {
                    options[i + 1] = optionObjects[i].name;
                }
                if (obj == optionObjects[i])
                {
                    selectedIndex = i + 1;
                }
            }

            EditorGUILayout.BeginHorizontal();
            int newIndex = EditorGUILayout.Popup(label, selectedIndex, options);
            if (newIndex == 0)
            {
                // Zero means '(None)'
                obj = null;
            }
            else
            {
                obj = optionObjects[newIndex - 1];
            }

            //draw the object field so people can click it
            obj = (Component)EditorGUILayout.ObjectField(obj, componentType, true);
            EditorGUILayout.EndHorizontal();

            return obj;
        }
#endif

    }

    // Displays a drop-down menu of GameObjects that are limited to the target object
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DropDownGameObjectAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public DropDownGameObjectAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Transform transform = (target as Component).transform;

            GameObject fieldValue = field.GetValue(target) as GameObject;
            fieldValue = DropDownGameObjectField(
                SplitCamelCase(field.Name),
                fieldValue,
                transform);
            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Transform transform = (target as Component).transform;

            GameObject propValue = prop.GetValue(target, null) as GameObject;
            propValue = DropDownGameObjectField(
                SplitCamelCase(prop.Name),
                propValue,
                transform);
            prop.SetValue(target, propValue, null);
        }

        private static GameObject DropDownGameObjectField(string label, GameObject obj, Transform transform)
        {
            Transform[] optionObjects = transform.GetComponentsInChildren<Transform>(true);
            int selectedIndex = 0;
            string[] options = new string[optionObjects.Length + 1];
            options[0] = "(None)";
            for (int i = 0; i < optionObjects.Length; i++)
            {
                options[i + 1] = optionObjects[i].name;
                if (obj == optionObjects[i].gameObject)
                {
                    selectedIndex = i + 1;
                }
            }

            EditorGUILayout.BeginHorizontal();
            int newIndex = EditorGUILayout.Popup(label, selectedIndex, options);
            if (newIndex == 0)
            {
                // Zero means '(None)'
                obj = null;
            }
            else
            {
                obj = optionObjects[newIndex - 1].gameObject;
            }

            //draw the object field so people can click it
            obj = (GameObject)EditorGUILayout.ObjectField(obj, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();

            return obj;
        }
#endif

    }

    // Displays a drop-down menu of Component objects that are limited to the scene (no prefabs)
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneComponentAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public SceneComponentAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Component fieldValue = field.GetValue(target) as Component;
            fieldValue = SceneObjectField(
                SplitCamelCase(field.Name),
                fieldValue,
                field.FieldType);
            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Component propValue = prop.GetValue(target, null) as Component;
            propValue = SceneObjectField(
                SplitCamelCase(prop.Name),
                propValue,
                prop.PropertyType);
            prop.SetValue(target, propValue, null);
        }

        public static Component SceneObjectField(string label, Component sceneObject, Type componentType)
        {

            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(label))
            {
                sceneObject = (Component)EditorGUILayout.ObjectField(sceneObject, componentType, true);
            }
            else
            {
                sceneObject = (Component)EditorGUILayout.ObjectField(label, sceneObject, componentType, true);
            }
            if (sceneObject != null && sceneObject.gameObject.scene.name == null)
            {
                // Don't allow objects that aren't in the scene!
                sceneObject = null;
            }

            UnityEngine.Object[] objectsInScene = GameObject.FindObjectsOfType(componentType);
            int selectedIndex = 0;
            string[] displayedOptions = new string[objectsInScene.Length + 1];
            displayedOptions[0] = "(None)";
            for (int i = 0; i < objectsInScene.Length; i++)
            {
                displayedOptions[i + 1] = objectsInScene[i].name;
                if (objectsInScene[i] == sceneObject)
                {
                    selectedIndex = i + 1;
                }
            }
            selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);
            if (selectedIndex == 0)
            {
                sceneObject = null;
            }
            else
            {
                sceneObject = (Component)objectsInScene[selectedIndex - 1];
            }
            EditorGUILayout.EndHorizontal();
            return sceneObject;
        }
#endif

    }

    // Displays a drop-down menu of GameObjects that are limited to the target object
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneGameObjectAttribute : DrawOverrideAttribute
    {
        public string CustomLabel { get; private set; }

        public SceneGameObjectAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            throw new NotImplementedException();
        }
#endif

    }

    // Adds a 'default' button to an animation curve that will supply default curve values
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AnimationCurveDefaultAttribute : DrawOverrideAttribute
    {
        public WrapMode PostWrap { get; private set; }
        public Keyframe StartVal { get; private set; }
        public Keyframe EndVal { get; private set; }

        public AnimationCurveDefaultAttribute(Keyframe startVal, Keyframe endVal, WrapMode postWrap = WrapMode.Loop)
        {
            PostWrap = postWrap;
            StartVal = startVal;
            EndVal = endVal;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            throw new NotImplementedException();
        }
#endif

    }

    // Adds a 'default' button to a color gradient that will supply default color values
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class GradientDefaultAttribute : DrawOverrideAttribute
    {
        // Used because you can't pass colors as attribute vars :/
        public enum ColorEnum
        {
            Black,
            Blue,
            Clear,
            Cyan,
            Gray,
            Green,
            Magenta,
            Red,
            White,
            Yellow
        }

        public GradientDefaultAttribute(ColorEnum startColor, ColorEnum endColor, float startAlpha = 1f, float endAlpha = 1f)
        {
            this.startColor = GetColor(startColor);
            this.endColor = GetColor(endColor);
            this.startColor.a = startAlpha;
            this.endColor.a = endAlpha;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Gradient gradientValue = field.GetValue(target) as Gradient;

            if (gradientValue == null || gradientValue.colorKeys == null || gradientValue.colorKeys.Length == 0)
                gradientValue = GetDefault();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.PropertyField(property);
            if (GUILayout.Button("Default"))
            {
                gradientValue = GetDefault();
            }
            EditorGUILayout.EndHorizontal();

            field.SetValue(target, gradientValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            throw new NotImplementedException();
        }
#endif

        private Gradient GetDefault()
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2] {
                    new GradientColorKey(startColor, 0f),
                    new GradientColorKey(endColor, 1f)
                };
            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2]
            {
                    new GradientAlphaKey(startColor.a, 0f),
                    new GradientAlphaKey(endColor.a, 0f),
            };
            gradient.SetKeys(colorKeys, alphaKeys);
            return gradient;
        }

        private Color startColor;
        private Color endColor;

        private static Color GetColor(ColorEnum color)
        {
            switch (color)
            {
                case ColorEnum.Black:
                    return Color.black;
                case ColorEnum.Blue:
                    return Color.blue;
                case ColorEnum.Clear:
                default:
                    return Color.clear;
                case ColorEnum.Cyan:
                    return Color.cyan;
                case ColorEnum.Gray:
                    return Color.gray;
                case ColorEnum.Green:
                    return Color.green;
                case ColorEnum.Magenta:
                    return Color.magenta;
                case ColorEnum.Red:
                    return Color.red;
                case ColorEnum.White:
                    return Color.white;
                case ColorEnum.Yellow:
                    return Color.yellow;
            }
        }
    }

    // Displays an enum value as a dropdown mask
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class EnumFlagsAttribute : DrawOverrideAttribute
    {
#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            int enumValue = Convert.ToInt32(field.GetValue(target));
            List<string> displayOptions = new List<string>();
            foreach (object value in Enum.GetValues(field.FieldType))
            {
                displayOptions.Add(value.ToString());
            }
            enumValue = EditorGUILayout.MaskField(SplitCamelCase(field.Name), enumValue, displayOptions.ToArray());
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            int enumValue = Convert.ToInt32(prop.GetValue(target, null));
            List<string> displayOptions = new List<string>();
            foreach (object value in Enum.GetValues(prop.PropertyType))
            {
                displayOptions.Add(value.ToString());
            }
            enumValue = EditorGUILayout.MaskField(SplitCamelCase(prop.Name), enumValue, displayOptions.ToArray());
        }
#endif
    }

    // Displays an enum value as a set of checkboxes
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class EnumCheckboxAttribute : DrawOverrideAttribute
    {
        public string DefaultName { get; private set; }
        public int DefaultValue { get; private set; }
        public int ValueOnZero { get; private set; }
        public bool IgnoreNone { get; private set; }
        public bool IgnoreAll { get; private set; }
        public string CustomLabel { get; private set; }

        public EnumCheckboxAttribute(string customLabel = null)
        {
            DefaultName = null;
            DefaultValue = 0;
            ValueOnZero = 0;
            IgnoreNone = true;
            IgnoreAll = true;
            CustomLabel = customLabel;
        }

        public EnumCheckboxAttribute(bool ignoreNone, bool ignoreAll, string customLabel = null)
        {
            DefaultName = null;
            DefaultValue = 0;
            ValueOnZero = 0;
            IgnoreNone = ignoreNone;
            IgnoreAll = ignoreAll;
            CustomLabel = customLabel;
        }

        public EnumCheckboxAttribute(string defaultName, object defaultValue, object valueOnZero = null, bool ignoreNone = true, bool ignoreAll = true, string customLabel = null)
        {
            DefaultName = defaultName;
            DefaultValue = Convert.ToInt32(defaultValue);
            ValueOnZero = Convert.ToInt32(valueOnZero);
            IgnoreNone = ignoreNone;
            IgnoreAll = ignoreAll;
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            int value = EnumCheckbox(
                (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(field.Name) : CustomLabel),
                field.GetValue(target),
                DefaultName,
                DefaultValue,
                ValueOnZero,
                IgnoreNone,
                IgnoreAll);

            field.SetValue(target, value);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            int value = EnumCheckbox(
                (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(prop.Name) : CustomLabel),
                prop.GetValue(target, null),
                DefaultName,
                DefaultValue,
                ValueOnZero,
                IgnoreNone,
                IgnoreAll);

            prop.SetValue(target, value, null);
        }

        private static int EnumCheckbox(string label, object enumObj, string defaultName, object defaultVal, object valOnZero, bool ignoreNone = true, bool ignoreAll = true)
        {
            if (!enumObj.GetType().IsEnum)
            {
                throw new ArgumentException("enumObj must be an enum.");
            }

            // Convert enum value to an int64 so we can treat it as a flag set
            int enumFlags = Convert.ToInt32(enumObj);

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(label, EditorStyles.miniLabel);

            GUIStyle styleHR = new GUIStyle(GUI.skin.box);
            styleHR.stretchWidth = true;
            styleHR.fixedHeight = 2;
            GUILayout.Box("", styleHR);

            System.Array enumVals = Enum.GetValues(enumObj.GetType());
            int lastvalue = Convert.ToInt32(enumVals.GetValue(enumVals.GetLength(0) - 1));

            foreach (object enumVal in enumVals)
            {
                int flagVal = Convert.ToInt32(enumVal);
                if (ignoreNone && flagVal == 0 && enumVal.ToString().ToLower() == "none")
                {
                    continue;
                }
                if (ignoreAll && flagVal == lastvalue && enumVal.ToString().ToLower() == "all")
                {
                    continue;
                }
                bool selected = (flagVal & enumFlags) != 0;
                selected = EditorGUILayout.Toggle(enumVal.ToString(), selected);
                // If it's selected add it to the enumObj, otherwise remove it
                if (selected)
                {
                    enumFlags |= flagVal;
                }
                else
                {
                    enumFlags &= ~flagVal;
                }
            }
            if (!string.IsNullOrEmpty(defaultName))
            {
                if (GUILayout.Button(defaultName, EditorStyles.miniButton))
                {
                    enumFlags = Convert.ToInt32(defaultVal);
                }
            }
            EditorGUILayout.EndVertical();

            if (enumFlags == 0)
            {
                enumFlags = Convert.ToInt32(valOnZero);
            }
            return enumFlags;
        }
#endif

    }

    // Displays a drop-down list of available material properties from the material supplied in a named member
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MaterialPropertyAttribute : DrawOverrideAttribute
    {
        public enum PropertyTypeEnum
        {
            Color,
            Float,
            Range,
            Vector,
        }

        public string Property { get; private set; }
        public PropertyTypeEnum PropertyType { get; private set; }
        public string MaterialMemberName { get; private set; }
        public bool AllowNone { get; private set; }
        public string DefaultProperty { get; private set; }
        public string CustomLabel { get; private set; }

        public MaterialPropertyAttribute(PropertyTypeEnum propertyType, string materialMemberName, bool allowNone = true, string defaultProperty = "_Color", string customLabel = null)
        {
            PropertyType = propertyType;
            MaterialMemberName = materialMemberName;
            AllowNone = allowNone;
            DefaultProperty = defaultProperty;
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            Material mat = GetMaterial(target);

            string fieldValue = MaterialPropertyName(
               (string)field.GetValue(target),
               mat,
               PropertyType,
               AllowNone,
               DefaultProperty,
               (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(field.Name) : CustomLabel));

            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop)
        {
            Material mat = GetMaterial(target);

            string propValue = MaterialPropertyName(
               (string)prop.GetValue(target, null),
               mat,
               PropertyType,
               AllowNone,
               DefaultProperty,
               (string.IsNullOrEmpty(CustomLabel) ? SplitCamelCase(prop.Name) : CustomLabel));

            prop.SetValue(target, propValue, null);
        }

        private Material GetMaterial(object target)
        {
            MemberInfo[] members = target.GetType().GetMember(MaterialMemberName);
            if (members.Length == 0)
            {
                Debug.LogError("Couldn't find material member " + MaterialMemberName);
                return null;
            }

            Material mat = null;

            switch (members[0].MemberType)
            {
                case MemberTypes.Field:
                    FieldInfo matField = target.GetType().GetField(MaterialMemberName);
                    mat = matField.GetValue(target) as Material;
                    break;

                case MemberTypes.Property:
                    PropertyInfo matProp = target.GetType().GetProperty(MaterialMemberName);
                    mat = matProp.GetValue(target, null) as Material;
                    break;

                default:
                    Debug.LogError("Couldn't find material member " + MaterialMemberName);
                    break;
            }

            return mat;
        }

        private static string MaterialPropertyName(string property, Material mat, PropertyTypeEnum type, bool allowNone, string defaultProperty, string labelName)
        {
            Color tColor = GUI.color;
            // Create a list of available color and value properties
            List<string> props = new List<string>();

            int selectedPropIndex = 0;

            if (allowNone)
            {
                props.Add("(None)");
            }

            if (mat != null)
            {
                int propertyCount = ShaderUtil.GetPropertyCount(mat.shader);
                string propName = string.Empty;
                for (int i = 0; i < propertyCount; i++)
                {
                    if (ShaderUtil.GetPropertyType(mat.shader, i).ToString() == type.ToString())
                    {
                        propName = ShaderUtil.GetPropertyName(mat.shader, i);
                        if (propName == property)
                        {
                            // We've found our current property
                            selectedPropIndex = props.Count;
                        }
                        props.Add(propName);
                    }
                }

                if (string.IsNullOrEmpty(labelName))
                {
                    labelName = type.ToString();
                }
                int newPropIndex = EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                if (allowNone)
                {
                    property = (newPropIndex > 0 ? props[newPropIndex] : string.Empty);
                }
                else
                {
                    if (props.Count > 0)
                    {
                        property = props[newPropIndex];
                    }
                    else
                    {
                        property = defaultProperty;
                    }
                }
                return property;
            }
            else
            {
                GUI.color = Color.Lerp(tColor, Color.gray, 0.5f);
                // Draw an empty property
                EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                GUI.color = tColor;
                return string.Empty;
            }
        }
#endif

    }

    // Validates object and displays an error or warning if validation fails
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ValidateUnityObjectAttribute : Attribute
    {
        public enum ActionEnum
        {
            Success,
            Warn,
            Error,
            HaltError,
        }

        public ActionEnum FailAction { get; private set; }
        public string MethodName { get; private set; }
        public string MessageOnFail { get; private set; }

        public ValidateUnityObjectAttribute(string methodName, string messageOnError, ActionEnum failAction = ActionEnum.Warn)
        {
            MethodName = methodName;
            MessageOnFail = messageOnError;
            FailAction = failAction;
        }

        public ActionEnum Validate(UnityEngine.Object target, System.Object source, out string messageOnFail)
        {
            if (source == null)
                throw new NullReferenceException("Source cannot be null.");

            MethodInfo m = source.GetType().GetMethod(MethodName);
            if (m == null)
                throw new MissingMethodException("Method " + MethodName + " not found in type " + source.GetType().ToString());

            bool result = (bool)m.Invoke(source, new System.Object[] { target });

            if (result)
            {
                messageOnFail = string.Empty;
                return ActionEnum.Success;
            }

            messageOnFail = MessageOnFail;
            return FailAction;
        }
    }

    // Sets the indent level for custom formatting
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SetIndentAttribute : Attribute
    {
        public int Indent { get; private set; }

        public SetIndentAttribute(int indent)
        {
            Indent = indent;
        }
    }

    // Class used to send members to bottom of drawing queue
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawLastAttribute : Attribute { }

    #endregion

    #region property drawers

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HeaderAttribute))]
    public class CustomHeaderDrawer : DecoratorDrawer
    {
        public override float GetHeight()
        {
            return MRDLEditor.ShowCustomEditors ? 0f : 24f;
        }

        public override void OnGUI(Rect position)
        {
            // If we're using MRDL custom editors, don't show the header
            if (MRDLEditor.ShowCustomEditors)
                return;

            // Otherwise draw it normally
            GUI.Label(position, (base.attribute as HeaderAttribute).header, EditorStyles.boldLabel);
        }
    }

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // If we're using MRDL custom edtiors, let the draw override property handle it
            if (MRDLEditor.ShowCustomEditors)
                return;

            // Otherwise draw a bitmask normally
            base.OnGUI(position, property, label);
        }
    }
#endif

    #endregion

    #region custom editor

#if UNITY_EDITOR
    /// <summary>
    /// To use this class in a Monobehavior or ScriptableObject, add this line at the bottom of your class:
    /// 
    /// public class ClassName {
    /// ...
    /// #if UNITY_EDITOR
    ///     [UnityEditor.CustomEditor(typeof(ClassName))]
    ///     public class CustomEditor : MRDLEditor { }
    /// #endif
    /// }
    /// 
    /// </summary>
    public class MRDLEditor : Editor
    {
        #region static vars
        // Toggles custom editors on / off
        public static bool ShowCustomEditors = true;
        public static GameObject lastTarget;

        // Styles
        private static GUIStyle toggleButtonOffStyle = null;
        private static GUIStyle toggleButtonOnStyle = null;
        private static GUIStyle sectionStyle = null;
        private static GUIStyle toolTipStyle = null;

        // Colors
        protected readonly static Color defaultColor = new Color(1f, 1f, 1f);
        protected readonly static Color disabledColor = new Color(0.6f, 0.6f, 0.6f);
        protected readonly static Color borderedColor = new Color(0.8f, 0.8f, 0.8f);
        protected readonly static Color warningColor = new Color(1f, 0.85f, 0.6f);
        protected readonly static Color errorColor = new Color(1f, 0.55f, 0.5f);
        protected readonly static Color successColor = new Color(0.8f, 1f, 0.75f);
        protected readonly static Color objectColor = new Color(0.85f, 0.9f, 1f);
        protected readonly static Color helpBoxColor = new Color(0.70f, 0.75f, 0.80f, 0.5f);
        protected readonly static Color sectionColor = new Color(0.85f, 0.9f, 1f);
        protected readonly static Color darkColor = new Color(0.1f, 0.1f, 0.1f);
        protected readonly static Color objectColorEmpty = new Color(0.75f, 0.8f, 0.9f);
        protected readonly static Color profileColor = new Color(0.88f, 0.7f, .97f);

        // Toggles visible tooltips
        private static bool showHelp = false;
        // Stores the show / hide values of displayed sections by target name + section name
        private static Dictionary<string, bool> displayedSections = new Dictionary<string, bool>();
        private static int indentOnSectionStart = 0;

        private static BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        #endregion

        public override void OnInspectorGUI()
        {
            CreateStyles();
            DrawInspectorHeader();
            Undo.RecordObject(target, target.name);

            if (ShowCustomEditors)
            {
                DrawCustomEditor();
                DrawCustomFooter();
            }
            else
            {
                base.DrawDefaultInspector();
            }

            SaveChanges();
        }

        /// <summary>
        /// Draws buttons for turning custom editors on/off, as well as DocType, Tutorial and UseWith attributes
        /// </summary>
        private void DrawInspectorHeader()
        {
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button(ShowCustomEditors ? "Toggle Custom Editors (ON)" : "Toggle Custom Editors (OFF)", ShowCustomEditors ? toggleButtonOnStyle : toggleButtonOffStyle))
            {
                ShowCustomEditors = !ShowCustomEditors;
            }
            if (ShowCustomEditors)
            {
                if (GUILayout.Button(showHelp ? "Toggle Help (ON)" : "Toggle Help (OFF)", showHelp ? toggleButtonOnStyle : toggleButtonOffStyle))
                {
                    showHelp = !showHelp;
                }
                if (GUILayout.Button("Expand Sections", toggleButtonOffStyle))
                {
                    Type targetType = target.GetType();
                    foreach (MemberInfo member in targetType.GetMembers(defaultBindingFlags))
                    {
                        if (member.IsDefined(typeof(HeaderAttribute), true))
                        {
                            HeaderAttribute h = member.GetCustomAttributes(typeof(HeaderAttribute), true)[0] as HeaderAttribute;
                            string lookupName = targetType.Name + h.header;
                            if (!displayedSections.ContainsKey(lookupName))
                                displayedSections.Add(lookupName, true);
                            else
                                displayedSections[lookupName] = true;
                        }
                    }
                }
                if (GUILayout.Button("Collapse Sections", toggleButtonOffStyle))
                {
                    Type targetType = target.GetType();
                    foreach (MemberInfo member in targetType.GetMembers(defaultBindingFlags))
                    {
                        if (member.IsDefined(typeof(HeaderAttribute), true))
                        {
                            HeaderAttribute h = member.GetCustomAttributes(typeof(HeaderAttribute), true)[0] as HeaderAttribute;
                            string lookupName = targetType.Name + h.header;
                            if (!displayedSections.ContainsKey(lookupName))
                                displayedSections.Add(lookupName, false);
                            else
                                displayedSections[lookupName] = false;
                        }
                    }
                }
            }
            GUILayout.EndHorizontal();

            if (ShowCustomEditors)
            {
                GUI.color = defaultColor;

                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                Type targetType = target.GetType();
                foreach (DocLinkAttribute attribute in targetType.GetCustomAttributes(typeof(DocLinkAttribute), true))
                {
                    string description = attribute.Description;
                    if (string.IsNullOrEmpty(description))
                        description = "Click for documentation about " + targetType.Name;

                    if (GUILayout.Button(description, EditorStyles.toolbarButton))
                        Application.OpenURL(attribute.DocURL);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                foreach (TutorialAttribute attribute in targetType.GetCustomAttributes(typeof(TutorialAttribute), true))
                {
                    string description = attribute.Description;
                    if (string.IsNullOrEmpty(description))
                        description = "Click for a tutorial on " + targetType.Name;

                    if (GUILayout.Button(description, EditorStyles.toolbarButton))
                        Application.OpenURL(attribute.TutorialURL);
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                List<Type> missingTypes = new List<Type>();
                foreach (UseWithAttribute attribute in targetType.GetCustomAttributes(typeof(UseWithAttribute), true))
                {
                    Component targetGo = (Component)target;
                    if (targetGo == null)
                        break;

                    foreach (Type type in attribute.UseWithTypes)
                    {
                        Component c = targetGo.GetComponent(type);
                        if (c == null)
                            missingTypes.Add(type);
                    }
                }
                if (missingTypes.Count > 0)
                {
                    string warningMessage = "This class is designed to be accompanied by scripts of (or inheriting from) types: \n";
                    for (int i = 0; i < missingTypes.Count; i++)
                    {
                        warningMessage += " - " + missingTypes[i].FullName;
                        if (i < missingTypes.Count - 1)
                            warningMessage += "\n";
                    }
                    warningMessage += "\nIt may not function correctly without them.";
                    DrawWarning(warningMessage);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            EditorGUILayout.Space();
        }

        /// <summary>
        /// Draws main editor
        /// </summary>
        protected void DrawCustomEditor()
        {
            EditorGUILayout.BeginVertical();

            Type targetType = target.GetType();
            // Get all the members of this type, public and private
            List<MemberInfo> members = new List<MemberInfo>(targetType.GetMembers(defaultBindingFlags));
            members.Sort(
                delegate (MemberInfo m1, MemberInfo m2)
                {
                    if (m1.IsDefined(typeof(DrawLastAttribute), true))
                    {
                        return 1;
                    }
                    return 0;
                }
            );

            // Start drawing the editor
            int currentIndentLevel = 0;
            bool insideSectionBlock = false;
            bool drawCurrentSection = true;

            foreach (MemberInfo member in members)
            {
                try
                {
                    // First get header and indent settings
                    if (member.IsDefined(typeof(HeaderAttribute), true))
                    {
                        HeaderAttribute header = member.GetCustomAttributes(typeof(HeaderAttribute), true)[0] as HeaderAttribute;
                        if (insideSectionBlock)
                            DrawSectionEnd();

                        insideSectionBlock = true;
                        drawCurrentSection = DrawSectionStart(target.GetType().Name, header.header);
                    }

                    // Then do basic show / hide based on ShowIfAttribute
                    if ((insideSectionBlock && !drawCurrentSection) || !ShouldDrawMember(member, targetType, target))
                        continue;

                    // Handle drawing stuff (indent, help)
                    if (showHelp)
                        DrawToolTip(member);

                    if (member.IsDefined(typeof(SetIndentAttribute), true))
                    {
                        SetIndentAttribute indent = member.GetCustomAttributes(typeof(SetIndentAttribute), true)[0] as SetIndentAttribute;
                        currentIndentLevel += indent.Indent;
                        EditorGUI.indentLevel = currentIndentLevel;
                    }

                    // Now get down to drawing the thing
                    // Get an array ready for our override attributes
                    object[] drawOverrideAttributes = null;
                    switch (member.MemberType)
                    {
                        case MemberTypes.Field:
                            FieldInfo field = targetType.GetField(member.Name, defaultBindingFlags);
                            if (!field.IsPrivate || field.IsDefined(typeof(SerializeField), true))
                            {
                                // If it's a profile field, take care of that first
                                if (IsSubclassOf(field.FieldType, typeof(ProfileBase)))
                                {
                                    UnityEngine.Object profile = (UnityEngine.Object)field.GetValue(target);
                                    profile = DrawProfileField(target, profile, field.FieldType);
                                    field.SetValue(target, profile);
                                }
                                else
                                {
                                    drawOverrideAttributes = field.GetCustomAttributes(typeof(DrawOverrideAttribute), true);
                                    // If we fine overrides, draw using those
                                    if (drawOverrideAttributes.Length > 0)
                                    {
                                        if (drawOverrideAttributes.Length > 1)
                                            DrawWarning("You should only use one DrawOverride attribute per member. Drawing " + drawOverrideAttributes[0].GetType().Name + " only.");

                                        (drawOverrideAttributes[0] as DrawOverrideAttribute).DrawEditor(target, field, serializedObject.FindProperty(field.Name));
                                    }
                                    else
                                    {
                                        // Otherwise just draw the default editor
                                        DrawSerializedField(serializedObject, field.Name);
                                    }
                                }
                            }
                            break;

                        case MemberTypes.Property:
                            // We have to draw properties manually
                            PropertyInfo prop = targetType.GetProperty(member.Name, defaultBindingFlags);
                            drawOverrideAttributes = prop.GetCustomAttributes(typeof(DrawOverrideAttribute), true);
                            // If it's a profile field, take care of that first
                            if (IsSubclassOf(prop.PropertyType, typeof(ProfileBase)))
                            {
                                UnityEngine.Object profile = (UnityEngine.Object)prop.GetValue(target, null);
                                profile = DrawProfileField(target, profile, prop.PropertyType);
                                prop.SetValue(target, profile, null);
                            }
                            // If we find overrides, draw using those
                            else if (drawOverrideAttributes.Length > 0)
                            {
                                if (drawOverrideAttributes.Length > 1)
                                    DrawWarning("You should only use one DrawOverride attribute per member. Drawing " + drawOverrideAttributes[0].GetType().Name + " only.");

                                (drawOverrideAttributes[0] as DrawOverrideAttribute).DrawEditor(target, prop);
                            }
                            break;

                        default:
                            // Don't do anything, it's not something we can use
                            break;
                    }
                }
                catch (Exception e)
                {
                    DrawWarning("There was a problem drawing the member " + member.Name + ":");
                    DrawError(System.Environment.NewLine + e.ToString());
                }
            }

            if (insideSectionBlock)
                DrawSectionEnd();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// override this if you want to draw a footer at the bottom of your editor
        /// Typically used for validation and error / warning messages that are too complex for Validate attributes
        /// </summary>
        protected virtual void DrawCustomFooter()
        {
            //...
        }

        /// <summary>
        /// Ensures changes are saved once editor is finished
        /// </summary>
        protected void SaveChanges()
        {
            if (serializedObject.ApplyModifiedProperties())
            {
                EditorUtility.SetDirty(target);
            }
        }

        #region drawing

        /// <summary>
        /// Determines whether this member should be shown in the editor
        /// </summary>
        /// <param name="member"></param>
        /// <param name="targetType"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool ShouldDrawMember(MemberInfo member, Type targetType, object target)
        {
            object[] hideAttributes = member.GetCustomAttributes(typeof(HideInInspector), true);
            if (hideAttributes != null && hideAttributes.Length > 0)
                return false;

            bool shouldBeVisible = true;

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    // Fields are visible by default unless they're hidden by a ShowIfAttribute
                    foreach (ShowIfAttribute attribute in member.GetCustomAttributes(typeof(ShowIfAttribute), true))
                    {
                        if (!attribute.ShouldShow(target))
                        {
                            shouldBeVisible = false;
                            break;
                        }
                    }
                    break;

                case MemberTypes.Property:
                    // Property types require at least one EditorAttribute to be visible
                    if (member.GetCustomAttributes(typeof(EditorAttribute), true).Length == 0)
                    {
                        shouldBeVisible = false;
                    }
                    else
                    {
                        // Even if they have an editor attribute, they can still be hidden by a ShowIfAttribute
                        foreach (ShowIfAttribute attribute in member.GetCustomAttributes(typeof(ShowIfAttribute), true))
                        {
                            if (!attribute.ShouldShow(target))
                            {
                                shouldBeVisible = false;
                                break;
                            }
                        }
                    }
                    break;

                default:
                    break;
            }
            return shouldBeVisible;
        }

        /// <summary>
        /// Draws default unity serialized field
        /// </summary>
        /// <param name="serializedObject"></param>
        /// <param name="propertyPath"></param>
        protected void DrawSerializedField(SerializedObject serializedObject, string propertyPath)
        {
            SerializedProperty prop = serializedObject.FindProperty(propertyPath);
            if (prop != null)
                EditorGUILayout.PropertyField(prop, true);
        }

        /// <summary>
        /// Draws a section start (initiated by the Header attribute)
        /// </summary>
        /// <param name="targetName"></param>
        /// <param name="headerName"></param>
        /// <param name="toUpper"></param>
        /// <param name="drawInitially"></param>
        /// <returns></returns>
        public static bool DrawSectionStart(string targetName, string headerName, bool toUpper = true, bool drawInitially = true)
        {
            string lookupName = targetName + headerName;
            if (!displayedSections.ContainsKey(lookupName))
                displayedSections.Add(lookupName, drawInitially);

            bool drawSection = displayedSections[lookupName];
            EditorGUILayout.Space();
            Color tColor = GUI.color;
            GUI.color = sectionColor;

            if (toUpper)
                headerName = headerName.ToUpper();

            drawSection = EditorGUILayout.Foldout(drawSection, headerName, true, sectionStyle);
            displayedSections[lookupName] = drawSection;
            EditorGUILayout.BeginVertical();
            GUI.color = tColor;

            indentOnSectionStart = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;// indentOnSectionStart + 1;

            return drawSection;
        }

        /// <summary>
        /// Draws section end (initiated by next Header attribute)
        /// </summary>
        public static void DrawSectionEnd()
        {
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = indentOnSectionStart;
        }

        /// <summary>
        /// Draws a tooltip as text in the editor
        /// </summary>
        /// <param name="member"></param>
        public static void DrawToolTip(MemberInfo member)
        {
            if (member.IsDefined(typeof(TooltipAttribute), true))
            {
                TooltipAttribute tooltip = member.GetCustomAttributes(typeof(TooltipAttribute), true)[0] as TooltipAttribute;
                Color prevColor = GUI.color;
                GUI.color = helpBoxColor;
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField(tooltip.tooltip, toolTipStyle);
                EditorGUI.indentLevel++;
                GUI.color = prevColor;
            }
        }

        public static void DrawWarning(string warning)
        {
            Color prevColor = GUI.color;

            GUI.color = warningColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(warning, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        public static void DrawError(string error)
        {
            Color prevColor = GUI.color;

            GUI.color = errorColor;
            EditorGUILayout.BeginVertical(EditorStyles.textArea);
            EditorGUILayout.LabelField(error, EditorStyles.wordWrappedMiniLabel);
            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
        }

        public static void DrawDivider()
        {
            GUIStyle styleHR = new GUIStyle(GUI.skin.box);
            styleHR.stretchWidth = true;
            styleHR.fixedHeight = 2;
            GUILayout.Box("", styleHR);
        }

        private void CreateStyles()
        {
            if (toggleButtonOffStyle == null)
            {
                toggleButtonOffStyle = "ToolbarButton";
                toggleButtonOffStyle.fontSize = 9;
                toggleButtonOnStyle = new GUIStyle(toggleButtonOffStyle);
                toggleButtonOnStyle.normal.background = toggleButtonOnStyle.active.background;


                sectionStyle = new GUIStyle(EditorStyles.foldout);
                sectionStyle.fontStyle = FontStyle.Bold;

                toolTipStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel);
                toolTipStyle.fontStyle = FontStyle.Normal;
                toolTipStyle.alignment = TextAnchor.LowerLeft;
            }
        }

        #endregion

        #region profiles

        /// <summary>
        /// Draws a field for scriptable object profiles
        /// Profiles are scriptable objects that contain shared information
        /// If base class is abstract, includes a button for creating a profile of each type that inherits from base class T
        /// Otherwise just includes button for creating a profile of type
        /// Also finds and draws the inspector for the profile
        /// </summary>
        /// <param name="target"></param>
        /// <param name="profile"></param>
        /// <param name="profileType"></param>
        /// <returns></returns>
        private static UnityEngine.Object DrawProfileField(UnityEngine.Object target, UnityEngine.Object profile, Type profileType)
        {
            Color prevColor = GUI.color;
            GUI.color = profileColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.Lerp(Color.white, Color.gray, 0.25f);
            EditorGUILayout.LabelField("Select a " + profileType.Name + " or create a new profile", EditorStyles.miniBoldLabel);
            UnityEngine.Object newProfile = profile;
            EditorGUILayout.BeginHorizontal();
            newProfile = EditorGUILayout.ObjectField(profile, profileType, false);
            // is this an abstract class? 
            if (profileType.IsAbstract)
            {
                EditorGUILayout.BeginVertical();
                List<Type> types = GetDerivedTypes(profileType, Assembly.GetAssembly(profileType));

                EditorGUILayout.BeginHorizontal();
                foreach (Type derivedType in types)
                {
                    if (GUILayout.Button("Create " + derivedType.Name))
                    {
                        profile = CreateProfile(derivedType);
                    }
                }
                if (GUILayout.Button("What's a profile?"))
                {
                    LaunchProfileHelp();
                }
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Create Profile"))
                {
                    profile = CreateProfile(profileType);
                }
                if (GUILayout.Button("What's a profile?"))
                {
                    LaunchProfileHelp();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndHorizontal();

            if (profile == null)
            {
                DrawError("You must choose a profile.");
            }
            else
            {
                EditorGUI.indentLevel++;
                // Draw the editor for this profile
                // Set it to false initially
                if (DrawSectionStart(target.GetType().Name, profile.name + " (Click to edit)", false, false))
                {
                    // Draw the profile inspector
                    Editor inspector = Editor.CreateEditor(profile);
                    ProfileInspector profileInspector = (ProfileInspector)inspector;
                    if (profileInspector != null)
                    {
                        profileInspector.targetComponent = target as Component;
                    }
                    inspector.OnInspectorGUI();
                }

                DrawSectionEnd();
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndVertical();

            GUI.color = prevColor;
            return newProfile;
        }

        /// <summary>
        /// Displays a help window explaning profile objects
        /// </summary>
        private static void LaunchProfileHelp()
        {
            EditorUtility.DisplayDialog(
                        "Profiles Help",
                        "Profiles are assets that contain a set of common settings like colors or sound files."
                        + "\n\nThose settings can be shared and used by any objects that keep a reference to the profile."
                        + "\n\nThey make changing the style of a set of objects quicker and easier, and they reduce memory usage."
                        + "\n\nA purple icon indicates that you're looking at a profile asset."
                        + "\n\nFor more information please see the documentation at:"
                        + "\n https://github.com/Microsoft/MRDesignLabs_Unity/"
                        , "OK");
        }

        /// <summary>
        /// Creates a new instance of the profile object
        /// </summary>
        /// <param name="profileType"></param>
        /// <returns></returns>
        private static UnityEngine.Object CreateProfile(Type profileType)
        {
            UnityEngine.Object asset = ScriptableObject.CreateInstance(profileType);
            if (asset != null)
            {
                AssetDatabase.CreateAsset(asset, "Assets/New" + profileType.Name + ".asset");
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError("Couldn't create profile of type " + profileType.Name);
            }
            return asset;
        }

        private static List<Type> GetDerivedTypes(Type baseType, Assembly assembly)
        {
            Type[] types = assembly.GetTypes();
            List<Type> derivedTypes = new List<Type>();

            for (int i = 0, count = types.Length; i < count; i++)
            {
                Type type = types[i];
                if (IsSubclassOf(type, baseType))
                {
                    derivedTypes.Add(type);
                }
            }

            return derivedTypes;
        }

        private static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null || type == baseType)
                return false;

            if (baseType.IsGenericType == false)
            {
                if (type.IsGenericType == false)
                    return type.IsSubclassOf(baseType);
            }
            else
            {
                baseType = baseType.GetGenericTypeDefinition();
            }

            type = type.BaseType;
            Type objectType = typeof(object);

            while (type != objectType && type != null)
            {
                Type curentType = type.IsGenericType ?
                    type.GetGenericTypeDefinition() : type;
                if (curentType == baseType)
                    return true;

                type = type.BaseType;
            }

            return false;
        }

        #endregion
    }

    /// <summary>
    /// Base class for profile inspectors
    /// Profiles are scriptable objects that contain shared information
    /// To ensure that developers understand that they're editing 'global' data, 
    /// this inspector automatically displays a CONSISTENT warning message and 'profile' color to the controls
    /// It also provides a 'target component' so inspectors can differentiate between local / global editing
    /// See compound button component inspectors for usage examples
    /// </summary>
    public abstract class ProfileInspector : MRDLEditor
    {
        public Component targetComponent;

        public override void OnInspectorGUI()
        {
            Undo.RecordObject(target, target.name);
            BeginProfileInspector();
            DrawCustomEditor();
            DrawCustomFooter();
            EndProfileInspector();
            SaveChanges();
        }

        private void BeginProfileInspector()
        {
            GUI.color = profileColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.Lerp(profileColor, Color.red, 0.5f);
            EditorGUILayout.LabelField("(Warning: this section edits the button profile. These changes will affect all objects that use this profile.)", EditorStyles.wordWrappedMiniLabel);
            GUI.color = defaultColor;
        }

        private void EndProfileInspector()
        {
            EditorGUILayout.EndVertical();
        }
    }
#endif

    #endregion
}