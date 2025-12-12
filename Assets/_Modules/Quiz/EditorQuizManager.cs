using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuizManager))]
public class EditorQuizManager : Editor
{
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        QuizManager quizManager = (QuizManager)target;
        
        GUILayout.Space(10);
        EditorGUILayout.LabelField("Quiz Testing", EditorStyles.boldLabel);
        
        // Basic Controls
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Initialize Quiz", GUILayout.Height(30)))
        {
            quizManager.Initialize();
        }
        
        if (GUILayout.Button("Close Quiz", GUILayout.Height(30)))
        {
            quizManager.CloseQuiz();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // Question Testing
        EditorGUILayout.LabelField("Question Controls", EditorStyles.miniBoldLabel);
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Show Random Question"))
        {
            quizManager.ShowRandomQuestion();
        }
        
        if (GUILayout.Button("Restart Quiz"))
        {
            quizManager.RestartQuiz();
        }
        GUILayout.EndHorizontal();
        
        GUILayout.Space(5);
        
        // Answer Testing
        EditorGUILayout.LabelField("Answer Testing", EditorStyles.miniBoldLabel);
        
        if (Application.isPlaying && quizManager.questions.Count > 0)
        {
            // Get current question info for testing
            int currentIndex = GetCurrentQuestionIndex(quizManager);
            if (currentIndex >= 0 && currentIndex < quizManager.questions.Count)
            {
                EditorGUILayout.HelpBox("Current Question: " + quizManager.questions[currentIndex], MessageType.Info);
                
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Answer: Choice 1"))
                {
                    quizManager.CheckAnswer(quizManager.choice1[currentIndex]);
                }
                
                if (GUILayout.Button("Answer: Choice 2")) 
                {
                    quizManager.CheckAnswer(quizManager.choice2[currentIndex]);
                }
                GUILayout.EndHorizontal();
                
                if (GUILayout.Button("Submit Correct Answer"))
                {
                    quizManager.CheckAnswer(quizManager.answers[currentIndex]);
                }
            }
        }
        
        GUILayout.Space(5);
        
        // Data Validation
        EditorGUILayout.LabelField("Data Validation", EditorStyles.miniBoldLabel);
        if (GUILayout.Button("Validate Quiz Data"))
        {
            ValidateQuizData(quizManager);
        }
        
        GUILayout.Space(10);
        
        // Runtime Status
        EditorGUILayout.LabelField("Runtime Status", EditorStyles.boldLabel);
        EditorGUI.BeginDisabledGroup(true);
        
        if (Application.isPlaying)
        {
            EditorGUILayout.IntField("Current Score", quizManager.GetScore());
            EditorGUILayout.IntField("Questions Answered", quizManager.GetTotalQuestions());
            EditorGUILayout.IntField("Total Available Questions", quizManager.questions.Count);
            
            float percentage = quizManager.GetTotalQuestions() > 0 ? 
                (float)quizManager.GetScore() / quizManager.GetTotalQuestions() * 100f : 0f;
            EditorGUILayout.FloatField("Success Rate %", percentage);
        }
        else
        {
            EditorGUILayout.LabelField("Quiz Status", "Not Running (Enter Play Mode)");
        }
        
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(5);
        
        // Quiz Statistics
        EditorGUILayout.LabelField("Quiz Statistics", EditorStyles.miniBoldLabel);
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.IntField("Total Questions in Database", quizManager.questions.Count);
        EditorGUILayout.Toggle("Questions Match Choices", CheckDataConsistency(quizManager));
        EditorGUILayout.Toggle("Questions Match Answers", CheckAnswerConsistency(quizManager));
        EditorGUI.EndDisabledGroup();
        
        GUILayout.Space(10);
        
        // Instructions
        EditorGUILayout.HelpBox(
            "Testing Instructions:\\n" +
            "• 'Initialize Quiz' starts a new quiz session\\n" +
            "• 'Show Random Question' displays a random question (UI pops up)\\n" +
            "• Use answer buttons to test correct/incorrect responses\\n" +
            "• 'Validate Quiz Data' checks for data consistency\\n" +
            "• Monitor statistics to verify quiz functionality\\n" +
            "• Test in Play Mode for full functionality\\n" +
            "• UI closes automatically after each answer",
            MessageType.Info
        );
    }
    
    private int GetCurrentQuestionIndex(QuizManager quizManager)
    {
        // This is a simplified way to get current question index
        // In a real scenario, you might want to add a public getter in QuizManager
        return 0; // You may need to expose currentQuestionIndex in QuizManager for better testing
    }
    

    
    private bool CheckDataConsistency(QuizManager quizManager)
    {
        return quizManager.questions.Count == quizManager.choice1.Count && 
               quizManager.questions.Count == quizManager.choice2.Count;
    }
    
    private bool CheckAnswerConsistency(QuizManager quizManager)
    {
        return quizManager.questions.Count == quizManager.answers.Count;
    }
    
    private void ValidateQuizData(QuizManager quizManager)
    {
        List<string> issues = new List<string>();
        
        // Check if lists have matching counts
        if (quizManager.questions.Count != quizManager.choice1.Count)
            issues.Add("Questions and Choice1 count mismatch");
            
        if (quizManager.questions.Count != quizManager.choice2.Count)
            issues.Add("Questions and Choice2 count mismatch");
            
        if (quizManager.questions.Count != quizManager.answers.Count)
            issues.Add("Questions and Answers count mismatch");
        
        // Check for empty entries
        for (int i = 0; i < quizManager.questions.Count; i++)
        {
            if (string.IsNullOrEmpty(quizManager.questions[i]))
                issues.Add($"Empty question at index {i}");
                
            if (i < quizManager.choice1.Count && string.IsNullOrEmpty(quizManager.choice1[i]))
                issues.Add($"Empty choice1 at index {i}");
                
            if (i < quizManager.choice2.Count && string.IsNullOrEmpty(quizManager.choice2[i]))
                issues.Add($"Empty choice2 at index {i}");
                
            if (i < quizManager.answers.Count && string.IsNullOrEmpty(quizManager.answers[i]))
                issues.Add($"Empty answer at index {i}");
        }
        
        // Check if answers match one of the choices
        for (int i = 0; i < quizManager.questions.Count; i++)
        {
            if (i < quizManager.answers.Count && i < quizManager.choice1.Count && i < quizManager.choice2.Count)
            {
                string answer = quizManager.answers[i];
                string choice1 = quizManager.choice1[i];
                string choice2 = quizManager.choice2[i];
                
                if (answer != choice1 && answer != choice2)
                {
                    issues.Add($"Answer '{answer}' at index {i} doesn't match either choice");
                }
            }
        }
        
        if (issues.Count == 0)
        {
            EditorUtility.DisplayDialog("Validation Result", "Quiz data is valid! No issues found.", "OK");
        }
        else
        {
            string message = "Quiz data validation found issues:\\n\\n" + string.Join("\\n", issues);
            EditorUtility.DisplayDialog("Validation Result", message, "OK");
        }
    }
}

