using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{
    /// <summary>
    /// Well this seems to be only here so we can select the waypoints now...
    /// Pretty dumb
    /// </summary>
    public class PatrolPathWaypoint : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }

}