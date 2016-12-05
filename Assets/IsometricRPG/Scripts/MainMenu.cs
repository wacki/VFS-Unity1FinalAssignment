using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace Wacki.IsoRPG
{
    
    public class MainMenu : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }

        public void Quit()
        {
            Application.Quit();
        }
    }
}
