// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Attributes;
using MixedRealityToolkit.UX.Buttons.Profiles;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace MixedRealityToolkit.Utilities.Inspectors.EditorScript
{
#if UNITY_EDITOR
    /// <summary>
    /// To use this class in a MonoBehavior or ScriptableObject, add this line at the bottom of your class:
    /// 
    /// public class ClassName {
    /// ...
    /// #if UNITY_EDITOR
    ///     [UnityEditor.CustomEditor(typeof(ClassName))]
    ///     public class CustomEditor : MRTKEditor { }
    /// #endif
    /// }
    /// 
    /// </summary>
    public class MRTKEditor : Editor
    {
        #region static vars
        const string mrtkShowEditorKey = "_Show_MRTK_Editors";

        // Toggles custom editors on / off
        public static bool ShowCustomEditors
        {
            get
            {
                return GetEditorPref(mrtkShowEditorKey, true);
            }
            private set
            {
                SetEditorPref(mrtkShowEditorKey, value);
            }
        }
        public static bool CustomEditorActive { get; private set; }
        public static GameObject lastTarget;

        // Styles
        private static GUIStyle toggleButtonOffStyle = null;
        private static GUIStyle toggleButtonOnStyle = null;
        private static GUIStyle sectionStyle = null;
        private static GUIStyle toolTipStyle = null;
        private static GUIStyle inProgressStyle = null;

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
        protected readonly static Color handleColorSquare = new Color(0.0f, 0.9f, 1f);
        protected readonly static Color handleColorCircle = new Color(1f, 0.5f, 1f);
        protected readonly static Color handleColorSphere = new Color(1f, 0.5f, 1f);
        protected readonly static Color handleColorAxis = new Color(0.0f, 1f, 0.2f);
        protected readonly static Color handleColorRotation = new Color(0.0f, 1f, 0.2f);
        protected readonly static Color handleColorTangent = new Color(0.1f, 0.8f, 0.5f, 0.7f);

        public const float DottedLineScreenSpace = 4.65f;

        // Toggles visible tooltips
        private static bool showHelp = false;
        // Stores the show / hide values of displayed sections by target name + section name
        private static Dictionary<string, bool> displayedSections = new Dictionary<string, bool>();
        private static int indentOnSectionStart = 0;

        private static BindingFlags defaultBindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        private bool recordingUndo = false;

        #endregion

        public override void OnInspectorGUI()
        {
            // Set this to true so we can track which kind of headers to draw
            CustomEditorActive = true;

            try
            {
                CreateStyles();
                DrawInspectorHeader();
                Undo.RecordObject(target, target.name);

                if (ShowCustomEditors)
                {
                    BeginInspectorStyle();
                    DrawCustomEditor();
                    DrawCustomFooter();
                    EndInspectorStyle();
                }
                else
                {
                    base.DrawDefaultInspector();
                }

                SaveChanges();
            } catch (Exception e)
            {
                DrawError(e.ToString());
            }

            CustomEditorActive = false;
        }

        public void OnSceneGUI()
        {
            recordingUndo = false;
            DrawCustomSceneGUI();
        }

        protected virtual void BeginInspectorStyle()
        {
            // Empty by default
        }

        protected virtual void EndInspectorStyle()
        {
            // Empty by default
        }

        protected virtual bool DisplayHeader { get { return true; } }

        /// <summary>
        /// Draws buttons for turning custom editors on/off, as well as DocType, Tutorial and UseWith attributes
        /// </summary>
        private void DrawInspectorHeader()
        {
            if (!DisplayHeader)
            {
                return;
            }

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
                            {
                                displayedSections.Add(lookupName, true);
                            }
                            else
                            {
                                displayedSections[lookupName] = true;
                            }
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
                            {
                                displayedSections.Add(lookupName, false);
                            }
                            else
                            {
                                displayedSections[lookupName] = false;
                            }
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
                    {
                        description = "Click for documentation about " + targetType.Name;
                    }

                    if (GUILayout.Button(description, EditorStyles.toolbarButton))
                    {
                        Application.OpenURL(attribute.DocURL);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                foreach (TutorialAttribute attribute in targetType.GetCustomAttributes(typeof(TutorialAttribute), true))
                {
                    string description = attribute.Description;
                    if (string.IsNullOrEmpty(description))
                    {
                        description = "Click for a tutorial on " + targetType.Name;
                    }

                    if (GUILayout.Button(description, EditorStyles.toolbarButton))
                    {
                        Application.OpenURL(attribute.TutorialURL);
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal();
                List<Type> missingTypes = new List<Type>();
                foreach (UseWithAttribute attribute in targetType.GetCustomAttributes(typeof(UseWithAttribute), true))
                {
                    Component targetGo = (Component)target;
                    if (targetGo == null)
                    {
                        break;
                    }

                    foreach (Type type in attribute.UseWithTypes)
                    {
                        Component c = targetGo.GetComponent(type);
                        if (c == null)
                        {
                            missingTypes.Add(type);
                        }
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

            // Start drawing the editor
            int currentIndentLevel = 0;
            bool insideSectionBlock = false;
            bool drawCurrentSection = true;

            foreach (MemberInfo member in members)
            {
                try
                {
                    switch (member.MemberType)
                    {
                        default:
                            // We only want to draw fields, properties and arrays
                            continue;

                        case MemberTypes.Field:
                        case MemberTypes.Property:
                            break;
                    }

                    // First get header and indent settings
                    if (member.IsDefined(typeof(HeaderAttribute), true))
                    {
                        HeaderAttribute header = member.GetCustomAttributes(typeof(HeaderAttribute), true)[0] as HeaderAttribute;
                        if (insideSectionBlock)
                        {
                            DrawSectionEnd();
                        }

                        insideSectionBlock = true;
                        drawCurrentSection = DrawSectionStart(target.GetType().Name, header.header);
                    }

                    // Then do basic show / hide based on ShowIfAttribute
                    if ((insideSectionBlock && !drawCurrentSection) || !ShouldDrawMember(member, targetType, target))
                    {
                        continue;
                    }

                    // Handle drawing stuff (indent, help)
                    if (showHelp)
                    {
                        DrawToolTip(member);
                    }

                    if (member.IsDefined(typeof(SetIndentAttribute), true))
                    {
                        SetIndentAttribute indent = member.GetCustomAttributes(typeof(SetIndentAttribute), true)[0] as SetIndentAttribute;
                        currentIndentLevel += indent.Indent;
                        EditorGUI.indentLevel = currentIndentLevel;
                    }

                    if (member.IsDefined(typeof(FeatureInProgressAttribute), true))
                    {
                        GUI.color = helpBoxColor;
                        EditorGUILayout.LabelField("(This field or property is marked as 'In Progress' and may have no effect.)", inProgressStyle);
                        GUI.color = Color.Lerp(disabledColor, Color.clear, 0.5f);
                    }
                    else
                    {
                        GUI.color = defaultColor;
                    }

                    // Now get down to drawing the thing
                    // Get an array ready for our override attributes
                    object[] drawOverrideAttributes = null;
                    if (member.MemberType == MemberTypes.Field)
                    {
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
                    }
                    else // Property
                    {
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
                            {
                                DrawWarning("You should only use one DrawOverride attribute per member. Drawing " + drawOverrideAttributes[0].GetType().Name + " only.");
                            }

                            (drawOverrideAttributes[0] as DrawOverrideAttribute).DrawEditor(target, prop);
                        }
                    }
                }
                catch (Exception e)
                {
                    DrawWarning("There was a problem drawing the member " + member.Name + ":");
                    DrawError(System.Environment.NewLine + e.ToString());
                }
            }

            if (insideSectionBlock)
            {
                DrawSectionEnd();
            }

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Override this to draw a scene editor
        /// </summary>
        protected virtual void DrawCustomSceneGUI()
        {

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
            serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(target);
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
                    // Property types require at least one Attribute to be visible
                    if (member.GetCustomAttributes(typeof(Attribute), true).Length == 0)
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
            {
                EditorGUILayout.PropertyField(prop, true);
            }
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
            {
                headerName = headerName.ToUpper();
            }

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
            GUIStyle styleHR = new GUIStyle(GUI.skin.box)
            {
                stretchWidth = true,
                fixedHeight = 2
            };
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

                sectionStyle = new GUIStyle(EditorStyles.foldout)
                {
                    fontStyle = FontStyle.Bold
                };

                toolTipStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
                {
                    fontStyle = FontStyle.Normal,
                    alignment = TextAnchor.LowerLeft
                };

                inProgressStyle = new GUIStyle(EditorStyles.wordWrappedMiniLabel)
                {
                    fontStyle = FontStyle.Italic,
                    alignment = TextAnchor.LowerLeft
                };
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
        /// Displays a help window explaining profile objects
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
                {
                    return type.IsSubclassOf(baseType);
                }
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
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
        #endregion

        #region handles
        // These are a set of handles for OnSceneGUI. They automatically register Undo for the target object and they have a consistent look.

        protected float AxisMoveHandle(Vector3 origin, Vector3 direction, float distance, float handleSize = 0.2f, bool autoSize = true)
        {
            Vector3 position = origin + (direction.normalized * distance);

            Handles.color = handleColorAxis;
            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Handles.DrawDottedLine(origin, position, DottedLineScreenSpace);
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(direction), handleSize * 2, EventType.Repaint);
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);
            if (recordingUndo)
                return distance;

            float newDistance = Vector3.Distance(origin, newPosition);

            if (distance != newDistance)
            {
                recordingUndo = true;
                Undo.RegisterCompleteObjectUndo(target, target.name);
                distance = newDistance;
            }
            return distance;
        }

        protected Vector3 CircleMoveHandle(Vector3 position, float handleSize = 0.2f, bool autoSize = true, float xScale = 1f, float yScale = 1f, float zScale = 1f)
        {
            Handles.color = handleColorCircle;
            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);
            if (recordingUndo)
                return position;

            if (position != newPosition)
            {
                recordingUndo = true;
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }
            return position;
        }

        protected Vector3 SquareMoveHandle(Vector3 position, float handleSize = 0.2f, bool autoSize = true, float xScale = 1f, float yScale = 1f, float zScale = 1f)
        {
            Handles.color = handleColorSquare;
            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply square handle to match other types
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 0.8f, Vector3.zero, Handles.RectangleHandleCap);
            if (recordingUndo)
                return position;

            if (position != newPosition)
            {
                recordingUndo = true;
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }
            return position;
        }

        protected Vector3 SphereMoveHandle(Vector3 position, float handleSize = 0.2f, bool autoSize = true, float xScale = 1f, float yScale = 1f, float zScale = 1f)
        {
            Handles.color = handleColorSphere;
            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply sphere handle size to match other types
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 2, Vector3.zero, Handles.SphereHandleCap);
            if (recordingUndo)
                return position;

            if (position != newPosition)
            {
                recordingUndo = true;
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }
            return position;
        }

        protected Vector3 VectorHandle(Vector3 origin, Vector3 vector, bool normalize = true, float handleLength = 1f, bool clamp = true, float handleSize = 0.1f, bool autoSize = true)
        {
            Handles.color = handleColorTangent;
            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(origin) * handleSize, 0.75f);
            }

            Vector3 handlePosition = origin + (vector * handleLength);
            float distanceToOrigin = Vector3.Distance(origin, handlePosition) / handleLength;

            if (normalize)
            {
                vector.Normalize();
            }
            else
            {
                // If the handle isn't normalized, brighten based on distance to origin
                Handles.color = Color.Lerp(Color.gray, handleColorTangent, distanceToOrigin * 0.85f);
                if (clamp)
                {
                    // To indicate that we're at the clamped limit, make the handle 'pop' slightly larger
                    if (distanceToOrigin >= 0.98f)
                    {
                        Handles.color = Color.Lerp(handleColorTangent, Color.white, 0.5f);
                        handleSize *= 1.5f;
                    }
                }
            }

            // Draw a line from origin to origin + direction
            Handles.DrawLine(origin, handlePosition);

            Quaternion rotation = Quaternion.identity;
            if (vector != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(vector);
            }

            Vector3 newPosition = Handles.FreeMoveHandle(handlePosition, rotation, handleSize, Vector3.zero, Handles.DotHandleCap);
            if (recordingUndo)
            {
                return vector;
            }

            if (handlePosition != newPosition)
            {
                recordingUndo = true;
                Undo.RegisterCompleteObjectUndo(target, target.name);
                vector = (newPosition - origin).normalized;

                // If we normalize, we're done
                // Otherwise, multiply the vector by the distance between origin and target
                if (!normalize)
                {
                    distanceToOrigin = Vector3.Distance(origin, newPosition) / handleLength;
                    if (clamp)
                    {
                        distanceToOrigin = Mathf.Clamp01(distanceToOrigin);
                    }
                    vector *= distanceToOrigin;
                }
            }
            return vector;
        }

        protected Quaternion RotationHandle(Vector3 position, Quaternion rotation, float handleSize = 0.2f, bool autoSize = true)
        {
            Handles.color = handleColorRotation;
            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Make rotation handles larger so they can overlay movement handles
            Quaternion newRotation = Handles.FreeRotateHandle(rotation, position, handleSize * 2);
            if (recordingUndo)
            {
                return newRotation;
            }

            Handles.color = Handles.zAxisColor;
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.forward), handleSize * 2, EventType.Repaint);
            Handles.color = Handles.xAxisColor;
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.right), handleSize * 2, EventType.Repaint);
            Handles.color = Handles.yAxisColor;
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.up), handleSize * 2, EventType.Repaint);

            if (rotation != newRotation)
            {
                recordingUndo = true;
                Undo.RegisterCompleteObjectUndo(target, target.name);

                rotation = newRotation;
            }
            return rotation;
        }
        #endregion

        #region editor prefs
        
        private static void SetEditorPref(string key, bool value)
        {
            EditorPrefs.SetBool(Application.productName + key, value);
        }

        private static bool GetEditorPref(string key, bool defaultValue)
        {
            if (EditorPrefs.HasKey(Application.productName + key))
            {
                return EditorPrefs.GetBool(Application.productName + key);
            }

            EditorPrefs.SetBool(Application.productName + key, defaultValue);
            return defaultValue;
        }

        #endregion
    }
#endif
}