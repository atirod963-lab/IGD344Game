using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/NPC Dialogue")]
public class NPCDialogue : ScriptableObject
{
    public string npcID;

    public string npcName;   // ⭐ เพิ่มตัวนี้
    public Sprite npcPortrait; // ⭐ รูป NPC

    [TextArea(3, 5)]
    public string[] dialogueLines;

    [Header("Quest Data (Optional)")]
    public string questName;
    public GoalType goalType;
    public int requiredAmount;
}