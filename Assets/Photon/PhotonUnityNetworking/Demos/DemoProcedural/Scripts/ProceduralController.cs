using UnityEngine;

namespace Photon.Pun.Demo.Procedural
{
    /// <summary>
    /// Simple Input Handler to control the camera.
    /// </summary>
    public class ProceduralController : MonoBehaviour
    {
        private Camera cam;

        #region UNITY

        public void Awake()
        {
            cam = Camera.main;
        }

        /// <summary>
        /// Use horizontal and vertical axes (by default WASD or the arrow keys) for moving for-, back- or sidewards.
        /// Use E or Q for 'zooming' in or out.
        /// Use the left mouse button to decrease a Block's height 
        /// or the right mouse button to increase a Block's height.
        /// </summary>
        public void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");

            if (h >= 0.1f)
            {
                cam.transform.position += Vector3.right * 10.0f * Time.deltaTime;
            }
            else if (h <= -0.1f)
            {
                cam.transform.position += Vector3.left * 10.0f * Time.deltaTime;
            }

            if (v >= 0.1f)
            {
                cam.transform.position += Vector3.forward * 10.0f * Time.deltaTime;
            }
            else if (v <= -0.1f)
            {
                cam.transform.position += Vector3.back * 10.0f * Time.deltaTime;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                cam.transform.position += Vector3.up * 10.0f * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                cam.transform.position += Vector3.down * 10.0f * Time.deltaTime;
            }

            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 100.0f))
                {
                    Block b = hit.transform.GetComponent<Block>();

                    if (b != null)
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            WorldGenerator.Instance.DecreaseBlockHeight(b.ClusterId, b.BlockId);
                        }
                        else if (Input.GetMouseButtonDown(1))
                        {
                            WorldGenerator.Instance.IncreaseBlockHeight(b.ClusterId, b.BlockId);
                        }
                    }
                }
            }
        }

        #endregion
    }
}