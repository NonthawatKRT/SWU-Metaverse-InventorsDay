
using UnityEngine;
using PurrNet;
using PurrNet.Modules;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

public class SceneManagement : NetworkBehaviour
{
    public GameObject WaitingforPlayersUI;
    [PurrScene] public string sceneToChange;
    
    private HashSet<PlayerID> readyPlayers = new HashSet<PlayerID>();
    private HashSet<PlayerID> connectedPlayers = new HashSet<PlayerID>();
    private int totalPlayers = 0;

    [ContextMenu("Change Scene")]

    public void ChangeScene()
    {
        PurrSceneSettings settings = new()
        {
            isPublic = false,
            mode = LoadSceneMode.Single
        };
        networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
    }

    [ServerRpc(requireOwnership: false)]
    private void RequestSceneChange(RPCInfo info = default)
    {
        var scene = SceneManager.GetSceneByName(sceneToChange);
        if (!scene.isLoaded)
        {
            // Load the scene if it's not already loaded
            PurrSceneSettings settings = new()
            {
                isPublic = false,
                mode = LoadSceneMode.Single
            };
            networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
            return;
        }

        if(networkManager.sceneModule.TryGetSceneID(scene, out SceneID sceneID))
        {
            networkManager.scenePlayersModule.AddPlayerToScene(info.sender, sceneID);
        }
    }

    [ContextMenu("Request Scene Change")]
    public void RequestSceneChangeClient()
    {
        RequestSceneChange();
    }

    // Call this method when a player clicks "Ready" button
    public void MarkPlayerReady()
    {
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
            PurrSceneSettings settings = new()
            {
                isPublic = false,
                mode = LoadSceneMode.Single
            };
            networkManager.sceneModule.LoadSceneAsync(sceneToChange, settings);
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
        RegisterPlayerServerRpc();
    }
}
