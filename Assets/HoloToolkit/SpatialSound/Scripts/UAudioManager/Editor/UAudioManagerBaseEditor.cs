// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEditor;
using UnityEngine;

namespace HoloToolkit.Unity
{
    public class UAudioManagerBaseEditor<TEvent, TBank> : Editor where TEvent : AudioEvent, new() where TBank : AudioBank<TEvent>, new()
    {
        protected UAudioManagerBase<TEvent, TBank> MyTarget;

        public void DrawExportToAsset()
        { 
            // Don't show this if there are no embedded events in this audio manager
            if (MyTarget.EditorEvents == null || MyTarget.EditorEvents.Length == 0)
                return;

            EditorGUILayout.HelpBox("Audio events have moved to an asset system.\nThey are no longer stored in the scene.\nPlease export these events to an event bank.", MessageType.Info, true);

            if (GUILayout.Button("Convert To Bank"))
            {
                TBank bank = ScriptableObject.CreateInstance<TBank>();

                bank.Events = MyTarget.EditorEvents;

                // Create the Asset
                AssetDatabase.CreateAsset(bank, "Assets/ConvertedAudioBank.asset");
                AssetDatabase.SaveAssets();

                // Remove the events from the manager
                this.serializedObject.Update();
                var eventsProperty = this.serializedObject.FindProperty("Events");
                eventsProperty.ClearArray();

                // Add the Asset to the Default Banks
                var defaultBanksProperty = this.serializedObject.FindProperty("DefaultBanks");
                defaultBanksProperty.InsertArrayElementAtIndex(defaultBanksProperty.arraySize);
                var newEntry = defaultBanksProperty.FindPropertyRelative("Array.data[" + (defaultBanksProperty.arraySize - 1) + "]");
                newEntry.objectReferenceValue = bank;

                this.serializedObject.ApplyModifiedProperties();
            }
        }

        public void DrawBankList()
        {
            this.serializedObject.Update();
            var bankList = this.serializedObject.FindProperty("DefaultBanks");

            EditorGUILayout.PropertyField(bankList, true);

            this.serializedObject.ApplyModifiedProperties();
        }

        protected void SetUpEditor()
        {
            if (MyTarget.DefaultBanks == null)
            {
                MyTarget.DefaultBanks = new TBank[0];
            }
        }

    }
}