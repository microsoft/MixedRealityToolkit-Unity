using Pixie.AnchorControl;
using System.Collections.Generic;
using UnityEngine;

namespace Pixie.Demos
{
    public class SimulatedAnchorService : MonoBehaviour, IAnchorDefinitions, IAnchorMatrixSource
    {
        [SerializeField]
        private Transform[] anchorDefinitions;

        public bool Ready
        {
            get { return true; }
        }

        public IEnumerable<AnchorDefinition> Definitions
        {
            get
            {
                foreach (Transform anchorTransform in anchorDefinitions)
                {
                    AnchorDefinition anchorDefinition = new AnchorDefinition();
                    anchorDefinition.ID = anchorTransform.name;
                    anchorDefinition.Position = anchorTransform.position;
                    anchorDefinition.Rotation = anchorTransform.eulerAngles;

                    yield return anchorDefinition;
                }
            }
        }

        public void FetchDefinitions()
        {
            return;
        }

        public bool GetAnchorMatrix(string anchorID, out Matrix4x4 matrix)
        {
            matrix = Matrix4x4.identity;
            foreach (Transform anchorTransform in anchorDefinitions)
            {
                if (anchorTransform.name == anchorID)
                {
                    matrix = anchorTransform.localToWorldMatrix;
                    return true;
                }
            }
            return false;
        }
    }
}