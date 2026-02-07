using UnityEngine;

public class _4DiceBattleController : MonoBehaviour
{
    [Header("Dice Settings")]
    public int numberOfDice = 4;
    private BaseUnit myStats;

    void Start() => myStats = GetComponent<BaseUnit>();

    // 🔥 เพิ่มฟังก์ชันนี้เพื่อให้ BattleManager เรียกใช้ได้
    // bool forcedGuard = false หมายความว่า ถ้าไม่ส่งค่ามา ให้ถือว่าไม่ได้ป้องกัน (false)
    // ก๊อปปี้ฟังก์ชันนี้ไปทับ ExecuteAttack ตัวเดิมครับ
    public void ExecuteAttack(BaseUnit target, bool forcedGuard = false)
    {
        if (target == null || target.hp <= 0) return;

        Debug.Log($"<color=white>--- 🎲 [4 Dice Battle] {myStats.unitName} VS {target.unitName} ---</color>");

        int myTotal = RollDices(myStats.luck, myStats.unitName);
        int targetTotal = RollDices(target.luck, target.unitName);

        Debug.Log($"📊 ผลลัพธ์: {myStats.unitName} ({myTotal}) vs {target.unitName} ({targetTotal})");

        // เงื่อนไขชนะ: แต้มเรามากกว่า และ อีกฝ่ายไม่ได้กดป้องกัน
        if (myTotal >= targetTotal && !forcedGuard)
        {
            Debug.Log($"<color=green>⚔️ [ATTACK WIN] {myStats.unitName} ชนะ! {target.unitName} ป้องกันพลาด (โดนดาเมจเต็ม)</color>");
            target.TakeDamage(myStats.atk, false); // โดนเต็ม
        }
        else
        {
            // แพ้เพราะแต้มน้อยกว่า หรือ อีกฝ่ายตั้งใจกดป้องกันมาแล้ว
            string reason = forcedGuard ? "อีกฝ่ายตั้งการ์ดป้องกัน" : "แต้มสู้ไม่ได้";
            Debug.Log($"<color=red>🛡️ [DEFENSE WIN] {target.unitName} ชนะ! ({reason}) ลดดาเมจได้สำเร็จ</color>");

            target.TakeDamage(myStats.atk, true); // ป้องกันได้
        }
    }

    private int RollDices(int luck, string ownerName)
    {
        int total = 0;
        for (int i = 0; i < numberOfDice; i++)
        {
            total += GetWeightedRoll(luck);
        }
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