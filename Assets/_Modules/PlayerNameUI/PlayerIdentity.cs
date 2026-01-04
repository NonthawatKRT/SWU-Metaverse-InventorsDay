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

    protected override void OnSpawned()
    {
        if (!isOwner)
            return;

        if (PlayerData.Instance != null &&
            !string.IsNullOrEmpty(PlayerData.Instance.UserName))
        {
            SendNameToServer(PlayerData.Instance.UserName);
        }
    }



    // Client → Server
    [ServerRpc]
    private void SendNameToServer(string name)
    {
        PlayerName = name; // server state
        BroadcastName(name);
    }

    [ObserversRpc]
    private void BroadcastName(string name)
    {
        if (isServer) return; // prevent double apply on host
        PlayerName = name;
    }


    public bool IsLocalPlayer()
    {
        var identity = GetComponent<NetworkIdentity>();
        return identity != null && identity.isOwner;
    }
}
