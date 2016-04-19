// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;

namespace HoloToolkit.Unity
{
    [CustomEditor(typeof(UAudioMiniManager))]
    public class UAudioMiniManagerEditor : UAudioManagerBaseEditor<MiniAudioEvent>
    {
        private void OnEnable()
        {
            this.myTarget = (UAudioMiniManager)target;
            SetUpEditor();
        }

        public override void OnInspectorGUI()
        {
            DrawInspectorGUI(true);
        }
    }
}
