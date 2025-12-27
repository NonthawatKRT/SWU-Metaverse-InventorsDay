using UnityEngine;
using PurrNet;

public class PlayerIdentity : NetworkBehaviour
{
    [SerializeField] private string playerName;

    public string PlayerName
    {
        get => playerName;
        set
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
                PlayerName = PlayerData.Instance.UserName;
            }
        }
    }
}
