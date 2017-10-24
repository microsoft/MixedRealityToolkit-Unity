using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace MRDL
{
    #region enums

    public enum ValidateResultEnum {
        Succeed,
        Fail,
    }

    public enum ValidateFailActionEnum {
        None,
        Warning,
        HaltEditor,
    }

    #endregion

    #region property drawers

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(HeaderAttribute))]
    public class CustomHeaderDrawer : DecoratorDrawer {
        public override float GetHeight() {
            return MRDLEditor.ShowCustomEditors ? 0f : 24f;
        }

        public override void OnGUI(Rect position) {
            // If we're using MRDL custom editors, don't show the header
            if (MRDLEditor.ShowCustomEditors)
                return;

            // Otherwise draw it normally
            GUI.Label(position, (base.attribute as HeaderAttribute).header, EditorStyles.boldLabel);
        }
    }

    [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
    public class EnumFlagsPropertyDrawer : PropertyDrawer {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            // If we're using MRDL custom edtiors, let the draw override property handle it
            if (MRDLEditor.ShowCustomEditors)
                return;

            // Otherwise draw a bitmask normally
            base.OnGUI(position, property, label);
        }
    }
#endif

    #endregion

    #region custom attributes

    // Base class for identifying members with special behavior
    public abstract class EditorAttribute : Attribute { }

    // DrawOverrideAttributes prevent the MRDLEditor from drawing a default property
    public abstract class DrawOverrideAttribute : EditorAttribute {
#if UNITY_EDITOR
        public abstract void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property);
        public abstract void DrawEditor(UnityEngine.Object target, PropertyInfo prop);
