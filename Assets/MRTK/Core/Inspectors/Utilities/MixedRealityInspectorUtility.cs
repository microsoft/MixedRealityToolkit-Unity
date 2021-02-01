// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.Toolkit.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Microsoft.MixedReality.Toolkit.Utilities.Editor
{
    /// <summary>
    /// This class has handy inspector utilities and functions.
    /// </summary>
    public static class MixedRealityInspectorUtility
    {
        #region Colors

        public static Color DefaultBackgroundColor
        {
            get
            {
                return EditorGUIUtility.isProSkin
                    ? new Color32(56, 56, 56, 255)
                    : new Color32(194, 194, 194, 255);
            }
        }

        public static readonly Color DisabledColor = new Color(0.6f, 0.6f, 0.6f);
        public static readonly Color WarningColor = new Color(1f, 0.85f, 0.6f);
        public static readonly Color ErrorColor = new Color(1f, 0.55f, 0.5f);
        public static readonly Color SuccessColor = new Color(0.8f, 1f, 0.75f);
        public static readonly Color SectionColor = new Color(0.85f, 0.9f, 1f);
        public static readonly Color DarkColor = new Color(0.1f, 0.1f, 0.1f);
        public static readonly Color HandleColorSquare = new Color(0.0f, 0.9f, 1f);
        public static readonly Color HandleColorCircle = new Color(1f, 0.5f, 1f);
        public static readonly Color HandleColorSphere = new Color(1f, 0.5f, 1f);
        public static readonly Color HandleColorAxis = new Color(0.0f, 1f, 0.2f);
        public static readonly Color HandleColorRotation = new Color(0.0f, 1f, 0.2f);
        public static readonly Color HandleColorTangent = new Color(0.1f, 0.8f, 0.5f, 0.7f);
        public static readonly Color LineVelocityColor = new Color(0.9f, 1f, 0f, 0.8f);

        #endregion Colors

        public const float DottedLineScreenSpace = 4.65f;
        public const string DefaultConfigProfileName = "DefaultMixedRealityToolkitConfigurationProfile";

        // StandardAssets/Textures/MRTK_Logo_Black.png
        private const string LogoLightThemeGuid = "c2c00ef21cc44bcfa09695879e0ebecd";
        // StandardAssets/Textures/MRTK_Logo_White.png
        private const string LogoDarkThemeGuid = "84643a20fa6b4fa7969ef84ad2e40992";

        public static readonly Texture2D LogoLightTheme = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(LogoLightThemeGuid));

        public static readonly Texture2D LogoDarkTheme = AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath(LogoDarkThemeGuid));

        private const string CloneProfileHelpLabel = "Currently viewing a MRTK default profile. It is recommended to clone defaults and modify a custom profile.";
        private const string CloneProfileHelpLockedLabel = "Clone this default profile to edit properties below";

        /// <summary>
        /// Check and make sure we have a Mixed Reality Toolkit and an active profile.
        /// </summary>
        /// <returns>True if the Mixed Reality Toolkit is properly initialized.</returns>
        public static bool CheckMixedRealityConfigured(bool renderEditorElements = false)
        {
            if (!MixedRealityToolkit.IsInitialized)
            {   // Don't proceed
                return false;
            }

            if (!MixedRealityToolkit.Instance.HasActiveProfile)
            {
                if (renderEditorElements)
                {
                    EditorGUILayout.HelpBox("No Active Profile set on the Mixed Reality Toolkit.", MessageType.Error);
                }
                return false;
            }

            return true;
        }

        /// <summary>
        /// If MRTK is not initialized in scene, adds and initializes instance to current scene
        /// </summary>
        public static void AddMixedRealityToolkitToScene(MixedRealityToolkitConfigurationProfile configProfile = null)
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                MixedRealityToolkit newInstance = new GameObject("MixedRealityToolkit").AddComponent<MixedRealityToolkit>();
                MixedRealityToolkit.SetActiveInstance(newInstance);
                Selection.activeObject = newInstance;

                if (configProfile == null)
                {
                    // if we don't have a profile set we get the default profile
                    newInstance.ActiveProfile = GetDefaultConfigProfile();
                }
                else
                {
                    newInstance.ActiveProfile = configProfile;
                }
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }

        /// <summary>
        /// Render the Mixed Reality Toolkit Logo.
        /// </summary>
        public static void RenderMixedRealityToolkitLogo()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label(EditorGUIUtility.isProSkin ? LogoDarkTheme : LogoLightTheme, GUILayout.MaxHeight(96f));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(3f);
        }

        /// <summary>
        /// Found at https://answers.unity.com/questions/960413/editor-window-how-to-center-a-window.html
        /// </summary>
        public static Rect GetEditorMainWindowPos()
        {
            var containerWinType = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(ScriptableObject)).FirstOrDefault(t => t.Name == "ContainerWindow");

            if (containerWinType == null)
            {
                throw new MissingMemberException("Can't find internal type ContainerWindow. Maybe something has changed inside Unity");
            }

            var showModeField = containerWinType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
            var positionProperty = containerWinType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);

            if (showModeField == null || positionProperty == null)
            {
                throw new MissingFieldException("Can't find internal fields 'm_ShowMode' or 'position'. Maybe something has changed inside Unity");
            }

            var windows = Resources.FindObjectsOfTypeAll(containerWinType);

            foreach (var win in windows)
            {

                var showMode = (int)showModeField.GetValue(win);
                if (showMode == 4) // main window
                {
                    var pos = (Rect)positionProperty.GetValue(win, null);
                    return pos;
                }
            }

            throw new NotSupportedException("Can't find internal main window. Maybe something has changed inside Unity");
        }

        private static Type[] GetAllDerivedTypes(this AppDomain appDomain, Type aType)
        {
            var result = new List<Type>();
            var assemblies = appDomain.GetAssemblies();

            foreach (var assembly in assemblies)
            {
                var types = assembly.GetLoadableTypes();
                result.AddRange(types.Where(type => type.IsSubclassOf(aType)));
            }

            return result.ToArray();
        }

        /// <summary>
        /// Centers an editor window on the main display.
        /// </summary>
        public static void CenterOnMainWin(this EditorWindow window)
        {
            var main = GetEditorMainWindowPos();
            var pos = window.position;
            float w = (main.width - pos.width) * 0.5f;
            float h = (main.height - pos.height) * 0.5f;
            pos.x = main.x + w;
            pos.y = main.y + h;
            window.position = pos;
        }

        #region Handles

        /// <summary>
        /// Draw an axis move handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="origin">The initial position of the axis.</param>
        /// <param name="direction">The direction the axis is facing.</param>
        /// <param name="distance">Distance from the axis.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see cref="float"/> value.</returns>
        public static float AxisMoveHandle(Object target, Vector3 origin, Vector3 direction, float distance, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Vector3 position = origin + (direction.normalized * distance);

            Handles.color = HandleColorAxis;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Handles.DrawDottedLine(origin, position, DottedLineScreenSpace);
            Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(direction), handleSize * 2, EventType.Repaint);
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);

            if (recordUndo)
            {
                float newDistance = Vector3.Distance(origin, newPosition);

                if (!distance.Equals(newDistance))
                {
                    Undo.RegisterCompleteObjectUndo(target, target.name);
                    distance = newDistance;
                }
            }

            return distance;
        }

        /// <summary>
        /// Returns the default config profile, if it exists.
        /// </summary>
        public static MixedRealityToolkitConfigurationProfile GetDefaultConfigProfile()
        {
            var allConfigProfiles = ScriptableObjectExtensions.GetAllInstances<MixedRealityToolkitConfigurationProfile>();
            return GetDefaultConfigProfile(allConfigProfiles);
        }

        /// <summary>
        /// Given a list of MixedRealityToolkitConfigurationProfile objects, returns
        /// the one that matches the default profile name.
        /// </summary>
        public static MixedRealityToolkitConfigurationProfile GetDefaultConfigProfile(MixedRealityToolkitConfigurationProfile[] allProfiles)
        {
            for (int i = 0; i < allProfiles.Length; i++)
            {
                if (allProfiles[i].name == DefaultConfigProfileName)
                {
                    return allProfiles[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Draw a Circle Move Handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="xScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="yScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="zScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 CircleMoveHandle(Object target, Vector3 position, float xScale = 1f, float yScale = 1f, float zScale = 1f, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorCircle;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize, Vector3.zero, Handles.CircleHandleCap);

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// Draw a square move handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="xScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="yScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="zScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 SquareMoveHandle(Object target, Vector3 position, float xScale = 1f, float yScale = 1f, float zScale = 1f, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorSquare;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply square handle to match other types
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 0.8f, Vector3.zero, Handles.RectangleHandleCap);

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// Draw a sphere move handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="xScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="yScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="zScale">Scale the new value on the x axis by this amount.</param>
        /// <param name="handleSize">Optional handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 SphereMoveHandle(Object target, Vector3 position, float xScale = 1f, float yScale = 1f, float zScale = 1f, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorSphere;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Multiply sphere handle size to match other types
            Vector3 newPosition = Handles.FreeMoveHandle(position, Quaternion.identity, handleSize * 2, Vector3.zero, Handles.SphereHandleCap);

            if (recordUndo && position != newPosition)
            {
                Undo.RegisterCompleteObjectUndo(target, target.name);

                position.x = Mathf.Lerp(position.x, newPosition.x, Mathf.Clamp01(xScale));
                position.y = Mathf.Lerp(position.z, newPosition.y, Mathf.Clamp01(yScale));
                position.z = Mathf.Lerp(position.y, newPosition.z, Mathf.Clamp01(zScale));
            }

            return position;
        }

        /// <summary>
        /// Draw a vector handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="normalize">Optional, Normalize the new vector value.</param>
        /// <param name="clamp">Optional, Clamp new vector's value based on the distance to the origin.</param>
        /// <param name="handleLength">Optional, handle length.</param>
        /// <param name="handleSize">Optional, handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Vector3.html">Vector3</see> value.</returns>
        public static Vector3 VectorHandle(Object target, Vector3 origin, Vector3 vector, bool normalize = true, bool clamp = true, float handleLength = 1f, float handleSize = 0.1f, bool recordUndo = true, bool autoSize = true)
        {
            Handles.color = HandleColorTangent;

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
                Handles.color = Color.Lerp(Color.gray, HandleColorTangent, distanceToOrigin * 0.85f);

                if (clamp)
                {
                    // To indicate that we're at the clamped limit, make the handle 'pop' slightly larger
                    if (distanceToOrigin >= 0.98f)
                    {
                        Handles.color = Color.Lerp(HandleColorTangent, Color.white, 0.5f);
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

            if (recordUndo && handlePosition != newPosition)
            {
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

        /// <summary>
        /// Draw a rotation handle.
        /// </summary>
        /// <param name="target"><see href="https://docs.unity3d.com/ScriptReference/Object.html">Object</see> that is undergoing the transformation. Also used for recording undo.</param>
        /// <param name="position">The position to draw the handle.</param>
        /// <param name="rotation">The rotation to draw the handle.</param>
        /// <param name="handleSize">Optional, handle size.</param>
        /// <param name="autoSize">Optional, auto sizes the handles based on position and handle size.</param>
        /// <param name="recordUndo">Optional, records undo state.</param>
        /// <returns>The new <see href="https://docs.unity3d.com/ScriptReference/Quaternion.html">Quaternion</see> value.</returns>
        public static Quaternion RotationHandle(Object target, Vector3 position, Quaternion rotation, float handleSize = 0.2f, bool autoSize = true, bool recordUndo = true)
        {
            Handles.color = HandleColorRotation;

            if (autoSize)
            {
                handleSize = Mathf.Lerp(handleSize, HandleUtility.GetHandleSize(position) * handleSize, 0.75f);
            }

            // Make rotation handles larger so they can overlay movement handles
            Quaternion newRotation = Handles.FreeRotateHandle(rotation, position, handleSize * 2);

            if (recordUndo)
            {
                Handles.color = Handles.zAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.forward), handleSize * 2, EventType.Repaint);
                Handles.color = Handles.xAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.right), handleSize * 2, EventType.Repaint);
                Handles.color = Handles.yAxisColor;
                Handles.ArrowHandleCap(0, position, Quaternion.LookRotation(newRotation * Vector3.up), handleSize * 2, EventType.Repaint);

                if (rotation != newRotation)
                {
                    Undo.RegisterCompleteObjectUndo(target, target.name);
                    rotation = newRotation;
                }
            }

            return rotation;
        }

        #endregion Handles

        #region Profiles

        private static readonly GUIContent NewProfileContent = new GUIContent("+", "Create New Profile");
        private static Dictionary<Object, UnityEditor.Editor> profileEditorCache = new Dictionary<Object, UnityEditor.Editor>();

        /// <summary>
        /// Draws an editor for a profile object.
        /// </summary>
        public static void DrawSubProfileEditor(Object profileObject, bool renderProfileInBox)
        {
            if (profileObject == null)
            {
                return;
            }

            UnityEditor.Editor subProfileEditor = null;
            if (!profileEditorCache.TryGetValue(profileObject, out subProfileEditor))
            {
                subProfileEditor = UnityEditor.Editor.CreateEditor(profileObject);
                profileEditorCache.Add(profileObject, subProfileEditor);
            }

            // If this is a default MRTK configuration profile, ask it to render as a sub-profile
            if (typeof(BaseMixedRealityToolkitConfigurationProfileInspector).IsAssignableFrom(subProfileEditor.GetType()))
            {
                BaseMixedRealityToolkitConfigurationProfileInspector configProfile = (BaseMixedRealityToolkitConfigurationProfileInspector)subProfileEditor;
                configProfile.RenderAsSubProfile = true;
            }

            var subProfile = profileObject as BaseMixedRealityProfile;
            if (subProfile != null && !subProfile.IsCustomProfile)
            {
                string msg = MixedRealityProjectPreferences.LockProfiles ? CloneProfileHelpLockedLabel : CloneProfileHelpLabel;
                EditorGUILayout.HelpBox(msg, MessageType.Warning);
            }

            if (renderProfileInBox)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }

            EditorGUILayout.Space();
            subProfileEditor.OnInspectorGUI();
            EditorGUILayout.Space();

            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a dropdown with all available profiles of profilyType.
        /// </summary>
        /// <returns>True if property was changed.</returns>
        public static bool DrawProfileDropDownList(SerializedProperty property, BaseMixedRealityProfile profile, Object oldProfileObject, Type profileType, bool showAddButton)
        {
            bool changed = false;

            using (new EditorGUILayout.HorizontalScope())
            {
                // Pull profile instances and profile content from cache
                ScriptableObject[] profileInstances = MixedRealityProfileUtility.GetProfilesOfType(profileType);
                GUIContent[] profileContent = MixedRealityProfileUtility.GetProfilePopupOptionsByType(profileType);
                // Set our selected index to our '(None)' option by default
                int selectedIndex = 0;
                // Find our selected index
                for (int i = 0; i < profileInstances.Length; i++)
                {
                    if (profileInstances[i] == oldProfileObject)
                    {   // Our profile content has a '(None)' option at the start
                        selectedIndex = i + 1;
                        break;
                    }
                }

                int newIndex = EditorGUILayout.Popup(
                    new GUIContent(oldProfileObject != null ? "" : property.displayName),
                    selectedIndex,
                    profileContent,
                    GUILayout.ExpandWidth(true));

                property.objectReferenceValue = (newIndex > 0) ? profileInstances[newIndex - 1] : null;
                changed = property.objectReferenceValue != oldProfileObject;

                // Draw a button that finds the profile in the project window
                if (property.objectReferenceValue != null)
                {
                    // The view asset button should always be enabled.
                    using (new GUIEnabledWrapper())
                    {
                        if (GUILayout.Button("View Asset", EditorStyles.miniButton, GUILayout.Width(80)))
                        {
                            EditorGUIUtility.PingObject(property.objectReferenceValue);
                        }
                    }
                }

                // Draw the clone button
                if (property.objectReferenceValue == null)
                {
                    if (showAddButton && MixedRealityProfileUtility.IsConcreteProfileType(profileType))
                    {
                        if (GUILayout.Button(NewProfileContent, EditorStyles.miniButton, GUILayout.Width(20f)))
                        {
                            ScriptableObject instance = ScriptableObject.CreateInstance(profileType);
                            var newProfile = instance.CreateAsset(AssetDatabase.GetAssetPath(Selection.activeObject)) as BaseMixedRealityProfile;
                            property.objectReferenceValue = newProfile;
                            property.serializedObject.ApplyModifiedProperties();
                            changed = true;
                        }
                    }
                }
                else
                {
                    var renderedProfile = property.objectReferenceValue as BaseMixedRealityProfile;
                    Debug.Assert(renderedProfile != null);
                    if (GUILayout.Button(new GUIContent("Clone", "Replace with a copy of the default profile."), EditorStyles.miniButton, GUILayout.Width(45f)))
                    {
                        MixedRealityProfileCloneWindow.OpenWindow(profile, renderedProfile, property);
                    }
                }
            }

            return changed;
        }

        #endregion
    }
}
