using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Quest NPC Dialogue")]
public class QuestNPCDialogue : NPCDialogue
{
    public string questName;

    public override void OnChoiceSelected(int index)
    {
        if (index == 0) // รับเควส
        {
            QuestManager.Instance.AddQuest(questName);
        }
    }
}