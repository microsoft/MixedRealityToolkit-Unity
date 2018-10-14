using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    public abstract class BaseMixedRealityInspector : Editor
    {
        /// <summary>
        /// Check and make sure we have a Mixed Reality Manager and an active profile.
        /// </summary>
        /// <returns>True if the Mixed Reality Manager is properly initialized.</returns>
        protected bool CheckMixedRealityManager(bool showHelpBox = true)
        {
            if (!MixedRealityOrchestrator.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityOrchestrator>();

                if (managerSearch.Length == 0)
                {
                    if (showHelpBox)
                    {
                        EditorGUILayout.HelpBox("No Mixed Reality Manager found in scene.", MessageType.Error);
                    }
                    return false;
                }

                MixedRealityOrchestrator.ConfirmInitialized();
            }

            if (!MixedRealityOrchestrator.HasActiveProfile)
            {
                if (showHelpBox)
                {
                    EditorGUILayout.HelpBox("No Active Profile set on the Mixed Reality Manager.", MessageType.Error);
                }
                return false;
            }

            return true;
        }
    }
}