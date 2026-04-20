using UnityEngine;
using UnityEngine.Playables;
using System.Collections;
using static cutsceneDialogueManager;

public class CutsceneController : MonoBehaviour
{
    public cutsceneDialogueManager dialogueManager;
    public PlayableDirector timeline;
    public GameObject skipCutsceneButton;

    private bool isSkipping = false; // 🔥 flag
    private Vector3 timelineEndPosition; // Store timeline end position

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

    // Start cutscene
    public void StartCutscene()
    {
        timeline.Play();
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

        // ลบ coroutine RestoreTimelineEndPosition ออกได้เลย ไม่จำเป็นแล้ว
        if (dialogueManager.playerMovement != null)
        {
            dialogueManager.playerMovement.enabled = true;
            dialogueManager.playerMovement.StopMovementImmediately();
        }
    }

    private IEnumerator RestoreTimelineEndPosition()
    {
        yield return new WaitForEndOfFrame(); // Wait for timeline to fully stop
        
        if (dialogueManager.playerMovement != null)
        {
            dialogueManager.playerMovement.transform.position = timelineEndPosition;
            dialogueManager.playerMovement.enabled = true;
            dialogueManager.playerMovement.StopMovementImmediately();
        }
    }

    // 🔥 Timeline เรียก Dialogue
    public void TriggerDialogue(int setIndex)
    {
        if (isSkipping) return;
        if (setIndex >= dialogueSets.Length) return;

        double pausedTime = timeline.time;

        // ✅ แทน timeline.Pause() — ให้ set speed = 0 แทน
        timeline.playableGraph.GetRootPlayable(0).SetSpeed(0);

        dialogueManager.StartDialogue(
            dialogueSets[setIndex].lines,
            onEnd: () =>
            {
                if (isSkipping) return;

                // ✅ Resume โดย set speed กลับเป็น 1
                timeline.playableGraph.GetRootPlayable(0).SetSpeed(1);
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

        // Capture timeline end position and restore it
        if (dialogueManager.playerMovement != null)
        {
            timelineEndPosition = dialogueManager.playerMovement.transform.position;
            StartCoroutine(RestoreTimelineEndPosition());
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