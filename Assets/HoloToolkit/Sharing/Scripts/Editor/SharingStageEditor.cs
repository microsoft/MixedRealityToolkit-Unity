//
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.
//

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing
{
    [CustomEditor(typeof(SharingStage))]
    public class SharingStageEditor : Editor
    {
        private Dictionary<object, bool> foldoutGUIMap = new Dictionary<object, bool>();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                SharingStage networkManager = (SharingStage)target;

                SyncRoot root = networkManager.Root;

                if (root != null)
                {
                    EditorGUILayout.Separator();
                    EditorGUILayout.BeginVertical();
                    {
                        EditorGUILayout.LabelField("Object Hierarchy");
                        DrawDataModelGUI(root, string.Empty);
                    }
                    EditorGUILayout.EndVertical();
                }

                // Force this inspector to repaint every frame so that it reflects changes to the undo stack
                // immediately rather than showing stale data until the user clicks on the inspector window
                Repaint();
            }
        }

        private void DrawDataModelGUI(SyncPrimitive syncPrimitive, string parentPath)
        {
            string fieldName = syncPrimitive.FieldName;
            object rawValue = syncPrimitive.RawValue;

            SyncObject syncObject = syncPrimitive as SyncObject;

            if (syncObject != null)
            {
                bool foldout = false;
                if (foldoutGUIMap.ContainsKey(syncObject))
                {
                    foldout = foldoutGUIMap[syncObject];
                }

                int ownerId = syncObject.OwnerId;
                string owner = ownerId == int.MaxValue ? string.Empty : ownerId.ToString();
                string objectType = syncObject.ObjectType;

                foldout = EditorGUILayout.Foldout(foldout, string.Format("{0} (Owner:{1} Type:{2})", fieldName, owner, objectType));
                foldoutGUIMap[syncObject] = foldout;

                if (foldout)
                {
                    EditorGUI.indentLevel++;

                    SyncPrimitive[] children = syncObject.GetChildren();
                    for (int i = 0; i < children.Length; i++)
                    {
                        DrawDataModelGUI(children[i], parentPath + "/" + fieldName);
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(fieldName, GUILayout.MaxWidth(125));
                EditorGUILayout.LabelField(rawValue != null ? rawValue.ToString() : "null", GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField(syncPrimitive.NetworkElement != null ? "live" : "local", GUILayout.MaxWidth(50));
                if (syncPrimitive.NetworkElement != null)
                {
                    EditorGUILayout.LabelField(parentPath + "/" + fieldName, GUILayout.MaxWidth(500));
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
