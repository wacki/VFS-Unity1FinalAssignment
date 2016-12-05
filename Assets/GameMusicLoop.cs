using UnityEngine;
using System.Collections;


namespace Wacki.IsoRPG
{
    public class GameMusicLoop : MonoBehaviour
    {

        void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

    }
}
