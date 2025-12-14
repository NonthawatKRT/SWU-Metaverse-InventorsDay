
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
    
    private LobbyManager GetLobbyManager()
    {
        // Try to find lobby manager in current scene or DontDestroyOnLoad
        return FindObjectOfType<LobbyManager>();
    }
    
    private HashSet<PlayerID> readyPlayers = new HashSet<PlayerID>();

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
        
        LobbyManager lobby = GetLobbyManager();
        if (lobby != null)
        {
            Debug.Log("[SceneManagement] Found Lobby Manager, setting lobby started");
            lobby.SetLobbyStarted();
        }
        else
        {
            Debug.LogWarning("[SceneManagement] No Lobby Manager found, proceeding without it");
        }
        
        SceneManager.LoadSceneAsync(sceneToChange);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestSceneChange(RPCInfo info = default)
    {
        Debug.Log($"[SceneManagement] Server received scene change request from player: {info.sender}");
        
        // Use the lobby system to transition all players
        LobbyManager lobby = GetLobbyManager();
        if (lobby != null)
        {
            Debug.Log("[SceneManagement] Found Lobby Manager, setting lobby started");
            lobby.SetLobbyStarted();
        }
        else
        {
            Debug.LogWarning("[SceneManagement] No Lobby Manager found, proceeding without it");
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

    [ServerRpc(requireOwnership: false)]
    private void MarkPlayerReadyServerRpc(RPCInfo info = default)
    {
        // Add player to ready list
        readyPlayers.Add(info.sender);
        
        // Get connected players from NetworkManager
        int totalConnected = GetConnectedPlayersCount();
        
        Debug.Log($"[SceneManagement] Player {info.sender} marked ready. Ready: {readyPlayers.Count}/{totalConnected}");
        
        // Notify all clients about ready status update
        UpdateReadyStatusClientRpc(readyPlayers.Count, totalConnected);
        
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
        int totalConnected = GetConnectedPlayersCount();
        return readyPlayers.Count >= totalConnected && totalConnected > 0;
    }

    private int GetConnectedPlayersCount()
    {
        if (networkManager == null) return 0;
        
        // Try different ways to get player count from PurrNet
        try
        {
            // Method 1: Check if there's a players collection
            var playersField = networkManager.GetType().GetField("players", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (playersField != null)
            {
                var players = playersField.GetValue(networkManager);
                if (players is System.Collections.ICollection collection)
                {
                    return collection.Count;
                }
            }
            
            // Method 2: Use lobby manager if available
            LobbyManager lobby = GetLobbyManager();
            if (lobby != null)
            {
                var lobbyPlayersField = lobby.GetType().GetField("connectedPlayers", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (lobbyPlayersField != null)
                {
                    var lobbyPlayers = lobbyPlayersField.GetValue(lobby);
                    if (lobbyPlayers is System.Collections.ICollection lobbyCollection)
                    {
                        return lobbyCollection.Count;
                    }
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogWarning($"[SceneManagement] Failed to get player count: {ex.Message}");
        }
        
        // Fallback: assume at least 1 if we have any ready players
        return readyPlayers.Count > 0 ? Mathf.Max(1, readyPlayers.Count) : 1;
    }

    private void ChangeSceneWhenAllReady()
    {
        if (AreAllPlayersReady())
        {
            Debug.Log("[SceneManagement] All players ready! Transitioning to game scene...");
            
            LobbyManager lobby = GetLobbyManager();
            if (lobby != null)
            {
                Debug.Log("[SceneManagement] Found Lobby Manager, setting lobby started");
                lobby.SetLobbyStarted();
            }
            else
            {
                Debug.LogWarning("[SceneManagement] No Lobby Manager found, proceeding without it");
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
        int totalConnected = GetConnectedPlayersCount();
        UpdateReadyStatusClientRpc(0, totalConnected);
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
        int totalConnected = GetConnectedPlayersCount();
        UpdateReadyStatusClientRpc(readyPlayers.Count, totalConnected);
    }

    // Optional: Call this to refresh ready status UI
    public void RefreshReadyStatus()
    {
        if (isServer && isSpawned)
        {
            int totalConnected = GetConnectedPlayersCount();
            UpdateReadyStatusClientRpc(readyPlayers.Count, totalConnected);
        }
    }

    [ContextMenu("Log All Players")]
    public void LogAllPlayers()
    {
        LogAllPlayersInScene();
    }

    public void LogAllPlayersInScene()
    {
        Debug.Log("=== ALL PLAYERS IN SCENE ===");
        
        if (networkManager == null)
        {
            Debug.LogWarning("[SceneManagement] NetworkManager is null - cannot get player list");
            return;
        }

        try
        {
            // Method 1: Try to get players from NetworkManager
            var playersField = networkManager.GetType().GetField("players", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (playersField != null)
            {
                var players = playersField.GetValue(networkManager);
                if (players is System.Collections.IDictionary playerDict)
                {
                    Debug.Log($"[SceneManagement] Found {playerDict.Count} players in NetworkManager:");
                    int index = 0;
                    foreach (System.Collections.DictionaryEntry entry in playerDict)
                    {
                        var playerId = entry.Key;
                        var isReady = readyPlayers.Contains((PlayerID)playerId);
                        Debug.Log($"  Player {index}: ID={playerId}, Ready={isReady}");
                        index++;
                    }
                    Debug.Log($"[SceneManagement] Ready Players: {readyPlayers.Count}/{playerDict.Count}");
                    return;
                }
                else if (players is System.Collections.ICollection playerCollection)
                {
                    Debug.Log($"[SceneManagement] Found {playerCollection.Count} players in NetworkManager:");
                    int index = 0;
                    foreach (var player in playerCollection)
                    {
                        var isReady = readyPlayers.Contains((PlayerID)player);
                        Debug.Log($"  Player {index}: ID={player}, Ready={isReady}");
                        index++;
                    }
                    Debug.Log($"[SceneManagement] Ready Players: {readyPlayers.Count}/{playerCollection.Count}");
                    return;
                }
            }

            // Method 2: Try lobby manager
            LobbyManager lobby = GetLobbyManager();
            if (lobby != null)
            {
                var lobbyPlayersField = lobby.GetType().GetField("connectedPlayers", 
                    System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                
                if (lobbyPlayersField != null)
                {
                    var lobbyPlayers = lobbyPlayersField.GetValue(lobby);
                    if (lobbyPlayers is System.Collections.ICollection lobbyCollection)
                    {
                        Debug.Log($"[SceneManagement] Found {lobbyCollection.Count} players in LobbyManager:");
                        int index = 0;
                        foreach (var player in lobbyCollection)
                        {
                            var isReady = readyPlayers.Contains((PlayerID)player);
                            Debug.Log($"  Player {index}: ID={player}, Ready={isReady}");
                            index++;
                        }
                        Debug.Log($"[SceneManagement] Ready Players: {readyPlayers.Count}/{lobbyCollection.Count}");
                        return;
                    }
                }
            }

            // Method 3: Just show ready players if we can't find the full list
            Debug.Log($"[SceneManagement] Could not access full player list. Ready players only:");
            int readyIndex = 0;
            foreach (var readyPlayer in readyPlayers)
            {
                Debug.Log($"  Ready Player {readyIndex}: ID={readyPlayer}");
                readyIndex++;
            }
            Debug.Log($"[SceneManagement] Total ready: {readyPlayers.Count}");
            
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[SceneManagement] Error logging players: {ex.Message}");
            
            // Fallback: just show what we know
            Debug.Log($"[SceneManagement] Fallback - Ready players: {readyPlayers.Count}");
            foreach (var readyPlayer in readyPlayers)
            {
                Debug.Log($"  Ready: {readyPlayer}");
            }
        }
        
        Debug.Log("=== END PLAYER LIST ===");
    }
}
