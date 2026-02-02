using UnityEngine;

public class _4DiceBattleController : MonoBehaviour // ใน C# ชื่อคลาสขึ้นต้นด้วยตัวเลขไม่ได้ จึงขอเติม _ นำหน้านะครับ
{
    [Header("Dice Settings")]
    public int numberOfDice = 4;
    public string targetTag = "Enemy";

    private BaseUnit myStats;

    void Start()
    {
        myStats = GetComponent<BaseUnit>();
        if (myStats == null) Debug.LogError($"ลืมใส่สคริปต์สแตทที่ตัว {gameObject.name} นะ!");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(targetTag))
        {
            BaseUnit targetStats = other.GetComponent<BaseUnit>();

            if (targetStats != null && targetStats.hp > 0)
            {
                StartAttack(targetStats);
            }
        }
    }

    public void StartAttack(BaseUnit target)
    {
        Debug.Log($"<color=white>--- [เริ่มการปะทะ] {myStats.unitName} VS {target.unitName} ---</color>");

        int myTotal = RollDices(myStats.luck, myStats.unitName);
        int targetTotal = RollDices(target.luck, target.unitName);

        Debug.Log($"<color=cyan><b>[ผลสรุปแต้ม]</b> {myStats.unitName}: {myTotal} VS {target.unitName}: {targetTotal}</color>");

        if (myTotal >= targetTotal)
        {
            Debug.Log($"<color=green>{myStats.unitName} ชนะการทอย! {target.unitName} ป้องกันไม่ได้</color>");
            target.TakeDamage(myStats.atk, false);
        }
        else
        {
            Debug.Log($"<color=orange>{myStats.unitName} แพ้การทอย! {target.unitName} ป้องกันได้บางส่วน</color>");
            target.TakeDamage(myStats.atk, true);
        }
    }

    private int RollDices(int luck, string ownerName)
    {
        int total = 0;
        string detailLog = "";

        for (int i = 0; i < numberOfDice; i++)
        {
            int roll = GetWeightedRoll(luck);
            total += roll;
            detailLog += $"[{roll}] ";
        }

        Debug.Log($"{ownerName} ทอยได้: {detailLog} | <color=yellow>รวม: {total}</color>");
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