using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace Expedition67.Core
{
    public class UIManager : MonoBehaviour
    {
        [Header("Panels")]
        public GameObject pausePanel;
        public GameObject gameOverPanel;
        public GameObject levelCompletePanel;

        private void OnEnable()
        {
            GameEvents.OnPlayerCaught += HandleGameOver;
            GameEvents.OnLevelCompleted += HandleLevelFinished;
            GameEvents.OnGamePaused += HandleGamePaused;
            GameEvents.OnGameResumed += HandleGameResumed;
        }

        private void OnDisable()
        {
            GameEvents.OnPlayerCaught -= HandleGameOver;
            GameEvents.OnLevelCompleted -= HandleLevelFinished;
            GameEvents.OnGamePaused -= HandleGamePaused;
            GameEvents.OnGameResumed -= HandleGameResumed;
        }

        private void Start()
        {
            if (pausePanel != null) pausePanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);
            if (levelCompletePanel != null) levelCompletePanel.SetActive(false);
        }

        private void HandleGamePaused()
        {
            pausePanel.SetActive(true);
        }

        private void HandleGameResumed()
        {
            pausePanel.SetActive(false);
        }

        private void HandleGameOver()
        {
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(true);
        }

        private void HandleLevelFinished()
        {
            pausePanel.SetActive(false);
            gameOverPanel.SetActive(false);
            levelCompletePanel.SetActive(true);
        }

        public void TogglePause()
        {
            GameManager.Instance.TogglePause();
        }

        public void ResumeGame()
        {
            GameManager.Instance.ResumeGame();
        }

        public void RestartGame()
        {
            GameManager.Instance.RestartLevel();
        }

        public void QuitGame()
        {
            GameManager.Instance.QuitGame();
        }
    }
}
