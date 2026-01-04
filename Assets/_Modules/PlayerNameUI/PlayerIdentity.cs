using UnityEngine;
using PurrNet;

public class PlayerIdentity : NetworkBehaviour
{
    [SerializeField] private string playerName;

    public string PlayerName => playerName;

    public event System.Action<string> OnPlayerNameChanged;

    protected override void OnSpawned()
    {
        // Refresh UI for late join / scene reload
        if (!string.IsNullOrEmpty(playerName))
            OnPlayerNameChanged?.Invoke(playerName);

        // Owner sends name to host
        if (isOwner && PlayerData.Instance != null)
        {
            SetNameServerRpc(PlayerData.Instance.UserName);
        }
    }

    [ServerRpc]
    private void SetNameServerRpc(string name)
    {
        playerName = name;
        SyncNameObserversRpc(name);
    }

    [ObserversRpc]
    private void SyncNameObserversRpc(string name)
    {
        playerName = name;
        OnPlayerNameChanged?.Invoke(name);
    }
}
