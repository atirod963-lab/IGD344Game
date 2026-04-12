using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthBarSync : MonoBehaviour
{
    [Header("Target Unit (ปล่อยว่างไว้ถ้าเป็นหลอดของศัตรู)")]
    public BaseUnit unit; // ของ Player ลากใส่ปกติ / ของ Enemy ให้ปล่อยเป็น None ไว้

    [Header("UI Elements")]
    public Image hpBarFill;
    public TextMeshProUGUI hpText;

    void Update()
    {
        // 1. ตั้งเป้าหมายเริ่มต้นเป็นตัวที่ลากใส่ใน Inspector (ถ้ามี)
        BaseUnit targetUnit = unit;

        // 2. ถ้าไม่ได้ลากอะไรใส่เลย (เป็นหลอดของศัตรู) ให้ไปดึงข้อมูลศัตรูตัวปัจจุบันจาก BattleManager
        if (targetUnit == null)
        {
            if (BattleManager.instance != null)
            {
                targetUnit = BattleManager.instance.enemy;
            }
        }

        // 3. ทำการอัปเดต UI เมื่อมีเป้าหมายให้ดึงข้อมูล
        if (targetUnit != null)
        {
            if (hpText != null)
                hpText.text = $"HP {targetUnit.hp}/{targetUnit.maxHp}";

            if (hpBarFill != null && targetUnit.maxHp > 0)
                hpBarFill.fillAmount = (float)targetUnit.hp / (float)targetUnit.maxHp;
        }
        else
        {
            // ถ้าไม่มีเป้าหมาย (เช่น ศัตรูตายแล้ว หรือยังไม่ได้เริ่มสู้) ให้รีเซ็ตหลอดเป็น 0
            if (hpBarFill != null) hpBarFill.fillAmount = 0;
            if (hpText != null) hpText.text = "HP 0/0";
        }
    }
}