using UnityEngine;

public abstract class QuestItem : MonoBehaviour, IInteract
{
    [Header("Quest Base")]
    public string questName;
    public GoalType goalType;
    public int amount = 1;

    public void Interact()
    {
        if (!CanInteract()) return;

        OnQuestInteract();
    }

    protected virtual bool CanInteract()
    {
        return QuestManager.Instance.HasActiveQuest();
    }

    protected void AddProgress()
    {
        QuestManager.Instance.AddProgress(
            goalType,
            questName,
            amount
        );
    }

    // 🔑 ให้แต่ละชนิดกำหนดพฤติกรรมเอง
    protected abstract void OnQuestInteract();
}