using UnityEngine;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;

namespace HoloToolkit.Unity.InputModule.Tests
{
    /// <summary>
    /// Test MonoBehaviour that can be attached to any game object which has TwoHandManipulatable(with Manipulation Mode as Rotate) 
    /// attached to make the rotation axis constraint toggle between X and Y axis on tapping the object.
    /// </summary>
    [RequireComponent(typeof(TwoHandManipulatable))]
    public class TestRotationConstraints : MonoBehaviour, IInputClickHandler
    {
        /// <summary>
        /// The TextMesh game object showing the description of the manipulation.
        /// </summary>
        [SerializeField]
        private TextMesh descriptionText = null;
        /// <summary>
        /// The TwoHandManipulatable object to change the rotation constraints at run time.
        /// </summary>
        private TwoHandManipulatable twoHandManipulatable;

        private const string DescriptionPrefix = "Rotate\n";
        private const string XAxisConstraint = "X axis Constraint";
        private const string YAxisConstraint = "Y axis Constraint";
        private const string DescriptionPostfix = "\nTap on the model to toggle between X and Y constraints";

        // Use this for initialization
        private void Start()
        {
            twoHandManipulatable = GetComponent<TwoHandManipulatable>();
        }

        public void OnInputClicked(InputClickedEventData eventData)
        {
            if (twoHandManipulatable != null)
            {
                bool isXAxisConstraint = twoHandManipulatable.RotationConstraint == AxisConstraint.XAxisOnly;
                twoHandManipulatable.RotationConstraint = isXAxisConstraint ? AxisConstraint.YAxisOnly : AxisConstraint.XAxisOnly;
                if (descriptionText != null)
                {
                    descriptionText.text = DescriptionPrefix + (isXAxisConstraint ? YAxisConstraint : XAxisConstraint) + DescriptionPostfix;
                }
            }
        }
    }
}