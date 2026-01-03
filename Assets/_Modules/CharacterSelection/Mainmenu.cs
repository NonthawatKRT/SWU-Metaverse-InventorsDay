using UnityEngine;

public class Mainmenu : MonoBehaviour
{
    [SerializeField] private GameObject[] characters; // Assign characters in the Inspector

    void Start()
    {
        DisableAllCharacters();
        ActivateSelectedCharacter();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void DisableAllCharacters()
    {
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }
    }

    public void ActivateSelectedCharacter()
    {
        int selectedIndex = CharacterManager.Instance.SelectedCharacterIndex;
        characters[selectedIndex].SetActive(true);
    }

    public void SelectCharacter(int index)
    {
        CharacterManager.Instance.SelectCharacter(index);
        DisableAllCharacters();
        characters[index].SetActive(true);
    }

    public void ConfirmButton()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Mainmap");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame(){
        UnityEngine.SceneManagement.SceneManager.LoadScene("Mainmap");
    }

    void Update()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
