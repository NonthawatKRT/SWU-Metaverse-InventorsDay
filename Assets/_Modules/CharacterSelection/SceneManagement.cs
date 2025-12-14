
using UnityEngine;
using PurrNet;
using PurrNet.Modules;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;
using PurrLobby;

public class SceneManagement : NetworkBehaviour
{
    public GameObject WaitingforPlayersUI;
    [PurrScene] public string sceneToChange;
    [SerializeField] private LobbyManager lobbyManager;
    
    private HashSet<PlayerID> readyPlayers = new HashSet<PlayerID>();
    private HashSet<PlayerID> connectedPlayers = new HashSet<PlayerID>();
    private int totalPlayers = 0;

    private void Awake()
    {
        Debug.Log($"[SceneManagement] Awake - GameObject: {gameObject.name}");
    }

    private void Start()
    {
        Debug.Log($"[SceneManagement] Start - IsSpawned: {isSpawned}, IsServer: {isServer}, IsOwner: {isOwner}");
        
        if (!isSpawned)
        {
            Debug.LogWarning("[SceneManagement] NetworkBehaviour is not spawned! Make sure this GameObject has a NetworkIdentity and is spawned by the NetworkManager.");
        }
    }

    [ContextMenu("Change Scene")]

    public void ChangeScene()
    {
        Debug.Log($"[SceneManagement] ChangeScene called for scene: {sceneToChange}");
        
        if (lobbyManager != null)
        {
            lobbyManager.SetLobbyStarted();
        }
        
        SceneManager.LoadSceneAsync(sceneToChange);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestSceneChange(RPCInfo info = default)
    {
        Debug.Log($"[SceneManagement] Server received scene change request from player: {info.sender}");
        
        // Use the lobby system to transition all players
        if (lobbyManager != null)
        {
            lobbyManager.SetLobbyStarted();
        }
        
        SceneManager.LoadSceneAsync(sceneToChange);
    }

    [ContextMenu("Request Scene Change")]
    public void RequestSceneChangeClient()
    {
        Debug.Log($"[SceneManagement] RequestSceneChangeClient called. IsSpawned: {isSpawned}, IsServer: {isServer}, IsOwner: {isOwner}");
        
        if (!isSpawned)
        {
            Debug.LogError("[SceneManagement] Cannot request scene change - NetworkBehaviour is not spawned!");
            return;
        }
        
        RequestSceneChange();
    }

    // Call this method when a player clicks "Ready" button
    public void MarkPlayerReady()
    {
        Debug.Log($"[SceneManagement] MarkPlayerReady called. IsSpawned: {isSpawned}, IsServer: {isServer}, IsOwner: {isOwner}");
        
        if (!isSpawned)
        {
            Debug.LogError("[SceneManagement] Cannot mark player ready - NetworkBehaviour is not spawned!");
            return;
        }
        
        MarkPlayerReadyServerRpc();
    }

    // Call this when a player joins the scene
    [ServerRpc(requireOwnership: false)]
    public void RegisterPlayerServerRpc(RPCInfo info = default)
    {
        connectedPlayers.Add(info.sender);
        totalPlayers = connectedPlayers.Count;
        UpdateReadyStatusClientRpc(readyPlayers.Count, totalPlayers);
    }

    [ServerRpc(requireOwnership: false)]
    private void MarkPlayerReadyServerRpc(RPCInfo info = default)
    {
        // Make sure player is registered first
        connectedPlayers.Add(info.sender);
        
        // Add player to ready list
        readyPlayers.Add(info.sender);
        
        // Update total player count
        totalPlayers = connectedPlayers.Count;
        
        // Notify all clients about ready status update
        UpdateReadyStatusClientRpc(readyPlayers.Count, totalPlayers);
        
        // Check if all players are ready
        if (AreAllPlayersReady())
        {
            // Wait a moment then change scene
            Invoke(nameof(ChangeSceneWhenAllReady), 1f);
        }
    }

    [ObserversRpc]
    private void UpdateReadyStatusClientRpc(int readyCount, int totalCount)
    {
        // Update UI to show ready status
        UpdateReadyUI(readyCount, totalCount);
        
        // Show/hide waiting UI based on ready status
        if (WaitingforPlayersUI != null)
        {
            WaitingforPlayersUI.SetActive(readyCount < totalCount);
        }
    }

    private bool AreAllPlayersReady()
    {
        return readyPlayers.Count >= totalPlayers && totalPlayers > 0;
    }

    private void ChangeSceneWhenAllReady()
    {
        if (AreAllPlayersReady())
        {
            Debug.Log("[SceneManagement] All players ready! Transitioning to game scene...");
            
            if (lobbyManager != null)
            {
                lobbyManager.SetLobbyStarted();
            }
            
            SceneManager.LoadSceneAsync(sceneToChange);
        }
    }

    private void UpdateReadyUI(int readyCount, int totalCount)
    {
        // You can customize this to update your UI
        Debug.Log($"Players Ready: {readyCount}/{totalCount}");
        
        // Example: Update a text component if you have one
        // readyStatusText.text = $"Players Ready: {readyCount}/{totalCount}";
    }

    // Reset ready status (call this when scene starts)
    [ServerRpc(requireOwnership: false)]
    public void ResetReadyStatusServerRpc()
    {
        readyPlayers.Clear();
        // Keep the connected players but reset ready status
        totalPlayers = connectedPlayers.Count;
        UpdateReadyStatusClientRpc(0, totalPlayers);
    }

    // Optional: Remove player from ready list
    public void MarkPlayerNotReady()
    {
        Debug.Log($"[SceneManagement] MarkPlayerNotReady called. IsSpawned: {isSpawned}, IsServer: {isServer}, IsOwner: {isOwner}");
        
        if (!isSpawned)
        {
            Debug.LogError("[SceneManagement] Cannot mark player not ready - NetworkBehaviour is not spawned!");
            return;
        }
        
        MarkPlayerNotReadyServerRpc();
    }

    [ServerRpc(requireOwnership: false)]
    private void MarkPlayerNotReadyServerRpc(RPCInfo info = default)
    {
        readyPlayers.Remove(info.sender);
        totalPlayers = connectedPlayers.Count;
        UpdateReadyStatusClientRpc(readyPlayers.Count, totalPlayers);
    }

    // Call this when player joins the scene
    public void RegisterPlayer()
    {
        Debug.Log($"[SceneManagement] RegisterPlayer called. IsSpawned: {isSpawned}, IsServer: {isServer}, IsOwner: {isOwner}");
        
        if (!isSpawned)
        {
            Debug.LogError("[SceneManagement] Cannot register player - NetworkBehaviour is not spawned!");
            return;
        }
        
        RegisterPlayerServerRpc();
    }
}
