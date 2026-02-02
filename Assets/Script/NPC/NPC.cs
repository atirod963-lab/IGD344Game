using UnityEngine;

public class NPC : MonoBehaviour
{
    public NPCDialogue dialogue;

    bool hasTalked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTalked) return;

        if (!other.CompareTag("Player")) return;

        
        DialogueManager.Instance.StartDialogue(dialogue);
    }
}