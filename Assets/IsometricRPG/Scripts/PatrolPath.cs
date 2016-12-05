using UnityEngine;
using System.Collections;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// Class handling a patrol path with waypoints
    /// In hindsight I would just use the path in NavMeshAgent
    /// and handle movement of the agent using a rigid body but yea..
    /// </summary>
    public class PatrolPath : MonoBehaviour
    {
        // list of waypoints  to traverse 
        public PatrolPathWaypoint[] waypoints;
        // should the player wrap around
        public bool wrapAround;
        

        // current movement direction
        private int _direction;
        // current waypoint index
        private int _currentIndex;

        // getter for current waypoint
        public PatrolPathWaypoint currentWaypoint
        {
            get
            {
                if (waypoints.Length < 1) return null;
                return waypoints[_currentIndex];
            }
        }

        void Awake()
        {
            transform.parent = null;
            _direction = 1;
        }

        // Advance to next waypoint
        public void NextWaypoint()
        {
            _currentIndex += _direction;

            if (!wrapAround && (_currentIndex == 0 || _currentIndex == (waypoints.Length - 1)))
            {
                _direction *= -1;
            }
            else {
                while (_currentIndex < 0)
                    _currentIndex += waypoints.Length;
                _currentIndex %= waypoints.Length;
            }
        }

        // Draw the path so we actually know what we're doing in the editor
        void OnDrawGizmos()
        {
            if (waypoints.Length < 2)
                return;

            PatrolPathWaypoint prevWp = waypoints[0];
            for (int i = 1; i < waypoints.Length; i++)
            {
                Gizmos.DrawLine(prevWp.transform.position, waypoints[i].transform.position);
                prevWp = waypoints[i];
            }

            if (wrapAround)
            {
                Gizmos.DrawLine(prevWp.transform.position, waypoints[0].transform.position);
            }
            
        }
        
    }

}