using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogManager : MonoBehaviour
{
    [Header("Dialog Content")]
    public string Name;
    public string Dialog;
    public Sprite CharacterSprite;
    
    [Header("UI Components")]
    public GameObject DialogUI;
    public TMP_Text NameText;
    public TMP_Text DialogText;
    public Image CharacterImage;
    
    
    [Header("Typing Animation")]
    public float typingSpeed = 0.05f; // Time between each character
    public bool skipOnClick = true; // Allow clicking to skip animation
    
    private Coroutine typingCoroutine;
    private bool isTyping = false;
    private DialogTrigger currentDialogTrigger;
    
    // Callback for when dialog line is complete
    public System.Action OnDialogComplete;

    private void Start() {
        DialogUI.SetActive(false);
    }
    public void Initialize()
    {
        Name = "";
        Dialog = "";
        NameText.text = Name;
        DialogText.text = Dialog;
    }

    // Main ShowDialog method with character sprite and trigger reference
    public void ShowDialog(string name, string dialog, Sprite characterSprite = null, DialogTrigger dialogTrigger = null)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        Name = name;
        Dialog = dialog;
        CharacterSprite = characterSprite;
        currentDialogTrigger = dialogTrigger;
        
        NameText.text = Name;
        DialogText.text = "";
        
        // Handle character image
        UpdateCharacterImage(characterSprite);
        
        DialogUI.SetActive(true);

        
        // Stop any existing typing animation
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        
        // Start typing animation
        typingCoroutine = StartCoroutine(TypeText(dialog));
    }

    public void HideDialog()
    {
        // Stop typing animation if running
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
        
        isTyping = false;
        currentDialogTrigger = null;
        DialogUI.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private IEnumerator TypeText(string textToType)
    {
        isTyping = true;
        DialogText.text = "";
        
        foreach (char character in textToType)
        {
            DialogText.text += character;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        isTyping = false;
        typingCoroutine = null;
        
        // Notify that dialog line is complete
        OnDialogComplete?.Invoke();
        
        // Check if there are more lines to show
        CheckForNextLine();
    }
    
    // Call this method when player clicks to skip typing animation or continue
    public void OnDialogClick()
    {
        if (isTyping && skipOnClick)
        {
            // Stop the typing coroutine and show full text immediately
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                typingCoroutine = null;
            }
            
            DialogText.text = Dialog;
            isTyping = false;
            
            // Notify that dialog line is complete
            OnDialogComplete?.Invoke();
            
            // Check if there are more lines to show
            CheckForNextLine();
        }
        else if (!isTyping)
        {
            // If not typing, check for next line or close dialog
            CheckForNextLine();
        }
    }
    
    // Overloaded ShowDialog for backward compatibility (without sprite)
    public void ShowDialog(string name, string dialog)
    {
        ShowDialog(name, dialog, null, null);
    }
    
    // Overloaded ShowDialog with sprite but no trigger
    public void ShowDialog(string name, string dialog, Sprite characterSprite)
    {
        ShowDialog(name, dialog, characterSprite, null);
    }
    
    // Check if there are more lines to show from current DialogTrigger
    private void CheckForNextLine()
    {
        if (currentDialogTrigger != null && currentDialogTrigger.HasNextLine())
        {
            // Small delay before showing next line for better UX
            StartCoroutine(ShowNextLineAfterDelay(0.5f));
        }
        else
        {
            // No more lines, close dialog after delay
            StartCoroutine(HideDialogAfterDelay(1f));
        }
    }
    
    private IEnumerator ShowNextLineAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (currentDialogTrigger != null)
        {
            currentDialogTrigger.ShowNextLine();
        }
    }
    
    private IEnumerator HideDialogAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        HideDialog();
    }
    
    // Update character image display
    private void UpdateCharacterImage(Sprite sprite)
    {
        if (CharacterImage == null) return;
        
        if (sprite != null)
        {
            CharacterImage.sprite = sprite;
            CharacterImage.gameObject.SetActive(true);
        }
        else
        {
            CharacterImage.gameObject.SetActive(false);
        }
    }
    
    // Set character image without showing new dialog
    public void SetCharacterImage(Sprite sprite)
    {
        CharacterSprite = sprite;
        UpdateCharacterImage(sprite);
    }
    
    // Check if currently typing (useful for other scripts)
    public bool IsTyping()
    {
        return isTyping;
    }
    
    // Get current character sprite
    public Sprite GetCurrentCharacterSprite()
    {
        return CharacterSprite;
    }

    private void Update() {
        // Check for mouse click to skip typing
        if (Input.GetMouseButtonDown(0))
        {
            OnDialogClick();
        }
    }
}
