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
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ถ้ามีตัวเก่าอยู่แล้ว ให้ลบทิ้ง
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public bool HasActiveQuest()
    {
        return currentStatus == QuestStatus.InProgress || currentStatus == QuestStatus.Completed;
    }

    // แก้ไขให้รับ QuestData ตามที่ DialogueManger ส่งมา
    public void AddQuest(QuestData data)
    {
        Debug.Log("กำลังพยายามเพิ่มเควส: " + data.questName); // เช็คว่าฟังก์ชันถูกเรียกไหม
        if (HasActiveQuest())
        {
            Debug.Log("มีเควสเดิมอยู่แล้ว เลยเพิ่มไม่ได้");
            return;
        }

        currentQuestName = data.questName;
        currentGoalType = data.goalType;
        targetAmount = data.requiredAmount;
        currentAmount = 0;
        
        currentStatus = QuestStatus.InProgress;

        UpdateQuestLog();
        Debug.Log("อัปเดต Quest Log เรียบร้อย");
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

    // เรียกใช้เมื่อกดปุ่ม Submit จาก DialogueManger
    public bool TrySubmitQuest()
    {
        Debug.Log($"TrySubmitQuest() - currentStatus: {currentStatus}, currentQuestName: {currentQuestName}, currentAmount: {currentAmount}/{targetAmount}");
        
        if (currentStatus == QuestStatus.Completed)
        {
            currentStatus = QuestStatus.None;
            currentQuestName = null;
            currentAmount = 0;
            targetAmount = 0;
            
            questLogText.text = "ไม่มีเควสปัจจุบัน";
            Debug.Log("Quest submitted successfully!");
            return true;
        }

        Debug.Log("Quest not completed yet");
        return false;
    }

    private void UpdateQuestLog()
    {
        string statusColor = (currentStatus == QuestStatus.Completed) ? "#00FF00" : "#FFFFFF";
        
        // เช็คว่า questLogText ยังคงอยู่หรือไม่
        if (questLogText != null)
        {
            questLogText.text = $"<b>เควสปัจจุบัน:</b>\n" +
                               $"- {currentQuestName}\n" +
                               $"<color={statusColor}>ความคืบหน้า: {currentAmount}/{targetAmount}</color>";

            if (currentStatus == QuestStatus.Completed)
            {
                questLogText.text += "\n\n<color=yellow>✔ เงื่อนไขครบ! ไปส่งเควสได้</color>";
            }
            
            Debug.Log("Quest Log updated - questLogText.gameObject.active: " + questLogText.gameObject.activeInHierarchy);
        }
        else
        {
            Debug.LogWarning("questLogText is null!");
        }
    }
}
