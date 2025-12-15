using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrNet;

public class DialogBroadCasting : NetworkBehaviour
{
    [Header("Dialog System Reference")]
    [SerializeField] private DialogManager dialogManager;
    
    [Header("Test Characters & Dialogs")]
    [SerializeField] private string[] testCharacters = { "Server Announcement", "Game Master", "System Alert", "Event Coordinator" };
    [SerializeField] private string[] testDialogs = {
        "Welcome all players to SWU Metaverse!",
        "This is a multiplayer dialog test.",
        "All players can see this message simultaneously.",
        "Broadcasting system is working correctly!"
    };
    [SerializeField] private Sprite[] characterSprites;

    private void Start()
    {
        // Find DialogManager if not assigned
        if (dialogManager == null)
        {
            dialogManager = FindObjectOfType<DialogManager>();
            if (dialogManager == null)
            {
                Debug.LogError("[DialogBroadCasting] No DialogManager found in scene!");
            }
        }
    }

    #region Public Broadcasting Methods

    /// <summary>
    /// Broadcast dialog to all players (any player can call this)
    /// </summary>
    public void BroadcastDialog(string characterName, string dialog, int spriteIndex = -1)
    {
        if (!isSpawned)
        {
            Debug.LogWarning("[DialogBroadCasting] Cannot broadcast - not spawned in network!");
            return;
        }

        Debug.Log($"[DialogBroadCasting] Player {owner?.ToString()} broadcasting dialog from {characterName}: {dialog}");
        BroadcastDialogRpc(characterName, dialog, spriteIndex);
    }

    /// <summary>
    /// Hide dialog for all players (any player can call this)
    /// </summary>
    public void BroadcastHideDialog()
    {
        if (!isSpawned)
        {
            Debug.LogWarning("[DialogBroadCasting] Cannot broadcast hide dialog - not spawned in network!");
            return;
        }

        Debug.Log($"[DialogBroadCasting] Player {owner?.ToString()} broadcasting hide dialog to all players");
        BroadcastHideDialogRpc();
    }

    #endregion

    #region Network RPC Methods

    [ObserversRpc]
    private void BroadcastDialogRpc(string characterName, string dialog, int spriteIndex)
    {
        if (dialogManager == null)
        {
            Debug.LogError("[DialogBroadCasting] DialogManager is null, cannot show broadcasted dialog!");
            return;
        }

        Sprite characterSprite = null;
        if (spriteIndex >= 0 && spriteIndex < characterSprites.Length)
        {
            characterSprite = characterSprites[spriteIndex];
        }

        Debug.Log($"[DialogBroadCasting] Received broadcast dialog: {characterName} - {dialog}");
        dialogManager.ShowDialog(characterName, dialog, characterSprite);
    }

    [ObserversRpc]
    private void BroadcastHideDialogRpc()
    {
        if (dialogManager == null)
        {
            Debug.LogError("[DialogBroadCasting] DialogManager is null, cannot hide dialog!");
            return;
        }

        Debug.Log("[DialogBroadCasting] Received broadcast hide dialog");
        dialogManager.HideDialog();
    }

    #endregion

    #region Test Methods (Only Owner)

    /// <summary>
    /// Test method 1: Send a welcome message
    /// </summary>
    [ContextMenu("Test: Welcome Message")]
    public void TestWelcomeMessage()
    {
        BroadcastDialog("Player Announcement", "Welcome all players to SWU Metaverse - Inventors Day! Get ready for an amazing experience!", 0);
    }

    /// <summary>
    /// Test method 2: Send game instructions
    /// </summary>
    [ContextMenu("Test: Game Instructions")]
    public void TestGameInstructions()
    {
        BroadcastDialog("Game Guide", "Please explore the metaverse and interact with objects. Use WASD to move and mouse to look around.", 1);
    }

    /// <summary>
    /// Test method 3: Send system alert
    /// </summary>
    [ContextMenu("Test: System Alert")]
    public void TestSystemAlert()
    {
        BroadcastDialog("Player Alert", "This is a player-triggered notification. All players receive this message simultaneously through the network.", 2);
    }

    /// <summary>
    /// Test method 4: Send event notification
    /// </summary>
    [ContextMenu("Test: Event Notification")]
    public void TestEventNotification()
    {
        BroadcastDialog("Event Trigger", "A player has triggered an event! Something interesting is happening in the metaverse.", 3);
    }

    /// <summary>
    /// Test method 5: Send random dialog
    /// </summary>
    [ContextMenu("Test: Random Dialog")]
    public void TestRandomDialog()
    {
        int randomIndex = Random.Range(0, testDialogs.Length);
        BroadcastDialog(testCharacters[randomIndex], testDialogs[randomIndex], randomIndex);
    }

    /// <summary>
    /// Test method 6: Hide all dialogs
    /// </summary>
    [ContextMenu("Test: Hide All Dialogs")]
    public void TestHideAllDialogs()
    {
        BroadcastHideDialog();
    }

    /// <summary>
    /// Test method 7: Send player interaction message
    /// </summary>
    [ContextMenu("Test: Player Interaction")]
    public void TestPlayerInteraction()
    {
        string playerName = owner != null ? $"Player {owner.ToString()}" : "Unknown Player";
        BroadcastDialog("Interaction", $"{playerName} has discovered something interesting! Come check it out!", 0);
    }

    /// <summary>
    /// Test method 8: Send trigger zone message
    /// </summary>
    [ContextMenu("Test: Trigger Zone")]
    public void TestTriggerZone()
    {
        string playerName = owner != null ? $"Player {owner.ToString()}" : "Unknown Player";
        BroadcastDialog("Zone Trigger", $"{playerName} has entered a special area. This is how trigger zones will work in the final version!", 1);
    }

    #endregion

    #region Debug Information

    /// <summary>
    /// Get information about the current state
    /// </summary>
    public void LogCurrentState()
    {
        Debug.Log($"[DialogBroadCasting] Player: {owner?.ToString()}, IsOwner: {isOwner}, IsSpawned: {isSpawned}, DialogManager: {(dialogManager != null ? "Found" : "Missing")}");
        
        if (dialogManager != null)
        {
            Debug.Log($"[DialogBroadCasting] DialogManager IsTyping: {dialogManager.IsTyping()}");
        }
        
        Debug.Log($"[DialogBroadCasting] Can Broadcast: {isSpawned} (Any player can broadcast when spawned)");
    }

    #endregion
}
