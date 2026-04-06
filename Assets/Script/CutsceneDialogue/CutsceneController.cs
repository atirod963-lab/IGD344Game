using UnityEngine;
using UnityEngine.Playables;
using static cutsceneDialogueManager;

public class CutsceneController : MonoBehaviour
{
    public cutsceneDialogueManager dialogueManager;
    public PlayableDirector timeline;

    [System.Serializable]
    public class DialogueSet
    {
        public string setName;
        public DialogueLine[] lines;
    }

    public DialogueSet[] dialogueSets;

    void Start()
    {
        // ตอน Timeline จบ → คืนการควบคุมให้ผู้เล่น
        timeline.stopped += OnTimelineFinished;
    }

    void OnDestroy()
    {
        timeline.stopped -= OnTimelineFinished;
    }

    // เรียกตอน Timeline เล่นจบ
    private void OnTimelineFinished(PlayableDirector director)
    {
        Debug.Log("Timeline หยุดแล้ว!");
        if (dialogueManager.playerMovement != null)
        {
            dialogueManager.playerMovement.enabled = true;
            Debug.Log("OnTimelineFinished: enable playerMovement แล้ว");
        }
    }

    public void TriggerDialogue(int setIndex)
    {
        if (setIndex >= dialogueSets.Length) return;

        double currentTime = timeline.time;
        timeline.Pause();

        // ปิด Timeline binding ชั่วคราวเพื่อไม่ให้มัน override ตำแหน่งตัวละคร
        timeline.playableGraph.IsValid();
        SetTimelineBindingsActive(false);

        dialogueManager.StartDialogue(
            dialogueSets[setIndex].lines,
            onEnd: () =>
            {
                // เปิด binding กลับก่อน Play ต่อ
                SetTimelineBindingsActive(true);
                timeline.time = currentTime;
                timeline.Play();
            }
        );
    }

    private void SetTimelineBindingsActive(bool active)
    {
        foreach (var output in timeline.playableAsset.outputs)
        {
            var binding = timeline.GetGenericBinding(output.sourceObject);
            Debug.Log($"Binding: {binding} → active: {active}"); // ← เพิ่มบรรทัดนี้
            if (binding is Behaviour behaviour)
                behaviour.enabled = active;
            else if (binding is GameObject go)
                go.SetActive(active);
        }
    }
}