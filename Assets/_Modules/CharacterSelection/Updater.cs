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

    protected override void OnSpawned()
    {
        if (!isOwner)
            return;

        int selectedIndex = CharacterManager.Instance.SelectedCharacterIndex;
        SetCharacterIndex(selectedIndex);
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
    // public void SetCharacterIndex(int newIndex)
    // {
    //     if (!isOwner)
    //     {
    //         Debug.LogWarning("[Updater] Only the owner can change character selection");
    //         return;
    //     }

    //     Debug.Log($"[Updater] Setting character index to: {newIndex}");
    //     UpdateCharacterDisplay(newIndex);
    //     UpdateCharacterIndexRpc(newIndex);
    // }

    public void SetCharacterIndex(int newIndex)
    {
        if (!isOwner)
            return;

        Debug.Log($"[Updater] Owner requests character index: {newIndex}");

        // Send request to server
        SetCharacterIndexServerRpc(newIndex);
    }

    [ServerRpc]
    private void SetCharacterIndexServerRpc(int newIndex)
    {
        Debug.Log($"[Updater] Server received character index: {newIndex} from {owner?.id}");

        // Optional validation
        if (newIndex < 0 || newIndex >= characters.Length)
            return;

        currentCharacterIndex = newIndex;

        // Broadcast to everyone
        UpdateCharacterIndexObserversRpc(newIndex);
    }

    [ObserversRpc]
    private void UpdateCharacterIndexObserversRpc(int newCharacterIndex)
    {
        Debug.Log($"[Updater] Observers update index {newCharacterIndex} for player {owner?.id}");

        currentCharacterIndex = newCharacterIndex;
        UpdateCharacterDisplay(newCharacterIndex);
    }




    /// <summary>
    /// Get the current character index
    /// </summary>
    public int GetCurrentCharacterIndex()
    {
        return currentCharacterIndex;
    }
}
