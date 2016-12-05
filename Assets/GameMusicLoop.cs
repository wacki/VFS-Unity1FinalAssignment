using UnityEngine;
using System.Collections;


namespace Wacki.IsoRPG
{
    public class GameMusicLoop : MonoBehaviour
    {
        public static GameMusicLoop _instance = null;
        public static GameMusicLoop instance { get { return _instance; } }

        void Start()
        {
            if(_instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;

            DontDestroyOnLoad(gameObject);

        }

    }
}
