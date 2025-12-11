using UnityEngine;
using UnityEngine.SceneManagement; // For scene management

public class PauseMenuUI : MonoBehaviour
{
    public GameObject pauseGameUI;  // Reference to the Pause Menu UI
    public GameManager gameManager; // Reference to the Game Manager
    private bool isPaused = false;  // Bool to check if the game is paused

    void Update()
    {
        // Listen for the Escape key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                ResumeGame();  // Resume the game if it's already paused
            }
            else
            {
                PauseGame();   // Pause the game
            }
        }
    }

    public void Test()
    {
        Debug.Log("Test");
    }

    public void ResumeGame()
    {
        pauseGameUI.SetActive(false);  // Hide the pause menu
        Time.timeScale = 1f;           // Resume the game (unpause)
        isPaused = false;              // Update paused state
        gameManager.ResumePlayer();    // Re-enable player movement

        // Hide and lock the cursor when resuming
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void PauseGame()
    {
        pauseGameUI.SetActive(true);   // Show the pause menu
        Time.timeScale = 0f;           // Freeze the game (pause)
        isPaused = true;               // Update paused state
        gameManager.PausePlayer();     // Disable player movement

        // Show and unlock the cursor when pausing
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ExitToMainMenu()
    {
        ResumeGame();  // Resume the game before exiting
        // Unlock and show cursor before exiting
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        // Load the main menu scene
        SceneManager.LoadScene("CharacterSelect");
    }
}
