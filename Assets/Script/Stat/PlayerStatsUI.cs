using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatsUI : MonoBehaviour
{
    [Header("Main Panels")]
    public GameObject statsMenuPanel; // หน้าต่างสแต็ทหลัก
    public GameObject fateInventoryPanel; // หน้าต่างเลือกเหรียญ Fate
    public GameObject weaponInventoryPanel; // หน้าต่างเลือกอาวุธ

    [Header("Player Reference")]
    public BaseUnit playerUnit; // ลากตัวละคร Player มาใส่ตรงนี้

    [Header("Text UI - Level & EXP")]
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;

    [Header("Text UI - Stats")]
    public TextMeshProUGUI hpText;
    public TextMeshProUGUI atkText;
    public TextMeshProUGUI defText;
    public TextMeshProUGUI spdText;
    public TextMeshProUGUI luckText;

    [Header("Equipment UI - Weapon")]
    public Image weaponIcon;
    public TextMeshProUGUI weaponNameText;

    [Header("Equipment UI - Fate Coin")]
    public Image fateCoinIcon;
    public TextMeshProUGUI fateCoinNameText;

    public Sprite emptySlotSprite; // รูปสี่เหลี่ยมว่างๆ ตอนที่ยังไม่ได้ใส่อะไร

    // ==========================================
    // ⚙️ ฟังก์ชันเปิด/ปิดหน้าต่าง
    // ==========================================
    public void ToggleStatsMenu()
    {
        bool isActive = !statsMenuPanel.activeSelf;
        statsMenuPanel.SetActive(isActive);

        // ถ้ากำลังเปิดหน้าต่าง ให้สั่งอัปเดตข้อมูลล่าสุดด้วย
        if (isActive)
        {
            UpdateStatsUI();
        }
    }

    // ==========================================
    // 📊 ฟังก์ชันดึงข้อมูลมาแสดงผล
    // ==========================================
    public void UpdateStatsUI()
    {
        if (playerUnit == null) return;

        // 1. อัปเดตเลเวลและ EXP
        levelText.text = $"Level: {playerUnit.currentLevel}";

        if (playerUnit.levelTable != null && playerUnit.currentLevel < playerUnit.levelTable.maxLevel)
            expText.text = $"EXP: {playerUnit.currentExp} / {playerUnit.requiredExp}";
        else
            expText.text = "EXP: MAX";

        // 2. อัปเดตสเตตัส (ดึงค่า Total ที่บวกโบนัสอาวุธแล้วมาโชว์)
        hpText.text = $"HP: {playerUnit.hp} / {playerUnit.maxHp}";
        atkText.text = $"ATK: {playerUnit.TotalAtk}";   // ดึง TotalAtk
        defText.text = $"DEF: {playerUnit.def}";
        spdText.text = $"SPD: {playerUnit.spd}";
        luckText.text = $"LUCK: {playerUnit.TotalLuck}"; // ดึง TotalLuck

        // 3. อัปเดตอาวุธที่ใส่อยู่
        if (playerUnit.equippedWeapon != null)
        {
            weaponIcon.sprite = playerUnit.equippedWeapon.weaponIcon;
            weaponNameText.text = playerUnit.equippedWeapon.weaponName;
            weaponIcon.color = Color.white;
        }
        else
        {
            weaponIcon.sprite = emptySlotSprite;
            weaponNameText.text = "ไม่มีอาวุธ";
        }

        // 4. อัปเดตเหรียญ Fate ที่ใส่อยู่
        if (playerUnit.currentFate != null)
        {
            // หมายเหตุ: สมมติว่าใน FateCoinData ของคุณมีตัวแปรชื่อ coinIcon นะครับ ถ้าชื่ออื่นให้แก้ให้ตรง
            // fateCoinIcon.sprite = playerUnit.currentFate.coinIcon; 
            fateCoinNameText.text = playerUnit.currentFate.coinName;
            fateCoinIcon.color = Color.white;
        }
        else
        {
            fateCoinIcon.sprite = emptySlotSprite;
            fateCoinNameText.text = "ไม่มีเหรียญ";
        }
    }

    // ==========================================
    // 🎒 ฟังก์ชันกดปุ่มไปหน้า Inventory
    // ==========================================
    public void OpenFateInventory()
    {
        // เปิดหน้ากระเป๋าเหรียญ (ถ้าคุณอยากให้หน้าสแต็ทปิดไปด้วย ให้เติม statsMenuPanel.SetActive(false);)
        if (fateInventoryPanel != null) fateInventoryPanel.SetActive(true);
    }

    public void OpenWeaponInventory()
    {
        // เปิดหน้ากระเป๋าอาวุธ
        if (weaponInventoryPanel != null) weaponInventoryPanel.SetActive(true);
    }
}