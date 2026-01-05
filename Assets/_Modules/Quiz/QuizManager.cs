using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PurrNet;

public class QuizManager : NetworkBehaviour
{
    [Header("Quiz Settings")]
    [SerializeField] private int currentQuestionIndex = 0;
    [SerializeField] private int score = 0;
    [SerializeField] private int totalQuestions = 0;
    
    [Header("Network Sync")]
    [SerializeField] private int playersAnsweredCorrectly = 0;
    private int totalPlayersInLobby = 0;
    
    [Header("References")]
    [SerializeField] private QuestManager questManager;
    
    [Header("UI References")]
    [SerializeField] private GameObject quizUI;
    [SerializeField] private TMPro.TMP_Text questionText;
    // [SerializeField] private TMPro.TMP_Text scoreText;
    [SerializeField] private UnityEngine.UI.Image questionImage;
    [SerializeField] private List<Sprite> questionSprites;
    [SerializeField] private List<TMPro.TMP_Text> choiceTexts;
    [SerializeField] private List<UnityEngine.UI.Button> answerButtons;


    
    private string currentCorrectAnswer;
    private List<int> usedQuestions = new List<int>();
    public List<string> questions = new List<string>()
    {
        "ตรากราฟของมหาลัยหมายถึงกราฟอะไร?",
        "มศว มีวิทยาเขตกี่แห่ง?",
        "สีประจำมหาวิทยาลัยคือสีอะไร?",
        "คณะ”ทั้งหมดของมศว”มีกี่คณะ?",
        "คณะวิศวกรรมศาสตร์ มศว ตั้งอยู่ที่ใหน?",
        "คณะที่วิทยาเขต “องครักษ์” มีกี่คณะ?",
        "ปรัชญาประจำมหาลัย มศว?",
        "มศว  ประมานมิตรตั้งอยู่ใกล้สถานี MRT ใด?",
        "ตึกที่สูงใน มศว(ประสานมิตร) คือตึกอะไร?",
        "รูปปั้นของ ดร.สาโรช อยู่ด้านใดเมือหันหน้าเข้าตึกเหลืองหอจดหมายเหตุ?",
    };

    public List<string> choice1 = new List<string>()
    {
        "Exponential",
        "1",
        "เทา/เเดง",
        "20",
        "ประสานมิตร",
        "7",
        "การศึกษาคือความเจริญงอกงาม",
        "อโศก",
        "400ล้าน",
        "ซ้าย",
    };

    public List<string> choice2 = new List<string>()
    {
        "Logarithmic",
        "2",
        "เทา/เหลือง",
        "21",
        "องครักษ์",
        "8",
        "การสร้างบัณฑิตที่รับใช้สังคมด้วยปัญญา",
        "เพรชุรี",
        "500ล้าน",
        "ขวา",
    };

    public List<string> answers = new List<string>()
    {
        "Exponential",
        "2",
        "เทา/เเดง",
        "20",
        "องครักษ์",
        "8",
        "การศึกษาคือความเจริญงอกงาม",
        "เพรชุรี",
        "400ล้าน",
        "ซ้าย",
    };

    public void Initialize()
    {
        currentQuestionIndex = 0;
        score = 0;
        totalQuestions = 0;
        usedQuestions.Clear();
        quizUI.SetActive(false);
        
        // Find QuestManager if not assigned
        if (questManager == null)
        {
            questManager = FindObjectOfType<QuestManager>();
        }
        
        // Get total players in lobby
        totalPlayersInLobby = NetworkManager.main.players.Count;
        
        Debug.Log("Quiz Manager initialized. Total players: " + totalPlayersInLobby);
    }

    public void ShowRandomQuestion()
    {
        // Show quiz UI
        quizUI.SetActive(true);
        
        // Get a random question (can repeat questions)
        int randomIndex = Random.Range(0, questions.Count);
        currentQuestionIndex = randomIndex;
        
        // Display the question and choices
        DisplayQuestion(randomIndex);
        
        Debug.Log("Quiz UI opened with random question: " + questions[randomIndex]);
    }
    
