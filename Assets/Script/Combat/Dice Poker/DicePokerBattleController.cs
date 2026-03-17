using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class DicePokerBattleController : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject dicePokerPanel;
    public TextMeshProUGUI statusText;
    public Button rollButton;

    [Header("Dice UI (Active & Kept)")]
    public Button[] activeDiceButtons = new Button[6];
    public Image[] activeDiceImages = new Image[6];

    // 🔥 แยก UI ฝั่ง Player และ Enemy ออกจากกัน (ฝั่งละ 5 ลูก)
    public Image[] playerKeptDiceImages = new Image[5];
    public Image[] enemyKeptDiceImages = new Image[5];

    [Header("Sprites")]
    public Sprite[] diceSprites;
    public Sprite emptySlotSprite;

    private BaseUnit attackerUnit;
    private BaseUnit defenderUnit;
    private bool isPlayerAttacking;
    private bool isCurrentTurnPlayer; // 🔥 เอาไว้เช็คว่าตอนนี้เทิร์นใคร

    private int currentRollCount = 0;
    private int[] currentActiveDiceValues = new int[6];
    private bool[] isDieSelectedToKeep = new bool[6];

    // 🔥 แยกข้อมูลเต๋าที่เก็บไว้ของแต่ละฝั่ง
    private List<int> playerKeptDice = new List<int>();
    private List<int> enemyKeptDice = new List<int>();
    private int newlyKeptThisRoll = 0;

    public enum PokerHandRank { HighCard, OnePair, TwoPair, ThreeOfAKind, Straight, FullHouse, FourOfAKind, FiveOfAKind }

    // ==========================================
    // ⚙️ การเริ่มมินิเกม
    // ==========================================
    public void StartDicePokerMinigame(BaseUnit attacker, BaseUnit defender, bool playerIsAttacker)
    {
        attackerUnit = attacker;
        defenderUnit = defender;
        isPlayerAttacking = playerIsAttacker;

        // ล้างข้อมูลเก่า
        playerKeptDice.Clear();
        enemyKeptDice.Clear();

        dicePokerPanel.SetActive(true);
        Time.timeScale = 0;

        StartCoroutine(GameFlowRoutine());
    }

    private IEnumerator GameFlowRoutine()
    {
        // 1. ฝ่ายโจมตีเล่นก่อน
        isCurrentTurnPlayer = isPlayerAttacking;
        yield return StartCoroutine(PlayTurn(attackerUnit, isPlayerAttacking));

        // 2. ฝ่ายป้องกันเล่นต่อ
        isCurrentTurnPlayer = !isPlayerAttacking;
        yield return StartCoroutine(PlayTurn(defenderUnit, !isPlayerAttacking));

        // 3. สรุปผลและคำนวณดาเมจ
        CalculateAndApplyDamage();

        yield return new WaitForSecondsRealtime(3f); // โชว์ผลค้างไว้ 3 วิ
        dicePokerPanel.SetActive(false);
        Time.timeScale = 1;
    }

    // ==========================================
    // 🎲 ระบบการเล่นใน 1 เทิร์น
    // ==========================================
    private IEnumerator PlayTurn(BaseUnit unit, bool isPlayer)
    {
        currentRollCount = 0;
        List<int> currentKeptList = isPlayer ? playerKeptDice : enemyKeptDice;

        statusText.text = $"ตาของ {unit.unitName} กำลังเตรียมตัว...";
        yield return new WaitForSecondsRealtime(1f);

        while (currentRollCount < 3 && currentKeptList.Count < 5)
        {
            currentRollCount++;
            newlyKeptThisRoll = 0;
            ResetActiveDiceSelection();

            int diceToRoll = (currentRollCount == 1) ? 6 : (6 - currentKeptList.Count);
            RollActiveDice(diceToRoll);
            UpdateUI();

            statusText.text = $"เทิร์น {unit.unitName} - ทอยครั้งที่ {currentRollCount}/3";

            if (isPlayer)
            {
                rollButton.interactable = true;
                bool hasRolledOrFinished = false;

                while (!hasRolledOrFinished)
                {
                    if (currentKeptList.Count + newlyKeptThisRoll >= 5)
                    {
                        hasRolledOrFinished = true;
                    }
                    yield return null;
                    if (playerConfirmedRoll)
                    {
                        playerConfirmedRoll = false;
                        hasRolledOrFinished = true;
                    }
                }
                rollButton.interactable = false;
                CommitSelectedDice(currentKeptList);
            }
            else
            {
                yield return new WaitForSecondsRealtime(1.5f);
                EnemyAISelectDice(currentKeptList);
                CommitSelectedDice(currentKeptList);
            }
        }

        statusText.text = $"{unit.unitName} จบเทิร์น!";
        UpdateUI();
        yield return new WaitForSecondsRealtime(1.5f);
    }

    // ==========================================
    // 🖱️ Player UI Interactions
    // ==========================================
    public void OnClick_ActiveDie(int index)
    {
        if (!activeDiceButtons[index].gameObject.activeSelf) return;

        List<int> currentKeptList = isCurrentTurnPlayer ? playerKeptDice : enemyKeptDice;

        if (!isDieSelectedToKeep[index] && currentKeptList.Count + newlyKeptThisRoll >= 5) return;

        isDieSelectedToKeep[index] = !isDieSelectedToKeep[index];
        newlyKeptThisRoll += isDieSelectedToKeep[index] ? 1 : -1;

        activeDiceImages[index].color = isDieSelectedToKeep[index] ? Color.gray : Color.white;
    }

    private bool playerConfirmedRoll = false;

    public void OnClick_RollButton()
    {
        List<int> currentKeptList = isCurrentTurnPlayer ? playerKeptDice : enemyKeptDice;

        if (currentRollCount == 1 && newlyKeptThisRoll < 1)
        {
            Debug.LogWarning("ต้องเก็บเต๋าอย่างน้อย 1 ลูกในการทอยครั้งแรก!");
            return;
        }
        if (currentRollCount == 2 && newlyKeptThisRoll < 1)
        {
            Debug.LogWarning("ต้องเก็บเต๋าอย่างน้อย 1 ลูกในการทอยครั้งที่สอง!");
            return;
        }
        if (currentRollCount == 3 && (currentKeptList.Count + newlyKeptThisRoll < 5))
        {
            Debug.LogWarning("ครั้งสุดท้าย ต้องเก็บเต๋าให้ครบ 5 ลูก!");
            return;
        }

        playerConfirmedRoll = true;
    }

    // ==========================================
    // ⚙️ Core Logic
    // ==========================================
    private void RollActiveDice(int count)
    {
        for (int i = 0; i < 6; i++)
        {
            if (i < count)
            {
                currentActiveDiceValues[i] = Random.Range(1, 7);
                activeDiceButtons[i].gameObject.SetActive(true);
            }
            else
            {
                currentActiveDiceValues[i] = 0;
                activeDiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void ResetActiveDiceSelection()
    {
        for (int i = 0; i < 6; i++)
        {
            isDieSelectedToKeep[i] = false;
            activeDiceImages[i].color = Color.white;
        }
    }

    private void CommitSelectedDice(List<int> currentKeptList)
    {
        for (int i = 0; i < 6; i++)
        {
            if (isDieSelectedToKeep[i] && currentActiveDiceValues[i] > 0)
            {
                currentKeptList.Add(currentActiveDiceValues[i]);
            }
        }
    }

    private void UpdateUI()
    {
        // 1. อัปเดตกระดานหลัก
        for (int i = 0; i < 6; i++)
        {
            if (currentActiveDiceValues[i] > 0)
                activeDiceImages[i].sprite = diceSprites[currentActiveDiceValues[i] - 1];
        }

        // 2. อัปเดตฝั่ง Player 🔥
        for (int i = 0; i < 5; i++)
        {
            if (i < playerKeptDice.Count)
                playerKeptDiceImages[i].sprite = diceSprites[playerKeptDice[i] - 1];
            else
                playerKeptDiceImages[i].sprite = emptySlotSprite;
        }

        // 3. อัปเดตฝั่ง Enemy 🔥
        for (int i = 0; i < 5; i++)
        {
            if (i < enemyKeptDice.Count)
                enemyKeptDiceImages[i].sprite = diceSprites[enemyKeptDice[i] - 1];
            else
                enemyKeptDiceImages[i].sprite = emptySlotSprite;
        }
    }

    private void EnemyAISelectDice(List<int> currentKeptList)
    {
        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (int v in currentKeptList) { if (!counts.ContainsKey(v)) counts[v] = 0; counts[v]++; }
        foreach (int v in currentActiveDiceValues) { if (v > 0) { if (!counts.ContainsKey(v)) counts[v] = 0; counts[v]++; } }

        int targetFace = counts.OrderByDescending(kvp => kvp.Value).First().Key;

        int selectedThisRound = 0;
        for (int i = 0; i < 6; i++)
        {
            if (currentActiveDiceValues[i] > 0 && currentKeptList.Count + selectedThisRound < 5)
            {
                if (currentActiveDiceValues[i] == targetFace || (currentRollCount <= 2 && selectedThisRound == 0 && i == currentActiveDiceValues.Count(v => v > 0) - 1))
                {
                    isDieSelectedToKeep[i] = true;
                    selectedThisRound++;
                }
            }
        }

        if (currentRollCount == 3)
        {
            for (int i = 0; i < 6 && currentKeptList.Count + selectedThisRound < 5; i++)
            {
                if (currentActiveDiceValues[i] > 0 && !isDieSelectedToKeep[i])
                {
                    isDieSelectedToKeep[i] = true;
                    selectedThisRound++;
                }
            }
        }
    }

    // ==========================================
    // ⚔️ ระบบคำนวณดาเมจ
    // ==========================================
    private void CalculateAndApplyDamage()
    {
        // 🔥 เช็คว่าใครเป็นคนบุก เพื่อโยน List เต๋าไปให้ถูกฝั่ง
        List<int> atkDice = isPlayerAttacking ? playerKeptDice : enemyKeptDice;
        List<int> defDice = !isPlayerAttacking ? playerKeptDice : enemyKeptDice;

        PokerHandRank atkRank = EvaluateRank(atkDice);
        PokerHandRank defRank = EvaluateRank(defDice);

        statusText.text = $"โจมตี: {atkRank} | ป้องกัน: {defRank}";
        Debug.Log($"Attacker Rank: {atkRank}, Defender Rank: {defRank}");

        float baseAtk = attackerUnit.atk;
        float baseDef = defenderUnit.def;

        int finalDamage = 0;

        if (atkRank > defRank)
        {
            float atkMultiplier = GetAttackerMultiplier(atkRank);
            finalDamage = Mathf.CeilToInt(baseAtk * atkMultiplier);
            Debug.Log($"<color=red>โจมตีสำเร็จ!</color> Base Atk {baseAtk} * {atkMultiplier} = {finalDamage}");
        }
        else if (atkRank < defRank)
        {
            float defMultiplier = GetDefenderMultiplier(defRank);
            int effectiveDef = Mathf.FloorToInt(baseDef * defMultiplier);

            finalDamage = Mathf.Max(0, Mathf.CeilToInt(baseAtk) - effectiveDef);
            Debug.Log($"<color=green>ป้องกันสำเร็จ!</color> Def {baseDef} * {defMultiplier} = {effectiveDef}. รับดาเมจจริง {finalDamage}");
        }
        else
        {
            int halfAtk = Mathf.FloorToInt(baseAtk * 0.5f);
            int halfDef = Mathf.FloorToInt(baseDef * 0.5f);
            finalDamage = Mathf.Max(0, halfAtk - halfDef);
            Debug.Log($"<color=yellow>เสมอกัน!</color> Atk/2 ({halfAtk}) - Def/2 ({halfDef}) = รับดาเมจจริง {finalDamage}");
        }

        if (finalDamage > 0)
        {
            defenderUnit.TakeDamage(finalDamage, isPlayerAttacking);
        }
    }

    private float GetAttackerMultiplier(PokerHandRank rank)
    {
        switch (rank)
        {
            case PokerHandRank.FiveOfAKind: return 2.5f;
            case PokerHandRank.FourOfAKind: return 2f;
            case PokerHandRank.FullHouse: return 1.3f;
            case PokerHandRank.Straight: return 1.2f;
            default: return 1.0f;
        }
    }

    private float GetDefenderMultiplier(PokerHandRank rank)
    {
        switch (rank)
        {
            case PokerHandRank.FiveOfAKind: return 2.5f;
            case PokerHandRank.FourOfAKind: return 2f;
            case PokerHandRank.FullHouse: return 1.3f;
            case PokerHandRank.Straight: return 1.2f;
            default: return 1.0f;
        }
    }

    private PokerHandRank EvaluateRank(List<int> dice)
    {
        if (dice.Count < 5) return PokerHandRank.HighCard;

        Dictionary<int, int> counts = new Dictionary<int, int>();
        foreach (int val in dice) { if (!counts.ContainsKey(val)) counts[val] = 0; counts[val]++; }

        List<int> sortedCounts = counts.Values.ToList();
        sortedCounts.Sort();
        sortedCounts.Reverse();

        List<int> sortedVals = dice.ToList();
        sortedVals.Sort();
        bool isStraight = (sortedVals[4] - sortedVals[0] == 4) && counts.Count == 5;

        if (sortedCounts[0] == 5) return PokerHandRank.FiveOfAKind;
        if (sortedCounts[0] == 4) return PokerHandRank.FourOfAKind;
        if (sortedCounts[0] == 3 && sortedCounts[1] == 2) return PokerHandRank.FullHouse;
        if (isStraight) return PokerHandRank.Straight;
        if (sortedCounts[0] == 3) return PokerHandRank.ThreeOfAKind;
        if (sortedCounts[0] == 2 && sortedCounts[1] == 2) return PokerHandRank.TwoPair;
        if (sortedCounts[0] == 2) return PokerHandRank.OnePair;

        return PokerHandRank.HighCard;
    }
}