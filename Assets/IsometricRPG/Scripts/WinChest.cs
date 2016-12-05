using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Wacki.IsoRPG
{

    /// <summary>
    /// Yea, so I had less time in the end than I thought. So this is now our win state.
    /// </summary>
    public class WinChest : MonoBehaviour
    {

        void OnTriggerEnter(Collider other)
        {
            if(other.GetComponent<PlayerController>() != null)
            {
                SceneManager.LoadScene("win_scene");
            }
        }
    }

}