using UnityEngine;

public static class StatCalculator
{
    // ฟังก์ชันคำนวณค่าที่จะ "บวกเพิ่ม" ในเลเวลนั้นๆ (Growth Delta)
    public static int GetStatGain(int levelReached, StatGrowthInfo info)
    {
        // 1. กฎ Milestone (ทับทุกกฎ)
        // Lv 5->+2, Lv 10->+3, Lv 15->+4, Lv 20->+5
        if (levelReached % 5 == 0)
        {
            return (levelReached / 5) + 1;
        }

        // 2. กฎ Seal (ถ้าเกินเลเวลตัน ไม่บวกเพิ่ม)
        if (levelReached > info.sealLevel)
        {
            return 0;
        }

        // 3. กฎ Fate vs Normal
        if (info.isFate)
        {
            // เขียว: บวกตาม Tier (Lv 1-4=+1, Lv 6-9=+2, etc.)
            // สูตรคำนวณ Tier คือ (เลเวล / 5) + 1
            return (levelReached / 5) + 1;
        }
        else
        {
            // ขาว: บวก 1 เสมอ
            return 1;
        }
    }
}