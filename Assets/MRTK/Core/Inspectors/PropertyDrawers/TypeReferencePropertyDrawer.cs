// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assembly = System.Reflection.Assembly;

namespace Microsoft.MixedReality.Toolkit.Editor
{
    /// <summary>
    /// Custom property drawer for <see cref="Utilities.SystemType"/> properties.
    /// </summary>
    [CustomPropertyDrawer(typeof(SystemType))]
    [CustomPropertyDrawer(typeof(SystemTypeAttribute), true)]
    public class SystemTypeReferencePropertyDrawer : PropertyDrawer
    {
        private static int selectionControlId;
        private static string selectedReference;
        private static readonly Dictionary<string, Type> TypeMap = new Dictionary<string, Type>();
        private static readonly int ControlHint = typeof(SystemTypeReferencePropertyDrawer).GetHashCode();
        private static readonly GUIContent TempContent = new GUIContent();
        private static readonly Color enabledColor = Color.white;
        private static readonly Color disabledColor = Color.Lerp(Color.white, Color.clear, 0.5f);
        private static readonly Color errorColor = Color.Lerp(Color.white, Color.red, 0.5f);

        #region Type Filtering

        /// <summary>
        /// Gets or sets a function that returns a collection of types that are
        /// to be excluded from drop-down. A value of <c>null</c> specifies that
        /// no types are to be excluded.
        /// </summary>
        /// <remarks>
        /// <para>This property must be set immediately before presenting a class
        /// type reference property field using <see href="https://docs.unity3d.com/ScriptReference/EditorGUI.PropertyField.html">EditorGUI.PropertyField</see>
        /// since the value of this property is reset to <c>null</c> each time the control is drawn.</para>
        /// <para>Since filtering makes extensive use of <see cref="System.Collections.Generic.ICollection{Type}.Contains"/>
        /// it is recommended to use a collection that is optimized for fast
        /// look ups such as HashSet for better performance.</para>
        /// </remarks>
        /// <example>
        /// <para>Exclude a specific type from being selected:</para>
        /// <code language="csharp"><![CDATA[
        /// private SerializedProperty someTypeReferenceProperty;
        /// 
        /// public override void OnInspectorGUI() {
        ///     serializedObject.Update();
        /// 
        ///     ClassTypeReferencePropertyDrawer.ExcludedTypeCollectionGetter = GetExcludedTypeCollection;
        ///     EditorGUILayout.PropertyField(someTypeReferenceProperty);
        /// 
        ///     serializedObject.ApplyModifiedProperties();
        /// }
        /// 
        /// private ICollection<Type> GetExcludedTypeCollection() {
        ///     var set = new HashSet<Type>();
        ///     set.Add(typeof(SpecialClassToHideInDropdown));
        ///     return set;
        /// }
        /// ]]></code>
        /// </example>
        public static Func<ICollection<Type>> ExcludedTypeCollectionGetter { get; set; }

        private static List<Type> GetFilteredTypes(SystemTypeAttribute filter)
        {
            var types = new List<Type>();
            var excludedTypes = ExcludedTypeCollectionGetter?.Invoke();

            // We prefer using this over CompilationPipeline.GetAssemblies() because
            // some types may come from plugins and other sources that have already
            // been compiled.
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                FilterTypes(assembly, filter, excludedTypes, types);
            }

            types.Sort((a, b) => string.Compare(a.FullName, b.FullName, StringComparison.Ordinal));
            return types;
        }

        private static void FilterTypes(Assembly assembly, SystemTypeAttribute filter, ICollection<Type> excludedTypes, List<Type> output)
        {
            foreach (var type in assembly.GetLoadableTypes())
            {
                bool isValid = type.IsValueType && !type.IsEnum || type.IsClass;
                if (!type.IsVisible || !isValid)
                {
                    continue;
                }

                if (filter != null && !filter.IsConstraintSatisfied(type))
                {
                    continue;
                }

                if (excludedTypes != null && excludedTypes.Contains(type))
                {
                    continue;
                }

                output.Add(type);
            }
        }

