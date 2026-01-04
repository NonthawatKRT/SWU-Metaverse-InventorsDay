using UnityEngine;
using PurrNet;

public class Updater : NetworkBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private Avatar[] avatars;
    [SerializeField] private Animator animator;

    private int currentCharacterIndex = -1;

    protected override void OnSpawned()
    {
        // 🔁 Always refresh visuals when spawned (late join safe)
        if (currentCharacterIndex >= 0)
            ApplyCharacter(currentCharacterIndex);

        // Owner tells host what they picked
        if (isOwner)
        {
            int selected = CharacterManager.Instance.SelectedCharacterIndex;
            SetCharacterServerRpc(selected);
        }
    }

    public void SetCharacterIndex(int index)
    {
        if (!isOwner)
            return;

        SetCharacterServerRpc(index);
    }

    [ServerRpc]
    private void SetCharacterServerRpc(int index)
    {
        if (index < 0 || index >= characters.Length)
            return;

        currentCharacterIndex = index;

        SyncCharacterObserversRpc(index);
    }

    [ObserversRpc]
    private void SyncCharacterObserversRpc(int index)
    {
        currentCharacterIndex = index;
        ApplyCharacter(index);
    }

    private void ApplyCharacter(int index)
    {
        for (int i = 0; i < characters.Length; i++)
            characters[i].SetActive(i == index);

        if (animator != null && index < avatars.Length)
            animator.avatar = avatars[index];
    }
}
