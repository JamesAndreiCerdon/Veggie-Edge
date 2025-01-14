using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverPanel;  // Drag your Game Over Panel into this field in the inspector

    private bool isGameOver = false;

    void Start()
    {
        gameOverPanel.SetActive(false);  // Hide the game over panel at the start
    }

    public void GameOver()
    {
        if (!isGameOver)
        {
            isGameOver = true;
            gameOverPanel.SetActive(true);  // Show the game over panel
            Time.timeScale = 0f;  // Pause the game (optional)
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;  // Unpause the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);  // Restart the current scene
    }

    // New function to return to the main menu
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;  // Ensure the game is unpaused
        SceneManager.LoadScene("MainMenu");  // Load the Main Menu scene (replace "MainMenu" with your actual scene name)
    }
}
