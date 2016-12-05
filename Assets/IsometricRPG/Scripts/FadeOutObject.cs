using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// This ended up being just a hide class after all. Not enough time to do this.
    /// (Transparent materials messed with Z ordering and torch effect didn't work)
    /// If I had more time I would implement this right. But there are other projects
    /// to get done sadly...
    /// </summary>
    public class FadeOutObject : MonoBehaviour
    {
        private Renderer _renderer;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();
        }

        public void FadeOut()
        {
            _renderer.enabled = false;
        }

        public void FadeIn()
        {
            _renderer.enabled = true;
        }
    }

}