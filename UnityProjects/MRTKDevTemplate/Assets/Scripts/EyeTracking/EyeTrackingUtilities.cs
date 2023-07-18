// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Examples
{
    /// <summary>
    /// General useful utility functions.
    /// </summary>
    public static class EyeTrackingUtilities
    {
        /// <summary>
        /// Returns the metric size in meters of a given Vector3 in visual angle in degrees and a given viewing distance in meters.
        /// </summary>
        /// <param name="visAngleInDegrees">In degrees.</param>
        /// <param name="distInMeters">In meters.</param>
        public static Vector3 VisAngleInDegreesToMeters(Vector3 visAngleInDegrees, float distInMeters)
        {
            return new Vector3(
                VisAngleInDegreesToMeters(visAngleInDegrees.x, distInMeters),
                VisAngleInDegreesToMeters(visAngleInDegrees.y, distInMeters),
                VisAngleInDegreesToMeters(visAngleInDegrees.z, distInMeters));
        }

        /// <summary>
        /// Computes the metric size (in meters) for a given visual angle size.
        /// </summary>
        /// <param name="visAngleInDegrees">In degrees.</param>
        /// <param name="distInMeters">In meters.</param>
        public static float VisAngleInDegreesToMeters(float visAngleInDegrees, float distInMeters)
        {
            return 2f * Mathf.Tan(Mathf.Deg2Rad * visAngleInDegrees / 2f) * distInMeters;
        }

        /// <summary>
        /// Loads a Unity scene with the given name after a given delay in seconds.
        /// </summary>
        /// <param name="sceneToBeLoaded">Name of the scene to be loaded.</param>
        /// <param name="delayInSeconds">Delay in seconds to wait before loading the new scene.</param>
        public static IEnumerator LoadNewScene(string sceneToBeLoaded, float delayInSeconds)
        {
            yield return new WaitForSeconds(delayInSeconds);
            SceneManager.LoadScene(sceneToBeLoaded);
        }

        /// <summary>
        /// Change the color of game object "gameObject".
        /// </summary>
        /// <param name="originalColor">Enter "null" in case you're passing the original object and want to save the original color.</param>
        public static void GameObject_ChangeColor(GameObject gobj, Color newColor, ref Color? originalColor, bool onlyApplyToRootObj)
        {
            try
            {
                Renderer[] renderers = gobj.GetComponents<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    Material[] mats = renderers[i].materials;
                    foreach (Material mat in mats)
                    {
                        Color c = mat.color;

                        if (!originalColor.HasValue)
                            originalColor = c;

                        mat.color = newColor;
                    }
                }

                if (!onlyApplyToRootObj)
                {
                    renderers = gobj.GetComponentsInChildren<Renderer>();
                    for (int i = 0; i < renderers.Length; i++)
                    {
                        Material[] mats = renderers[i].materials;
                        foreach (Material mat in mats)
                        {
                            mat.color = newColor;
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Just ignore. Usually happens after the game object already got destroyed, but the update sequence had already been started.
            }
        }

        /// <summary>
        /// Change the transparency of game object "gameObject" with a transparency value between 0 and 1;
        /// </summary>
        public static void GameObject_ChangeTransparency(GameObject gameObject, float newTransparency)
        {
            float originalTransparency = 0; // just a dummy variable to reuse the following function
            GameObject_ChangeTransparency(gameObject, newTransparency, ref originalTransparency);
        }

        /// <summary>
        /// Change the transparency of game object "gameObject" with a transparency value between 0 and 255 with the option to 
        /// receive the original transparency value back.
        /// </summary>
        /// <param name="transparency">Expected values range from 0 (fully transparent) to 1 (fully opaque).</param>
        /// <param name="originalTransparency">Input "-1" if you don't know the original transparency yet.</param>
        public static void GameObject_ChangeTransparency(GameObject gameObject, float transparency, ref float originalTransparency)
        {
            try
            {
                // Go through renderers in main object
                Renderers_ChangeTransparency(gameObject.GetComponents<Renderer>(), transparency, ref originalTransparency);

                // Go through renderers in children objects
                Renderers_ChangeTransparency(gameObject.GetComponentsInChildren<Renderer>(), transparency, ref originalTransparency);
            }
            catch (System.Exception)
            {
                // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
            }
        }

        /// <summary>
        /// Change the transparency of a given array of renderers to a given transparency value between 0 and 255; 
        /// </summary>
        /// <param name="renderers">Array of renderers to apply a new transparency value to.</param>
        /// <param name="transparency">Value between 0 and 255.</param>
        /// <param name="originalTransparency">Option to return the original transparency value.</param>
        private static void Renderers_ChangeTransparency(Renderer[] renderers, float transparency, ref float originalTransparency)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                Material[] materials = renderers[i].materials;
                foreach (Material mat in materials)
                {
                    Color color = mat.color;

                    if (originalTransparency == -1f)
                        originalTransparency = color.a;

                    if (transparency < 1)
                    {
                        ChangeRenderMode.ChangeRenderModes(mat, ChangeRenderMode.BlendMode.Fade);
                    }
                    else
                    {
                        ChangeRenderMode.ChangeRenderModes(mat, ChangeRenderMode.BlendMode.Opaque);
                    }

                    mat.color = new Color(color.r, color.g, color.b, transparency);
                }
            }
        }
    }
}