    private void DisplayQuestion(int questionIndex)
    {
        // Show cursor for interaction
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
        // Set question text
        questionText.text = questions[questionIndex];
        currentCorrectAnswer = answers[questionIndex];
        
        // Display question image if available
        if (questionImage != null && questionSprites != null && questionIndex < questionSprites.Count && questionSprites[questionIndex] != null)
        {
            questionImage.sprite = questionSprites[questionIndex];
            questionImage.gameObject.SetActive(true);
            Debug.Log("Question image displayed for question " + questionIndex);
        }
        else if (questionImage != null)
        {
            questionImage.gameObject.SetActive(false);
            Debug.Log("No image available for question " + questionIndex);
        }
        
        // Create list of choices and shuffle them
        List<string> shuffledChoices = new List<string>
        {
            choice1[questionIndex],
            choice2[questionIndex]
        };
        
        // Shuffle the choices
        for (int i = 0; i < shuffledChoices.Count; i++)
        {
            string temp = shuffledChoices[i];
            int randomIndex = Random.Range(i, shuffledChoices.Count);
            shuffledChoices[i] = shuffledChoices[randomIndex];
            shuffledChoices[randomIndex] = temp;
        }
        
        // Update choice texts and buttons
        for (int i = 0; i < choiceTexts.Count && i < shuffledChoices.Count; i++)
        {
            choiceTexts[i].text = shuffledChoices[i];
            answerButtons[i].gameObject.SetActive(true);
            
            // Remove old listeners and add new ones
            answerButtons[i].onClick.RemoveAllListeners();
            string choiceText = shuffledChoices[i];
            answerButtons[i].onClick.AddListener(() => CheckAnswer(choiceText));
        }
        
        // Hide unused buttons if there are more buttons than choices
        for (int i = shuffledChoices.Count; i < answerButtons.Count; i++)
        {
            answerButtons[i].gameObject.SetActive(false);
        }
    }
    
    public void CheckAnswer(string selectedAnswer)
    {
        totalQuestions++;
        
        if (selectedAnswer == currentCorrectAnswer)
        {
            score++;
            Debug.Log("✅ CORRECT! Player selected: " + selectedAnswer);
            Debug.Log("Current Score: " + score + "/" + totalQuestions);
            
            // Notify server about correct answer
            NotifyCorrectAnswerServerRpc();
            
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            // Close quiz UI after correct answer
            quizUI.SetActive(false);
            Debug.Log("Quiz UI closed after correct answer");
        }
        else
        {
            Debug.Log("❌ WRONG! Player selected: " + selectedAnswer);
            Debug.Log("Correct answer was: " + currentCorrectAnswer);
            Debug.Log("Current Score: " + score + "/" + totalQuestions);
            
            // Show another random question if wrong
            Debug.Log("Showing another random question...");
            ShowRandomQuestion();
        }
    }
    
    // Method to manually show a specific question by index
    public void ShowQuestionByIndex(int index)
    {
        if (index >= 0 && index < questions.Count)
        {
            quizUI.SetActive(true);
            currentQuestionIndex = index;
            DisplayQuestion(index);
            Debug.Log("Quiz UI opened with question " + index + ": " + questions[index]);
        }
        else
        {
            Debug.LogWarning("Question index " + index + " is out of range!");
        }
    }
    
    private void UpdateScoreDisplay()
    {
        Debug.Log($"Quiz Score: {score}/{totalQuestions}");
    }
    
    private void EndQuiz()
    {
        questionText.text = "Quiz Complete!\nFinal Score: " + score + "/" + totalQuestions;
        
        // Hide all answer buttons
        foreach (var button in answerButtons)
        {
            button.gameObject.SetActive(false);
        }
        
        Debug.Log("Quiz finished! Final score: " + score + "/" + totalQuestions);
    }
    
    public void RestartQuiz()
    {
        Initialize();
    }
    
    public void CloseQuiz()
    {
        quizUI.SetActive(false);
    }
    
    // Get current score (useful for other scripts)
    public int GetScore()
    {
        return score;
    }
    
    public int GetTotalQuestions()
    {
        return totalQuestions;
    }
    
    // Network RPC Methods
    [ServerRpc(requireOwnership = false)]
    private void NotifyCorrectAnswerServerRpc()
    {
        playersAnsweredCorrectly++;
        Debug.Log($"[Server] Player answered correctly. Progress: {playersAnsweredCorrectly}/{totalPlayersInLobby}");
        
        // Check if all players have answered correctly
        if (playersAnsweredCorrectly >= totalPlayersInLobby)
        {
            Debug.Log("[Server] All players answered correctly! Calling RepairServer...");
            
            // Call RepairServer in QuestManager
            if (questManager != null)
            {
                questManager.RepairServer();
            }
            else
            {
                Debug.LogError("[Server] QuestManager reference is null!");
            }
            
            // Reset the counter for next quiz
            playersAnsweredCorrectly = 0;
        }
    }
    
    // Method to manually reset the correct answer counter (if needed)
    [ServerRpc(requireOwnership = false)]
    public void ResetCorrectAnswerCountServerRpc()
    {
        playersAnsweredCorrectly = 0;
        Debug.Log("[Server] Correct answer counter reset.");
    }

}
