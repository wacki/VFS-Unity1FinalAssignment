using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{
    /// <summary>
    /// Continously pan uv coordinates of a material
    /// </summary>
    public class AnimatedUVs : MonoBehaviour
    {
        // panning speed in uvs/s
        public Vector2 panningSpeed;
        Material _mat;

        // Use this for initialization
        void Awake()
        {
            _mat = GetComponent<Renderer>().material;
        }

        // Update is called once per frame
        void Update()
        {
            _mat.mainTextureOffset = _mat.mainTextureOffset + panningSpeed * Time.deltaTime;
        }
    }

}