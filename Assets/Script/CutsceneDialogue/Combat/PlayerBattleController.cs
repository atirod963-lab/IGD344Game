using UnityEngine;

public class PlayerBattleController : MonoBehaviour
{
    [Header("Dice Settings")]
    public int numberOfDice = 4;

    private PlayerStats myStats; // เปลี่ยนเป็น PlayerStats ตามโครงสร้างใหม่

    void Start()
    {
        myStats = GetComponent<PlayerStats>();
        if (myStats == null) Debug.LogError("ลืมใส่ PlayerStats ที่ตัว Player นะ!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemyStats = other.GetComponent<EnemyStats>();

            if (enemyStats != null && enemyStats.hp > 0)
            {
                StartAttack(enemyStats);
            }
        }
    }

    public void StartAttack(EnemyStats enemy)
    {
        Debug.Log($"<color=white>--- [เริ่มการปะทะ] {myStats.unitName} VS {enemy.unitName} ---</color>");

        // 1. ทอยลูกเต๋าพร้อมโชว์ Log รายลูก
        int playerTotal = RollDices(myStats.luck, "Player");
        int enemyTotal = RollDices(enemy.luck, "Enemy");

        Debug.Log($"<color=cyan><b>[ผลสรุปแต้ม]</b> Player: {playerTotal} VS Enemy: {enemyTotal}</color>");

        // 2. เงื่อนไข: ถ้าทอยชนะ/เสมอ ตีเข้าเต็มๆ | ถ้าทอยแพ้ มอนใช้ Def ป้องกัน
        if (playerTotal >= enemyTotal)
        {
            Debug.Log("<color=green>ทอยชนะ! มอนสเตอร์ป้องกันไม่ได้</color>");
            enemy.TakeDamage(myStats.atk, false); // false = ไม่มีการป้องกัน
        }
        else
        {
            Debug.Log("<color=orange>ทอยแพ้! มอนสเตอร์ตั้งการ์ดป้องกันได้บางส่วน</color>");
            enemy.TakeDamage(myStats.atk, true); // true = ใช้ค่า Def หักลบดาเมจ
        }
    }

    private int RollDices(int luck, string owner)
    {
        int total = 0;
        string detailLog = ""; // เก็บรายละเอียดแต่ละลูกไว้โชว์ทีเดียว

        for (int i = 0; i < numberOfDice; i++)
        {
            int roll = GetWeightedRoll(luck);
            total += roll;
            detailLog += $"ลูกที่ {i + 1}: [{roll}]  ";
        }

        // Debug ออกมาว่าใครทอยได้เท่าไหร่บ้าง
        Debug.Log($"{owner} ทอยได้: {detailLog} | <color=yellow>รวม: {total}</color>");
        return total;
    }

    private int GetWeightedRoll(int luck)
    {
        float chanceRoll = Random.Range(0f, 100f);
        float highThreshold = 15f + (luck * 0.5f);
        float midThreshold = highThreshold + 60f;

        if (chanceRoll < highThreshold)
            return Random.Range(16, 21);
        else if (chanceRoll < midThreshold)
            return Random.Range(6, 16);
        else
            return Random.Range(1, 6);
    }
}