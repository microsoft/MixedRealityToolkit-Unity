using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class uguirenderer : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        CanvasRenderer cr = GetComponent<CanvasRenderer>();
        UIVertex vert = new UIVertex(); 
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        List<UIVertex> pVertexList = new List<UIVertex>();
        int[] indices = mesh.GetIndices(0);
        for (int i = 0; i < indices.Length; i += 3)
        {
            for (int p = 0; p < 3; p++)
            {
                vert = new UIVertex();
                vert.position = mesh.vertices[indices[i + p]];
                vert.normal = mesh.normals[indices[i + p]];
                vert.uv0 = mesh.uv[indices[i + p]];
                //do the same for tangent, uv1, and color if you need to.
                pVertexList.Add(vert);
            }
            //This just adds the last vertex twice to fit the quad format.
            pVertexList.Add(vert);
        }
        cr.Clear();
#pragma warning disable 618
        cr.SetVertices(pVertexList);
#pragma warning restore 618
        cr.SetMaterial(GetComponent<Image>().material, null);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