        #endregion Type Filtering

        #region Type Utility

        private static Type ResolveType(string classRef)
        {
            Type type;
            if (!TypeMap.TryGetValue(classRef, out type))
            {
                type = !string.IsNullOrEmpty(classRef) ? Type.GetType(classRef) : null;
                TypeMap[classRef] = type;
            }

            return type;
        }

        #endregion Type Utility

        #region Control Drawing / Event Handling

        private static string DrawTypeSelectionControl(Rect position, GUIContent label, string classRef, SystemTypeAttribute filter, bool typeResolved)
        {
            if (label != null && label != GUIContent.none)
            {
                position = EditorGUI.PrefixLabel(position, label);
            }

            int controlId = GUIUtility.GetControlID(ControlHint, FocusType.Keyboard, position);

            bool triggerDropDown = false;

            switch (Event.current.GetTypeForControl(controlId))
            {
                case EventType.ExecuteCommand:
                    if (Event.current.commandName == "TypeReferenceUpdated")
                    {
                        if (selectionControlId == controlId)
                        {
                            if (classRef != selectedReference)
                            {
                                classRef = selectedReference;
                                GUI.changed = true;
                            }

                            selectionControlId = 0;
                            selectedReference = null;
                        }
                    }

                    break;

                case EventType.MouseDown:
                    if (GUI.enabled && position.Contains(Event.current.mousePosition))
                    {
                        GUIUtility.keyboardControl = controlId;
                        triggerDropDown = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.KeyDown:
                    if (GUI.enabled && GUIUtility.keyboardControl == controlId)
                    {
                        if (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.Space)
                        {
                            triggerDropDown = true;
                            Event.current.Use();
                        }
                    }

                    break;

                case EventType.Repaint:
                    // Remove assembly name and namespace from content of pop-up control.
                    var classRefParts = classRef.Split(',');
                    var className = classRefParts[0].Trim();
                    className = className.Substring(className.LastIndexOf(".", StringComparison.Ordinal) + 1);
                    TempContent.text = className;

                    if (TempContent.text == string.Empty)
                    {
                        TempContent.text = "(None)";
                    }
                    else if (!typeResolved)
                    {
                        TempContent.text += " {Missing}";
                    }

                    EditorStyles.popup.Draw(position, TempContent, controlId);
                    break;
            }

            if (triggerDropDown)
            {
                selectionControlId = controlId;
                selectedReference = classRef;

                DisplayDropDown(position, GetFilteredTypes(filter), ResolveType(classRef), filter?.Grouping ?? TypeGrouping.ByNamespaceFlat);
            }

            return classRef;
        }

        private static void DrawTypeSelectionControl(Rect position, SerializedProperty property, GUIContent label, SystemTypeAttribute filter)
        {
            try
            {
                Color restoreColor = GUI.color;
                bool restoreShowMixedValue = EditorGUI.showMixedValue;
                bool typeResolved = string.IsNullOrEmpty(property.stringValue) || ResolveType(property.stringValue) != null;
                EditorGUI.showMixedValue = property.hasMultipleDifferentValues;

                GUI.color = enabledColor;

                if (typeResolved)
                {
                    property.stringValue = DrawTypeSelectionControl(position, label, property.stringValue, filter, true);
                }
                else
                {
                    if (SelectRepairedTypeWindow.WindowOpen)
                    {
                        GUI.color = disabledColor;
                        DrawTypeSelectionControl(position, label, property.stringValue, filter, false);
                    }
                    else
                    {
                        Rect dropdownPosition = new Rect(position.x, position.y, position.width - 90, position.height);
                        Rect buttonPosition = new Rect(position.x + position.width - 75, position.y, 75, position.height);

                        Color defaultColor = GUI.color;
                        GUI.color = errorColor;
                        property.stringValue = DrawTypeSelectionControl(dropdownPosition, label, property.stringValue, filter, false);
                        GUI.color = defaultColor;

                        if (GUI.Button(buttonPosition, "Try Repair", EditorStyles.miniButton))
                        {
                            string typeNameWithoutAssembly = property.stringValue.Split(new string[] { "," }, StringSplitOptions.None)[0];
                            string typeNameWithoutNamespace = System.Text.RegularExpressions.Regex.Replace(typeNameWithoutAssembly, @"[.\w]+\.(\w+)", "$1");

                            Type[] repairedTypeOptions = FindTypesByName(typeNameWithoutNamespace, filter);
                            if (repairedTypeOptions.Length > 1)
                            {
                                SelectRepairedTypeWindow.Display(repairedTypeOptions, property);
                            }
                            else if (repairedTypeOptions.Length > 0)
                            {
                                property.stringValue = SystemType.GetReference(repairedTypeOptions[0]);
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("No types found", "No types with the name '" + typeNameWithoutNamespace + "' were found.", "OK");
                            }
                        }
                    }
                }

                GUI.color = restoreColor;
                EditorGUI.showMixedValue = restoreShowMixedValue;
            }
            finally
            {
                ExcludedTypeCollectionGetter = null;
            }
        }

        private static Type[] FindTypesByName(string typeName, SystemTypeAttribute filter)
        {
            List<Type> types = new List<Type>();
            foreach (Type t in GetFilteredTypes(filter))
            {
                if (t.Name.Equals(typeName))
                {
                    types.Add(t);
                }
            }
            return types.ToArray();
        }

        private static void DisplayDropDown(Rect position, List<Type> types, Type selectedType, TypeGrouping grouping)
        {
            var menu = new GenericMenu();

            if (types.Count == 0)
            {
                menu.AddItem(new GUIContent("No types available"), selectedType == null, OnSelectedTypeName, null);
            }
            else
            {
                for (int i = 0; i < types.Count; ++i)
                {
                    string menuLabel = FormatGroupedTypeName(types[i], grouping);

                    if (string.IsNullOrEmpty(menuLabel)) { continue; }

                    var content = new GUIContent(menuLabel);
                    menu.AddItem(content, types[i] == selectedType, OnSelectedTypeName, types[i]);
                }
            }

            menu.DropDown(position);
        }

        private static string FormatGroupedTypeName(Type type, TypeGrouping grouping)
        {
            string name = type.FullName;

            switch (grouping)
            {
                case TypeGrouping.None:
                    return name;
                case TypeGrouping.ByNamespace:
                    return string.IsNullOrEmpty(name) ? string.Empty : name.Replace('.', '/');
                case TypeGrouping.ByNamespaceFlat:
                    int lastPeriodIndex = string.IsNullOrEmpty(name) ? -1 : name.LastIndexOf('.');
                    if (lastPeriodIndex != -1)
                    {
                        name = string.IsNullOrEmpty(name)
                            ? string.Empty
                            : $"{name.Substring(0, lastPeriodIndex)}/{name.Substring(lastPeriodIndex + 1)}";
                    }

                    return name;
                case TypeGrouping.ByAddComponentMenu:
                    var addComponentMenuAttributes = type.GetCustomAttributes(typeof(AddComponentMenu), false);
                    if (addComponentMenuAttributes.Length == 1)
                    {
                        return ((AddComponentMenu)addComponentMenuAttributes[0]).componentMenu;
                    }

                    Debug.Assert(type.FullName != null);
                    return $"Scripts/{type.FullName.Replace('.', '/')}";
                default:
                    throw new ArgumentOutOfRangeException(nameof(grouping), grouping, null);
            }
        }

        private static void OnSelectedTypeName(object userData)
        {
            selectedReference = SystemType.GetReference(userData as Type);
            var typeReferenceUpdatedEvent = EditorGUIUtility.CommandEvent("TypeReferenceUpdated");
            EditorWindow.focusedWindow.SendEvent(typeReferenceUpdatedEvent);
        }

        #endregion Control Drawing / Event Handling

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorStyles.popup.CalcHeight(GUIContent.none, 0);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DrawTypeSelectionControl(position, property.FindPropertyRelative("reference"), label, attribute as SystemTypeAttribute);
        }
    }
}
