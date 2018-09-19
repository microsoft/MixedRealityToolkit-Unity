using UnityEngine;
using System.Collections.Generic;

/// <summary>
///     Handles the network culling.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkCullingHandler : MonoBehaviour
{
    #region VARIABLES
    
    private int orderIndex;

    private CullArea cullArea;

    private List<int> previousActiveCells, activeCells;
    
    private PhotonView pView;

    private Vector3 lastPosition, currentPosition;

    #endregion

    #region UNITY_FUNCTIONS

    /// <summary>
    ///     Gets references to the PhotonView component and the cull area game object.
    /// </summary>
    private void OnEnable()
    {
        if (pView == null)
        {
            pView = GetComponent<PhotonView>();

            if (!pView.isMine)
            {
                return;
            }
        }
        
        if (cullArea == null)
        {
            cullArea = GameObject.FindObjectOfType<CullArea>();
        }
        
        previousActiveCells = new List<int>(0);
        activeCells = new List<int>(0);

        currentPosition = lastPosition = transform.position;
    }

    /// <summary>
    ///     Initializes the right interest group or prepares the permanent change of the interest group of the PhotonView component.
    /// </summary>
    private void Start()
    {
        if (!pView.isMine)
        {
            return;
        }

        if (PhotonNetwork.inRoom)
        {
            if (cullArea.NumberOfSubdivisions == 0)
            {
                pView.group = cullArea.FIRST_GROUP_ID;

                PhotonNetwork.SetReceivingEnabled(cullArea.FIRST_GROUP_ID, true);
                PhotonNetwork.SetSendingEnabled(cullArea.FIRST_GROUP_ID, true);
            }
            else
            {
                CheckGroupsChanged();
                InvokeRepeating("UpdateActiveGroup", 0.0f, 1.0f / PhotonNetwork.sendRateOnSerialize);
            }
        }
    }

    /// <summary>
    ///     Checks if the player has moved perviously and updates the interest groups if necessary.
    /// </summary>
    private void Update()
    {
        if (!pView.isMine)
        {
            return;
        }

        lastPosition = currentPosition;
        currentPosition = transform.position;

        // This is a simple position comparison of the current and the previous position. 
        // When using Network Culling in a bigger project keep in mind that there might
        // be more transform-related options, e.g. the rotation, or other options to check.
        if (currentPosition != lastPosition)
        {
            CheckGroupsChanged();
        }
    }

    /// <summary>
    ///     Cancels all (upcoming) invoke calls.
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke();
    }

    #endregion
    
    /// <summary>
    ///     Checks if the interest groups have changed and perform action if necessary.
    /// </summary>
    private void CheckGroupsChanged()
    {
        if (cullArea.NumberOfSubdivisions == 0)
        {
            return;
        }

        previousActiveCells = new List<int>(activeCells);
        activeCells = cullArea.GetActiveCells(transform.position);

        if (activeCells.Count != previousActiveCells.Count)
        {
            UpdateInterestGroups();
            return;
        }

        foreach (int groupId in activeCells)
        {
            if (!previousActiveCells.Contains(groupId))
            {
                UpdateInterestGroups();
                return;
            }
        }
    }

    /// <summary>
    ///     Unsubscribes from old and subscribes to new interest groups.
    /// </summary>
    private void UpdateInterestGroups()
    {
        foreach (int groupId in previousActiveCells)
        {
            PhotonNetwork.SetReceivingEnabled(groupId, false);
            PhotonNetwork.SetSendingEnabled(groupId, false);
        }

        foreach (int groupId in activeCells)
        {
            PhotonNetwork.SetReceivingEnabled(groupId, true);
            PhotonNetwork.SetSendingEnabled(groupId, true);
        }
    }

    /// <summary>
    ///     Updates the current group of the PhotonView component.
    /// </summary>
    private void UpdateActiveGroup()
    {
        // If the player leaves the area we insert the whole area itself as an active cell.
        // This can be removed if it is sure that the player is not able to leave the area.
        while (activeCells.Count <= cullArea.NumberOfSubdivisions)
        {
            activeCells.Add(cullArea.FIRST_GROUP_ID);
        }

        if (cullArea.NumberOfSubdivisions == 1)
        {
            orderIndex = (++orderIndex % cullArea.SUBDIVISION_FIRST_LEVEL_ORDER.Length);
            pView.group = activeCells[cullArea.SUBDIVISION_FIRST_LEVEL_ORDER[orderIndex]];
        }
        else if (cullArea.NumberOfSubdivisions == 2)
        {
            orderIndex = (++orderIndex % cullArea.SUBDIVISION_SECOND_LEVEL_ORDER.Length);
            pView.group = activeCells[cullArea.SUBDIVISION_SECOND_LEVEL_ORDER[orderIndex]];
        }
        else if (cullArea.NumberOfSubdivisions == 3)
        {
            orderIndex = (++orderIndex % cullArea.SUBDIVISION_THIRD_LEVEL_ORDER.Length);
            pView.group = activeCells[cullArea.SUBDIVISION_THIRD_LEVEL_ORDER[orderIndex]];
        }
    }

    /// <summary>
    ///     Drawing informations.
    /// </summary>
    private void OnGUI()
    {
        if (!pView.isMine)
        {
            return;
        }
        
        string subscribedAndActiveCells = "Inside cells:\n";
        string subscribedCells = "Subscribed cells:\n";

        for (int index = 0; index < activeCells.Count; ++index)
        {
            if (index <= cullArea.NumberOfSubdivisions)
            {
                subscribedAndActiveCells += activeCells[index] + "  ";
            }

            subscribedCells += activeCells[index] + "  ";
        }
        
        GUI.Label(new Rect(20.0f, Screen.height - 100.0f, 200.0f, 40.0f), "<color=white>" + subscribedAndActiveCells + "</color>", new GUIStyle() { alignment = TextAnchor.UpperLeft, fontSize = 16 } );
        GUI.Label(new Rect(20.0f, Screen.height - 60.0f, 200.0f, 40.0f), "<color=white>" + subscribedCells + "</color>", new GUIStyle() { alignment = TextAnchor.UpperLeft, fontSize = 16 } );
    }
}