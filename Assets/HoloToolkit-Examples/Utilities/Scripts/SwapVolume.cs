using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;

public class SwapVolume : MonoBehaviour, IInputClickHandler
{

    public GameObject HideThis;
    public GameObject SpawnThis;
    public bool UpdateSolverTargetToClickSource = true;

    private SolverHandler solverHandler;
    private Vector3 defaultPosition;
    private Quaternion defaultRotation;
    private bool isOn = false;
    private GameObject spawnedObject;

	// Use this for initialization
	void Start ()
    {
        solverHandler = SpawnThis.GetComponent<SolverHandler>();
        defaultPosition = HideThis.transform.position;
        defaultRotation = HideThis.transform.rotation;
    }

    public void OnInputClicked(InputClickedEventData eventData)
    {
        if (isOn)
        {
            if (spawnedObject != null)
            {
                Destroy(spawnedObject);
            }
            if (HideThis != null)
            {
                HideThis.SetActive(true);
            }
        }
        else
        {
            spawnedObject = Instantiate(SpawnThis, defaultPosition, defaultRotation);

            if (UpdateSolverTargetToClickSource)
            {
                solverHandler = spawnedObject.GetComponent<SolverHandler>();

                InteractionSourceInfo sourceKind;
                if (eventData.InputSource.TryGetSourceKind(eventData.SourceId, out sourceKind))
                {

                    switch (sourceKind)
                    {
                        case InteractionSourceInfo.Controller:
                            solverHandler.TrackedObjectToReference = SolverHandler.TrackedObjectToReferenceEnum.MotionControllerRight;
                            break;
                        default:
                            Debug.LogError("The click event came from a device that isn't tracked. Nothing to attach to! Use a controller to select an example.");
                            break;
                    }
                }
            }
            if (HideThis != null)
            {
                HideThis.SetActive(false);
            }
        }
        isOn = !isOn;
        eventData.Use(); // Mark the event as used, so it doesn't fall through to other handlers.
    }
}
