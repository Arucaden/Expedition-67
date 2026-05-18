using UnityEngine;
using Expedition67.Player;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Expedition67.Core
{
    public class GameManager : MonoBehaviour
    {
        public PlayerController player;
        public InputActionReference pauseAction;

        private bool gameEnded = false;
        private bool isPaused = false;

        public static GameManager Instance { get; private set; }

        private void OnEnable()
        {
            GameEvents.OnPlayerCaught += HandlePlayerCaught;
            GameEvents.OnLevelCompleted += HandleLevelFinished;

            if (pauseAction != null)
            {
                pauseAction.action.Enable();
                pauseAction.action.performed += OnPausePerformed;
            }
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerCaught -= HandlePlayerCaught;
            GameEvents.OnLevelCompleted -= HandleLevelFinished;

            if (pauseAction != null)
            {
                pauseAction.action.performed -= OnPausePerformed;
                pauseAction.action.Disable();
            }
        }

        private void Awake()
        {
            Instance = this;
            Time.timeScale = 1f;
        }

        public PlayerController GetPlayer()
        {
            return player;
        }

        private void OnPausePerformed(InputAction.CallbackContext context)
        {
            TogglePause();
        }

        public void TogglePause()
        {
            if (gameEnded) 
            {
                return;
            }

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            if (gameEnded || isPaused) 
            {
                return;
            }

            isPaused = true;
            Time.timeScale = 0f;
            GameEvents.OnGamePaused?.Invoke();
        }

        public void ResumeGame()
        {
            if (gameEnded || !isPaused) 
            {
                return;
            }

            isPaused = false;
            Time.timeScale = 1f;
            GameEvents.OnGameResumed?.Invoke();
        }

        public void RestartLevel()
        {
            Time.timeScale = 1f;
            GameEvents.OnLevelRestarted?.Invoke();
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        public void QuitGame()
        {
            Time.timeScale = 1f;
            Debug.Log("QUIT");
            Application.Quit();
        }

        private void HandlePlayerCaught()
        {
            if (gameEnded) 
            {
                return;
            }

            gameEnded = true;
            Time.timeScale = 0f;

            Debug.Log("GAME OVER");
            KillPlayer();
        }

        private void HandleLevelFinished()
        {
            if (gameEnded) 
            {
                return;
            }
            gameEnded = true;
            Time.timeScale = 0f;

            Debug.Log("LEVEL COMPLETED");
            KillPlayer();
        }

        private void KillPlayer()
        {
            if (player != null)
            {
                Destroy(player.gameObject);
            }
        }
    }
}
