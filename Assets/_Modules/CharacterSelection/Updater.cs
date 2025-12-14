using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrNet;

public class Updater : NetworkBehaviour
{
    [SerializeField] private GameObject[] characters; // Assign characters in the Inspector

    [SerializeField] private Avatar[] avatars;
    [SerializeField] private Animator animators;

    private int currentCharacterIndex = 0;

    void Start()
    {
        if (isOwner)
        {
            ActivateSelectedCharacter();
        }
    }
    
    private void ActivateSelectedCharacter()
    {
        if (!isOwner) return;
        
        int selectedIndex = CharacterManager.Instance.SelectedCharacterIndex;
        Debug.Log($"[Updater] Owner setting character index to: {selectedIndex}");
        
        // Update local display
        UpdateCharacterDisplay(selectedIndex);
        
        // Notify all other players about the character change
        UpdateCharacterIndexRpc(selectedIndex);
    }
    
    [ObserversRpc]
    private void UpdateCharacterIndexRpc(int newCharacterIndex)
    {
        Debug.Log($"[Updater] Received character update: {newCharacterIndex} for player {owner?.id}");
        currentCharacterIndex = newCharacterIndex;
        
        // Update display for all players (including sender)
        UpdateCharacterDisplay(newCharacterIndex);
    }
    
    private void UpdateCharacterDisplay(int selectedIndex)
    {
        Debug.Log($"[Updater] Updating character display to index: {selectedIndex} for player {owner?.id}");
        
        // Validate index
        if (selectedIndex < 0 || selectedIndex >= characters.Length)
        {
            Debug.LogError($"[Updater] Invalid character index: {selectedIndex}");
            return;
        }
        
        // Disable all characters first
        foreach (GameObject character in characters)
        {
            character.SetActive(false);
        }

        // Activate the selected character
        characters[selectedIndex].SetActive(true);

        // Update animator avatar
        if (animators != null && selectedIndex < avatars.Length)
        {
            animators.avatar = avatars[selectedIndex];
        }
        
        currentCharacterIndex = selectedIndex;
    }
    
    /// <summary>
    /// Public method to update character selection (only works for owner)
    /// </summary>
    public void SetCharacterIndex(int newIndex)
    {
        if (!isOwner)
        {
            Debug.LogWarning("[Updater] Only the owner can change character selection");
            return;
        }
        
        Debug.Log($"[Updater] Setting character index to: {newIndex}");
        UpdateCharacterDisplay(newIndex);
        UpdateCharacterIndexRpc(newIndex);
    }
    
    /// <summary>
    /// Get the current character index
    /// </summary>
    public int GetCurrentCharacterIndex()
    {
        return currentCharacterIndex;
    }
}
