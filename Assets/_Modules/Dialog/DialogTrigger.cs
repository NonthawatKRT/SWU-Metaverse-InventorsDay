using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    public string characterName = "NPC Guide";
    public string dialogText = "Hello there! Welcome to our world. Feel free to explore and interact with everything around you.";
    public Sprite characterSprite;
    public int currentLineIndex = 0;

    private bool hasTriggered = false;

    public List <string> dialogLines = new List<string>()
    {
        "Hello there! Welcome to our world. Feel free to explore and interact with everything around you.",
        "Did you know? You can click on objects to learn more about them.",
        "Have fun and don't hesitate to ask if you need any help!"
    };

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            DialogManager dialogManager = FindObjectOfType<DialogManager>();
            if (dialogManager != null && currentLineIndex < dialogLines.Count)
            {
                dialogManager.ShowDialog(characterName, dialogLines[currentLineIndex], characterSprite, this);
                currentLineIndex++;
            }
        }
    }
    public void ShowNextLine()
    {
        if (currentLineIndex < dialogLines.Count)
        {
            DialogManager dialogManager = FindObjectOfType<DialogManager>();
            if (dialogManager != null)
            {
                dialogManager.ShowDialog(characterName, dialogLines[currentLineIndex], characterSprite, this);
                currentLineIndex++;
            }
        }
    }
    
    public bool HasNextLine()
    {
        return currentLineIndex < dialogLines.Count;
    }
    
    // Reset dialog to beginning
    public void ResetDialog()
    {
        currentLineIndex = 0;
        hasTriggered = false;
    }
    
    // Start dialog conversation manually
    public void StartDialog()
    {
        if (dialogLines.Count > 0)
        {
            currentLineIndex = 0;
            DialogManager dialogManager = FindObjectOfType<DialogManager>();
            if (dialogManager != null)
            {
                dialogManager.ShowDialog(characterName, dialogLines[currentLineIndex], characterSprite, this);
                currentLineIndex++;
            }
        }
    }
}
