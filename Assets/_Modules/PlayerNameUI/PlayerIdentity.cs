using UnityEngine;

public class PlayerIdentity : MonoBehaviour
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
    
    private void Awake()
    {
        // If this is the local player, set the name from PlayerData
        if (IsLocalPlayer())
        {
            if (PlayerData.Instance != null && !string.IsNullOrEmpty(PlayerData.Instance.UserName))
            {
                PlayerName = PlayerData.Instance.UserName;
            }
        }
    }
    
    private bool IsLocalPlayer()
    {
        // Check if this is the local player by checking for ownership or other local player markers
        // This will depend on your networking setup - adjust as needed
        var networkIdentity = GetComponent<PurrNet.NetworkIdentity>();
        return networkIdentity != null && networkIdentity.isOwner;
    }
}
