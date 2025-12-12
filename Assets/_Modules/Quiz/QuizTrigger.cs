using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuizTrigger : MonoBehaviour
{
    public QuizManager quizManager;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
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
}
