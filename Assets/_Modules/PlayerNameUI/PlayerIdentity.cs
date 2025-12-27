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
                SendNameToServer(PlayerData.Instance.UserName);
            }
        }
    }

    // Client → Server
    [ServerRpc]
    private void SendNameToServer(string name)
    {
        // ✅ Update host locally
        PlayerName = name;

        // ✅ Update all clients
        BroadcastName(name);
    }

    // Server → Clients ONLY (not host)
    [ObserversRpc]
    private void BroadcastName(string name)
    {
        PlayerName = name;
    }

    public bool IsLocalPlayer()
    {
        var identity = GetComponent<NetworkIdentity>();
        return identity != null && identity.isOwner;
    }
}
