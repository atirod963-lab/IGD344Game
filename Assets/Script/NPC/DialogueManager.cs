using UnityEngine;

using UnityEngine.UI;

using TMPro;

using System.Collections;

using UnityEngine.EventSystems;



public class DialogueManger : MonoBehaviour

{

    public static DialogueManger Instance;

    private bool justClosedDialogue = false;

    private bool isProcessingSubmit = false;





    [Header("UI")]

    public GameObject dialoguePanel;

    public TextMeshProUGUI dialogueText;

    public TextMeshProUGUI npcNameText;

    public Image npcPortraitImage;

    public ClickToMove2D playerMovement;







    public Button acceptButton;

    public Button submitButton;

    public Button cancelButton;



    [Header("Typewriter")]

    [SerializeField] float typingSpeed = 0.03f;



    NPCDialogue currentDialogue;



    bool isDialogueActive;

    bool isTyping;



    int currentLineIndex;



    Coroutine typingCoroutine;



    void Awake()

    {

        if (Instance != null && Instance != this)

        {

            Destroy(gameObject);

            return;

        }



        Instance = this;

        DontDestroyOnLoad(gameObject);

    }



    void Start()

    {

        dialoguePanel.SetActive(false);



        acceptButton.onClick.AddListener(AcceptQuest);

        submitButton.onClick.AddListener(SubmitQuest);

        cancelButton.onClick.AddListener(CancelDialogue);



        HideButtons();

    }



    void Update()

