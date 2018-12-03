// ----------------------------------------------------------------------------
// <copyright file="PhotonViewInspector.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Custom inspector for the PhotonView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

using System;
using UnityEditor;
using UnityEngine;

using Photon.Realtime;

namespace Photon.Pun
{
    [CustomEditor(typeof(PhotonView))]
    public class PhotonViewInspector : Editor
    {
        private PhotonView m_Target;

        public override void OnInspectorGUI()
        {
            this.m_Target = (PhotonView)this.target;
			bool isProjectPrefab = PhotonEditorUtils.IsPrefab(this.m_Target.gameObject);

            if (this.m_Target.ObservedComponents == null)
            {
                this.m_Target.ObservedComponents = new System.Collections.Generic.List<Component>();
            }

            if (this.m_Target.ObservedComponents.Count == 0)
            {
                this.m_Target.ObservedComponents.Add(null);
            }

            EditorGUILayout.BeginHorizontal();
            // Owner
            if (isProjectPrefab)
            {
                EditorGUILayout.LabelField("Owner:", "Set at runtime");
            }
            else if (!this.m_Target.IsOwnerActive)
            {
                EditorGUILayout.LabelField("Owner", "Scene");
            }
            else
            {
                Player owner = this.m_Target.Owner;
                string ownerInfo = (owner != null) ? owner.NickName : "<no Player found>";

                if (string.IsNullOrEmpty(ownerInfo))
                {
                    ownerInfo = "<no playername set>";
                }

                EditorGUILayout.LabelField("Owner", "[" + this.m_Target.OwnerActorNr + "] " + ownerInfo);
            }

            // ownership requests
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            OwnershipOption own = (OwnershipOption) EditorGUILayout.EnumPopup(this.m_Target.OwnershipTransfer, GUILayout.Width(100));
            if (own != this.m_Target.OwnershipTransfer)
            {
                // jf: fixed 5 and up prefab not accepting changes if you quit Unity straight after change.
                // not touching the define nor the rest of the code to avoid bringing more problem than solving.
                EditorUtility.SetDirty(this.m_Target);

                Undo.RecordObject(this.m_Target, "Change PhotonView Ownership Transfer");
                this.m_Target.OwnershipTransfer = own;
            }
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndHorizontal();


            // View ID
            if (isProjectPrefab)
            {
                EditorGUILayout.LabelField("View ID", "Set at runtime");
            }
            else if (EditorApplication.isPlaying)
            {
                EditorGUILayout.LabelField("View ID", this.m_Target.ViewID.ToString());
            }
            else
            {
                int idValue = EditorGUILayout.IntField("View ID [1.." + (PhotonNetwork.MAX_VIEW_IDS - 1) + "]", this.m_Target.ViewID);
                if (this.m_Target.ViewID != idValue)
                {
                    Undo.RecordObject(this.m_Target, "Change PhotonView viewID");
                    this.m_Target.ViewID = idValue;
                }
            }

            // Locally Controlled
            if (EditorApplication.isPlaying)
            {
                string masterClientHint = PhotonNetwork.IsMasterClient ? "(master)" : "";
                EditorGUILayout.Toggle("Controlled locally: " + masterClientHint, this.m_Target.IsMine);
            }

            // ViewSynchronization (reliability)
            if (this.m_Target.Synchronization == ViewSynchronization.Off)
            {
                GUI.color = Color.grey;
            }

            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("Synchronization"), new GUIContent("Observe option:"));

            if (this.m_Target.Synchronization != ViewSynchronization.Off && this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0)
            {
                GUILayout.BeginVertical(GUI.skin.box);
                GUILayout.Label("Warning", EditorStyles.boldLabel);
                GUILayout.Label("Setting the synchronization option only makes sense if you observe something.");
                GUILayout.EndVertical();
            }

            GUI.color = Color.white;
            this.DrawObservedComponentsList();

            // Cleanup: save and fix look
            if (GUI.changed)
            {
                PhotonViewHandler.HierarchyChange(); // TODO: check if needed
            }

            GUI.color = Color.white;
        }

        private int GetObservedComponentsCount()
        {
            int count = 0;

            for (int i = 0; i < this.m_Target.ObservedComponents.Count; ++i)
            {
                if (this.m_Target.ObservedComponents[i] != null)
                {
                    count++;
                }
            }

            return count;
        }

