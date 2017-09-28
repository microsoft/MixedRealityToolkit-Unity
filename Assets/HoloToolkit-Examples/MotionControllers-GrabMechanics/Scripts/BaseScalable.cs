using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class responsible for two hand scale. Objects with a child of this class attached 
/// </summary>

public abstract class BaseScalable : MonoBehaviour
{
    //keeps track of all grabbers attached to this object
    public List<GameObject> ScalarsAttachedList { get { return scalarsAttachedList; } set { scalarsAttachedList = value; } }

    /// <summary>
    /// scale needs to subscribe to grab events in order to add more scalars to the list of scalars
    /// </summary>
    protected virtual void OnEnable()
    {
        BaseGrabbable.GrabStarted += ScalarAdd;
        BaseGrabbable.GrabEnded += ScalarRemove;
    }

    protected virtual void OnDisable()
    {
        BaseGrabbable.GrabStarted -= ScalarAdd;
        BaseGrabbable.GrabEnded -= ScalarRemove;
    }

    /// <summary>
    /// Taking a snap shot of scale at the moment of grab is important so that we can perform the scale relative to the original size of the game object
    /// </summary>
    protected virtual void SnapShotOfScale()
    {
        snapShotOfScaleVec = transform.localScale;
    }

    /// <summary>
    /// We have two options when we attempt to scale: the first is by velocity and the second is based on the distance between the 
    /// two+ grabbers
    /// </summary>
    public void AttemptScale()
    {
        if (scalarsAttachedList.Count >= minScalarNumForScale)
        {
            //Velocity
            //Multiply scale of this scalable object by the velocity of scalar1 and scalar2 (or however many)
            if (scaleByVelocity)
            {
                int i = 0;
                foreach (GameObject sclr in scalarsAttachedList)
                {
                    Debug.Log("Velocity of scalar obj " + MotionControllerInfoTemp.GetVelocity(scalarsAttachedList[i]));
                    i++;
                }
            }

            //Distance
            //snapshot a standard distance that the controls are when the scalable object is engaged
            //That standard distance between controllers corresponds to the localScale * scaleMultiplier
            if (scaleByDistance)
            {
                if (scalarsAttachedList.Count == 2)
                {
                    float dist = Vector3.Distance(scalarsAttachedList[0].transform.position, scalarsAttachedList[1].transform.position);
                    snapShotDistance = dist;
                    snapShotOfScale = transform.localScale.x;
                    currentlyScaling = true;
                    StartCoroutine(PerformScaling());
                }
            }
        }
    }

    /// <summary>
    /// Adding a grabber object to the list of scalars means adding it to the list of scalars and always attempting a scale if there are enough scalars attached
    /// </summary>
    /// <param name="grabber"></param>

    public void ScalarAdd(GameObject grabber)
    {
        if (!scalarsAttachedList.Contains(grabber))
        {
            //if (grabber.Equals(GetComponent<BaseGrabbable>().MyGrabber.gameObject))
            //{
                scalarsAttachedList.Add(grabber);
                AttemptScale();
            //}
        }
    }


    /// <summary>
    /// Remove a gameobject from the list of scalars when the grabber let's go.
    /// </summary>
    /// <param name="grabber"></param>
    public virtual void ScalarRemove(GameObject grabber)
    {
        if (scalarsAttachedList.Contains(grabber))
        {
            scalarsAttachedList.Remove(grabber);
        }
    }

    /// <summary>
    /// scaling can be amplified by increasing the scaling mulitplier 
    /// scaling functionality can also be modified by recording a distance from the user. 
    /// (For example, an object that is further away might scale up more because it is further away from the user)
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator PerformScaling()
    {
        while (currentlyScaling)
        {
            if (scalarsAttachedList.Count == minScalarNumForScale)
            {
                float currDistance = Vector3.Distance(scalarsAttachedList[0].transform.position, scalarsAttachedList[1].transform.position);
                transform.localScale = Vector3.one * ((currDistance / snapShotDistance) * snapShotOfScale) /*multiplier * distFromUser*/;
            }
            yield return 0;
        }
        currentlyScaling = false;
        yield return null;
    }

    [Range(1, 5)]
    private float scaleMultiplier = 1.0f;
    [SerializeField]
    public bool scaleByVelocity = false;
    [SerializeField]
    private bool scaleByDistance = true;
    private bool readyToScale;
    private Vector3 snapShotOfScaleVec;
    private float snapShotOfScale;
    private int minScalarNumForScale = 2;
    private bool currentlyScaling;
    private float snapShotDistance;
    //Add to the list if your object requires more than two scalars attached
    [SerializeField]
    private List<GameObject> scalarsAttachedList;

}
