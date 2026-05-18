using UnityEngine;
using UnityEngine.SceneManagement;

namespace Expedition67.Core
{
    public class MainMenu : MonoBehaviour
    {
        public string levelToLoad = "Lvl1";

        public void PlayGame()
        {
            SceneManager.LoadScene(levelToLoad);
        }

        public void QuitGame()
        {
            Debug.Log("EXIT");
            
            Application.Quit();
        }
    }
}
