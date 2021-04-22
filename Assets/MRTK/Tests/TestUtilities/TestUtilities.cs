// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using Microsoft.MixedReality.Toolkit.Utilities.Editor;
using Microsoft.MixedReality.Toolkit.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

#if WINDOWS_UWP
using UnityEngine.Assertions;
#endif

namespace Microsoft.MixedReality.Toolkit.Tests
{
    public static class TestUtilities
    {
        const string primaryTestSceneTemporarySavePath = "Assets/__temp_primary_test_scene.unity";
        const string additiveTestSceneTemporarySavePath = "Assets/__temp_additive_test_scene_#.unity";
        public static Scene primaryTestScene;
        public static Scene[] additiveTestScenes = System.Array.Empty<Scene>();

        /// <summary>
        /// Destroys all scene assets that were created over the course of testing.
        /// Used only in editor tests.
        /// </summary>
        public static void EditorTearDownScenes()
        {
#if UNITY_EDITOR
            if (!EditorApplication.isPlaying)
            {
                // If any of our scenes were saved, tear down the assets
                SceneAsset primaryTestSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(primaryTestSceneTemporarySavePath);
                if (primaryTestSceneAsset != null)
                {
                    AssetDatabase.DeleteAsset(primaryTestSceneTemporarySavePath);
                }

                for (int i = 0; i < additiveTestScenes.Length; i++)
                {
                    string path = additiveTestSceneTemporarySavePath.Replace("#", i.ToString());
                    SceneAsset additiveTestSceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
                    if (additiveTestSceneAsset != null)
                    {
                        AssetDatabase.DeleteAsset(path);
                    }
                }
                AssetDatabase.Refresh();
            }
#endif
        }

        /// <summary>
        /// Creates a number of scenes and loads them additively for testing. Must create a minimum of 1.
        /// Used only in editor tests.
        /// </summary>
        public static void EditorCreateScenes(int numScenesToCreate = 1)
        {
            // Create default test scenes.
            // In the editor this can be done using EditorSceneManager with a default setup.
            // In playmode the scene needs to be set up manually.

#if UNITY_EDITOR
            Debug.Assert(!EditorApplication.isPlaying, "This method should only be called during edit mode tests. Use PlaymodeTestUtilities.");

            List<Scene> additiveTestScenesList = new List<Scene>();

            if (numScenesToCreate == 1)
            {   // No need to save this scene, we're just creating one
                primaryTestScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
            }
            else
            {
                // Make the first scene single so it blows away previously loaded scenes
                primaryTestScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                // Save the scene (temporarily) so we can load additively on top of it
                EditorSceneManager.SaveScene(primaryTestScene, primaryTestSceneTemporarySavePath);

                for (int i = 1; i < numScenesToCreate; i++)
                {
                    string path = additiveTestSceneTemporarySavePath.Replace("#", additiveTestScenesList.Count.ToString());
                    // Create subsequent scenes additively
                    Scene additiveScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);
                    additiveTestScenesList.Add(additiveScene);
                    // Save the scene (temporarily) so we can load additively on top of it
                    EditorSceneManager.SaveScene(additiveScene, path);
                }
            }

            additiveTestScenes = additiveTestScenesList.ToArray();
#endif
        }

        /// <summary>
        /// Pose to create MRTK playspace's parent transform at.
        /// </summary>
        public static Pose ArbitraryParentPose { get; set; } = new Pose(new Vector3(-2.0f, 1.0f, -3.0f), Quaternion.Euler(-30.0f, -90.0f, 0.0f));

        /// <summary>
        /// Pose to set playspace at, when using <see cref="PlayspaceToArbitraryPose"/>. 
        /// </summary>
        public static Pose ArbitraryPlayspacePose { get; set; } = new Pose(new Vector3(6.0f, 2.0f, 8.0f), Quaternion.Euler(10.0f, 120.0f, 15.0f));

