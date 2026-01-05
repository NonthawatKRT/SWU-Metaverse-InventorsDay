using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Video;
public class QuestManager : MonoBehaviour
{
    public enum Quest
    {
        None,
        StartCutScene,
        FindOutWhatHappened,
        RepairAllServerRacks,
        EndCutScene,
        GoToSWUMetaverse
    }

    public Quest currentQuest = Quest.None;
    private int repairedServerCount = 0;
    private const int totalServers = 3;

    [Header("Quest Objects")]
    public AudioSource QuestFinishSource;
    public GameObject StartCutSceneUI;
    public GameObject EndCutSceneUI;
    public GameObject QueststatusUI;
    public TMP_Text QuestStatusText;

    public VideoPlayer StartcutscenePlayer;
    public VideoPlayer EndcutscenePlayer;

    public List<GameObject> ServerRacks;

    public void OnEnable()
    {
        currentQuest = Quest.None;
        UpdateQuestUI();
    }

    public void UpdateQuestUI()
    {
        switch (currentQuest)
        {
            case Quest.None:
                StartCutSceneUI.SetActive(false);
                QueststatusUI.SetActive(false);
                break;
            case Quest.StartCutScene:
                StartCutSceneUI.SetActive(true);
                QueststatusUI.SetActive(false);
                StartCoroutine(PlayStartCutsceneAndAdvance());
                break;
            case Quest.FindOutWhatHappened:
                StartCutSceneUI.SetActive(false);
                QueststatusUI.SetActive(true);
                WritenewQuest("พูดคุยกับหุ่นยนต์ที่อยู่ใต้ยานรบ");
                break;
            case Quest.RepairAllServerRacks:
                HightlightServerRacks();
                WritenewQuest($"ซ่อมเซิร์ฟเวอร์ {repairedServerCount} / {totalServers}");
                break;
            case Quest.EndCutScene:
                EndCutSceneUI.SetActive(true);
                QueststatusUI.SetActive(false);
                StartCoroutine(PlayEndCutsceneAndAdvance());
                break;
            case Quest.GoToSWUMetaverse:
                QueststatusUI.SetActive(true);
                WritenewQuest("เข้าประตูมิติเพื่อไปยัง SWU Metaverse");
                break;
        }
    }

    public void WritenewQuest(string newQuest)
    {
        StartCoroutine(WriteQuestCharacterByCharacter(newQuest));
    }

    private IEnumerator WriteQuestCharacterByCharacter(string newQuest)
    {
        QuestStatusText.text = "";
        foreach (char c in newQuest)
        {
            QuestStatusText.text += c;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public void AdvanceToNextQuest()
    {
        if (currentQuest < Quest.GoToSWUMetaverse)
        {
            currentQuest++;
            QuestFinishSource.Play();
            if (currentQuest == Quest.RepairAllServerRacks)
            {
                repairedServerCount = 0;
            }
            UpdateQuestUI();
        }
    }

    public void RepairServer()
    {
        if (currentQuest == Quest.RepairAllServerRacks && repairedServerCount < totalServers)
        {
            repairedServerCount++;
            WritenewQuest($"ซ่อมเซิร์ฟเวอร์ {repairedServerCount} / {totalServers}");
            
            if (repairedServerCount >= totalServers)
            {
                AdvanceToNextQuest();
            }
        }
    }

    private IEnumerator PlayStartCutsceneAndAdvance()
    {
        StartcutscenePlayer.Play();
        
        // Wait for video to start playing
        yield return new WaitUntil(() => StartcutscenePlayer.isPlaying);
        
        // Wait until video finishes
        yield return new WaitUntil(() => !StartcutscenePlayer.isPlaying);
        
        AdvanceToNextQuest();
    }

    private IEnumerator PlayEndCutsceneAndAdvance()
    {
        EndcutscenePlayer.Play();
        
        // Wait for video to start playing
        yield return new WaitUntil(() => EndcutscenePlayer.isPlaying);
        
        // Wait until video finishes
        yield return new WaitUntil(() => !EndcutscenePlayer.isPlaying);
        
        AdvanceToNextQuest();
    }

    private void HightlightServerRacks()
    {
        foreach (var rack in ServerRacks)
        {
            var highlight = rack.GetComponent<EPOOutline.Outlinable>();
            if (highlight != null)
            {
                highlight.enabled = true;
                
                // Configure the outline to be visible
                highlight.OutlineParameters.Enabled = true;
                highlight.OutlineParameters.Color = Color.yellow; // Highlight color
                highlight.OutlineParameters.DilateShift = 1f; // Outline width
                highlight.OutlineParameters.BlurShift = 1f; // Outline blur
            }
            else{
                Debug.LogWarning($"No Outlinable component found on {rack.name}");
            }
        }
    }

}
