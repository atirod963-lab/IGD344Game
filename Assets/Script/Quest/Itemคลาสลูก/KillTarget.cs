using UnityEngine;

public class KillTarget : QuestItem
{
    protected override void OnQuestInteract()
    {
        AddProgress();
        // ไม่ destroy ตรงนี้ → ไป destroy ตอน HP = 0
    }
}