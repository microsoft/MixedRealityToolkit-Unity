// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using MixedRealityToolkit.Utilities.Inspectors.EditorScript;
using UnityEditor;

namespace MixedRealityToolkit.InputModule.Pointers 
{
    [CustomEditor(typeof(BaseControllerPointer))]
    public class BaseControllerPointerEditor : MRTKEditor { }

    [CustomEditor(typeof(LinePointer))]
    public class LinePointerEditor : BaseControllerPointerEditor { }

    [CustomEditor(typeof(TeleportPointer))]
    public class TeleportPointerEditor : LinePointerEditor { }

    [CustomEditor(typeof(ParabolicTeleportPointer))]
    public class ParabolicTeleportPointerEditor : TeleportPointerEditor { }
}