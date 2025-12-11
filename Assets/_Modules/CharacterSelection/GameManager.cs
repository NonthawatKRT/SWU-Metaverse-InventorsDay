using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject[] characters; // Assign characters in the Inspector
    public GameObject pausegameUI;  // Reference to the pause menu UI
    private bool isPlayerPaused = false;  // Flag to check if the player is paused

    void Start()
    {
        ActivateSelectedCharacter();
        ResumePlayer();  // Ensure game starts unpaused
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPlayerPaused)
                ResumePlayer();
            else
                PausePlayer();
        }
    }

    private void ActivateSelectedCharacter()
    {
        int selectedIndex = CharacterManager.Instance.SelectedCharacterIndex;

        // Disable all characters first
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // Activate the selected character
        characters[selectedIndex].SetActive(true);
    }

    public void PausePlayer()
    {
        isPlayerPaused = true;
        pausegameUI.SetActive(true);
        Time.timeScale = 0f;

        // Show the cursor
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Disable Player Controls
        /*var playerController = characters[CharacterManager.Instance.SelectedCharacterIndex]
            .GetComponentInChildren<StarterAssets.ThirdPersonController>();

        if (playerController != null)
        {
            playerController.TogglePause(true); // Call the pause function inside ThirdPersonController
        }
        else
        {
            Debug.LogWarning("Player controller not found in children!");
        }*/
    }

    public void ResumePlayer()
    {
        isPlayerPaused = false;
        pausegameUI.SetActive(false);
        Time.timeScale = 1f;

        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Enable Player Controls
        /*var playerController = characters[CharacterManager.Instance.SelectedCharacterIndex]
            .GetComponentInChildren<StarterAssets.ThirdPersonController>();

        if (playerController != null)
        {
            playerController.TogglePause(false); // Resume player controls
        }*/
    }

}
