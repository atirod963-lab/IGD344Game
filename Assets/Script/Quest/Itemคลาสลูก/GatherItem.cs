using UnityEngine;

public class GatherItem : QuestItem
{
    protected override void OnQuestInteract()
    {
        AddProgress();
        Destroy(gameObject);
    }
}