        private void DrawObservedComponentsList()
        {
            GUILayout.Space(5);
            SerializedProperty listProperty = this.serializedObject.FindProperty("ObservedComponents");

            if (listProperty == null)
            {
                return;
            }

            float containerElementHeight = 22;
            float containerHeight = listProperty.arraySize * containerElementHeight;

            bool isOpen = PhotonGUI.ContainerHeaderFoldout("Observed Components (" + this.GetObservedComponentsCount() + ")", this.serializedObject.FindProperty("ObservedComponentsFoldoutOpen").boolValue);
            this.serializedObject.FindProperty("ObservedComponentsFoldoutOpen").boolValue = isOpen;

            if (isOpen == false)
            {
                containerHeight = 0;
            }

            //Texture2D statsIcon = AssetDatabase.LoadAssetAtPath( "Assets/Photon Unity Networking/Editor/PhotonNetwork/PhotonViewStats.png", typeof( Texture2D ) ) as Texture2D;

            Rect containerRect = PhotonGUI.ContainerBody(containerHeight);
            bool wasObservedComponentsEmpty = this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0;
            if (isOpen == true)
            {
                for (int i = 0; i < listProperty.arraySize; ++i)
                {
                    Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + containerElementHeight * i, containerRect.width, containerElementHeight);
                    {
                        Rect texturePosition = new Rect(elementRect.xMin + 6, elementRect.yMin + elementRect.height / 2f - 1, 9, 5);
                        ReorderableListResources.DrawTexture(texturePosition, ReorderableListResources.texGrabHandle);

                        Rect propertyPosition = new Rect(elementRect.xMin + 20, elementRect.yMin + 3, elementRect.width - 45, 16);

                        // keep track of old type to catch when a new type is observed
                        Type _oldType = listProperty.GetArrayElementAtIndex(i).objectReferenceValue != null ? listProperty.GetArrayElementAtIndex(i).objectReferenceValue.GetType() : null;

                        EditorGUI.PropertyField(propertyPosition, listProperty.GetArrayElementAtIndex(i), new GUIContent());

                        // new type, could be different from old type
                        Type _newType = listProperty.GetArrayElementAtIndex(i).objectReferenceValue != null ? listProperty.GetArrayElementAtIndex(i).objectReferenceValue.GetType() : null;

                        // the user dropped a Transform, we must change it by adding a PhotonTransformView and observe that instead
                        if (_oldType != _newType)
                        {
                            if (_newType == typeof(PhotonView))
                            {
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = null;
                                Debug.LogError("PhotonView Detected you dropped a PhotonView, this is not allowed. \n It's been removed from observed field.");

                            }
                            else if (_newType == typeof(Transform))
                            {

                                // try to get an existing PhotonTransformView ( we don't want any duplicates...)
                                PhotonTransformView _ptv = this.m_Target.gameObject.GetComponent<PhotonTransformView>();
                                if (_ptv == null)
                                {
                                    // no ptv yet, we create one and enable position and rotation, no scaling, as it's too rarely needed to take bandwidth for nothing
                                    _ptv = Undo.AddComponent<PhotonTransformView>(this.m_Target.gameObject);
                                }
                                // switch observe from transform to _ptv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _ptv;
                                Debug.Log("PhotonView has detected you dropped a Transform. Instead it's better to observe a PhotonTransformView for better control and performances");
                            }
                            else if (_newType == typeof(Rigidbody))
                            {

                                Rigidbody _rb = listProperty.GetArrayElementAtIndex(i).objectReferenceValue as Rigidbody;

                                // try to get an existing PhotonRigidbodyView ( we don't want any duplicates...)
                                PhotonRigidbodyView _prbv = _rb.gameObject.GetComponent<PhotonRigidbodyView>();
                                if (_prbv == null)
                                {
                                    // no _prbv yet, we create one
                                    _prbv = Undo.AddComponent<PhotonRigidbodyView>(_rb.gameObject);
                                }
                                // switch observe from transform to _prbv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _prbv;
                                Debug.Log("PhotonView has detected you dropped a RigidBody. Instead it's better to observe a PhotonRigidbodyView for better control and performances");
                            }
                            else if (_newType == typeof(Rigidbody2D))
                            {

                                // try to get an existing PhotonRigidbody2DView ( we don't want any duplicates...)
                                PhotonRigidbody2DView _prb2dv = this.m_Target.gameObject.GetComponent<PhotonRigidbody2DView>();
                                if (_prb2dv == null)
                                {
                                    // no _prb2dv yet, we create one
                                    _prb2dv = Undo.AddComponent<PhotonRigidbody2DView>(this.m_Target.gameObject);
                                }
                                // switch observe from transform to _prb2dv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _prb2dv;
                                Debug.Log("PhotonView has detected you dropped a Rigidbody2D. Instead it's better to observe a PhotonRigidbody2DView for better control and performances");
                            }
                            else if (_newType == typeof(Animator))
                            {

                                // try to get an existing PhotonAnimatorView ( we don't want any duplicates...)
                                PhotonAnimatorView _pav = this.m_Target.gameObject.GetComponent<PhotonAnimatorView>();
                                if (_pav == null)
                                {
                                    // no _pav yet, we create one
                                    _pav = Undo.AddComponent<PhotonAnimatorView>(this.m_Target.gameObject);
                                }
                                // switch observe from transform to _prb2dv
                                listProperty.GetArrayElementAtIndex(i).objectReferenceValue = _pav;
                                Debug.Log("PhotonView has detected you dropped a Animator, so we switched to PhotonAnimatorView so that you can serialized the Animator variables");
                            }
                            else if (!typeof(IPunObservable).IsAssignableFrom(_newType))
                            {
                                bool _ignore = false;
                                #if PLAYMAKER
                                _ignore = _newType == typeof(PlayMakerFSM);// Photon Integration for PlayMaker will swap at runtime to a proxy using iPunObservable.
                                #endif

                                if (_newType == null || _newType == typeof(Rigidbody) || _newType == typeof(Rigidbody2D))
                                {
                                    _ignore = true;
                                }

                                if (!_ignore)
                                {
                                    listProperty.GetArrayElementAtIndex(i).objectReferenceValue = null;
                                    Debug.LogError("PhotonView Detected you dropped a Component missing IPunObservable Interface,\n You dropped a <" + _newType + "> instead. It's been removed from observed field.");
                                }
                            }
                        }

                        //Debug.Log( listProperty.GetArrayElementAtIndex( i ).objectReferenceValue.GetType() );
                        //Rect statsPosition = new Rect( propertyPosition.xMax + 7, propertyPosition.yMin, statsIcon.width, statsIcon.height );
                        //ReorderableListResources.DrawTexture( statsPosition, statsIcon );

                        Rect removeButtonRect = new Rect(elementRect.xMax - PhotonGUI.DefaultRemoveButtonStyle.fixedWidth,
                                                         elementRect.yMin + 2,
                                                         PhotonGUI.DefaultRemoveButtonStyle.fixedWidth,
                                                         PhotonGUI.DefaultRemoveButtonStyle.fixedHeight);

                        GUI.enabled = listProperty.arraySize > 1;
                        if (GUI.Button(removeButtonRect, new GUIContent(ReorderableListResources.texRemoveButton), PhotonGUI.DefaultRemoveButtonStyle))
                        {
                            listProperty.DeleteArrayElementAtIndex(i);
                        }
                        GUI.enabled = true;

                        if (i < listProperty.arraySize - 1)
                        {
                            texturePosition = new Rect(elementRect.xMin + 2, elementRect.yMax, elementRect.width - 4, 1);
                            PhotonGUI.DrawSplitter(texturePosition);
                        }
                    }
                }
            }

            if (PhotonGUI.AddButton())
            {
                listProperty.InsertArrayElementAtIndex(Mathf.Max(0, listProperty.arraySize - 1));
            }

            this.serializedObject.ApplyModifiedProperties();

            bool isObservedComponentsEmpty = this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0;

            if (wasObservedComponentsEmpty == true && isObservedComponentsEmpty == false && this.m_Target.Synchronization == ViewSynchronization.Off)
            {
                Undo.RecordObject(this.m_Target, "Change PhotonView");
                this.m_Target.Synchronization = ViewSynchronization.UnreliableOnChange;
                this.serializedObject.Update();
            }

            if (wasObservedComponentsEmpty == false && isObservedComponentsEmpty == true)
            {
                Undo.RecordObject(this.m_Target, "Change PhotonView");
                this.m_Target.Synchronization = ViewSynchronization.Off;
                this.serializedObject.Update();
            }
        }

        private static GameObject GetPrefabParent(GameObject mp)
        {
            return PrefabUtility.GetPrefabParent(mp) as GameObject;
        }
    }
}