        /// <summary>
        /// Creates a playspace and moves it into a default position.
        /// </summary>
        public static void InitializePlayspace()
        {
            if (MixedRealityPlayspace.Transform.parent == null)
            {
                GameObject gameObject = new GameObject("MRTKPlayspaceTestParent");
                MixedRealityPlayspace.Transform.parent = gameObject.transform;
                gameObject.transform.position = ArbitraryParentPose.position;
                gameObject.transform.rotation = ArbitraryParentPose.rotation;
            }
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = new Vector3(1.0f, 1.5f, -2.0f);
                p.LookAt(Vector3.zero);
            });
        }

        /// <summary>
        /// Forces the playspace camera to origin facing forward along +Z.
        /// </summary>
        public static void PlayspaceToOriginLookingForward()
        {
            // Move the camera to origin looking at +z to more easily see the target at 0,0,+Z
            PlayspaceToPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        /// <summary>
        /// Force the playspace camera into the specified position and orientation.
        /// </summary>
        /// <param name="position">World space position for the playspace.</param>
        /// <param name="rotation">World space orientation for the playspace.</param>
        /// <remarks>
        /// <para>Note that this has no effect on the camera's local space transform, but
        /// will change the camera's world space position. If and only if the camera's
        /// local transform is identity with the camera's world transform equal the playspace's.</para>
        /// </remarks>
        public static void PlayspaceToPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            MixedRealityPlayspace.PerformTransformation(
            p =>
            {
                p.position = position;
                p.rotation = rotation;
            });
        }

        /// <summary>
        /// Set the playspace to an arbitrary (but known) non-identity pose.
        /// </summary>
        /// <remarks>
        /// <para>When using this arbitrary pose imposed on the playspace to better validate compliance with
        /// real world scenarios, it can be convenient to use the *RelativeToPlayspace() helpers below.
        /// For example, to place an object directly 8 meters in front of the camera, set its position
        /// to TestUtilities.PositionRelativeToPlayspace(0.0f, 0.0f, 8.0f).
        /// See also <see cref="PlaceRelativeToPlayspace(Transform)"/> to convert an object's local
        /// transform into a transform relative to the playspace.</para>
        /// </remarks>
        public static void PlayspaceToArbitraryPose()
        {
            PlayspaceToPositionAndRotation(ArbitraryPlayspacePose.position, ArbitraryPlayspacePose.rotation);
        }

        /// <summary>
        /// Concatenate two transform poses.
        /// </summary>
        /// <param name="lhs">The pose to prepend.</param>
        /// <param name="rhs">The pose to append.</param>
        /// <returns>The concatenated pose.</returns>
        private static Pose Multiply(Pose lhs, Pose rhs)
        {
            return new Pose(lhs.position + lhs.rotation * rhs.position, lhs.rotation * rhs.rotation);
        }

        /// <summary>
        /// Compute the world position corresponding to the input local position in playspace.
        /// </summary>
        /// <param name="localPosition">The local position.</param>
        /// <returns>The world position.</returns>
        public static Vector3 PositionRelativeToPlayspace(Vector3 localPosition)
        {
            return MixedRealityPlayspace.TransformPoint(localPosition);
        }

        /// <summary>
        /// Compute the world direction corresponding to the input local direction in playspace.
        /// </summary>
        /// <param name="localDirection">The local direction.</param>
        /// <returns>The world direction.</returns>
        public static Vector3 DirectionRelativeToPlayspace(Vector3 localDirection)
        {
            return MixedRealityPlayspace.TransformDirection(localDirection);
        }

        /// <summary>
        /// Compute the world rotation corresponding to the input local rotation in playspace.
        /// </summary>
        /// <param name="localRotation">The rotation in local space.</param>
        /// <returns>The rotation in world space.</returns>
        public static Quaternion RotationRelativeToPlayspace(Quaternion localRotation)
        {
            return MixedRealityPlayspace.Rotation * localRotation;
        }

        /// <summary>
        /// Return a pose equivalent to the input local pose appended to the current playspace pose.
        /// </summary>
        /// <param name="localPosition">Position relative to playspace.</param>
        /// <param name="localRotation">Orientation relative to playspace.</param>
        /// <returns>Equivalent pose.</returns>
        /// <remarks>
        /// <para>This computes the world pose an object with the input local pose would have if it were
        /// a child of the playspace. </para>
        /// </remarks>
        public static Pose PlaceRelativeToPlayspace(Vector3 localPosition, Quaternion localRotation)
        {
            var playspaceTransform = MixedRealityPlayspace.Transform;
            Pose worldFromPlayspace = new Pose(playspaceTransform.position, playspaceTransform.rotation);
            Pose playspaceFromLocal = new Pose(localPosition, localRotation);
            Pose worldFromLocal = Multiply(worldFromPlayspace, playspaceFromLocal);
            return worldFromLocal;
        }

        /// <summary>
        /// Place the transform in world space as if it had the input local pose and was attached to the playspace.
        /// </summary>
        /// <param name="transform">The transform to place.</param>
        /// <param name="localPosition">The local position in playspace.</param>
        /// <param name="localRotation">The local rotation in playspace.</param>
        /// <remarks>
        /// Note that the transform's current local and world poses are ignored and overwritten.
        /// </remarks>
        public static void PlaceRelativeToPlayspace(Transform transform, Vector3 localPosition, Quaternion localRotation)
        {
            Pose worldFromLocal = PlaceRelativeToPlayspace(localPosition, localRotation);
            transform.position = worldFromLocal.position;
            transform.rotation = worldFromLocal.rotation;
        }

        /// <summary>
        /// Place the transform in world space as if its current world pose were its local pose and it was attached to the playspace.
        /// </summary>
        /// <param name="transform">The transform to place.</param>
        /// <remarks>
        /// <para>If the transform has no parent, then this is equivalent to the following sequence:</para>
        /// <para>1) transform.SetParent(MixedRealityPlayspace.Transform, false);</para>
        /// <para>2) transform.SetParent(null, true);</para>
        /// <para>However, if the transform has a parent, then the transform's world transform, not just its local transform,
        /// will determine its final pose relative to the playspace.</para>
        /// </remarks>
        public static void PlaceRelativeToPlayspace(Transform transform)
        {
            PlaceRelativeToPlayspace(transform, transform.position, transform.rotation);
        }

        /// <summary>
        /// Creates the requested number of scenes, then creates one instance of the MixedRealityToolkit in the active scene.
        /// </summary>
        public static void InitializeMixedRealityToolkitAndCreateScenes(bool useDefaultProfile = false, int numScenesToCreate = 1)
        {
            // Setup
            EditorCreateScenes(numScenesToCreate);
            InitializeMixedRealityToolkit(useDefaultProfile);
        }

        public static void InitializeCamera()
        {
            Camera[] cameras = GameObject.FindObjectsOfType<Camera>();

            if (cameras.Length == 0)
            {
                new GameObject("Main Camera", typeof(Camera), typeof(AudioListener)) { tag = "MainCamera" }.GetComponent<Camera>();
            }
        }

        public static void InitializeMixedRealityToolkit(MixedRealityToolkitConfigurationProfile configuration)
        {
            InitializeCamera();
#if UNITY_EDITOR
            MixedRealityInspectorUtility.AddMixedRealityToolkitToScene(configuration, true);
#endif

            // Todo: this condition shouldn't be here.
            // It's here due to some edit mode tests initializing MRTK instance in Edit mode, causing some of 
            // event handler registration to live over tests and cause next tests to fail.
            // Exact reason requires investigation.
            if (Application.isPlaying)
            {
                BaseEventSystem.enableDanglingHandlerDiagnostics = true;
            }

            Debug.Assert(MixedRealityToolkit.IsInitialized);
            Debug.Assert(MixedRealityToolkit.Instance != null);

            Debug.Assert(MixedRealityToolkit.Instance.ActiveProfile != null);
        }

        public static void InitializeMixedRealityToolkit(bool useDefaultProfile = false)
        {
            var configuration = useDefaultProfile
                ? GetDefaultMixedRealityProfile<MixedRealityToolkitConfigurationProfile>()
                : ScriptableObject.CreateInstance<MixedRealityToolkitConfigurationProfile>();

            Debug.Assert(configuration != null, "Failed to find the Default Mixed Reality Configuration Profile");
            InitializeMixedRealityToolkit(configuration);
        }

        public static void ShutdownMixedRealityToolkit()
        {
            MixedRealityToolkit.SetInstanceInactive(MixedRealityToolkit.Instance);
            if (Application.isPlaying)
            {
                MixedRealityPlayspace.Destroy();
            }

            BaseEventSystem.enableDanglingHandlerDiagnostics = false;
        }

        public static T GetDefaultMixedRealityProfile<T>() where T : BaseMixedRealityProfile
        {
#if UNITY_EDITOR
            return ScriptableObjectExtensions.GetAllInstances<T>().FirstOrDefault(profile => profile.name.Equals($"Default{typeof(T).Name}"));
#else
            return ScriptableObject.CreateInstance<T>();
#endif
        }

        public static void AssertAboutEqual(Vector3 actual, Vector3 expected, string message, float tolerance = 0.01f)
        {
            var dist = (actual - expected).magnitude;
            Debug.Assert(dist < tolerance, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}");
        }

        public static void AssertAboutEqual(Quaternion actual, Quaternion expected, string message, float tolerance = 0.01f)
        {
            var angle = Quaternion.Angle(actual, expected);
            Debug.Assert(angle < tolerance, $"{message}, expected {expected.ToString("0.000")}, was {actual.ToString("0.000")}");
        }

        public static void AssertNotAboutEqual(Vector3 val1, Vector3 val2, string message, float tolerance = 0.01f)
        {
            var dist = (val1 - val2).magnitude;
            Debug.Assert(dist >= tolerance, $"{message}, val1 {val1.ToString("0.000")} almost equals val2 {val2.ToString("0.000")}");
        }

        public static void AssertNotAboutEqual(Quaternion val1, Quaternion val2, string message, float tolerance = 0.01f)
        {
            var angle = Quaternion.Angle(val1, val2);
            Debug.Assert(angle >= tolerance, $"{message}, val1 {val1.ToString("0.000")} almost equals val2 {val2.ToString("0.000")}");
        }

        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.LessOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like LessThanOrEqual(2.00000024, 2.0) to still pass.
        /// </remarks>
        public static void AssertLessOrEqual(float observed, float expected, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed < expected));
        }

        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.LessOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like LessThanOrEqual(2.00000024, 2.0) to still pass.
        /// </remarks>
        public static void AssertLessOrEqual(float observed, float expected, string message, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed < expected), message);
        }

        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.GreaterOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like GreaterThanOrEqual(1.999999999, 2.0) to still pass.
        /// </remarks>
        public static void AssertGreaterOrEqual(float observed, float expected, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed > expected));
        }
        /// <summary>
        /// Equivalent to NUnit.Framework.Assert.GreaterOrEqual, except this also
        /// applies a slight tolerance on the equality check.
        /// </summary>
        /// <remarks>
        /// This allows for things like GreaterThanOrEqual(1.999999999, 2.0) to still pass.
        /// </remarks>
        public static void AssertGreaterOrEqual(float observed, float expected, string message, float tolerance = 0.01f)
        {
            Debug.Assert((Mathf.Abs(observed - expected) <= tolerance) || (observed > expected), message);
        }

