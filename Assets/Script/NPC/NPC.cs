using System.Collections;
using UnityEngine;

public class NPC : MonoBehaviour, IInteract
{
    public NPCDialogue dialogue;
    public bool isQuestGiver = true;

    private bool isCoolingDown = false;

    public void Interact()
    {
        if (isCoolingDown || DialogueManger.Instance.IsDialogueActive()) return;

        StartCoroutine(InteractFlow());
    }

    IEnumerator InteractFlow()
    {
        isCoolingDown = true;

        // --- Quest ---
        if (isQuestGiver && dialogue != null)
        {
            TempQuestHolder.Name = dialogue.questName;
            TempQuestHolder.Type = dialogue.goalType;
            TempQuestHolder.Amount = dialogue.requiredAmount;
        }

        DialogueManger.Instance.StartDialogue(dialogue);

        // รอจน dialog ปิด
        yield return new WaitUntil(() => !DialogueManger.Instance.IsDialogueActive());

        // 🔥 เพิ่มดีเลย์กันคลิกซ้ำ
        yield return new WaitForSeconds(0.3f);

        isCoolingDown = false;
    }

    IEnumerator StartCooldownAfterDialogue()
    {
        isCoolingDown = true;

        yield return new WaitUntil(() => !DialogueManger.Instance.IsDialogueActive());

        yield return new WaitForSeconds(1.5f);

        isCoolingDown = false;
    }
}