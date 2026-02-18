using UnityEngine;
using UnityEngine.UI;
using TMPro; // เผื่อใช้ TextMeshPro

public class SimpleInventoryUI : MonoBehaviour
{
    public FateInventory inventory;
    public GameObject buttonPrefab;
    public Transform contentPanel;

    [Header("Fix Canvas")]
    public Canvas mainCanvas;

    [Header("Key to Open")]
    public KeyCode toggleKey = KeyCode.Tab;
    private bool isOpen = false;

    void Start()
    {
        if (inventory == null)
        {
            inventory = GetComponent<FateInventory>();
            if (inventory == null) inventory = FindObjectOfType<FateInventory>();
        }

        if (mainCanvas != null) mainCanvas.gameObject.SetActive(true);
        CloseMenu();
    }

    void Update()
    {
        if (mainCanvas != null && mainCanvas.gameObject.activeSelf == false)
            mainCanvas.gameObject.SetActive(true);

        if (Input.GetKeyDown(toggleKey))
        {
            if (isOpen) CloseMenu();
            else OpenMenu();
        }
    }

    void OpenMenu()
    {
        isOpen = true;
        if (contentPanel != null) contentPanel.gameObject.SetActive(true);
        RefreshUI(); // วาดปุ่มใหม่ทุกครั้งที่เปิด
    }

    void CloseMenu()
    {
        isOpen = false;
        if (contentPanel != null) contentPanel.gameObject.SetActive(false);
    }

    // 🔥 แก้ไขฟังก์ชันนี้: ให้ปุ่มกดแล้ว Refresh หน้าจอด้วย
    void RefreshUI()
    {
        // ลบปุ่มเก่าทิ้ง
        foreach (Transform child in contentPanel) Destroy(child.gameObject);

        // วนลูปสร้างปุ่มใหม่
        for (int i = 0; i < inventory.ownedCoins.Count; i++)
        {
            int index = i; // เก็บ index ไว้ใช้ในปุ่ม
            FateCoinData coin = inventory.ownedCoins[i];

            GameObject btn = Instantiate(buttonPrefab, contentPanel);

            // ตั้งชื่อปุ่ม
            var tmPro = btn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmPro != null) tmPro.text = coin.coinName;
            else
            {
                var oldText = btn.GetComponentInChildren<Text>();
                if (oldText != null) oldText.text = coin.coinName;
            }

            // ตั้งคำสั่งเมื่อกดปุ่ม
            btn.GetComponent<Button>().onClick.AddListener(() =>
            {
                // 1. สั่งสลับเหรียญ
                inventory.EquipCoinByIndex(index);

                // 2. 🔥 สั่งวาดหน้าจอใหม่ทันที (เพื่อให้ชื่อปุ่มเปลี่ยนเป็นเหรียญเก่าที่เราสลับเข้าไป)
                RefreshUI();

                Debug.Log($"UI: สลับเหรียญเรียบร้อย รีเฟรชหน้าจอ");
            });
        }
    }
}