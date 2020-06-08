using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(ParentConstraint))]
public class DemoSceneUnderstandingStuffController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var source = new ConstraintSource
        {
            sourceTransform = Camera.main.transform,
            weight = 1.0f
        };
        GetComponent<ParentConstraint>().AddSource(source);
    }
}
