using UnityEngine;

public class UnitStats : MonoBehaviour
{
    [Header("Settings")]
    public MonsterBaseData baseData;
    public FateCoinData currentFate;

    [Header("Level Control")]
    [Range(0, 50)] public int level = 1;

    [Header("Result Stats (Preview)")]
    // แก้เป็น float ตาม BaseUnit ตัวใหม่ (ยกเว้น Luck)
    public float hp;
    public float atk;
    public float def;
    public float spd;
    public int luck;

    void OnValidate()
    {
        CalculatePreview();
    }

    void Start()
    {
        CalculatePreview();
    }

    public void CalculatePreview()
    {
        if (baseData == null || currentFate == null) return;

        // 1. ตั้งค่าเริ่มต้นจาก Base
        hp = baseData.baseHp;
        atk = baseData.baseAtk;
        def = baseData.baseDef;
        spd = baseData.baseSpd;
        luck = baseData.baseLuck;

        // 2. วนลูปจำลองการอัพเลเวลตั้งแต่ 1 ถึงเลเวลปัจจุบัน
        for (int i = 1; i <= level; i++)
        {
            // ใช้ GetStatGain ตัวใหม่ มาบวกทบเข้าไป
            hp += StatCalculator.GetStatGain(i, currentFate.hp);
            atk += StatCalculator.GetStatGain(i, currentFate.atk);
            def += StatCalculator.GetStatGain(i, currentFate.def);
            spd += StatCalculator.GetStatGain(i, currentFate.spd);
            luck += StatCalculator.GetStatGain(i, currentFate.luck);
        }
    }
}