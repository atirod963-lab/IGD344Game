using UnityEngine;

public abstract class NPCDialogue : ScriptableObject
{
    public string npcID;

    [TextArea(3, 5)]
    public string dialogueText;

    public abstract void OnChoiceSelected(int index);
}