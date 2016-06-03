using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    /// <summary>
    /// Contains menu items for building HoloLens projects
    /// </summary>
    public static class BuildMenu
    {
        /// <summary>
        /// Builds the project configured for the HoloLens and on success optionally opens the output solution.
        /// </summary>
        [MenuItem("HoloToolkit/Builds/Build For HoloLens")]
        public static void BuildHoloLens()
        {
            string error = BuildCommands.BuildForHololens();

            if (string.IsNullOrEmpty(error))
            {
                bool openSln = EditorUtility.DisplayDialog("Successful Build!", "Would you like to open the solution?", "Yes, Open", "No");
                if (openSln)
                    OpenSolution();
            }
        }

        /// <summary>
        /// Checks if a project solution at the BuildCommands.BuildLocation exists, and if so opens it. Otherwise
        /// will ask if you'd like to build.
        /// </summary>
        [MenuItem("HoloToolkit/Builds/Open Visual Studio Solution")]
        public static void OpenSolution()
        {
            FileInfo solution = new FileInfo(Path.Combine(BuildCommands.BuildLocation, PlayerSettings.productName + ".sln"));
            if (solution.Exists)
            {
                System.Diagnostics.Process.Start(solution.FullName);
            }
            else
            {
                bool buildNow = EditorUtility.DisplayDialog("Solution Not Found", "We couldn't find the solution. Would you like to Build?", "Yes, Build", "No");
                if (buildNow)
                    BuildHoloLens();
            }
        }
    }
}