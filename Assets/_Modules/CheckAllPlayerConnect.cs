using System.Collections;
using UnityEngine;

public class CheckAllPlayerConnect : MonoBehaviour
{
    [SerializeField] private QuestManager questManager;
    [SerializeField] private GameObject loadingCanvas;
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private float delayAfterLoadingHidden = 0f;
    
    private bool hasStartedQuest = false;
    private bool wasLoadingActive = true;

    private void Start()
    {
        // Try to find LoadingCanvas if not assigned
        if (loadingCanvas == null)
        {
            loadingCanvas = GameObject.Find("LoadingCanvas");
            if (loadingCanvas == null)
            {
                Debug.LogWarning("[CheckAllPlayerConnect] LoadingCanvas not found in scene!");
            }
        }

        // Try to find QuestManager if not assigned
        if (questManager == null)
        {
            questManager = FindObjectOfType<QuestManager>();
            if (questManager == null)
            {
                Debug.LogError("[CheckAllPlayerConnect] QuestManager not found!");
            }
        }

        // Start checking for loading canvas
        StartCoroutine(CheckLoadingCanvas());
    }

    private IEnumerator CheckLoadingCanvas()
    {
        // Wait a frame for initialization
        yield return null;

        while (!hasStartedQuest)
        {
            if (loadingCanvas != null)
            {
                bool isCurrentlyActive = loadingCanvas.activeSelf;

                // Detect when loading canvas becomes inactive
                if (wasLoadingActive && !isCurrentlyActive)
                {
                    Debug.Log("[CheckAllPlayerConnect] LoadingCanvas is now hidden! Starting quest...");
                    hasStartedQuest = true;
                    StartCoroutine(StartQuestAfterDelay());
                    yield break;
                }

                wasLoadingActive = isCurrentlyActive;
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator StartQuestAfterDelay()
    {
        yield return new WaitForSeconds(delayAfterLoadingHidden);
        
        if (questManager != null)
        {
            Debug.Log("[CheckAllPlayerConnect] Starting quest and cutscene!");
            questManager.AdvanceToNextQuest();
        }
        else
        {
            Debug.LogError("[CheckAllPlayerConnect] QuestManager reference is missing!");
        }
    }

    // Public method to manually trigger quest start (for testing)
    [ContextMenu("Force Start Quest")]
    public void ForceStartQuest()
    {
        if (!hasStartedQuest && questManager != null)
        {
            hasStartedQuest = true;
            questManager.AdvanceToNextQuest();
        }
    }
}
