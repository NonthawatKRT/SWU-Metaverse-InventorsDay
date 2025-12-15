using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogBroadCasting))]
public class EditorDialogBroadcasting : Editor
{
    private string[] testCharacters = { "Server Announcement", "Game Master", "System Alert", "Event Coordinator", "NPC Guide" };
    private string[] shortBroadcasts = {
        "Welcome all players!",
        "Server maintenance in 5 minutes.",
        "New event starting now!"
    };
    
    private string[] longBroadcasts = {
        "Welcome to SWU Metaverse - Inventors Day! This multiplayer dialog system allows the server to broadcast messages to all connected players simultaneously. Everyone will see this message at the same time.",
        "Attention all players! A special collaborative event is about to begin. Please gather at the main plaza where you will work together to solve puzzles and unlock new areas of the metaverse.",
        "System notification: The multiplayer dialog broadcasting system is now active. This demonstrates how server-side messages can be sent to all players for announcements, instructions, and coordinated activities."
    };
    
    // Custom dialog input
    private string customCharacterName = "Custom Character";
    private string customDialog = "Enter your custom broadcast message here...";
    private int selectedSpriteIndex = 0;
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DialogBroadCasting dialogBroadcasting = (DialogBroadCasting)target;
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Multiplayer Dialog Broadcasting", EditorStyles.boldLabel);
        
        // Network Status Check
        bool canBroadcast = Application.isPlaying && dialogBroadcasting.isSpawned;
        string statusText = !Application.isPlaying ? "Not in Play Mode" : 
                           !dialogBroadcasting.isSpawned ? "Not Spawned in Network" : "Ready to Broadcast";
        
        EditorGUILayout.HelpBox($"Status: {statusText}", 
            canBroadcast ? MessageType.Info : MessageType.Warning);
        
        EditorGUI.BeginDisabledGroup(!canBroadcast);
        
        // Basic Controls
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Hide All Dialogs", GUILayout.Height(25)))
        {
            dialogBroadcasting.BroadcastHideDialog();
        }
        
        if (GUILayout.Button("Log Current State", GUILayout.Height(25)))
        {
            dialogBroadcasting.LogCurrentState();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // Quick Test Methods
        EditorGUILayout.LabelField("Quick Test Broadcasts", EditorStyles.miniBoldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Welcome Message"))
        {
            dialogBroadcasting.TestWelcomeMessage();
        }
        if (GUILayout.Button("Game Instructions"))
        {
            dialogBroadcasting.TestGameInstructions();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("System Alert"))
        {
            dialogBroadcasting.TestSystemAlert();
        }
        if (GUILayout.Button("Event Notification"))
        {
            dialogBroadcasting.TestEventNotification();
        }
        GUILayout.EndHorizontal();
        
        if (GUILayout.Button("Random Dialog"))
        {
            dialogBroadcasting.TestRandomDialog();
        }
        
        GUILayout.Space(5);
        
        // Short Broadcast Tests
        EditorGUILayout.LabelField("Short Broadcasts", EditorStyles.miniBoldLabel);
        for (int i = 0; i < shortBroadcasts.Length; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Short Broadcast {i + 1}"))
            {
                dialogBroadcasting.BroadcastDialog(testCharacters[i % testCharacters.Length], shortBroadcasts[i], i);
            }
            if (GUILayout.Button("No Image", GUILayout.Width(80)))
            {
                dialogBroadcasting.BroadcastDialog(testCharacters[i % testCharacters.Length], shortBroadcasts[i], -1);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Space(5);
        
        // Long Broadcast Tests  
        EditorGUILayout.LabelField("Long Broadcasts", EditorStyles.miniBoldLabel);
        for (int i = 0; i < longBroadcasts.Length; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Long Broadcast {i + 1}"))
            {
                dialogBroadcasting.BroadcastDialog(testCharacters[i % testCharacters.Length], longBroadcasts[i], i);
            }
            if (GUILayout.Button("No Image", GUILayout.Width(80)))
            {
                dialogBroadcasting.BroadcastDialog(testCharacters[i % testCharacters.Length], longBroadcasts[i], -1);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Space(5);
        
        // Custom Dialog Broadcasting
        EditorGUILayout.LabelField("Custom Broadcast", EditorStyles.miniBoldLabel);
        customCharacterName = EditorGUILayout.TextField("Character Name", customCharacterName);
        
        // Multi-line text area for custom dialog
        GUIStyle textAreaStyle = EditorStyles.textArea;
        textAreaStyle.wordWrap = true;
        customDialog = EditorGUILayout.TextArea(customDialog, textAreaStyle, GUILayout.Height(60));
        
        selectedSpriteIndex = EditorGUILayout.IntSlider("Sprite Index (-1 = none)", selectedSpriteIndex, -1, 4);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Send Custom Broadcast"))
        {
            dialogBroadcasting.BroadcastDialog(customCharacterName, customDialog, selectedSpriteIndex);
        }
        if (GUILayout.Button("Clear Fields"))
        {
            customCharacterName = "Custom Character";
            customDialog = "Enter your custom broadcast message here...";
            selectedSpriteIndex = 0;
        }
        GUILayout.EndHorizontal();
        
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(10);
        
        // Runtime Status
        EditorGUILayout.LabelField("Network Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Is Playing", Application.isPlaying);
        EditorGUILayout.Toggle("Is Owner", Application.isPlaying ? dialogBroadcasting.isOwner : false);
        EditorGUILayout.Toggle("Is Spawned", Application.isPlaying ? dialogBroadcasting.isSpawned : false);
        EditorGUILayout.Toggle("Can Broadcast", Application.isPlaying ? dialogBroadcasting.isSpawned : false);
        
        DialogManager dialogManager = FindObjectOfType<DialogManager>();
        EditorGUILayout.ObjectField("Dialog Manager", dialogManager, typeof(DialogManager), true);
        EditorGUI.EndDisabledGroup();
        
        // Instructions
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Multiplayer Broadcasting Instructions:\n" +
            "• Must be in Play Mode and spawned in network to broadcast\n" +
            "• ANY PLAYER can send broadcasts to all other players\n" +
            "• Use test buttons to send pre-defined messages to all players\n" +
            "• Create custom broadcasts using the text fields\n" +
            "• 'Hide All Dialogs' closes dialogs for all connected players\n" +
            "• Perfect for trigger zones and player interactions\n" +
            "• Test with multiple clients to see synchronized dialogs",
            MessageType.Info
        );
        
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox(
                "Enter Play Mode to test multiplayer broadcasting functionality.",
                MessageType.Warning
            );
        }
        else if (!dialogBroadcasting.isSpawned)
        {
            EditorGUILayout.HelpBox(
                "Player needs to be spawned in the network to broadcast messages.",
                MessageType.Warning
            );
        }
        else
        {
            EditorGUILayout.HelpBox(
                "Ready to broadcast! Any player can send messages to all connected players.",
                MessageType.Info
            );
        }
    }
}
