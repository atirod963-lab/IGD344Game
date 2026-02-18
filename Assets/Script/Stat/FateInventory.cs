using UnityEngine;
using System.Collections.Generic;

public class FateInventory : MonoBehaviour
{
    [Header("Settings")]
    public BaseUnit playerUnit;

    [Header("Collection")]
    public List<FateCoinData> ownedCoins = new List<FateCoinData>();

    public FateCoinData CurrentEquipped => playerUnit.currentFate;

    void Start()
    {
        if (playerUnit == null) playerUnit = GetComponent<BaseUnit>();
    }

    void Update()
    {
        // ปุ่มลัดสำหรับเทส (กด 1, 2)
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipCoinByIndex(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipCoinByIndex(1);
    }

    public void AddCoin(FateCoinData coin)
    {
        // อนุญาตให้มีเหรียญซ้ำได้ไหม? ถ้าได้ก็เอา !ownedCoins.Contains ออก
        ownedCoins.Add(coin);
        Debug.Log($"🎒 ได้รับเหรียญใหม่: {coin.coinName}");
    }

    // 🔥 แก้ไขฟังก์ชันนี้: ให้เป็นระบบสลับ (Swap)
    public void EquipCoinByIndex(int index)
    {
        if (index < 0 || index >= ownedCoins.Count) return;

        // 1. จำเหรียญใหม่ที่จะหยิบมาใส่
        FateCoinData newCoin = ownedCoins[index];

        // 2. จำเหรียญเก่าที่ใส่อยู่ (ถ้ามี)
        FateCoinData oldCoin = playerUnit.currentFate;

        // 3. เริ่มการสลับ
        if (oldCoin != null)
        {
            // ถ้ามีของเก่า: เอาของเก่า ยัดกลับเข้าไปในช่องเดิมของเหรียญใหม่เลย
            ownedCoins[index] = oldCoin;
            Debug.Log($"🔄 สลับเหรียญ: เก็บ {oldCoin.coinName} เข้ากระเป๋า -> หยิบ {newCoin.coinName} มาใส่");
        }
        else
        {
            // ถ้าตัวเปล่า: ลบเหรียญใหม่ออกจากกระเป๋าเฉยๆ (เพราะหยิบมาใส่แล้ว)
            ownedCoins.RemoveAt(index);
            Debug.Log($"👕 สวมใส่: หยิบ {newCoin.coinName} มาใส่ (ในกระเป๋าจะหายไป)");
        }

        // 4. สวมเหรียญใหม่เข้าตัว
        playerUnit.EquipFateCoin(newCoin);
    }
}