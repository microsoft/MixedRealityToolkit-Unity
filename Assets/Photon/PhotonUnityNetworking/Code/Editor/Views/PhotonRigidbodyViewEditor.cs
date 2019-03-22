// ----------------------------------------------------------------------------
// <copyright file="PhotonRigidbodyViewEditor.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   This is a custom editor for the RigidbodyView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
	using UnityEditor;
	using UnityEngine;


    [CustomEditor(typeof (PhotonRigidbodyView))]
    public class PhotonRigidbodyViewEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (Application.isPlaying)
            {
                EditorGUILayout.HelpBox("Editing is disabled in play mode.", MessageType.Info);
                return;
            }

            PhotonRigidbodyView view = (PhotonRigidbodyView)target;

            view.m_TeleportEnabled = PhotonGUI.ContainerHeaderToggle("Enable teleport for large distances", view.m_TeleportEnabled);

            if (view.m_TeleportEnabled)
            {
                Rect rect = PhotonGUI.ContainerBody(20.0f);
                view.m_TeleportIfDistanceGreaterThan = EditorGUI.FloatField(rect, "Teleport if distance greater than", view.m_TeleportIfDistanceGreaterThan);
            }

            view.m_SynchronizeVelocity = PhotonGUI.ContainerHeaderToggle("Synchronize Velocity", view.m_SynchronizeVelocity);
            view.m_SynchronizeAngularVelocity = PhotonGUI.ContainerHeaderToggle("Synchronize Angular Velocity", view.m_SynchronizeAngularVelocity);
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(view);
            }
        }
    }
}