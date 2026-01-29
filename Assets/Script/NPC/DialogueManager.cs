using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public Button acceptButton;
    public Button cancelButton;

    NPCDialogue currentDialogue;
    bool isDialogueActive = false;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    public void StartDialogue(NPCDialogue dialogue)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        currentDialogue = dialogue;

        dialoguePanel.SetActive(true);
        dialogueText.text = dialogue.dialogueText;

        PauseGame();
    }

    public void AcceptQuest()
    {
        currentDialogue.OnChoiceSelected(0);
        EndDialogue();
    }

    public void CancelDialogue()
    {
        EndDialogue();
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
        ResumeGame();
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
    }

    void ResumeGame()
    {
        Time.timeScale = 1f;
    }

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}