using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    public QuizManager quizManager;
    public QuestManager questManager;
    public GameObject SpotPointVFX;

    private bool hasTriggered = false;

    public List<GameObject> ServerRacks;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            SpotPointVFX.SetActive(false);
            ChangeHighlightColor();
            if (quizManager != null)
            {
                quizManager.ShowRandomQuestion();
            }
        }
    }

    // Reset trigger to allow re-triggering the quiz
    public void ResetTrigger()
    {
        hasTriggered = false;
    }

    private void ChangeHighlightColor()
    {
        foreach (var rack in ServerRacks)
        {
            var highlight = rack.GetComponent<EPOOutline.Outlinable>();
            if (highlight != null)
            {
                highlight.enabled = true;
                
                // Configure the outline to be visible
                highlight.OutlineParameters.Enabled = true;
                highlight.OutlineParameters.Color = Color.green; // Highlight color
                highlight.OutlineParameters.DilateShift = 1f; // Outline width
                highlight.OutlineParameters.BlurShift = 1f; // Outline blur
            }
            else{
                Debug.LogWarning($"No Outlinable component found on {rack.name}");
            }
        }
    }
}
