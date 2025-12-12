using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DialogManager))]
public class EditorDialogManager : Editor
{
    private string[] testCharacters = { "NPC Guide", "Professor Smith", "Student Alice", "Robot Assistant" };
    private string[] shortDialogs = {
        "Hello! Welcome to the metaverse.",
        "This is a short test message.",
        "Quick dialog for testing."
    };
    
    private string[] longDialogs = {
        "Welcome to SWU Metaverse - Inventors Day! This is a longer dialog to test the typing animation with multiple sentences. You can see how the typewriter effect works with extended text content.",
        "Greetings, inventor! Today marks a special occasion as we showcase innovative technologies and creative solutions. The metaverse opens up endless possibilities for learning, collaboration, and discovery.",
        "This dialog contains multiple sentences to test the smooth typing animation. Notice how each character appears one by one, creating an engaging reading experience. The animation can be skipped by clicking if needed."
    };
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        DialogManager dialogManager = (DialogManager)target;
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Dialog Testing", EditorStyles.boldLabel);
        
        // Basic Controls
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Hide Dialog", GUILayout.Height(25)))
        {
            dialogManager.HideDialog();
        }
        
        if (GUILayout.Button("Skip Animation", GUILayout.Height(25)))
        {
            dialogManager.OnDialogClick();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // Short Dialog Tests
        EditorGUILayout.LabelField("Short Dialogs", EditorStyles.miniBoldLabel);
        for (int i = 0; i < shortDialogs.Length; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Short Dialog " + (i + 1)))
            {
                dialogManager.ShowDialog(testCharacters[i % testCharacters.Length], shortDialogs[i]);
            }
            if (GUILayout.Button("With Image " + (i + 1), GUILayout.Width(80)))
            {
                // Test with current CharacterSprite if available
                dialogManager.ShowDialog(testCharacters[i % testCharacters.Length], shortDialogs[i], dialogManager.CharacterSprite);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Space(5);
        
        // Long Dialog Tests  
        EditorGUILayout.LabelField("Long Dialogs", EditorStyles.miniBoldLabel);
        for (int i = 0; i < longDialogs.Length; i++)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Long Dialog " + (i + 1)))
            {
                dialogManager.ShowDialog(testCharacters[i % testCharacters.Length], longDialogs[i]);
            }
            if (GUILayout.Button("With Image " + (i + 1), GUILayout.Width(80)))
            {
                dialogManager.ShowDialog(testCharacters[i % testCharacters.Length], longDialogs[i], dialogManager.CharacterSprite);
            }
            GUILayout.EndHorizontal();
        }
        
        GUILayout.Space(5);
        
        // Special Characters Test
        EditorGUILayout.LabelField("Special Tests", EditorStyles.miniBoldLabel);
        if (GUILayout.Button("Dialog with Numbers & Symbols"))
        {
            dialogManager.ShowDialog("System", "Test 123! Special characters: @#$%^&*()_+ Check numbers: 1234567890 and punctuation: .,;:!?");
        }
        
        if (GUILayout.Button("Empty Dialog Test"))
        {
            dialogManager.ShowDialog("Empty", "");
        }
        
        if (GUILayout.Button("Single Character"))
        {
            dialogManager.ShowDialog("Minimal", "Hi!");
        }
        
        GUILayout.Space(5);
        
        // Image Testing
        EditorGUILayout.LabelField("Character Image Testing", EditorStyles.miniBoldLabel);
        
        // Character Sprite field
        dialogManager.CharacterSprite = (Sprite)EditorGUILayout.ObjectField("Test Character Sprite", dialogManager.CharacterSprite, typeof(Sprite), false);
        
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Set Character Image"))
        {
            dialogManager.SetCharacterImage(dialogManager.CharacterSprite);
        }
        if (GUILayout.Button("Clear Image"))
        {
            dialogManager.SetCharacterImage(null);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(10);
        
        // Runtime Status
        EditorGUILayout.LabelField("Runtime Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Toggle("Is Typing", dialogManager.IsTyping());
        EditorGUILayout.TextField("Current Name", dialogManager.Name ?? "None");
        EditorGUILayout.TextField("Current Dialog", dialogManager.Dialog ?? "None");
        EditorGUILayout.ObjectField("Current Sprite", dialogManager.GetCurrentCharacterSprite(), typeof(Sprite), false);
        EditorGUI.EndDisabledGroup();
        
        // Instructions
        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Testing Instructions:\n" +
            "• Use 'Short/Long Dialog' buttons to test typewriter effect\n" +
            "• 'Skip Animation' simulates player clicking during typing\n" +
            "• 'Hide Dialog' closes the dialog system\n" +
            "• Monitor 'Is Typing' status to see animation state\n" +
            "• Test in Play Mode for full cursor lock/unlock behavior",
            MessageType.Info
        );
    }
}