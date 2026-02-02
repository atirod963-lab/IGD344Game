using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;

    public Button acceptButton;
    public Button submitButton;
    public Button cancelButton;

    NPCDialogue currentDialogue;
    bool isDialogueActive;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        dialoguePanel.SetActive(false);

        acceptButton.onClick.AddListener(AcceptQuest);
        submitButton.onClick.AddListener(SubmitQuest);
        cancelButton.onClick.AddListener(CancelDialogue);
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
        currentDialogue?.OnChoiceSelected(0);
        EndDialogue();
    }

    public void SubmitQuest()
    {
        bool success = QuestManager.Instance.TrySubmitQuest();

        if (!success)
        {
            ShowMessage("เควสยังไม่สำเร็จ");
            return;
        }

        EndDialogue();
    }

    public void CancelDialogue()
    {
        EndDialogue();
    }

    public void ShowMessage(string message)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = message;
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        currentDialogue = null;

        dialoguePanel.SetActive(false);
        ResumeGame();
    }

    void PauseGame() => Time.timeScale = 0f;
    void ResumeGame() => Time.timeScale = 1f;

    public bool IsDialogueActive()
    {
        return isDialogueActive;
    }
}
