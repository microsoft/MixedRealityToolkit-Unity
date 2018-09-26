using Microsoft.MixedReality.Toolkit.Core.EventDatum.Input;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem;
using Microsoft.MixedReality.Toolkit.Core.Interfaces.InputSystem.Handlers;
using Microsoft.MixedReality.Toolkit.SDK.Input.Handlers;
//using Microsoft.MixedReality.Toolkit.InputSystem;
//using HoloToolkit.Unity.InputModule;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickSharer : BaseFocusHandler, IMixedRealityInputHandler, IMixedRealityPointerHandler, IMixedRealityFocusChangedHandler
{

    public int sharingID;
    public bool TimeOut = false;
    public bool isOriginal; //By default, this is false. We'll set this to true if our main script created this, to detect of this is an original ClickSharer.
    public string objectName; //name of the object, to keep track of clones

    public IMixedRealityPointer[] Pointers
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    public bool HasFocus() { throw new System.NotImplementedException(); }

    public bool FocusEnabled { get { throw new System.NotImplementedException(); } set { throw new System.NotImplementedException(); } }


    public List<IMixedRealityPointer> Focusers (){ throw new System.NotImplementedException();}

    // Use this for initialization
    void Start () {   
    }

    void OnEnable()
    {
        StartCoroutine(CheckIfOriginal());
    }

    IEnumerator CheckIfOriginal()
    {
        yield return new WaitForSeconds(5f); //Add a short delay before checking to see if this was a cloned object or originally created by the Sharing ID Generator

        if (gameObject.name != objectName)
        {
            print("Assuming object: " + gameObject.name + " is cloned - updating sharingID for this object.");
            sharingID = SeanSharingManager.Instance.gameObject.GetComponent<OngSharingIDgenerator>().sharingID++;
            objectName = gameObject.name;

        }

        //if (isOriginal == false)
        //{
        //    print("Assuming object: " + gameObject.name + " is cloned - updating sharingID for this object.");
        //    sharingID = SeanSharingManager.Instance.gameObject.GetComponent<OngSharingIDgenerator>().sharingID++;
        //}

        ////Set isOriginal to false, so that if this object is cloned in the future - it will be caught be the if statement above.
        //isOriginal = false;
    }
	

    public void time()
    {
        TimeOut = true;
        StartCoroutine(timeout());
    }

    IEnumerator timeout()
    {
        yield return null;
        yield return null;
        yield return null;
        TimeOut = false;
    }


    //public void OnInputDown(InputEventData eventData)
    //{
    //    if (SeanSharingManager.IED != null)
    //    {
    //        if (!TimeOut)
    //        {
    //            SeanSharingManager.Instance.InputDown(sharingID);
    //            print("Sending OnInputDown from: " + gameObject.name);
    //        }
    //    }
    //    else
    //    {
    //        SeanSharingManager.IED = eventData;
    //        //InputManager.Instance.RemoveGlobalListener(gameObject);
    //    }
    //}

    public void OnPointerDown(MixedRealityPointerEventData eventData)
    {
        if (!TimeOut)
        {
            SeanSharingManager.Instance.InputDown(sharingID);
            print("Sending OnInputDown from: " + gameObject.name);
        }
    }

    //public void OnInputClicked(InputClickedEventData eventData)
    //{
    //    if (!TimeOut)
    //        SeanSharingManager.Instance.InputClicked(sharingID);


    //    if (SeanSharingManager.IED3 == null)
    //    {
    //        SeanSharingManager.IED3 = eventData;
    //    }
    //}

    public void OnPointerClicked(MixedRealityPointerEventData eventData)
    {
        if (!TimeOut)
            SeanSharingManager.Instance.InputClicked(sharingID);
    }

    //public void OnInputUp(InputEventData eventData)
    //{
    //    if (!TimeOut)
    //        SeanSharingManager.Instance.InputUp(sharingID);
    //}

    public void OnPointerUp(MixedRealityPointerEventData eventData)
    {
        if (!TimeOut)
            SeanSharingManager.Instance.InputUp(sharingID);
    }

    override
    public void OnFocusEnter(FocusEventData eventData)
    {
        if (!TimeOut)
            SeanSharingManager.Instance.FocusEnter(sharingID);
    }

    //public void OnFocusEnter(FocusEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    override
    public void OnFocusExit(FocusEventData eventData)
    {
        if (!TimeOut)
            SeanSharingManager.Instance.FocusExit(sharingID);
    }

    //public void OnFocusExit(FocusEventData eventData)
    //{
    //    throw new System.NotImplementedException();
    //}

    public new bool Equals(object x, object y)
    {
        throw new System.NotImplementedException();
    }

    public int GetHashCode(object obj)
    {
        throw new System.NotImplementedException();
    }
    
    public void OnBeforeFocusChange(FocusEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnFocusChanged(FocusEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnInputPressed(InputEventData<float> eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnPositionInputChanged(InputEventData<Vector2> eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnInputUp(InputEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnInputDown(InputEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}
