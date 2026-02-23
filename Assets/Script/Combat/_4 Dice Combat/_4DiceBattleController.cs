using UnityEngine;
using System.Linq; // อย่าลืมใส่ using นี้เพื่อให้ใช้ .Sum() ได้

public class _4DiceBattleController : MonoBehaviour
{
    [Header("Dice Settings")]
    public int numberOfDice = 4;
    private BaseUnit myStats;

    void Start()
    {
        if (myStats == null) myStats = GetComponent<BaseUnit>();
    }

    // 🔥 1. ฟังก์ชันใหม่: ทอยลูกเต๋า 4 ลูกด้วยสูตร Luck เดิม แล้วแพ็คใส่ Array ส่งไปให้ UI 
    public int[] GetDiceRollsArray()
    {
        if (myStats == null) myStats = GetComponent<BaseUnit>();

        int[] rolls = new int[numberOfDice];
        for (int i = 0; i < numberOfDice; i++)
        {
            rolls[i] = GetWeightedRoll(myStats.luck);
        }
        return rolls;
    }

    // 🔥 2. ฟังก์ชันใหม่: รับแต้มรวมที่ผ่านการโชว์มินิเกมมาแล้ว มาตัดสินผลแพ้ชนะตามคอนเซ็ปต์คุณ
    public void ExecuteAttackWithPreRolls(BaseUnit target, int myTotal, int targetTotal, bool forcedGuard = false)
    {
        if (target == null || target.hp <= 0) return;

        Debug.Log($"<color=white>--- 🎲 [4 Dice Battle] {myStats.unitName} VS {target.unitName} ---</color>");
        Debug.Log($"📊 ผลลัพธ์: {myStats.unitName} ({myTotal}) vs {target.unitName} ({targetTotal})");

        // เงื่อนไขชนะ: แต้มเรามากกว่า(หรือเท่ากับ) และ อีกฝ่ายไม่ได้กดป้องกัน
        if (myTotal >= targetTotal && !forcedGuard)
        {
            Debug.Log($"<color=green>⚔️ [ATTACK WIN] {myStats.unitName} ชนะ! {target.unitName} ป้องกันพลาด (โดนดาเมจเต็ม)</color>");
            target.TakeDamage(myStats.atk, false); // โดนเต็ม
        }
        else
        {
            // แพ้เพราะแต้มน้อยกว่า หรือ อีกฝ่ายตั้งใจกดป้องกันมาแล้ว
            string reason = forcedGuard ? "อีกฝ่ายตั้งการ์ดป้องกัน" : "แต้มสู้ไม่ได้";
            Debug.Log($"<color=red>🛡️ [DEFENSE WIN] {target.unitName} ชนะ/รอด! ({reason}) ลดดาเมจได้สำเร็จ</color>");

            target.TakeDamage(myStats.atk, true); // ป้องกันได้
        }
    }

    // (เก็บฟังก์ชัน ExecuteAttack ตัวเก่าไว้ เผื่อคุณอยากใช้ทอยแบบไม่ง้อ UI ในอนาคต)
    public void ExecuteAttack(BaseUnit target, bool forcedGuard = false)
    {
        int myTotal = RollDices(myStats.luck, myStats.unitName);
        int targetTotal = RollDices(target.luck, target.unitName);
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