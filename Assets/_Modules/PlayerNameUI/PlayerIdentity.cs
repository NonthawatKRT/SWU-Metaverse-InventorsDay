using UnityEngine;
using PurrNet;

public class PlayerIdentity : NetworkBehaviour
{
    [SerializeField] private string playerName;

    public string PlayerName
    {
        get => playerName;
        private set
        {
            if (playerName != value)
            {
                playerName = value;
                OnPlayerNameChanged?.Invoke(value);
            }
        }
    }

    public event System.Action<string> OnPlayerNameChanged;

    private void Start()
    {
        var identity = GetComponent<NetworkIdentity>();

        if (identity != null && identity.isOwner)
        {
            if (PlayerData.Instance != null &&
                !string.IsNullOrEmpty(PlayerData.Instance.UserName))
            {
                // Send name to server
                SendNameToServer(PlayerData.Instance.UserName);
            }
        }
    }

    // 🔹 Owner → Server
    [ServerRpc]
    private void SendNameToServer(string name)
    {
        // Server validates if needed
        BroadcastName(name);
    }

    // 🔹 Server → Everyone
    [ObserversRpc]
    private void BroadcastName(string name)
    {
        PlayerName = name;
    }
}
