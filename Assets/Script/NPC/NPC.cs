using System.Collections; // ต้องมีอันนี้ด้วย
using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCDialogue dialogue;
    public bool isQuestGiver = true;

    private bool isCoolingDown = false; // ตัวแปรกันลูป

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้าอยู่ในช่วงคูลดาวน์ หรือกำลังคุยอยู่ ให้หยุดทำงานทันที
        if (isCoolingDown || !other.CompareTag("Player") || DialogueManger.Instance.IsDialogueActive()) return;

        // --- ส่วนการเริ่มคุยเดิมของคุณ ---
        if (isQuestGiver && dialogue != null)
        {
            TempQuestHolder.Name = dialogue.questName;
            TempQuestHolder.Type = dialogue.goalType;
            TempQuestHolder.Amount = dialogue.requiredAmount;
        }

        DialogueManger.Instance.StartDialogue(dialogue);

        // เมื่อเริ่มคุยแล้ว ให้เข้าสู่สถานะรอคูลดาวน์หลังจากคุยเสร็จ
        StartCoroutine(StartCooldownAfterDialogue());
    }

    IEnumerator StartCooldownAfterDialogue()
    {
        isCoolingDown = true;

        // รอจนกว่า DialogueManager จะบอกว่าปิดหน้าต่างแล้วจริงๆ
        yield return new WaitUntil(() => !DialogueManger.Instance.IsDialogueActive());

        // หลังจากปิดหน้าต่าง ให้รออีก 1.5 วินาที เพื่อให้ผู้เล่นเดินหนีออกมาได้
        yield return new WaitForSeconds(1.5f);

        isCoolingDown = false;
    }
}