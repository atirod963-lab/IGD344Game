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
    public CutsceneController cutsceneController; // 👈 เพิ่ม

    [Header("Typing Effect")]
    public float typingSpeed = 0.05f;

    [Header("Character Portraits")]
    public Image leftPortrait;
    public Image rightPortrait;
    public Color activeColor = Color.white;
    public Color inactiveColor = new Color(0.5f, 0.5f, 0.5f, 1.0f);

    public enum DialogueSide { Left, Right, None }

    private Queue<DialogueLine> lines = new Queue<DialogueLine>();
    private bool isTyping = false;
    private bool isDialogueActive = false;
    private string currentLine = "";
   

    private System.Action onDialogueEnd;

    void Start()
    {
        dialoguePanel.SetActive(false);

        if (skipButton != null)
            skipButton.onClick.AddListener(SkipDialogue);

        FindPlayer();
    }

    void Update()
    {
        if (isDialogueActive && Input.GetMouseButtonDown(0))
            OnNextButtonPressed();
    }

    public void StartDialogue(DialogueLine[] dialogue, System.Action onEnd)
    {
        // 🔥 กัน dialogue โผล่ตอน skip
        if (cutsceneController != null && cutsceneController.IsSkipping())
            return;

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

    // 🔥 Skip ใหม่ (ข้ามทั้ง Cutscene)
    public void SkipDialogue()
    {
        StopAllCoroutines();
        lines.Clear();

        if (cutsceneController != null)
        {
            cutsceneController.SkipCutscene(); // 👈 ข้ามทั้งระบบ
            return;
        }

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

        HandlePortraits(line);

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
            playerMovement.StopMovementImmediately();
        }

        onDialogueEnd?.Invoke();
    }

    private void FindPlayer()
    {
        if (playerMovement == null)
            playerMovement = Object.FindAnyObjectByType<ClickToMove2D>();
    }

    [System.Serializable]
    public class DialogueLine
    {
        public string characterName;
        [TextArea(2, 5)]
        public string text;
        public Sprite characterSprite;
        public DialogueSide side;

        public bool hasOtherCharacter; // 👈 ใช้ควบคุม ซ่อน/เทา
    }

    private void HandlePortraits(DialogueLine line)
    {
        // ซ่อนก่อนทุกครั้ง
        if (leftPortrait != null)
            leftPortrait.gameObject.SetActive(false);

        if (rightPortrait != null)
            rightPortrait.gameObject.SetActive(false);

        if (line.characterSprite == null)
            return;

        // 🟢 มี 2 ตัว → ใช้ระบบเทา
        if (line.hasOtherCharacter)
        {
            if (leftPortrait != null)
                leftPortrait.gameObject.SetActive(true);

            if (rightPortrait != null)
                rightPortrait.gameObject.SetActive(true);

            if (line.side == DialogueSide.Left)
            {
                leftPortrait.sprite = line.characterSprite;
                leftPortrait.color = activeColor;
                rightPortrait.color = inactiveColor;
            }
            else if (line.side == DialogueSide.Right)
            {
                rightPortrait.sprite = line.characterSprite;
                rightPortrait.color = activeColor;
                leftPortrait.color = inactiveColor;
            }
        }
        // 🔴 มีคนเดียว → อีกฝั่งหาย
        else
        {
            if (line.side == DialogueSide.Left && leftPortrait != null)
            {
                leftPortrait.sprite = line.characterSprite;
                leftPortrait.color = activeColor;
                leftPortrait.gameObject.SetActive(true);
            }
            else if (line.side == DialogueSide.Right && rightPortrait != null)
            {
                rightPortrait.sprite = line.characterSprite;
                rightPortrait.color = activeColor;
                rightPortrait.gameObject.SetActive(true);
            }
        }
    }

    public void ForceEndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);
    }
}