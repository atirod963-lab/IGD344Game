using UnityEngine;

 [CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/NPC Dialogue")]
     public class NPCDialogue : ScriptableObject
{
    public string npcID;

    [TextArea(3, 5)]
    public string dialogueText;
     [Header("Quest Data (Optional)")]
    public string questName;
    public GoalType goalType;
    public int requiredAmount;
     
   

    
}