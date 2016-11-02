using UnityEngine;
using System.Collections;

namespace HoloToolkit.Unity.InputModule
{
    /// <summary>
    /// Cursor Modifier Interface returning basic overrides.
    /// </summary>
    public interface ICursorModifier
    {
        void RegisterCursor(ICursor cursor);
        bool GetCursorVisibility();
        Vector3 GetPosition();
        Quaternion GetRotation();
        Vector3 GetScale();
        void GetModifierTranslation(out Vector3 position, out Quaternion rotation, out Vector3 scale);
    }
}