#if UNITY_EDITOR
        [MenuItem("Mixed Reality/Toolkit/Utilities/Update/Icons/Tests")]
        private static void UpdateTestScriptIcons()
        {
            Texture2D icon = null;

            foreach (string iconPath in MixedRealityToolkitFiles.GetFiles(MixedRealityToolkitModuleType.StandardAssets, "Icons"))
            {
                if (iconPath.EndsWith("test_icon.png"))
                {
                    icon = AssetDatabase.LoadAssetAtPath<Texture2D>(iconPath);
                    break;
                }
            }

            if (icon == null)
            {
                Debug.Log("Couldn't find test icon.");
                return;
            }

            IEnumerable<string> testDirectories = MixedRealityToolkitFiles.GetDirectories(MixedRealityToolkitModuleType.Tests);

            foreach (string directory in testDirectories)
            {
                string[] scriptGuids = AssetDatabase.FindAssets("t:MonoScript", new string[] { MixedRealityToolkitFiles.GetAssetDatabasePath(directory) });

                for (int i = 0; i < scriptGuids.Length; i++)
                {
                    string scriptPath = AssetDatabase.GUIDToAssetPath(scriptGuids[i]);

                    EditorUtility.DisplayProgressBar("Updating Icons...", $"{i} of {scriptGuids.Length} {scriptPath}", i / (float)scriptGuids.Length);

                    MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(scriptPath);

                    Texture2D currentIcon = getIconForObject?.Invoke(null, new object[] { script }) as Texture2D;
                    if (currentIcon == null || !currentIcon.Equals(icon))
                    {
                        setIconForObject?.Invoke(null, new object[] { script, icon });
                        copyMonoScriptIconToImporters?.Invoke(null, new object[] { script });
                    }
                }
            }

            EditorUtility.ClearProgressBar();
        }

        private static readonly MethodInfo getIconForObject = typeof(EditorGUIUtility).GetMethod("GetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo setIconForObject = typeof(EditorGUIUtility).GetMethod("SetIconForObject", BindingFlags.Static | BindingFlags.NonPublic);
        private static readonly MethodInfo copyMonoScriptIconToImporters = typeof(MonoImporter).GetMethod("CopyMonoScriptIconToImporters", BindingFlags.Static | BindingFlags.NonPublic);
#endif
    }
}
