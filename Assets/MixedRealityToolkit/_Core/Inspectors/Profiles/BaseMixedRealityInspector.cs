using Microsoft.MixedReality.Toolkit.Core.Managers;
using UnityEditor;

namespace Microsoft.MixedReality.Toolkit.Core.Inspectors.Profiles
{
    public abstract class BaseMixedRealityInspector : Editor
    {
        /// <summary>
        /// Check and make sure we have a Mixed Reality Orchestrator and an active profile.
        /// </summary>
        /// <returns>True if the Mixed Reality Orchestrator is properly initialized.</returns>
        protected static bool CheckMixedRealityConfigured(bool showHelpBox = true)
        {
            if (!MixedRealityOrchestrator.IsInitialized)
            {
                // Search the scene for one, in case we've just hot reloaded the assembly.
                var managerSearch = FindObjectsOfType<MixedRealityOrchestrator>();

                if (managerSearch.Length == 0)
                {
                    if (showHelpBox)
                    {
                        EditorGUILayout.HelpBox("No Mixed Reality Orchestrator found in scene.", MessageType.Error);
                    }

                    return false;
                }

                MixedRealityOrchestrator.ConfirmInitialized();
            }

            if (!MixedRealityOrchestrator.HasActiveProfile)
            {
                if (showHelpBox)
                {
                    EditorGUILayout.HelpBox("No Active Profile set on the Mixed Reality Orchestrator.", MessageType.Error);
                }

                return false;
            }

            return true;
        }
    }
}