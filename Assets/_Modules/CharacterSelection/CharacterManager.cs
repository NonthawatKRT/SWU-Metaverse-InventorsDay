using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    public static CharacterManager Instance { get; private set; }

    public int SelectedCharacterIndex { get; private set; } = 0; // Default character index

    private void Awake()
    {
        // Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Persist across scenes
        }
        else
        {
            Destroy(this.gameObject); // Prevent duplicates
        }
    }

    public void SelectCharacter(int index)
    {
        SelectedCharacterIndex = index;
        Debug.Log("Selected character: " + index);
    }
}
