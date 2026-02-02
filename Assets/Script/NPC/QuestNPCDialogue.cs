using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/Quest NPC Dialogue")]
public class QuestNPCDialogue : NPCDialogue
{
    public string questName;
    [TextArea] public string questBlockedText = "ทำเควสปัจจุบันให้เสร็จก่อน";

    public override void OnChoiceSelected(int index)
    {
        // ปุ่มรับเควส
        if (index == 0)
        {
            if (QuestManager.Instance.HasActiveQuest())
            {
                DialogueManager.Instance.ShowMessage(questBlockedText);
                return;
            }

            QuestManager.Instance.AddQuest(questName);
        }
    }
}
