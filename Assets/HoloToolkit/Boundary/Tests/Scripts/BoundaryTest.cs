using UnityEngine;

namespace HoloToolkit.Unity.Boundary.Tests
{
    public class BoundaryTest : MonoBehaviour
    {
        private Material[] defaultMaterials;

        private void Start()
        {
            BoundaryManager.Instance.RenderBoundary = true;
            BoundaryManager.Instance.RenderFloor = true;

            defaultMaterials = GetComponent<Renderer>().materials;
        
            if (BoundaryManager.Instance.ContainsObject(gameObject.transform.position))
            {
                Debug.LogFormat("Object is within established boundary. Position: {0}", gameObject.transform.position);

                for (int i = 0; i < defaultMaterials.Length; i++)
                {
                    // Highlight the material if object is within specified boundary.
                    //defaultMaterials[i].SetFloat("_Highlight", 0.35f);
                    Color highlightColor = Color.green;
                    defaultMaterials[i].SetColor("_Color", highlightColor);
                }
            }
        }
    }
}