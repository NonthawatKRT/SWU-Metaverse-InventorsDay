using UnityEngine;
using PurrNet;

public class Updater : NetworkBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private Avatar[] avatars;
    [SerializeField] private Animator animators;

    private int currentCharacterIndex = -1;

    protected override void OnSpawned()
    {
        // 🔁 Late join fix:
        // When this object spawns on ANY client,
        // re-apply the current character if we already have one
        if (currentCharacterIndex >= 0)
        {
            UpdateCharacterDisplay(currentCharacterIndex);
        }

        // Owner sends their selection ONCE
        if (isOwner)
        {
            int selectedIndex = CharacterManager.Instance.SelectedCharacterIndex;
            SendCharacterToHost(selectedIndex);
        }
    }

    public void SetCharacterIndex(int newIndex)
    {
        if (!isOwner)
            return;

        SendCharacterToHost(newIndex);
    }

    private void SendCharacterToHost(int index)
    {
        if (index < 0 || index >= characters.Length)
            return;

        // Owner updates host
        SetCharacterIndexServerRpc(index);
    }

    [ServerRpc]
    private void SetCharacterIndexServerRpc(int index)
    {
        currentCharacterIndex = index;

        // 🔔 Broadcast to ALL current observers
        UpdateCharacterIndexObserversRpc(index);
    }

    [ObserversRpc]
    private void UpdateCharacterIndexObserversRpc(int index)
    {
        currentCharacterIndex = index;
        UpdateCharacterDisplay(index);
    }

    private void UpdateCharacterDisplay(int index)
    {
        if (index < 0 || index >= characters.Length)
            return;

        foreach (var character in characters)
            character.SetActive(false);

        characters[index].SetActive(true);

        if (animators != null && index < avatars.Length)
            animators.avatar = avatars[index];
    }

    public int GetCurrentCharacterIndex() => currentCharacterIndex;
}
