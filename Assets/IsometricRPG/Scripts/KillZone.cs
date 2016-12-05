using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    public class KillZone : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            var unit = other.GetComponent<BaseUnit>();
            if(unit != null)
            {
                unit.Kill();
            }
        }
    }

}