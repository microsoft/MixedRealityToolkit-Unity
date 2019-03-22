// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the TransformView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEditor;
    using UnityEngine;


    [CustomEditor(typeof(PhotonTransformView))]
    public class PhotonTransformViewEditor : Editor
    {
        private bool helpToggle = false;

        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Editing is disabled in play mode.", MessageType.Info);
                return;
            }

            PhotonTransformView view = (PhotonTransformView)target;


            EditorGUILayout.LabelField("Synchronize Options");

            EditorGUI.indentLevel += 2;
            view.m_SynchronizePosition = EditorGUILayout.ToggleLeft(" Position", view.m_SynchronizePosition);
            view.m_SynchronizeRotation = EditorGUILayout.ToggleLeft(" Rotation", view.m_SynchronizeRotation);
            view.m_SynchronizeScale = EditorGUILayout.ToggleLeft(" Scale", view.m_SynchronizeScale);
            EditorGUI.indentLevel -= 2;


            this.helpToggle = EditorGUILayout.Foldout(this.helpToggle, "Info");
            if (this.helpToggle)
            {
                EditorGUI.indentLevel += 1;
                EditorGUILayout.HelpBox("The Photon Transform View of PUN 2 is simple by design.\nReplace it with the Photon Transform View Classic if you want the old options.\nThe best solution is a custom IPunObservable implementation.", MessageType.Info, true);
                EditorGUI.indentLevel -= 1;
            }
        }
    }
}