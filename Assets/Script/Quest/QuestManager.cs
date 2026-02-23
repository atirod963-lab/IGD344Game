using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI Reference")]
    public TextMeshProUGUI questLogText;

    [Header("Current Quest Status")]
    public string currentQuestName;
    public QuestStatus currentStatus = QuestStatus.None;

    [Header("Goal Tracking")]
    public GoalType currentGoalType;
    public int currentAmount;
    public int targetAmount;

    void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasActiveQuest()
    {
        return currentStatus == QuestStatus.InProgress || currentStatus == QuestStatus.Completed;
    }

    // แก้ไขให้รับ QuestData ตามที่ DialogueManager ส่งมา
    public void AddQuest(QuestData data)
    {
        if (HasActiveQuest()) return;

        currentQuestName = data.questName;
        currentGoalType = data.goalType;
        targetAmount = data.requiredAmount;
        currentAmount = 0;
        
        currentStatus = QuestStatus.InProgress;

        UpdateQuestLog();
    }

    // ฟังก์ชันสำหรับ Item หรือ Monster เรียกใช้เพื่อนับจำนวน
    public void AddProgress(GoalType type, string itemName, int amount)
    {
        // เช็คว่าเควสที่ทำอยู่ ประเภทตรงกัน และชื่อไอเทมตรงกับชื่อเควสหรือไม่
        if (currentStatus == QuestStatus.InProgress && 
            currentGoalType == type && 
            currentQuestName == itemName)
        {
            currentAmount += amount;

            // ถ้าครบจำนวนที่กำหนด
            if (currentAmount >= targetAmount)
            {
                currentAmount = targetAmount;
                currentStatus = QuestStatus.Completed;
                Debug.Log("ภารกิจสำเร็จ! กลับไปคุยกับ NPC");
            }

            UpdateQuestLog();
        }
    }

    // เรียกใช้เมื่อกดปุ่ม Submit จาก DialogueManager
    public bool TrySubmitQuest()
    {
        if (currentStatus == QuestStatus.Completed)
        {
            currentStatus = QuestStatus.None;
            currentQuestName = null;
            currentAmount = 0;
            targetAmount = 0;
            
            questLogText.text = "ไม่มีเควสปัจจุบัน";
            return true;
        }

        return false;
    }

    private void UpdateQuestLog()
    {
        string statusColor = (currentStatus == QuestStatus.Completed) ? "#00FF00" : "#FFFFFF";
        
        questLogText.text = $"<b>เควสปัจจุบัน:</b>\n" +
                           $"- {currentQuestName}\n" +
                           $"<color={statusColor}>ความคืบหน้า: {currentAmount}/{targetAmount}</color>";

        if (currentStatus == QuestStatus.Completed)
        {
            questLogText.text += "\n\n<color=yellow>✔ เงื่อนไขครบ! ไปส่งเควสได้</color>";
        }
    }
}
