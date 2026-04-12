using UnityEngine;
using System.Linq;

public class _4DiceBattleController : MonoBehaviour
{
    [Header("Dice Settings")]
    public int numberOfDice = 4;
    private BaseUnit myStats;

    void Start()
    {
        if (myStats == null) myStats = GetComponent<BaseUnit>();
    }

    public int[] GetDiceRollsArray()
    {
        if (myStats == null) myStats = GetComponent<BaseUnit>();

        int[] rolls = new int[numberOfDice];
        for (int i = 0; i < numberOfDice; i++)
        {
            // 🔥 ดึง TotalLuck มาใช้
            rolls[i] = GetWeightedRoll(myStats.TotalLuck);
        }
        return rolls;
    }

    public void ExecuteAttackWithPreRolls(BaseUnit target, int myTotal, int targetTotal, bool forcedGuard = false)
    {
        if (target == null || target.hp <= 0) return;

        Debug.Log($"<color=white>--- 🎲 [4 Dice Battle] {myStats.unitName} VS {target.unitName} ---</color>");
        Debug.Log($"📊 ผลลัพธ์: {myStats.unitName} ({myTotal}) vs {target.unitName} ({targetTotal})");

        if (myTotal >= targetTotal && !forcedGuard)
        {
            Debug.Log($"<color=green>⚔️ [ATTACK WIN] {myStats.unitName} ชนะ! {target.unitName} ป้องกันพลาด (โดนดาเมจเต็ม)</color>");
            // 🔥 ตีด้วย TotalAtk
            target.TakeDamage(myStats.TotalAtk, false);
        }
        else
        {
            string reason = forcedGuard ? "อีกฝ่ายตั้งการ์ดป้องกัน" : "แต้มสู้ไม่ได้";
            Debug.Log($"<color=red>🛡️ [DEFENSE WIN] {target.unitName} ชนะ/รอด! ({reason}) ลดดาเมจได้สำเร็จ</color>");
            // 🔥 ตีด้วย TotalAtk
            target.TakeDamage(myStats.TotalAtk, true);
        }
    }

    public void ExecuteAttack(BaseUnit target, bool forcedGuard = false)
    {
        int myTotal = RollDices(myStats.TotalLuck, myStats.unitName);
        int targetTotal = RollDices(target.TotalLuck, target.unitName);
        ExecuteAttackWithPreRolls(target, myTotal, targetTotal, forcedGuard);
    }

    private int RollDices(int luck, string ownerName)
    {
        int total = 0;
        for (int i = 0; i < numberOfDice; i++) total += GetWeightedRoll(luck);
        return total;
    }

    private int GetWeightedRoll(int luck)
    {
        float chanceRoll = Random.Range(0f, 100f);
        float highThreshold = 15f + (luck * 0.5f);
        float midThreshold = highThreshold + 60f;

        if (chanceRoll < highThreshold) return Random.Range(16, 21);
        else if (chanceRoll < midThreshold) return Random.Range(6, 16);
        else return Random.Range(1, 6);
    }
}