using UnityEngine;

public class GatherItem : QuestItem
{
    protected override void OnQuestInteract()
    {
       

        AddProgress();
        Destroy(gameObject); 
        Debug.Log($"[Interact] เก็บผัก: {questName} (+{amount})");
    }
}