    {

        if (!isDialogueActive) return;



        if (Input.GetMouseButtonDown(0))

        {

            // เช็คว่าเมาส์กำลังชี้อยู่บน "ปุ่ม" หรือไม่

            bool isOverButton = false;

            if (EventSystem.current.currentSelectedGameObject != null)

            {

                // ถ้าสิ่งที่เลือกอยู่มีคอมโพเนนต์ Button แสดงว่าเป็นปุ่ม

                if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>() != null)

                {

                    isOverButton = true;

                }

            }



            // ถ้าไม่ได้กดโดนปุ่ม (คือจิ้มที่ว่าง หรือจิ้มบน Panel) ให้ทำงานต่อได้

            if (!isOverButton)

            {

                if (isTyping)

                {

                    StopCoroutine(typingCoroutine);

                    dialogueText.text = currentDialogue.dialogueLines[currentLineIndex];

                    isTyping = false;



                    if (currentLineIndex >= currentDialogue.dialogueLines.Length - 1)

                    {

                        ShowButtons();

                    }

                }

                else

                {

                    if (currentLineIndex < currentDialogue.dialogueLines.Length - 1)

                    {

                        NextDialogue();

                    }

                }

            }

        }

    }



    public void StartDialogue(NPCDialogue dialogue)

    {

        if (isDialogueActive) return;



        isDialogueActive = true;

        currentDialogue = dialogue;

        currentLineIndex = 0;



        dialoguePanel.SetActive(true);

        npcNameText.text = dialogue.npcName;

        npcPortraitImage.sprite = dialogue.npcPortrait;

       

        if (playerMovement != null)//ทำให้playerหยุดเดินไม่ไปชนกับเควสซ้ำ

            playerMovement.StopMovementImmediately();



        HideButtons();

        PauseGame();



        ShowCurrentLine();

    }



    void ShowCurrentLine()

    {

        if (typingCoroutine != null)

            StopCoroutine(typingCoroutine);



        // เช็คว่า currentDialogue และ dialogLines ไม่เป็น null

        if (currentDialogue == null || currentDialogue.dialogueLines == null) return;

        

        typingCoroutine = StartCoroutine(TypeText(currentDialogue.dialogueLines[currentLineIndex]));

    }



    IEnumerator TypeText(string text)

    {

        isTyping = true;

        dialogueText.text = "";



        foreach (char c in text)

        {

            // เช็คว่า Object ยังไม่ถูกทำลาย หรือ Panel ยังเปิดอยู่

            if (dialogueText == null || currentDialogue == null) yield break;



            dialogueText.text += c;

            yield return new WaitForSecondsRealtime(typingSpeed);

        }



        isTyping = false;



        // เพิ่มการตรวจสอบก่อนเข้าถึง dialogLines

        if (currentDialogue != null && currentDialogue.dialogueLines != null && 

            currentLineIndex >= currentDialogue.dialogueLines.Length - 1)

        {

            ShowButtons();

        }

    }



    public void NextDialogue()

    {

        // เช็คว่า currentDialogue และ dialogLines ไม่เป็น null

        if (currentDialogue == null || currentDialogue.dialogueLines == null) return;

        

        // เช็คว่าถ้าเป็นบรรทัดสุดท้ายแล้ว ไม่ต้องสั่ง EndDialogue

        if (currentLineIndex < currentDialogue.dialogueLines.Length - 1)

        {

            currentLineIndex++;

            ShowCurrentLine();

        }

        // ลบ else { EndDialogue(); } ออกไปเลยครับ

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

        FindFirstObjectByType<PlayerInteract>()?.IgnoreClickThisFrame();



        QuestManager.Instance.AddQuest(newData);



        // รีเซ็ตการเดินก่อนจบไดอะล็อก

        ResetPlayerMovement();

        EndDialogue();

    }



    public void SubmitQuest()

    {

        // ตรวจสอบว่าปุ่มถูกปิดอยู่แล้วหรือไม่

        if (!submitButton.interactable)

        {

            Debug.Log("Submit button is disabled, ignoring click");

            return;

        }



        // ปิดปุ่ม Submit ทันทีเพื่อป้องกันการกดซ้ำ

        submitButton.interactable = false;

        

        // ป้องกันการคลิกซ้ำทันทีก่อนทำงานอะไร

        FindFirstObjectByType<PlayerInteract>()?.IgnoreClickThisFrame();



        // ใช้ coroutine เพื่อให้แน่ใจว่า event ถูกประมวลผลเรียบร้อย

        StartCoroutine(SubmitQuestCoroutine());

    }



    IEnumerator SubmitQuestCoroutine()

    {

        // รอสักครู่เพื่อให้แน่ใจว่า UI event จบการทำงาน

        yield return new WaitForEndOfFrame();



        // เก็บชื่อเควสไว้ก่อนที่จะรีเซ็ต

        string questName = QuestManager.Instance.currentQuestName;

        

        bool success = QuestManager.Instance.TrySubmitQuest();



        if (!success)

        {

            ShowMessage("เควสยังไม่สำเร็จ");

            submitButton.interactable = true; // เปิดปุ่มกลับถ้าเควสยังไม่สำเร็จ

            yield break;

        }



        // ตั้งค่า questCompleted = true ให้ NPC ที่เกี่ยวข้อง (ใช้ชื่อที่เก็บไว้)

        QuestNPCDialogue[] questNPCs = FindObjectsByType<QuestNPCDialogue>(FindObjectsSortMode.None);

        foreach (QuestNPCDialogue npc in questNPCs)

        {

            if (npc.questName == questName)

            {

                npc.MarkQuestCompleted();

                break;

            }

        }



        // รีเซ็ตการเดินก่อนจบไดอะล็อก

        ResetPlayerMovement();

        EndDialogue();

        

        // เปิดปุ่มกลับหลังจากปิดไดอะล็อก (ถ้าจำเป็น)

        submitButton.interactable = true;

    }



    public void CancelDialogue()

    {

        FindFirstObjectByType<PlayerInteract>()?.IgnoreClickThisFrame();

        // รีเซ็ตการเดินก่อนจบไดอะล็อก

        ResetPlayerMovement();

        EndDialogue();

    }



    public void ShowMessage(string message)

    {

        Debug.Log("ShowMessage() called - opening panel with: " + message);

        dialoguePanel.SetActive(true);



        HideButtons();



        if (typingCoroutine != null)

            StopCoroutine(typingCoroutine);



        typingCoroutine = StartCoroutine(TypeText(message));

    }



    void EndDialogue()

    {

        Debug.Log("EndDialogue() called - closing panel");

        isDialogueActive = false;

        currentDialogue = null;



        if (typingCoroutine != null)

        {

            StopCoroutine(typingCoroutine);

            typingCoroutine = null;

        }



        dialoguePanel.SetActive(false);



        ResumeGame();



        StartCoroutine(DialogueCloseCooldown());

    }



    IEnumerator DialogueCloseCooldown()

    {

        justClosedDialogue = true;

        yield return new WaitForSeconds(0.2f);

        justClosedDialogue = false;

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



    void ResetPlayerMovement()

    {

        if (playerMovement != null)

        {

            playerMovement.StopMovementImmediately();



            // แถม: ให้ตำแหน่งเป้าหมายของ Player กลายเป็นจุดที่ยืนอยู่ปัจจุบันทันที

            // เพื่อป้องกันไม่ให้ Script ClickToMove ทำงานค้างจากค่าเก่า

            playerMovement.transform.position = playerMovement.transform.position;

        }

    }



    public bool JustClosedDialogue()

    {

        return justClosedDialogue;

    }

}

