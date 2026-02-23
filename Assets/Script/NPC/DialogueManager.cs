using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI npcNameText;

    public Button acceptButton;
    public Button submitButton;
    public Button cancelButton;

    [Header("Typewriter")]
    [SerializeField] float typingSpeed = 0.03f;

    NPCDialogue currentDialogue;
    bool isDialogueActive;
    Coroutine typingCoroutine;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }


    }

    void Start()
    {
        dialoguePanel.SetActive(false);

        acceptButton.onClick.AddListener(AcceptQuest);
        submitButton.onClick.AddListener(SubmitQuest);
        cancelButton.onClick.AddListener(CancelDialogue);

        HideButtons();
    }

    public void StartDialogue(NPCDialogue dialogue)
    {
        if (isDialogueActive) return;

        isDialogueActive = true;
        currentDialogue = dialogue;

        dialoguePanel.SetActive(true);
        
        HideButtons();
        PauseGame();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(dialogue.dialogueText));
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }

        // พิมพ์จบแล้ว → แสดงปุ่ม
        ShowButtons();
    }

    void HideButtons()
    {
        acceptButton.gameObject.SetActive(false);
        submitButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);
    }

    void ShowButtons()
    {
        acceptButton.gameObject.SetActive(true);
        submitButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
    }

    public void AcceptQuest()
    {
        QuestData newData = new QuestData
        {
            questName = TempQuestHolder.Name,
            goalType = TempQuestHolder.Type,
            requiredAmount = TempQuestHolder.Amount
        };

        QuestManager.Instance.AddQuest(newData);
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
        HideButtons();

        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(message));
    }

    void EndDialogue()
    {
        isDialogueActive = false;
        currentDialogue = null;


       
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
