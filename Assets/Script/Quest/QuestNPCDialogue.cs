using UnityEngine;

public class QuestNPCDialogue : MonoBehaviour
{
    public bool isQuestGiver = true; // ถ้าติ๊กออก จะกลายเป็น NPC คุยธรรมดา
    private bool questCompleted = false; // เก็บสถานะว่าเควสนี้เคยส่งแล้วหรือไม่

    [Header("General Dialogue")]
    [TextArea(3, 10)]
    public string casualDialogue = "วันนี้อากาศดีนะว่าไหม?";

    [Header("Quest Settings (If isQuestGiver = true)")]
    public string questName = "กำจัดสไลม์";
    public GoalType goalType = GoalType.Kill;
    public int requiredAmount = 5;

    [Header("Quest Dialogues")]
    [TextArea(3, 10)] public string dialogueBeforeQuest;
    [TextArea(3, 10)] public string dialogueDuringQuest;
    [TextArea(3, 10)] public string dialogueQuestComplete;

    public void Interact()
    {
        // ป้องกันการโต้ตอบทันทีหลังจากปิดไดอะล็อก
        if (DialogueManger.Instance.JustClosedDialogue())
        {
            return;
        }

        // 1. ถ้าไม่ใช่ NPC ให้เควส ให้แสดงบทสนทนาทั่วไปแล้วจบเลย
        if (!isQuestGiver)
        {
            ShowSimpleDialogue(casualDialogue);
            return;
        }

        // 2. ถ้าเป็น NPC ให้เควส ให้ใช้ Logic เดิมที่เราเขียนไว้
        HandleQuestDialogue();
    }

    private void ShowSimpleDialogue(string text)
    {
        DialogueManger.Instance.dialoguePanel.SetActive(true);
        DialogueManger.Instance.dialogueText.text = text;

        // ปิดปุ่ม Accept และ Submit เพราะเป็นการคุยธรรมดา
        DialogueManger.Instance.acceptButton.gameObject.SetActive(false);
        DialogueManger.Instance.submitButton.gameObject.SetActive(false);
        DialogueManger.Instance.cancelButton.gameObject.SetActive(true); 
        // ปุ่ม Cancel ใน DialogueManger ของคุณทำหน้าที่เป็นปุ่ม "ปิด" ได้เลย
    }

    private void HandleQuestDialogue()
    {
        QuestStatus status = QuestManager.Instance.currentStatus;
        bool isMyQuest = (QuestManager.Instance.currentQuestName == questName);

        // ถ้าเควสนี้เคยส่งแล้ว ให้แสดงบทสนทนาทั่วไปเสมอ
        if (questCompleted)
        {
            ShowSimpleDialogue("ขอบคุณมากสำหรับการช่วยเหลือ!");
            return;
        }

        if (status == QuestStatus.None)
            OpenDialogue(dialogueBeforeQuest, true, false);
        else if (isMyQuest && status == QuestStatus.InProgress)
            OpenDialogue(dialogueDuringQuest, false, false);
        else if (isMyQuest && status == QuestStatus.Completed)
            OpenDialogue(dialogueQuestComplete, false, true);
        else if (status == QuestStatus.None && string.IsNullOrEmpty(QuestManager.Instance.currentQuestName))
        {
            // กรณีที่เพิ่งส่งเควสไปหมายถึงเควสนี้เสร็จแล้ว ให้แสดงบทสนทนาทั่วไป
            ShowSimpleDialogue("ขอบคุณมากสำหรับการช่วยเหลือ!");
        }
    }

    private void OpenDialogue(string text, bool showAccept, bool showSubmit)
    {
        DialogueManger.Instance.dialoguePanel.SetActive(true);
        DialogueManger.Instance.dialogueText.text = text;
        DialogueManger.Instance.acceptButton.gameObject.SetActive(showAccept);
        DialogueManger.Instance.submitButton.gameObject.SetActive(showSubmit);
        DialogueManger.Instance.cancelButton.gameObject.SetActive(true);
        
        // เก็บข้อมูลเควสเผื่อไว้ถ้ามีการกด Accept
        TempQuestHolder.Name = questName;
        TempQuestHolder.Type = goalType;
        TempQuestHolder.Amount = requiredAmount;
    }

    public void MarkQuestCompleted()
    {
        questCompleted = true;
    }
    
}

