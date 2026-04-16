using UnityEngine;
using UnityEngine.Playables;
using static cutsceneDialogueManager;

public class CutsceneController : MonoBehaviour
{
    public cutsceneDialogueManager dialogueManager;
    public PlayableDirector timeline;
    public GameObject skipCutsceneButton;

    private bool isSkipping = false; // 🔥 flag

    [System.Serializable]
    public class DialogueSet
    {
        public string setName;
        public DialogueLine[] lines;
    }

    public DialogueSet[] dialogueSets;

    void Start()
    {
        timeline.stopped += OnTimelineFinished;

        if (skipCutsceneButton != null)
            skipCutsceneButton.SetActive(true);
    }

    void OnDestroy()
    {
        timeline.stopped -= OnTimelineFinished;
    }

    private void OnTimelineFinished(PlayableDirector director)
    {
        if (skipCutsceneButton != null)
            skipCutsceneButton.SetActive(false);

        if (isSkipping) return;

        if (dialogueManager.playerMovement != null)
            dialogueManager.playerMovement.enabled = true;
    }

    // 🔥 Timeline เรียก Dialogue
    public void TriggerDialogue(int setIndex)
    {
        if (isSkipping) return; // 🔥 กัน dialogue ตอน skip

        if (setIndex >= dialogueSets.Length) return;

        double currentTime = timeline.time;
        timeline.Pause();

        dialogueManager.StartDialogue(
            dialogueSets[setIndex].lines,
            onEnd: () =>
            {
                if (isSkipping) return;

                timeline.time = currentTime;
                timeline.Play();
            }
        );
    }

    // 🔥 SKIP ทั้งหมด (ตัวสำคัญสุด)
    public void SkipCutscene()
    {
        if (isSkipping) return;
        isSkipping = true;

        Debug.Log("⏭️ Skip ไปท้าย Timeline");

        // ⭐ ไปท้าย timeline
        timeline.time = timeline.duration;
        timeline.Evaluate();
        timeline.Stop();

        // ❌ ปิด dialogue ทันที
        if (dialogueManager != null)
        {
            dialogueManager.StopAllCoroutines();
            dialogueManager.ForceEndDialogue(); // 👈 สำคัญ
        }

        // 🎮 คืน control player
        if (dialogueManager.playerMovement != null)
        {
            dialogueManager.playerMovement.enabled = true;
            dialogueManager.playerMovement.StopMovementImmediately();
        }

        // ❌ ปิดปุ่ม skip
        if (skipCutsceneButton != null)
            skipCutsceneButton.SetActive(false);
    }

    // 🔥 ให้ DialogueManager เช็คได้
    public bool IsSkipping()
    {
        return isSkipping;
    }


}