using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Experimental.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirage : MonoBehaviour
{
    public Transform Target;

    public float MirageDistance;

    private bool shouldMirror = true;

    private float lastScaleFactor = 1.0f;

    private Vector3? lastTargetScale;
    private Vector3? lastTargetPosition;

    private Vector3 realTargetPosition;
    private Vector3 realTargetScale;

    // Start is called before the first frame update
    void Start()
    {
        BoundsControl bc = GetComponent<BoundsControl>();

        bc?.TranslateStarted.AddListener(StartManipulation);
        bc?.RotateStarted.AddListener(StartManipulation);
        bc?.ScaleStarted.AddListener(StartManipulation);

        bc?.TranslateStopped.AddListener(StopManipulation);
        bc?.RotateStopped.AddListener(StopManipulation);
        bc?.ScaleStopped.AddListener(StopManipulation);

        realTargetPosition = Target.position;
        realTargetScale = Target.localScale;

    }

    // Update is called once per frame
    void LateUpdate()
    {
        bool flag = false;
        // Find all valid pointers
        foreach (var inputSource in CoreServices.InputSystem.DetectedInputSources)
        {
            foreach (var pointer in inputSource.Pointers)
            {
                if (pointer.IsActive && inputSource.SourceType == InputSourceType.Hand)
                {
                    if((pointer.Position - transform.position).magnitude < 0.3f)
                    {
                        flag = true;
                        if(shouldMirror == true)
                        {
                            shouldMirror = false;
                            Debug.Log("Aaaah!");
                            lastScaleFactor = (realTargetPosition - Camera.main.transform.position).magnitude / (transform.position - Camera.main.transform.position).magnitude;
                            lastTargetScale = Target.localScale;
                            lastTargetPosition = Target.position;
                        }
                    }
                }
            }
        }

        if (!flag)
        {
            shouldMirror = true;
        }

        Vector3 headPosition = Camera.main.transform.position;

        if (shouldMirror)
        {
            transform.rotation = Target.rotation;

            

            Vector3 targetDelta = Target.position - headPosition;

            Vector3 mirageDelta = targetDelta.normalized * MirageDistance; // Always magnitude = MirageDistance

            Vector3 miragePosition = headPosition + mirageDelta;

            Vector3 mirageScale = (MirageDistance / targetDelta.magnitude) * Target.lossyScale;

            transform.position = miragePosition;
            Target.position = Vector3.Lerp(Target.position, realTargetPosition, 0.1f);
            Target.localScale = Vector3.Lerp(Target.localScale, realTargetScale, 0.1f);


            
            transform.localScale = Vector3.Scale(mirageScale, GetInverseScaleConversion(transform)) * 1.0f;
        }
        else
        {
            
            Vector3 mirageDelta = transform.position - headPosition;
            

            //Vector3 targetDelta = mirageDelta.normalized * MirageDistance * lastScaleFactor;

            //Target.position = headPosition + mirageDelta * lastScaleFactor;
            realTargetPosition = headPosition + mirageDelta * lastScaleFactor;


            //Debug.Log($"MirageDelta mag: {mirageDelta.magnitude}, lastScale = {lastScaleFactor}, targetDelta = {(mirageDelta * lastScaleFactor).magnitude}");
            Target.position = Vector3.Lerp(Target.position, transform.position, 0.2f);

            Target.localScale = Vector3.Lerp(Target.localScale, transform.localScale, 0.2f);


            Target.rotation = transform.rotation;
        }
        
    }

    private Vector3 GetInverseScaleConversion(Transform t)
    {
        Vector3 scaleConversion = t.parent?.lossyScale ?? Vector3.one;
        return new Vector3(1.0f / scaleConversion.x, 1.0f / scaleConversion.y, 1.0f / scaleConversion.z);
    }

    public void StartManipulation()
    {
        
    }

    public void StopManipulation()
    {
        //shouldMirror = true;
    }
}
