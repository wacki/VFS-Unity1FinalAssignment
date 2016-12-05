using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Wacki.IsoRPG
{

    public class IngameMenu : MonoBehaviour
    {
        public void BackToMainMenu()
        {
            SceneManager.LoadScene(0);
        }
    }
}