#endif

        protected string SplitCamelCase(string str) {
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

    // Class used to send members to bottom of drawing queue
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class DrawLastAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class DocLinkAttribute : Attribute {
        public DocLinkAttribute(string docURL, string description = null) {
            DocURL = docURL;
            Description = description;
        }

        public string DocURL { get; private set; }
        public string Description { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class TutorialAttribute : Attribute {
        public TutorialAttribute(string tutorialURL, string description = null) {
            TutorialURL = tutorialURL;
            Description = description;
        }

        public string TutorialURL { get; private set; }
        public string Description { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class UseWithAttribute : Attribute {
        // IL2CPP doesn't support attributes with object arguments that are array types
        public UseWithAttribute(Type useWithType1, Type useWithType2 = null, Type useWithType3 = null, Type useWithType4 = null, Type useWithType5 = null) {
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

        public Type[] UseWithTypes { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TextAreaProp : DrawOverrideAttribute
    {
        public TextAreaProp (int fontSize = -1) {
            FontSize = fontSize;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
            string propValue = (string)prop.GetValue(target, null);
            EditorGUILayout.LabelField(SplitCamelCase(prop.Name), EditorStyles.miniBoldLabel);
            GUIStyle textAreaStyle = EditorStyles.textArea;
            if (FontSize > 0) {
                textAreaStyle.fontSize = FontSize;
            }
            propValue = EditorGUILayout.TextArea(propValue, textAreaStyle);
            prop.SetValue(target, propValue, null);
        }
#endif
        
        public int FontSize { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class RangePropAttribute : DrawOverrideAttribute {
        public enum TypeEnum {
            Float,
            Int,
        }

        public RangePropAttribute(float min, float max) {
            MinFloat = min;
            MaxFloat = max;
            Type = TypeEnum.Float;
        }

        public RangePropAttribute(int min, int max) {
            MinInt = min;
            MaxInt = max;
            Type = TypeEnum.Int;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            // (safe since this is property-only attribute)
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
            if (prop.PropertyType == typeof(int)) {
                int propIntValue = (int)prop.GetValue(target, null);
                propIntValue = EditorGUILayout.IntSlider(SplitCamelCase(prop.Name), propIntValue, MinInt, MaxInt);
                prop.SetValue(target, propIntValue, null);
            } else if (prop.PropertyType == typeof(float)) {
                float propFloatValue = (float)prop.GetValue(target, null);
                propFloatValue = EditorGUILayout.Slider(SplitCamelCase(prop.Name), propFloatValue, MinFloat, MaxFloat);
                prop.SetValue(target, propFloatValue, null);
            }
        }
#endif

        public float MinFloat { get; private set; }
        public float MaxFloat { get; private set; }
        public int MinInt { get; private set; }
        public int MaxInt { get; private set; }
        public TypeEnum Type { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class EditablePropAttribute : DrawOverrideAttribute {
        public EditablePropAttribute(string customLabel = null) {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            // (safe since this is property-only attribute)
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
            switch (prop.PropertyType.Name) {
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

        public string CustomLabel { get; private set; }
    }

    /// <summary>
    /// Shows or hides fields & properties in the editor based on the value of a member in the target object
    /// </summary>
    public abstract class ShowIfAttribute : EditorAttribute {
#if UNITY_EDITOR
        public abstract bool ShouldShow(object target);
#endif
        public string MemberName { get; protected set; }
        public bool ShowIfConditionMet { get; protected set; }

#if UNITY_EDITOR
        protected static object GetMemberValue(object target, string memberName) {
            if (target == null)
                throw new NullReferenceException("Target cannot be null.");

            if (string.IsNullOrEmpty(memberName))
                throw new NullReferenceException("MemberName cannot be null.");

            Type targetType = target.GetType();

            MemberInfo[] members = targetType.GetMember(memberName);
            if (members.Length == 0)
                throw new MissingMemberException("Couldn't find member '" + memberName + "'");

            object memberValue;

            switch (members[0].MemberType) {
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

        protected static bool IsNullable(object target, string memberName) {
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public sealed class ShowIfBoolValueAttribute : ShowIfAttribute {
        public ShowIfBoolValueAttribute(string boolMemberName, bool showIfConditionMet = true) {
            MemberName = boolMemberName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target) {
            bool conditionMet = (bool)GetMemberValue(target, MemberName);
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class HideInMRDLInspector : ShowIfAttribute
    {
        public HideInMRDLInspector () { }

        public override bool ShouldShow(object target) {
            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfEnumValueAttribute : ShowIfAttribute
    {
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
        public override bool ShouldShow(object target) {
            bool conditionMet = false;
            int memberValue = Convert.ToInt32(GetMemberValue(target, MemberName));
            for (int i = 0; i < ShowValues.Length; i++) {
                if (ShowValues[i] == memberValue) {
                    conditionMet = true;
                    break;
                }
            }
            return ShowIfConditionMet ? conditionMet : !conditionMet;
        }
#endif

        public int[] ShowValues { get; private set; }

        private static object GetAsUnderlyingType(Enum enval) {
            Type entype = enval.GetType();
            Type undertype = Enum.GetUnderlyingType(entype);
            return Convert.ChangeType(enval, undertype);
        }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ShowIfNullAttribute : ShowIfAttribute
    {
        public ShowIfNullAttribute(string nullableMemberName, bool showIfConditionMet = false) {
            MemberName = nullableMemberName;
            ShowIfConditionMet = showIfConditionMet;
        }

#if UNITY_EDITOR
        public override bool ShouldShow(object target) {
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ValidateUnityObjectAttribute : Attribute
    {
        public ValidateUnityObjectAttribute(string methodName, string messageOnError, ValidateFailActionEnum failAction = ValidateFailActionEnum.Warning)
        {
            MethodName = methodName;
            MessageOnFail = messageOnError;
            FailAction = failAction;
        }

        public ValidateResultEnum Validate(UnityEngine.Object target, System.Object source, out string messageOnFail, out ValidateFailActionEnum failAction)
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
                failAction = ValidateFailActionEnum.None;
                return ValidateResultEnum.Succeed;
            }

            messageOnFail = MessageOnFail;
            failAction = FailAction;
            return ValidateResultEnum.Fail;
        }

        public ValidateFailActionEnum FailAction { get; private set; }
        public string MethodName { get; private set; }
        public string MessageOnFail { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DropDownComponentAttribute : DrawOverrideAttribute
    {
        public DropDownComponentAttribute(bool showComponentNames = false, bool autoFill = false, string customLabel = null)
        {
            ShowComponentNames = showComponentNames;
            CustomLabel = customLabel;
            AutoFill = autoFill;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            Transform transform = (target as Component).transform;

            Component componentValue = field.GetValue(target) as Component;

            Type targetType = field.FieldType;
            if (targetType == typeof(MonoBehaviour))
                targetType = typeof(Component);

            if (componentValue == null && AutoFill) {
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

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
            Transform transform = (target as Component).transform;

            Component componentValue = prop.GetValue(target, null) as Component;

            Type targetType = prop.PropertyType;
            if (targetType == typeof(MonoBehaviour))
                targetType = typeof(Component);

            if (componentValue == null && AutoFill) {
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

        public bool AutoFill { get; private set; }
        public bool ShowComponentNames { get; private set; }
        public string CustomLabel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class DropDownGameObjectAttribute : DrawOverrideAttribute
    {
        public DropDownGameObjectAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            Transform transform = (target as Component).transform;

            GameObject fieldValue = field.GetValue(target) as GameObject;
            fieldValue = DropDownGameObjectField(
                SplitCamelCase(field.Name),
                fieldValue,
                transform);
            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
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

        public string CustomLabel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneComponentAttribute : DrawOverrideAttribute
    {
        public SceneComponentAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            Component fieldValue = field.GetValue(target) as Component;
            fieldValue = SceneObjectField(
                SplitCamelCase(field.Name),
                fieldValue,
                field.FieldType);
            field.SetValue(target, fieldValue);
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
            Component propValue = prop.GetValue(target, null) as Component;
            propValue = SceneObjectField(
                SplitCamelCase(prop.Name),
                propValue,
                prop.PropertyType);
            prop.SetValue(target, propValue, null);
        }

        public static Component SceneObjectField(string label, Component sceneObject, Type componentType) {

            EditorGUILayout.BeginHorizontal();
            if (string.IsNullOrEmpty(label)) {
                sceneObject = (Component)EditorGUILayout.ObjectField(sceneObject, componentType, true);
            } else {
                sceneObject = (Component)EditorGUILayout.ObjectField(label, sceneObject, componentType, true);
            }
            if (sceneObject != null && sceneObject.gameObject.scene.name == null) {
                // Don't allow objects that aren't in the scene!
                sceneObject = null;
            }

            UnityEngine.Object[] objectsInScene = GameObject.FindObjectsOfType(componentType);
            int selectedIndex = 0;
            string[] displayedOptions = new string[objectsInScene.Length + 1];
            displayedOptions[0] = "(None)";
            for (int i = 0; i < objectsInScene.Length; i++) {
                displayedOptions[i + 1] = objectsInScene[i].name;
                if (objectsInScene[i] == sceneObject) {
                    selectedIndex = i + 1;
                }
            }
            selectedIndex = EditorGUILayout.Popup(selectedIndex, displayedOptions);
            if (selectedIndex == 0) {
                sceneObject = null;
            } else {
                sceneObject = (Component)objectsInScene[selectedIndex - 1];
            }
            EditorGUILayout.EndHorizontal();
            return sceneObject;
        }
#endif

        public string CustomLabel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SceneGameObjectAttribute : DrawOverrideAttribute
    {
        public SceneGameObjectAttribute(string customLabel = null)
        {
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
            throw new NotImplementedException();
        }

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
            throw new NotImplementedException();
        }
#endif

        public string CustomLabel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public sealed class AnimationCurveDefaultAttribute : DrawOverrideAttribute
    {
        public AnimationCurveDefaultAttribute (Keyframe startVal, Keyframe endVal, WrapMode postWrap = WrapMode.Loop)
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

        public WrapMode PostWrap { get; private set; }
        public Keyframe StartVal { get; private set; }
        public Keyframe EndVal { get; private set; }
    }

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
    
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class EnumFlagsAttribute : DrawOverrideAttribute
    {
#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property)
        {
            int enumValue = Convert.ToInt32(field.GetValue(target));
            List<string> displayOptions = new List<string>();
            foreach (object value in Enum.GetValues(field.FieldType)) {
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

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class EnumCheckboxAttribute : DrawOverrideAttribute
    {
        public EnumCheckboxAttribute(string customLabel = null) {
            DefaultName = null;
            DefaultValue = 0;
            ValueOnZero = 0;
            IgnoreNone = true;
            IgnoreAll = true;
            CustomLabel = customLabel;
        }

        public EnumCheckboxAttribute(bool ignoreNone, bool ignoreAll, string customLabel = null) {
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
            DefaultValue = Convert.ToInt32 (defaultValue);
            ValueOnZero = Convert.ToInt32(valueOnZero);
            IgnoreNone = ignoreNone;
            IgnoreAll = ignoreAll;
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
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

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
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

        private static int EnumCheckbox(string label, object enumObj, string defaultName, object defaultVal, object valOnZero, bool ignoreNone = true, bool ignoreAll = true) {
            if (!enumObj.GetType().IsEnum) {
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

            foreach (object enumVal in enumVals) {
                int flagVal = Convert.ToInt32(enumVal);
                if (ignoreNone && flagVal == 0 && enumVal.ToString().ToLower() == "none") {
                    continue;
                }
                if (ignoreAll && flagVal == lastvalue && enumVal.ToString().ToLower() == "all") {
                    continue;
                }
                bool selected = (flagVal & enumFlags) != 0;
                selected = EditorGUILayout.Toggle(enumVal.ToString(), selected);
                // If it's selected add it to the enumObj, otherwise remove it
                if (selected) {
                    enumFlags |= flagVal;
                } else {
                    enumFlags &= ~flagVal;
                }
            }
            if (!string.IsNullOrEmpty(defaultName)) {
                if (GUILayout.Button(defaultName, EditorStyles.miniButton)) {
                    enumFlags = Convert.ToInt32(defaultVal);
                }
            }
            EditorGUILayout.EndVertical();

            if (enumFlags == 0) {
                enumFlags = Convert.ToInt32(valOnZero);
            }
            return enumFlags;
        }
#endif

        public string DefaultName { get; private set; }
        public int DefaultValue { get; private set; }
        public int ValueOnZero { get; private set; }
        public bool IgnoreNone { get; private set; }
        public bool IgnoreAll { get; private set; }
        public string CustomLabel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class MaterialPropertyAttribute : DrawOverrideAttribute
    {
        public enum PropertyTypeEnum {
            Color,
            Float,
            Range,
            Vector,
        }

        public MaterialPropertyAttribute(PropertyTypeEnum propertyType, string materialMemberName, bool allowNone = true, string defaultProperty = "_Color", string customLabel = null)
        {
            PropertyType = propertyType;
            MaterialMemberName = materialMemberName;
            AllowNone = allowNone;
            DefaultProperty = defaultProperty;
            CustomLabel = customLabel;
        }

#if UNITY_EDITOR
        public override void DrawEditor(UnityEngine.Object target, FieldInfo field, SerializedProperty property) {
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

        public override void DrawEditor(UnityEngine.Object target, PropertyInfo prop) {
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

        private Material GetMaterial (object target) {
            MemberInfo[] members = target.GetType().GetMember(MaterialMemberName);
            if (members.Length == 0) {
                Debug.LogError("Couldn't find material member " + MaterialMemberName);
                return null;
            }

            Material mat = null;

            switch (members[0].MemberType) {
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

        private static string MaterialPropertyName(string property, Material mat, PropertyTypeEnum type, bool allowNone, string defaultProperty, string labelName) {
            Color tColor = GUI.color;
            // Create a list of available color and value properties
            List<string> props = new List<string>();

            int selectedPropIndex = 0;

            if (allowNone) {
                props.Add("(None)");
            }

            if (mat != null) {
                int propertyCount = ShaderUtil.GetPropertyCount(mat.shader);
                string propName = string.Empty;
                for (int i = 0; i < propertyCount; i++) {
                    if (ShaderUtil.GetPropertyType(mat.shader, i).ToString() == type.ToString()) {
                        propName = ShaderUtil.GetPropertyName(mat.shader, i);
                        if (propName == property) {
                            // We've found our current property
                            selectedPropIndex = props.Count;
                        }
                        props.Add(propName);
                    }
                }
                
                if (string.IsNullOrEmpty(labelName)) {
                    labelName = type.ToString();
                }
                int newPropIndex = EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                if (allowNone) {
                    property = (newPropIndex > 0 ? props[newPropIndex] : string.Empty);
                } else {
                    if (props.Count > 0) {
                        property = props[newPropIndex];
                    } else {
                        property = defaultProperty;
                    }
                }
                return property;
            } else {
                GUI.color = Color.Lerp (tColor, Color.gray, 0.5f);
                // Draw an empty property
                EditorGUILayout.Popup(labelName, selectedPropIndex, props.ToArray());
                GUI.color = tColor;
                return string.Empty;
            }
        }
#endif

        public string Property { get; private set; }
        public PropertyTypeEnum PropertyType { get; private set; }
        public string MaterialMemberName { get; private set; }
        public bool AllowNone { get; private set; }
        public string DefaultProperty { get; private set; }
        public string CustomLabel { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class FilterTagsAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class ValidateSettingAttribute : Attribute
    {        
        public ValidateSettingAttribute (string validateMethodName, string fixMethodName = null)
        {
            ValidateMethodName = validateMethodName;
            FixMethodName = fixMethodName;
        }
        
        public string ValidateMethodName { get; private set; }
        public string FixMethodName { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public sealed class SetIndentAttribute : Attribute
    {
        public SetIndentAttribute(int indent)
        {
            Indent = indent;
        }

        public int Indent { get; private set; }
    }

#endregion

#region custom editors

#if UNITY_EDITOR
    public class MRDLEditor : Editor
    {
#region static vars
        public static bool ShowCustomEditors = true;
        public static GameObject lastTarget;

        private static bool showHelp = false;
        private static int indentOnSectionStart = 0;

        private static GUIStyle toggleButtonOffStyle = null;
        private static GUIStyle toggleButtonOnStyle = null;
        private static GUIStyle sectionStyle = null;
        private static GUIStyle toolTipStyle = null;

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

        private static BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private static Dictionary<string, bool> displayedSections = new Dictionary<string, bool>();
        
#endregion

        public override void OnInspectorGUI()
        {
            CreateStyles();
            DrawInspectorHeader();
            Undo.RecordObject(target, target.name);

            if (ShowCustomEditors) {
                DrawCustomEditor();
                DrawCustomFooter();
            } else {
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

        protected void DrawCustomEditor()
        {
            EditorGUILayout.BeginVertical();

            Type targetType = target.GetType();
            // Get all the members of this type, public and private
            List<MemberInfo> members = new List<MemberInfo>(targetType.GetMembers(defaultBindingFlags));
            members.Sort(
                delegate (MemberInfo m1, MemberInfo m2) {
                    if (m1.IsDefined(typeof(DrawLastAttribute), true)) {
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
                    if ((insideSectionBlock && !drawCurrentSection) || !ShowMember(member, targetType, target))
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
        protected virtual void DrawCustomFooter() {

        }

        protected void SaveChanges() {
            if (serializedObject.ApplyModifiedProperties()) {
                EditorUtility.SetDirty(target);
            }
        }

        private bool ShowMember(MemberInfo member, Type targetType, object target)
        {
            object[] hideAttributes = member.GetCustomAttributes(typeof(HideInInspector), true);
            if (hideAttributes != null && hideAttributes.Length > 0)
                return false;

            bool shouldBeVisible = true;

            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    // Fields are visible by default unless they're hidden by a ShowIfAttribute
                    foreach (ShowIfAttribute attribute in member.GetCustomAttributes(typeof(ShowIfAttribute), true)) {
                        if (!attribute.ShouldShow(target)) {
                            shouldBeVisible = false;
                            break;
                        }
                    }
                    break;

                case MemberTypes.Property:
                    // Property types require at least one EditorAttribute to be visible
                    if (member.GetCustomAttributes(typeof(EditorAttribute), true).Length == 0) {
                        shouldBeVisible = false;
                    } else {
                        // Even if they have an editor attribute, they can still be hidden by a ShowIfAttribute
                        foreach (ShowIfAttribute attribute in member.GetCustomAttributes(typeof(ShowIfAttribute), true)) {
                            if (!attribute.ShouldShow(target)) {
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

#region drawing

        protected void DrawSerializedField(SerializedObject serializedObject, string propertyPath)
        {
            SerializedProperty prop = serializedObject.FindProperty(propertyPath);
            if (prop != null)
                EditorGUILayout.PropertyField(prop, true);
        }        
        
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

        public static void DrawSectionEnd()
        {
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel = indentOnSectionStart;
        }

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

#endregion

#region profiles

        /// Draws a field for scriptable object profiles
        /// If base class is abstract, includes a button for creating a profile of each type that inherits from base class T
        /// Otherwise just includes button for creating a profile of type
        /// Also finds and draws the inspector for the profile
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
    /// Adds a 'target component' so inspectors can differentiate between local / global editing
    /// See compound button component inspectors for usage examples
    /// </summary>
    public abstract class ProfileInspector : MRDLEditor
    {
        public Component targetComponent;

        public override void OnInspectorGUI() {
            Undo.RecordObject(target, target.name);
            BeginProfileInspector();
            DrawCustomEditor();
            DrawCustomFooter();
            EndProfileInspector();
            SaveChanges();
        }

        private void BeginProfileInspector() {
            GUI.color = profileColor;
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUI.color = Color.Lerp(profileColor, Color.red, 0.5f);
            EditorGUILayout.LabelField("(Warning: this section edits the button profile. These changes will affect all objects that use this profile.)", EditorStyles.wordWrappedMiniLabel);
            GUI.color = defaultColor;
        }

        private void EndProfileInspector() {
            EditorGUILayout.EndVertical();
        }
    }
#endif

    #endregion
}