using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCDialogue dialogue;
    public bool isQuestGiver = true; // เลือกได้ว่าเป็น NPC เควสหรือคุยธรรมดา

    bool hasTalked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 1. เช็คเงื่อนไขพื้นฐาน
        if (hasTalked) return;
        if (!other.CompareTag("Player")) return;
        if (DialogueManager.Instance.IsDialogueActive()) return;

        // 2. ถ้าเป็น NPC เควส ให้ฝากข้อมูลเควสไว้ที่ TempQuestHolder ก่อน
        // ข้อมูลพวกนี้ดึงมาจากไฟล์ NPCDialogue ของคุณ
        if (isQuestGiver && dialogue != null)
        {
            TempQuestHolder.Name = dialogue.questName;   // ต้องมีตัวแปรนี้ใน NPCDialogue
            TempQuestHolder.Type = dialogue.goalType;    // ต้องมีตัวแปรนี้ใน NPCDialogue
            TempQuestHolder.Amount = dialogue.requiredAmount; 
        }

        // 3. เริ่มการสนทนาผ่าน Manager
        DialogueManager.Instance.StartDialogue(dialogue);
        
        // hasTalked = true; // เปิดใช้ถ้าอยากให้คุยแค่ครั้งเดียวแล้วเงียบไปเลย
    }
}
