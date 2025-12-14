
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
    [Header("Debug Settings")]
    public bool allowSinglePlayerTesting = false;
    
    private LobbyManager GetLobbyManager()
    {
        // Try to find lobby manager in current scene or DontDestroyOnLoad
        return FindObjectOfType<LobbyManager>();
    }
    
    private HashSet<PlayerID> readyPlayers = new HashSet<PlayerID>();
    private HashSet<PlayerID> connectedPlayers = new HashSet<PlayerID>();
    private bool hasTriedToRegister = false;

    // Call this when a player joins the character selection scene
    [ServerRpc(requireOwnership: false)]
    public void RegisterPlayerServerRpc(RPCInfo info = default)
    {
        if (connectedPlayers.Contains(info.sender))
        {
            Debug.LogWarning($"[SceneManagement] Player {info.sender} already registered! Ignoring duplicate registration.");
            return;
        }
        
        connectedPlayers.Add(info.sender);
        Debug.Log($"[SceneManagement] Player {info.sender} registered. Total connected: {connectedPlayers.Count}");
        
        // List all connected players for debugging
        Debug.Log($"[SceneManagement] All connected players: [{string.Join(", ", connectedPlayers)}]");
        
        // Update UI for all clients
        UpdateReadyStatusClientRpc(readyPlayers.Count, connectedPlayers.Count);
    }
    
    // Call this when a player leaves
    [ServerRpc(requireOwnership: false)]
    public void UnregisterPlayerServerRpc(RPCInfo info = default)
    {
        connectedPlayers.Remove(info.sender);
        readyPlayers.Remove(info.sender); // Also remove from ready list
        Debug.Log($"[SceneManagement] Player {info.sender} unregistered. Total connected: {connectedPlayers.Count}");
        
        // Update UI for all clients
        UpdateReadyStatusClientRpc(readyPlayers.Count, connectedPlayers.Count);
    }

    public void RegisterPlayer()
    {
        if (hasTriedToRegister)
        {
            Debug.LogWarning("[SceneManagement] Player registration already attempted, skipping duplicate registration");
            return;
        }
        
        if (isSpawned)
        {
            Debug.Log($"[SceneManagement] Registering player for character selection. IsServer: {isServer}, IsOwner: {isOwner}");
            hasTriedToRegister = true;
            RegisterPlayerServerRpc();
        }
        else
        {
            Debug.LogWarning("[SceneManagement] Cannot register - not spawned yet, will retry...");
            // Retry after a short delay
            Invoke(nameof(RegisterPlayer), 0.2f);
        }
    }
    
    [ContextMenu("Register This Player")]
    public void ManualRegisterPlayer()
    {
        RegisterPlayer();
    }
    
    // Optional: Call this if you want to clear all player registrations
    [ContextMenu("Reset All Players")]
    public void ResetAllPlayers()
    {
        if (isServer)
        {
            ResetAllPlayersServerRpc();
            ResetRegistrationFlagsRpc(); // Also reset flags on all clients
        }
    }
    
    [ContextMenu("Clean Duplicate Players")]
    public void CleanDuplicatePlayers()
    {
        if (isServer)
        {
            // Keep only unique players (this is a debug helper)
            var uniqueConnected = new HashSet<PlayerID>(connectedPlayers);
            var uniqueReady = new HashSet<PlayerID>(readyPlayers);
            
            connectedPlayers = uniqueConnected;
            readyPlayers = uniqueReady;
            
            Debug.Log($"[SceneManagement] Cleaned duplicates. Connected: {connectedPlayers.Count}, Ready: {readyPlayers.Count}");
            UpdateReadyStatusClientRpc(readyPlayers.Count, connectedPlayers.Count);
        }
    }
    
    [ServerRpc(requireOwnership: false)]
    public void ResetAllPlayersServerRpc()
    {
        connectedPlayers.Clear();
        readyPlayers.Clear();
        Debug.Log("[SceneManagement] All players cleared from server");
        UpdateReadyStatusClientRpc(0, 0);
    }
    
    // Reset registration flag (call this on clients too)
    [ObserversRpc]
    private void ResetRegistrationFlagsRpc()
    {
        hasTriedToRegister = false;
        Debug.Log("[SceneManagement] Reset registration flags on all clients");
    }

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
        
        // Auto-register this player when they enter the character selection scene
        if (!hasTriedToRegister)
        {
            StartCoroutine(WaitAndRegisterPlayer());
        }
    }
    
    private System.Collections.IEnumerator WaitAndRegisterPlayer()
    {
        // Wait for networking to be properly initialized
        yield return new WaitForSeconds(0.5f);
        
        // Keep trying until we're spawned (max 10 seconds)
        float timeout = 10f;
        while (!isSpawned && timeout > 0)
        {
            Debug.Log("[SceneManagement] Waiting for spawn before registering...");
            yield return new WaitForSeconds(0.2f);
            timeout -= 0.2f;
        }
        
        if (isSpawned)
        {
            Debug.Log("[SceneManagement] Network spawned, attempting auto-registration");
            RegisterPlayer();
        }
        else
        {
            Debug.LogWarning("[SceneManagement] Timeout waiting for spawn. Registration will happen during ready marking as fallback.");
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
        
        if (isServer)
        {
            // Tell ALL clients to change scenes
            ChangeSceneForAllPlayersRpc();
        }
        else
        {
            // If not server, just change locally (fallback)
            SceneManager.LoadSceneAsync(sceneToChange);
        }
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
        
        // Tell ALL clients to change scenes (including server)
        ChangeSceneForAllPlayersRpc();
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
        Debug.Log($"[SceneManagement] MarkPlayerReady called by Player ID: {info.sender}");
        
        // Auto-register if not registered (fallback registration)
        if (!connectedPlayers.Contains(info.sender))
        {
            Debug.Log($"[SceneManagement] Player {info.sender} not registered! Auto-registering now...");
            connectedPlayers.Add(info.sender);
            Debug.Log($"[SceneManagement] Player {info.sender} auto-registered during ready marking");
        }
        
        // Add player to ready list (prevent duplicates)
        if (readyPlayers.Contains(info.sender))
        {
            Debug.LogWarning($"[SceneManagement] Player {info.sender} already marked ready! Ignoring.");
            return;
        }
        
        readyPlayers.Add(info.sender);
        
        // Get connected players count
        int totalConnected = GetConnectedPlayersCount();
        
        Debug.Log($"[SceneManagement] Player {info.sender} marked ready. Ready: {readyPlayers.Count}/{totalConnected}");
        Debug.Log($"[SceneManagement] Connected players: [{string.Join(", ", connectedPlayers)}]");
        Debug.Log($"[SceneManagement] Ready players: [{string.Join(", ", readyPlayers)}]");
        
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
        bool allReady = readyPlayers.Count >= totalConnected && totalConnected > 0;
        
        Debug.Log($"[SceneManagement] Ready check: {readyPlayers.Count}/{totalConnected} ready. All ready: {allReady}");
        
        // Safety check: require at least 1 player to be ready and don't allow single player to trigger scene change unless explicitly configured
        if (readyPlayers.Count == 0)
        {
            Debug.Log("[SceneManagement] No players ready, scene change not allowed");
            return false;
        }
        
        // If we detect only 1 connected player, check if single player testing is enabled
        if (totalConnected == 1 && readyPlayers.Count == 1)
        {
            if (allowSinglePlayerTesting)
            {
                Debug.Log("[SceneManagement] Single player testing enabled - allowing scene change with 1 player");
                return true;
            }
            else
            {
                Debug.LogWarning("[SceneManagement] Only detecting 1 player. Enable 'allowSinglePlayerTesting' in inspector for single player testing.");
                return false;
            }
        }
        
        return allReady;
    }

    private int GetConnectedPlayersCount()
    {
        // Filter out server from player count
        int realPlayerCount = 0;
        foreach (var playerId in connectedPlayers)
        {
            // Skip server IDs - they're not real players
            string playerIdStr = playerId.ToString();
            if (!playerIdStr.Contains("Server") && !playerIdStr.Equals("Server"))
            {
                realPlayerCount++;
            }
        }
        
        Debug.Log($"[SceneManagement] Total registered: {connectedPlayers.Count}, Real players (excluding server): {realPlayerCount}");
        
        // If no real players registered yet, try fallback
        if (realPlayerCount == 0)
        {
            Debug.LogWarning("[SceneManagement] No real players registered yet!");
            
            // Fallback: assume at least 1 player if we have ready players
            if (readyPlayers.Count > 0)
            {
                Debug.Log($"[SceneManagement] Using ready player count as fallback: {readyPlayers.Count}");
                return readyPlayers.Count;
            }
            
            Debug.LogWarning("[SceneManagement] No player data available, defaulting to 1");
            return 1;
        }
        
        return realPlayerCount;
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
            
            // Tell ALL clients to change scenes
            ChangeSceneForAllPlayersRpc();
        }
    }

    [ObserversRpc]
    private void ChangeSceneForAllPlayersRpc()
    {
        Debug.Log("[SceneManagement] Received command to change scene for all players");
        SceneManager.LoadSceneAsync(sceneToChange);
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

    [ContextMenu("Force Scene Change (Testing Only)")]
    public void ForceSceneChange()
    {
        if (isServer)
        {
            Debug.Log("[SceneManagement] FORCE: Scene change triggered manually for testing");
            ChangeSceneForAllPlayersRpc();
        }
        else
        {
            Debug.Log("[SceneManagement] FORCE: Requesting scene change from client");
            RequestSceneChange();
        }
    }

    [ContextMenu("Debug Ready Status")]
    public void DebugReadyStatus()
    {
        int totalConnected = GetConnectedPlayersCount();
        bool allReady = AreAllPlayersReady();
        
        Debug.Log("=== READY STATUS DEBUG ===");
        Debug.Log($"Ready Players: {readyPlayers.Count}");
        Debug.Log($"Total Connected: {totalConnected}");
        Debug.Log($"All Ready: {allReady}");
        Debug.Log($"Is Server: {isServer}");
        Debug.Log($"Is Spawned: {isSpawned}");
        
        Debug.Log("Connected Players:");
        foreach (var player in connectedPlayers)
        {
            Debug.Log($"  Connected: {player}");
        }
        
        Debug.Log("Ready Players:");
        foreach (var player in readyPlayers)
        {
            Debug.Log($"  Ready: {player}");
        }
        Debug.Log("=== END DEBUG ===");
    }
    
    // Utility methods for UI or other scripts
    public int GetReadyPlayerCount() => readyPlayers.Count;
    public int GetTotalPlayerCount() => GetConnectedPlayersCount();
    public bool IsPlayerReady(PlayerID playerId) => readyPlayers.Contains(playerId);
    public bool IsPlayerConnected(PlayerID playerId) => connectedPlayers.Contains(playerId);
    
    // Get readable status for UI
    public string GetReadyStatusText()
    {
        return $"Players Ready: {readyPlayers.Count}/{GetConnectedPlayersCount()}";
    }
    
    // Check if current player is registered (for UI)
    public bool IsCurrentPlayerRegistered()
    {
        return hasTriedToRegister;
    }
    
    [ContextMenu("Check Registration Status")]
    public void CheckRegistrationStatus()
    {
        Debug.Log($"[SceneManagement] Has tried to register: {hasTriedToRegister}");
        Debug.Log($"[SceneManagement] Is Owner: {isOwner}, Is Server: {isServer}, Is Spawned: {isSpawned}");
    }

    public void LogAllPlayersInScene()
    {
        Debug.Log("=== ALL PLAYERS IN SCENE (Manual Tracking) ===");
        
        Debug.Log($"[SceneManagement] Total tracked: {connectedPlayers.Count}");
        int index = 0;
        int realPlayerCount = 0;
        foreach (var playerId in connectedPlayers)
        {
            var isReady = readyPlayers.Contains(playerId);
            string playerIdStr = playerId.ToString();
            bool isServer = playerIdStr.Contains("Server") || playerIdStr.Equals("Server");
            
            Debug.Log($"  Player {index}: ID={playerId}, Ready={isReady}, IsServer={isServer}");
            
            if (!isServer) realPlayerCount++;
            index++;
        }
        
        Debug.Log($"[SceneManagement] Real Players (excluding server): {realPlayerCount}");
        Debug.Log($"[SceneManagement] Ready Players: {readyPlayers.Count}/{realPlayerCount}");
        
        if (readyPlayers.Count > 0)
        {
            Debug.Log("[SceneManagement] Ready Player IDs:");
            foreach (var readyPlayer in readyPlayers)
            {
                Debug.Log($"  Ready: {readyPlayer}");
            }
        }
        
        Debug.Log("=== END PLAYER LIST ===");
    }
}
