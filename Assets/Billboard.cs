using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// Very simple "billboard" script
    /// technically not correct
    /// </summary>
    public class Billboard : MonoBehaviour
    {
        void Update()
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            transform.LookAt(cam.transform);
        }
    }

}