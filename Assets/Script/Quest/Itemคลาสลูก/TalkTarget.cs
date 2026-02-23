using UnityEngine;

public class TalkTarget : QuestItem
{
    protected override void OnQuestInteract()
    {
        AddProgress();
    }
}