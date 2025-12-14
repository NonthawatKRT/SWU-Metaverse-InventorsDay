using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Updater : MonoBehaviour
{
    [SerializeField] private GameObject[] characters; // Assign characters in the Inspector

    [SerializeField] private Avatar[] avatars;
    [SerializeField] private Animator animators;
    void Start()
    {
        ActivateSelectedCharacter();
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

        animators.avatar = avatars[selectedIndex];
    }
}
