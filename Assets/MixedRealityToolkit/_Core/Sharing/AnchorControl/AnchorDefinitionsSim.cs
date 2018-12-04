using System.Collections.Generic;
using UnityEngine;

namespace Pixie.AnchorControl
{
    public class AnchorDefinitionsSim : MonoBehaviour, IAnchorDefinitions, IAnchorMatrixSource
    {
        public bool Ready
        {
            get { return true; }
        }

        public IEnumerable<AnchorDefinition> Definitions
        {
            get
            {
                foreach (Transform simulatedAnchor in simulatedAnchors)
                {
                    AnchorDefinition definition = new AnchorDefinition();
                    definition.Position = simulatedAnchor.position;
                    definition.Rotation = simulatedAnchor.eulerAngles;
                    definition.ID = simulatedAnchor.name;

                    yield return definition;
                }
                yield break;
            }
        }

        public bool GetAnchorMatrix(string anchorID, out Matrix4x4 matrix)
        {
            foreach (Transform simulatedAnchor in simulatedAnchors)
            {
                if (simulatedAnchor.name == anchorID)
                {
                    matrix = simulatedAnchor.localToWorldMatrix;
                    return true;
                }
            }
            matrix = Matrix4x4.identity;
            return false;
        }

        [SerializeField]
        private Transform[] simulatedAnchors;

        public void FetchDefinitions()
        {
            // Do nothing
        }
    }
}