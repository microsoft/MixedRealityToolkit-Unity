using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.iOS;

namespace UnityEngine.XR.iOS
{
public class UnityARUserAnchorComponent : MonoBehaviour {

    private string m_AnchorId;

	public string AnchorId  { get { return m_AnchorId; } }

	void Awake()
	{
		UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent += GameObjectAnchorUpdated;
		UnityARSessionNativeInterface.ARUserAnchorRemovedEvent += AnchorRemoved;
		this.m_AnchorId = UnityARSessionNativeInterface.GetARSessionNativeInterface ().AddUserAnchorFromGameObject(this.gameObject).identifierStr; 
	}
	void Start () {

	}

	public void AnchorRemoved(ARUserAnchor anchor)
	{
		if (anchor.identifier.Equals(m_AnchorId))
		{
			Destroy(this.gameObject);
		}
	}

    void OnDestroy() {
		UnityARSessionNativeInterface.ARUserAnchorUpdatedEvent -= GameObjectAnchorUpdated;
		UnityARSessionNativeInterface.ARUserAnchorRemovedEvent -= AnchorRemoved;
		UnityARSessionNativeInterface.GetARSessionNativeInterface ().RemoveUserAnchor(this.m_AnchorId); 
    }

	private void GameObjectAnchorUpdated(ARUserAnchor anchor)
	{
		
	}
}
}