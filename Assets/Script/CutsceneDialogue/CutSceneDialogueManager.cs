using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class cutsceneDialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI characterNameText;
    public Button skipButton;

    [Header("References")]
    public ClickToMove2D playerMovement;

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private string currentLine = "";

    // Callback ที่จะเรียกตอน Dialogue จบ (ให้ Timeline เดินต่อ)
    private System.Action onDialogueEnd;

    void Start()
    {
        dialoguePanel.SetActive(false);
        if (skipButton != null)
            skipButton.onClick.AddListener(SkipDialogue);
    }

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
            OnNextButtonPressed();
    }

    // CutsceneController จะเรียก Function นี้ พร้อมส่ง callback มาด้วย
    public void StartDialogue(DialogueLine[] dialogue, System.Action onEnd)
    {
        onDialogueEnd = onEnd;

        dialoguePanel.SetActive(true);
        isDialogueActive = true;

        if (playerMovement != null)
        {
            playerMovement.StopMovementImmediately();
            playerMovement.enabled = false;
        }

        lines.Clear();
        foreach (var line in dialogue)
            lines.Enqueue(line);

        DisplayNextLine();
    }

    public void OnNextButtonPressed()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            dialogueText.text = currentLine;
            isTyping = false;
        }
        else
        {
            DisplayNextLine();
        }
    }

    public void SkipDialogue()
    {
        StopAllCoroutines();
        lines.Clear();
        EndDialogue();
    }

    private void DisplayNextLine()
    {
        if (lines.Count == 0)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = lines.Dequeue();
        characterNameText.text = line.characterName;
        currentLine = line.text;

        StopAllCoroutines();
        StartCoroutine(TypeLine(line.text));
    }

    private IEnumerator TypeLine(string line)
    {
        isTyping = true;
        dialogueText.text = "";

        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }

    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        if (playerMovement != null)
        {
            playerMovement.enabled = true;
            Debug.Log("enabled = " + playerMovement.enabled);
            Debug.Log("GameObject active = " + playerMovement.gameObject.activeSelf);
        }
        else
        {
            Debug.LogWarning("playerMovement เป็น null!");
        }

        onDialogueEnd?.Invoke();
        Debug.Log("Dialogue จบแล้ว!");
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea(2, 5)]
        public string text;
    }
}