using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiceMinigameController : MonoBehaviour
{
    [Header("UI Panels")]
    public GameObject minigamePanel;
    public TextMeshProUGUI titleText; // เอาไว้โชว์คำว่า "เทิร์นของคุณ" / "เทิร์นศัตรู"

    [Header("Summary Panel")]
    public GameObject summaryPanel; // หน้าต่างสรุปผลคะแนน
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI enemyScoreText;

    [Header("Phase 1: Single Dice")]
    public GameObject singleDiceObj;
    public CanvasGroup singleDiceCG;
    public Button singleDiceButton;

    [Header("Phase 2: The Box")]
    public GameObject boxObj;
    public CanvasGroup boxCG;
    public BoxInteract boxInteract;

    [Header("Phase 3: Four Dice")]
    public GameObject fourDiceContainer;
    public CanvasGroup fourDiceCG;
    public TextMeshProUGUI[] diceTexts;

    private System.Action onMinigameComplete;
    private bool singleDiceClicked = false;

    void Start()
    {
        if (singleDiceButton != null)
            singleDiceButton.onClick.AddListener(() => singleDiceClicked = true);

        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(false);
    }

    // 🔥 ฟังก์ชันนี้แหละครับที่ BattleManager หาไม่เจอ
    public void StartDoubleMinigame(int[] playerDice, int[] enemyDice, System.Action callback)
    {
        onMinigameComplete = callback;
        minigamePanel.SetActive(true);
        summaryPanel.SetActive(false);
        StartCoroutine(DoubleMinigameRoutine(playerDice, enemyDice));
    }

    private IEnumerator DoubleMinigameRoutine(int[] playerDice, int[] enemyDice)
    {
        // --- รอบที่ 1: ผู้เล่น (ต้องกดเอง) ---
        if (titleText != null) titleText.text = "ทอยลูกเต๋าของคุณ!";
        yield return StartCoroutine(PlayOneRound(playerDice, false));
        int pTotal = playerDice.Sum();

        yield return new WaitForSeconds(1.5f);

        // --- รอบที่ 2: ศัตรู (เล่นออโต้) ---
        if (titleText != null) titleText.text = "ศัตรูกำลังทอยลูกเต๋า...";
        yield return StartCoroutine(PlayOneRound(enemyDice, true));
        int eTotal = enemyDice.Sum();

        yield return new WaitForSeconds(1.5f);

        // --- รอบที่ 3: โชว์สรุปผล ---
        fourDiceContainer.SetActive(false);
        if (titleText != null) titleText.text = "สรุปผลการปะทะ!";

        summaryPanel.SetActive(true);
        if (playerScoreText != null) playerScoreText.text = $"คุณ: {pTotal}";
        if (enemyScoreText != null) enemyScoreText.text = $"ศัตรู: {eTotal}";

        yield return new WaitForSeconds(2.5f);

        // จบงาน ส่งกลับไปคำนวณดาเมจ
        minigamePanel.SetActive(false);
        onMinigameComplete?.Invoke();
    }

    private IEnumerator PlayOneRound(int[] lockedDice, bool isAuto)
    {
        singleDiceClicked = false;
        boxInteract.ResetBox();

        singleDiceObj.SetActive(true);
        singleDiceCG.alpha = 1f;
        singleDiceObj.transform.localScale = Vector3.one;

        boxObj.SetActive(false);
        boxCG.alpha = 0f;
        boxObj.transform.localScale = Vector3.zero;

        fourDiceContainer.SetActive(false);
        if (fourDiceCG != null) fourDiceCG.alpha = 0f;

        if (!isAuto) yield return new WaitUntil(() => singleDiceClicked);
        else yield return new WaitForSeconds(0.5f);

        boxObj.SetActive(true);
        float t = 0; float duration = 0.5f;
        while (t < duration)
        {
            t += Time.deltaTime;
            singleDiceCG.alpha = Mathf.Lerp(1, 0, t / duration);
            singleDiceObj.transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, t / duration);
            boxCG.alpha = Mathf.Lerp(0, 1, t / duration);
            boxObj.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t / duration);
            yield return null;
        }
        singleDiceObj.SetActive(false);

        if (!isAuto)
        {
            yield return new WaitUntil(() => boxInteract.hasFinishedShaking);
        }
        else
        {
            boxInteract.transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
            yield return new WaitForSeconds(0.8f);
            boxInteract.transform.localScale = Vector3.one;
        }

        t = 0;
        while (t < duration)
        {
            t += Time.deltaTime;
            boxCG.alpha = Mathf.Lerp(1, 0, t / duration);
            yield return null;
        }
        boxObj.SetActive(false);

        fourDiceContainer.SetActive(true);
        t = 0;
        float fadeDuration = 0.5f;
        float nextShuffle = 0;
        while (t < fadeDuration + 1.0f)
        {
            t += Time.deltaTime;
            if (t <= fadeDuration && fourDiceCG != null)
                fourDiceCG.alpha = Mathf.Lerp(0, 1, t / fadeDuration);

            if (t >= nextShuffle)
            {
                nextShuffle += 0.05f;
                ShuffleDiceNumbers();
            }
            yield return null;
        }

        for (int i = 0; i < diceTexts.Length; i++)
        {
            diceTexts[i].text = lockedDice[i].ToString();
        }
    }

    private void ShuffleDiceNumbers()
    {
        for (int i = 0; i < diceTexts.Length; i++)
            diceTexts[i].text = Random.Range(1, 10).ToString();
    }
}