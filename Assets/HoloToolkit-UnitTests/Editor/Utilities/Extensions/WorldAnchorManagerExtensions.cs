using System;
using UnityEngine;

namespace HoloToolkit.Unity.Tests.Extensions
{
    /// <summary>
    /// This class extends the world anchor manager with methods constrained to test logic.
    /// </summary>
    public static class WorldAnchorManagerExtensions
    {
        internal static string GenerateAnchorName(this WorldAnchorManager anchorManager, GameObject gameObjectToAnchor, string anchorName = null)
        {
            return string.IsNullOrEmpty(anchorName) ? gameObjectToAnchor.name : anchorName;
        }
    }
}