// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Microsoft.MixedReality.Toolkit.Examples.Demos.EyeTracking
{
    /// <summary>
    /// General useful utility functions.
    /// </summary>
    public static class EyeTrackingDemoUtils
    {
        /// <summary>
        /// Returns a correctly formatted filename removing invalid characters if necessary.
        /// </summary>
        /// <param name="unvalidatedFilename">The unvalidated filename</param>
        /// <returns>Validated filename.</returns>
        public static string GetValidFilename(string unvalidatedFilename)
        {
            char[] invalidChars = new char[] { '/', ':', '*', '?', '"', '<', '>', '|', '\\' };
            string formattedFilename = unvalidatedFilename;

            foreach (char invalidChar in invalidChars)
            {
                formattedFilename = formattedFilename.Replace((invalidChar.ToString()), "");
            }

            return formattedFilename;
        }

        /// <summary>
        /// Returns the full name of a given GameObject in the scene graph.
        /// </summary>
        public static string GetFullName(GameObject go)
        {
            bool valid;
            return GetFullName(go, out valid);
        }

        /// <summary>
        /// Returns the full name of a given GameObject in the scene graph.
        /// </summary>
        public static string GetFullName(GameObject go, out bool valid)
        {
            valid = false;
            if (go != null)
            {
                string goName = go.name;
                Transform g = go.transform;
                while (g.parent != null)
                {
                    goName = g.parent.name + "\\" + goName;
                    g = g.transform.parent;
                }
                valid = true;
                return goName;
            }
            return "";
        }

        /// <summary>
        /// Normalize the given value based on the provided min and max values.
        /// </summary>
        public static float Normalize(float value, float min, float max)
        {
            if (value > max)
                return 1;
            if (value < min)
                return 0;

            return (value - min) / (max - min);
        }

        /// <summary>
        /// Shuffles the entries in a given array and returns the shuffled array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public static T[] RandomizeListOrder<T>(T[] array)
        {
            T[] arr = (T[])array.Clone();

            for (int i = 0; i < arr.Length; i++)
            {
                T temp = arr[i];
                int randomIndex = UnityEngine.Random.Range(i, arr.Length);
                arr[i] = arr[randomIndex];
                arr[randomIndex] = temp;
            }
            return arr;
        }

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
            return (2 * Mathf.Tan(Mathf.Deg2Rad * visAngleInDegrees / 2) * distInMeters);
        }

        /// <summary>
        /// Loads a Unity scene with the given name.
        /// </summary>
        /// <param name="sceneToBeLoaded">Name of the scene to be loaded.</param>
        public static IEnumerator LoadNewScene(string sceneToBeLoaded)
        {
            return LoadNewScene(sceneToBeLoaded, 0.5f);
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
        /// Change the color of game object "gobj".
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
        /// Change the transparency of game object "gobj" with a transparency value between 0 and 1;
        /// </summary>
        public static void GameObject_ChangeTransparency(GameObject gobj, float newTransparency)
        {
            float origTransp = 0; // just a dummy variable to reuse the following function
            GameObject_ChangeTransparency(gobj, newTransparency, ref origTransp);
        }

        /// <summary>
        /// Change the transparency of game object "gobj" with a transparency value between 0 and 255 with the option to 
        /// receive the original transparency value back.
        /// </summary>
        /// <param name="transparency">Expected values range from 0 (fully transparent) to 1 (fully opaque).</param>
        /// <param name="originalTransparency">Input "-1" if you don't know the original transparency yet.</param>
        public static void GameObject_ChangeTransparency(GameObject gobj, float transparency, ref float originalTransparency)
        {
            try
            {
                // Go through renderers in main object
                Renderers_ChangeTransparency(gobj.GetComponents<Renderer>(), transparency, ref originalTransparency);

                // Go through renderers in children objects
                Renderers_ChangeTransparency(gobj.GetComponentsInChildren<Renderer>(), transparency, ref originalTransparency);
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
                Material[] mats = renderers[i].materials;
                foreach (Material mat in mats)
                {
                    Color c = mat.color;

                    if (originalTransparency == -1)
                        originalTransparency = c.a;

                    if (transparency < 1)
                    {
                        ChangeRenderMode.ChangeRenderModes(mat, ChangeRenderMode.BlendMode.Fade);
                    }
                    else
                    {
                        ChangeRenderMode.ChangeRenderModes(mat, ChangeRenderMode.BlendMode.Opaque);
                    }

                    mat.color = new Color(c.r, c.g, c.b, transparency);
                }
            }
        }
    }
}
