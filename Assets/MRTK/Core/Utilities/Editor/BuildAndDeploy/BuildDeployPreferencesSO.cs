using System;
using System.IO;
using UnityEngine;

[CreateAssetMenu(fileName = "SavedBuildDeployPreferences", menuName = "BuildDeployPreferences")]
public class BuildDeployPreferencesSO : ScriptableObject
{
    /// <summary>
    /// The Build Directory that the Mixed Reality Toolkit will build to.
    /// </summary>
    /// <remarks>
    /// This is a root build folder path. Each platform build will be put into a child directory with the name of the current active build target.
    /// </remarks>
    public string BuildDirectory = "Builds";

    /// <summary>
    /// Current setting to increment build visioning.
    /// </summary>
    public bool IncrementBuildVersion = false;

    public string LiveCubeModelLocation;
}