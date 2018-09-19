// ----------------------------------------------------------------------------
// <copyright file="PhotonViewInspector.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2011 Exit Games GmbH
// </copyright>
// <summary>
//   Custom inspector for the PhotonView component.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------

#if UNITY_5 && !UNITY_5_0 && !UNITY_5_1 && !UNITY_5_2
#define UNITY_MIN_5_3
#endif


using System;
using Rotorz.ReorderableList.Internal;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (PhotonView))]
public class PhotonViewInspector : Editor
{
    private PhotonView m_Target;

    public override void OnInspectorGUI()
    {
        this.m_Target = (PhotonView)target;
        bool isProjectPrefab = EditorUtility.IsPersistent(this.m_Target.gameObject);

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
        else if (!this.m_Target.isOwnerActive)
        {
            EditorGUILayout.LabelField("Owner", "Scene");
        }
        else
        {
            PhotonPlayer owner = this.m_Target.owner;
            string ownerInfo = (owner != null) ? owner.name : "<no PhotonPlayer found>";

            if (string.IsNullOrEmpty(ownerInfo))
            {
                ownerInfo = "<no playername set>";
            }

            EditorGUILayout.LabelField("Owner", "[" + this.m_Target.ownerId + "] " + ownerInfo);
        }

        // ownership requests
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        OwnershipOption own = (OwnershipOption)EditorGUILayout.EnumPopup(this.m_Target.ownershipTransfer, GUILayout.Width(100));
        if (own != this.m_Target.ownershipTransfer)
        {
			// jf: fixed 5 and up prefab not accepting changes if you quit Unity straight after change.
			// not touching the define nor the rest of the code to avoid bringing more problem than solving.
			EditorUtility.SetDirty(this.m_Target);

            Undo.RecordObject(this.m_Target, "Change PhotonView Ownership Transfer");
            this.m_Target.ownershipTransfer = own;
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
            EditorGUILayout.LabelField("View ID", this.m_Target.viewID.ToString());
        }
        else
        {
            int idValue = EditorGUILayout.IntField("View ID [1.." + (PhotonNetwork.MAX_VIEW_IDS - 1) + "]", this.m_Target.viewID);
            if (this.m_Target.viewID != idValue)
            {
                Undo.RecordObject(this.m_Target, "Change PhotonView viewID");
                this.m_Target.viewID = idValue;
            }
        }


        // Locally Controlled
        if (EditorApplication.isPlaying)
        {
            string masterClientHint = PhotonNetwork.isMasterClient ? "(master)" : "";
            EditorGUILayout.Toggle("Controlled locally: " + masterClientHint, this.m_Target.isMine);
        }


        //DrawOldObservedItem();
        ConvertOldObservedItemToObservedList();


        // ViewSynchronization (reliability)
        if (this.m_Target.synchronization == ViewSynchronization.Off)
        {
            GUI.color = Color.grey;
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("synchronization"), new GUIContent("Observe option:"));

        if (this.m_Target.synchronization != ViewSynchronization.Off && this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0)
        {
            GUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Warning", EditorStyles.boldLabel);
            GUILayout.Label("Setting the synchronization option only makes sense if you observe something.");
            GUILayout.EndVertical();
        }

        /*ViewSynchronization vsValue = (ViewSynchronization)EditorGUILayout.EnumPopup("Observe option:", m_Target.synchronization);
        if (vsValue != m_Target.synchronization)
        {
            m_Target.synchronization = vsValue;
            if (m_Target.synchronization != ViewSynchronization.Off && m_Target.observed == null)
            {
                EditorUtility.DisplayDialog("Warning", "Setting the synchronization option only makes sense if you observe something.", "OK, I will fix it.");
            }
        }*/

        DrawSpecificTypeSerializationOptions();

        GUI.color = Color.white;
        DrawObservedComponentsList();

        // Cleanup: save and fix look
        if (GUI.changed)
        {
            #if !UNITY_MIN_5_3
            EditorUtility.SetDirty(this.m_Target);
            #endif
            PhotonViewHandler.HierarchyChange(); // TODO: check if needed
        }

        GUI.color = Color.white;
        #if !UNITY_MIN_5_3
        EditorGUIUtility.LookLikeControls();
        #endif
    }

    private void DrawSpecificTypeSerializationOptions()
    {
        if (this.m_Target.ObservedComponents.FindAll(item => item != null && item.GetType() == typeof (Transform)).Count > 0 ||
            (this.m_Target.observed != null && this.m_Target.observed.GetType() == typeof (Transform)))
        {
            this.m_Target.onSerializeTransformOption = (OnSerializeTransform)EditorGUILayout.EnumPopup("Transform Serialization:", this.m_Target.onSerializeTransformOption);
        }
        else if (this.m_Target.ObservedComponents.FindAll(item => item != null && item.GetType() == typeof (Rigidbody)).Count > 0 ||
                 (this.m_Target.observed != null && this.m_Target.observed.GetType() == typeof (Rigidbody)) ||
                 this.m_Target.ObservedComponents.FindAll(item => item != null && item.GetType() == typeof (Rigidbody2D)).Count > 0 ||
                 (this.m_Target.observed != null && this.m_Target.observed.GetType() == typeof (Rigidbody2D)))
        {
            this.m_Target.onSerializeRigidBodyOption = (OnSerializeRigidBody)EditorGUILayout.EnumPopup("Rigidbody Serialization:", this.m_Target.onSerializeRigidBodyOption);
        }
    }

