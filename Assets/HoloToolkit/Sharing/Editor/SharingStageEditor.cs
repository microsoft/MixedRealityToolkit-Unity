//
// Copyright (C) Microsoft. All rights reserved.
// TODO This needs to be validated for HoloToolkit integration
//

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

namespace HoloToolkit.Sharing
{
    [CustomEditor(typeof (SharingStage))]
    public class SharingStageEditor : Editor
    {
        private Dictionary<object, bool> foldoutGUIMap = new Dictionary<object, bool>();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (Application.isPlaying)
            {
                SharingStage networkManager = target as SharingStage;

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
            string name = syncPrimitive.FieldName;
            object value = syncPrimitive.RawValue;

            SyncObject SyncObject = syncPrimitive as SyncObject;

            if (SyncObject != null)
            {
                bool foldout = false;
                if (foldoutGUIMap.ContainsKey(SyncObject))
                {
                    foldout = foldoutGUIMap[SyncObject];
                }

                int ownerId = SyncObject.OwnerId;
                string owner = ownerId == int.MaxValue ? string.Empty : ownerId.ToString();
                string objectType = SyncObject.ObjectType;

                foldout = EditorGUILayout.Foldout(foldout, string.Format("{0} (Owner:{1} Type:{2})", name, owner, objectType));
                foldoutGUIMap[SyncObject] = foldout;

                if (foldout)
                {
                    EditorGUI.indentLevel++;

                    SyncPrimitive[] children = SyncObject.GetChildren();
                    for (int i = 0; i < children.Length; i++)
                    {
                        DrawDataModelGUI(children[i], parentPath + "/" + name);
                    }

                    EditorGUI.indentLevel--;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(name, GUILayout.MaxWidth(125));
                EditorGUILayout.LabelField(value != null ? value.ToString() : "null", GUILayout.MaxWidth(200));
                EditorGUILayout.LabelField(syncPrimitive.NetworkElement != null ? "live" : "local",
                    GUILayout.MaxWidth(50));
                if (syncPrimitive.NetworkElement != null)
                {
                    EditorGUILayout.LabelField(parentPath + "/" + name, GUILayout.MaxWidth(500));
                }

                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
