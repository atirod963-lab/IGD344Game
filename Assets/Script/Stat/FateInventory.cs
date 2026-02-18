using UnityEngine;
using System.Collections.Generic;

public class FateInventory : MonoBehaviour
{
    [Header("Settings")]
    public BaseUnit playerUnit; // ลากตัว Player มาใส่

    [Header("Collection")]
    // รายชื่อเหรียญที่เราครอบครอง
    public List<FateCoinData> ownedCoins = new List<FateCoinData>();

    // เหรียญที่ใส่อยู่ปัจจุบัน (เอาไว้โชว์ใน UI)
    public FateCoinData CurrentEquipped => playerUnit.currentFate;

    void Start()
    {
        // ถ้ายังไม่ได้ลากใส่ ให้หาเองจากตัวที่สคริปต์นี้แปะอยู่
        if (playerUnit == null) playerUnit = GetComponent<BaseUnit>();
    }

    // ฟังก์ชันเพิ่มเหรียญเข้ากระเป๋า (เช่น ตอนชนะเดิมพัน)
    public void AddCoin(FateCoinData coin)
    {
        if (!ownedCoins.Contains(coin))
        {
            ownedCoins.Add(coin);
            Debug.Log($"🎒 ได้รับเหรียญใหม่: {coin.coinName}");
        }
    }

    // ฟังก์ชันสั่งสวมใส่เหรียญ (ตาม Index ในลิสต์)
    public void EquipCoinByIndex(int index)
    {
        if (index >= 0 && index < ownedCoins.Count)
        {
            FateCoinData coinToEquip = ownedCoins[index];
            playerUnit.EquipFateCoin(coinToEquip);
        }
        else
        {
            Debug.LogWarning("❌ ไม่มีเหรียญในช่องนี้!");
        }
    }

    // ฟังก์ชันสั่งสวมใส่เหรียญ (ตามชื่อ หรือ Object)
    public void EquipCoin(FateCoinData coin)
    {
        if (ownedCoins.Contains(coin))
        {
            playerUnit.EquipFateCoin(coin);
        }
        else
        {
            Debug.LogWarning("❌ คุณยังไม่มีเหรียญนี้ในกระเป๋า!");
        }
    }
}