    private void DrawSpecificTypeOptions()
    {
        if (this.m_Target.observed != null)
        {
            Type type = this.m_Target.observed.GetType();
            if (type == typeof (Transform))
            {
                this.m_Target.onSerializeTransformOption = (OnSerializeTransform)EditorGUILayout.EnumPopup("Serialization:", this.m_Target.onSerializeTransformOption);
            }
            else if (type == typeof (Rigidbody))
            {
                this.m_Target.onSerializeRigidBodyOption = (OnSerializeRigidBody)EditorGUILayout.EnumPopup("Serialization:", this.m_Target.onSerializeRigidBodyOption);
            }
        }
    }

    private void ConvertOldObservedItemToObservedList()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (this.m_Target.observed != null)
        {
            Undo.RecordObject(this.m_Target, null);
            if (this.m_Target.ObservedComponents.Contains(this.m_Target.observed) == false)
            {
                bool wasAdded = false;

                for (int i = 0; i < this.m_Target.ObservedComponents.Count; ++i)
                {
                    if (this.m_Target.ObservedComponents[i] == null)
                    {
                        this.m_Target.ObservedComponents[i] = this.m_Target.observed;
                        wasAdded = true;
                    }
                }

                if (wasAdded == false)
                {
                    this.m_Target.ObservedComponents.Add(this.m_Target.observed);
                }
            }

            this.m_Target.observed = null;
            #if !UNITY_MIN_5_3
            EditorUtility.SetDirty(this.m_Target);
            #endif
        }
    }


    private void DrawOldObservedItem()
    {
        EditorGUILayout.BeginHorizontal();

        // Using a lower version then 3.4? Remove the TRUE in the next line to fix an compile error
        string typeOfObserved = string.Empty;
        if (this.m_Target.observed != null)
        {
            int firstBracketPos = this.m_Target.observed.ToString().LastIndexOf('(');
            if (firstBracketPos > 0)
            {
                typeOfObserved = this.m_Target.observed.ToString().Substring(firstBracketPos);
            }
        }


        Component componenValue = (Component)EditorGUILayout.ObjectField("Observe: " + typeOfObserved, this.m_Target.observed, typeof (Component), true);
        if (this.m_Target.observed != componenValue)
        {
            if (this.m_Target.observed == null)
            {
                this.m_Target.synchronization = ViewSynchronization.UnreliableOnChange; // if we didn't observe anything yet. use unreliable on change as default
            }
            if (componenValue == null)
            {
                this.m_Target.synchronization = ViewSynchronization.Off;
            }

            this.m_Target.observed = componenValue;
        }

        EditorGUILayout.EndHorizontal();
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
        SerializedProperty listProperty = serializedObject.FindProperty("ObservedComponents");

        if (listProperty == null)
        {
            return;
        }

        float containerElementHeight = 22;
        float containerHeight = listProperty.arraySize*containerElementHeight;

        bool isOpen = PhotonGUI.ContainerHeaderFoldout("Observed Components (" + GetObservedComponentsCount() + ")", serializedObject.FindProperty("ObservedComponentsFoldoutOpen").boolValue);
        serializedObject.FindProperty("ObservedComponentsFoldoutOpen").boolValue = isOpen;

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
                Rect elementRect = new Rect(containerRect.xMin, containerRect.yMin + containerElementHeight*i, containerRect.width, containerElementHeight);
                {
                    Rect texturePosition = new Rect(elementRect.xMin + 6, elementRect.yMin + elementRect.height/2f - 1, 9, 5);
                    ReorderableListResources.DrawTexture(texturePosition, ReorderableListResources.texGrabHandle);

                    Rect propertyPosition = new Rect(elementRect.xMin + 20, elementRect.yMin + 3, elementRect.width - 45, 16);
                    EditorGUI.PropertyField(propertyPosition, listProperty.GetArrayElementAtIndex(i), new GUIContent());

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

        serializedObject.ApplyModifiedProperties();

        bool isObservedComponentsEmpty = this.m_Target.ObservedComponents.FindAll(item => item != null).Count == 0;

        if (wasObservedComponentsEmpty == true && isObservedComponentsEmpty == false && this.m_Target.synchronization == ViewSynchronization.Off)
        {
            Undo.RecordObject(this.m_Target, "Change PhotonView");
            this.m_Target.synchronization = ViewSynchronization.UnreliableOnChange;
            #if !UNITY_MIN_5_3
            EditorUtility.SetDirty(this.m_Target);
            #endif
            serializedObject.Update();
        }

        if (wasObservedComponentsEmpty == false && isObservedComponentsEmpty == true)
        {
            Undo.RecordObject(this.m_Target, "Change PhotonView");
            this.m_Target.synchronization = ViewSynchronization.Off;
            #if !UNITY_MIN_5_3
            EditorUtility.SetDirty(this.m_Target);
            #endif
            serializedObject.Update();
        }
    }

    private static GameObject GetPrefabParent(GameObject mp)
    {
        #if UNITY_2_6_1 || UNITY_2_6 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4
        // Unity 3.4 and older use EditorUtility
        return (EditorUtility.GetPrefabParent(mp) as GameObject);
        #else
        // Unity 3.5 uses PrefabUtility
        return PrefabUtility.GetPrefabParent(mp) as GameObject;
        #endif
    }
}
