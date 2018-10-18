using Microsoft.MixedReality.Toolkit.Core.Services;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    public abstract class BaseMixedRealityInspector : Editor
    {
        /// <summary>
        /// Check and make sure we have a Mixed Reality Toolkit and an active profile.
        /// </summary>
        /// <returns>True if the Mixed Reality Toolkit is properly initialized.</returns>
        protected static bool CheckMixedRealityConfigured(bool showHelpBox = true)
        {
            if (!MixedRealityToolkit.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityToolkit>();

                if (managerSearch.Length == 0)
                {
                    if (showHelpBox)
                    {
                        EditorGUILayout.HelpBox("No Mixed Reality Toolkit found in scene.", MessageType.Error);
                    }

                    return false;
                }

                MixedRealityToolkit.ConfirmInitialized();
            }

            if (!MixedRealityToolkit.HasActiveProfile)
            {
                if (showHelpBox)
                {
                    EditorGUILayout.HelpBox("No Active Profile set on the Mixed Reality Toolkit.", MessageType.Error);
                }

                return false;
            }

            return true;
        }
    }
}