using UnityEngine;
using System.Collections.Generic;


namespace Wacki.IsoRPG
{

    /// <summary>
    /// This class will keep track of objects in its trigger space
    /// The objects need to match the given mask
    /// </summary>
    public class EnemyDetector : MonoBehaviour
    {
        private List<BaseUnit> _enemyCache = new List<BaseUnit>();
        public List<BaseUnit> enemyCache { get { return _enemyCache; } }

        public LayerMask enemyLayers;


        void OnTriggerEnter(Collider other)
        {
            var enemy = other.GetComponent<BaseUnit>();
            int objLayerMask = (1 << other.gameObject.layer);
            if (!((enemyLayers.value & objLayerMask) > 0) || enemy == null)
                return;

            if (!_enemyCache.Contains(enemy))
                _enemyCache.Add(enemy);
        }

        void OnTriggerExit(Collider other)
        {
            var enemy = other.GetComponent<BaseUnit>();
            int objLayerMask = (1 << other.gameObject.layer);
            if (!((enemyLayers.value & objLayerMask) > 0) || enemy == null)
                return;


            if (_enemyCache.Contains(enemy))
                _enemyCache.Remove(enemy);
        }
    }

}