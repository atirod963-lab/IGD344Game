using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    public TextMeshProUGUI questLogText;

    public string currentQuestName;
    public QuestStatus currentStatus = QuestStatus.None;

    void Awake()
    {
        Instance = this;
    }

    public bool HasActiveQuest()
    {
        return currentStatus == QuestStatus.InProgress;
    }

    public void AddQuest(string questName)
    {
        if (HasActiveQuest()) return;

        currentQuestName = questName;
        currentStatus = QuestStatus.InProgress;

        questLogText.text = $"เควสปัจจุบัน:\n- {questName}";
    }

    // เรียกจากระบบเกม เช่น ฆ่าศัตรูครบ / เก็บของครบ
    public void MarkQuestComplete()
    {
        if (!HasActiveQuest()) return;

        currentStatus = QuestStatus.Completed;
    }

    public bool TrySubmitQuest()
    {
        if (currentStatus == QuestStatus.Completed)
        {
            questLogText.text += "\n\n✔ เควสสำเร็จ!";
            currentQuestName = null;
            currentStatus = QuestStatus.None;
            return true;
        }

        return false;
    }
}
