using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{
    /// <summary>
    /// Quickly hacked together uv scaling, because unity's level design tools
    /// suuuuck
    /// Would be handy if this worked in editor but there's no time...
    /// </summary>
    public class DynamicUVScaling : MonoBehaviour
    {

        // how much is the mesh scaled down?
        public Vector2 scaleFactor;

        public Vector2 offsetModifier = new Vector2(1, 1);

        // keep updating while game is running
        public bool continuesUpdate = false;

        // keep track of original uv scale
        private Vector2 originalUvScale;

        // Use this for initialization
        void Awake()
        {
            var mat = GetComponent<Renderer>().material;
            originalUvScale = mat.mainTextureScale;

            UpdateUVScale();
        }

        void Update()
        {
            if (continuesUpdate)
                UpdateUVScale();
        }

        // do the actual work
        private void UpdateUVScale()
        {
            var mat = GetComponent<Renderer>().material;
            // we want to set the uv scaling based on the transforms local scale factor
            var localXYScale = new Vector2(transform.localScale.x, transform.localScale.z);
            localXYScale.x *= originalUvScale.x;
            localXYScale.y *= originalUvScale.y;
            
            mat.mainTextureScale = localXYScale;

            // take rotation into account 
            // needed for walls to properly tile
            var localXYOffset = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.y) * new Vector2(transform.position.x, transform.position.z);
            //localXYScale = new Vector2(transform.localScale.x, transform.localScale.z);
            localXYOffset.x *= originalUvScale.x;
            localXYOffset.y *= originalUvScale.y;
            localXYOffset.x *= offsetModifier.x;
            localXYOffset.y *= offsetModifier.y;

            mat.mainTextureOffset = -localXYOffset;
        }
    }

}