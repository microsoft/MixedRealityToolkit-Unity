using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FFITest : MonoBehaviour
{
    public TextMeshPro TimeText;
    public TextMeshPro DisplacementText;
    public TextMeshPro OrientationText;

    public TextMeshPro infoText;

    public Transform testObject;
    public Transform testGoal;

    public MeshRenderer floorplan;

    public float displacementTolerance;
    public float orientationTolerance;

    private bool testHasStarted = false;
    private bool testHasCompleted = false;

    private float testTime = 0.0f;

    private Vector3 initialPosition = Vector3.zero;
    private Quaternion initialRotation = Quaternion.identity;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void StartTest()
    {
        testHasStarted = true;
    }

    // Update is called once per frame
    void Update()
    {

        if (testHasStarted)
        {
            var displacementError = testObject.position - testGoal.position;
            var orientationError = Quaternion.Angle(testObject.rotation, testGoal.rotation);

            TimeText.text = "Elapsed time: " + testTime.ToString("F2") + " sec";
            DisplacementText.text = $"Displacement error: {(displacementError.magnitude * 100.0f):F2} cm (required: <{(displacementTolerance * 100.0f):F2} cm)";
            OrientationText.text = $"Orientation error: {orientationError:F2} deg (required: <{orientationTolerance:F2} deg)";

            if (displacementError.magnitude < displacementTolerance && orientationError < orientationTolerance)
            {
                testHasCompleted = true;
            } else
            {
                if (testHasCompleted)
                {
                    TimeText.color = Color.white;
                    DisplacementText.color = Color.white;
                    OrientationText.color = Color.white;
                    floorplan.material.color = Color.white;
                }
                testHasCompleted = false;
                testTime += Time.deltaTime;
            }
        }

        if (testHasCompleted)
        {
            TimeText.text = "Test complete! " + testTime.ToString("F2") + " sec";
            TimeText.color = Color.green;
            DisplacementText.color = Color.green;
            OrientationText.color = Color.green;
            floorplan.material.color = Color.green;
        }

        var x_a = (new Vector3(testObject.position.x, 0, testObject.position.z)).magnitude;
        var theta_b = Vector3.Angle(testObject.position, testGoal.position - testObject.position);
        var x_b = (testObject.position - testGoal.position).magnitude;
        var theta_c = (testGoal.rotation.eulerAngles).y;

        infoText.text = $"x_a = {x_a} m\ntheta_b = {theta_b} deg\nx_b = {x_b} m\ntheta_c = {theta_c} deg";
    }
}
