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

    // 🔥 ลบ void Update() ที่มี Input.GetKeyDown ออกไปแล้ว 
    // เพราะเราจะเปลี่ยนไปใช้ปุ่มใน UI แทน

    public void AddCoin(FateCoinData coin)
    {
        ownedCoins.Add(coin);
        Debug.Log($"🎒 ได้รับเหรียญใหม่: {coin.coinName}");
    }

    public void EquipCoinByIndex(int index)
    {
        if (index < 0 || index >= ownedCoins.Count) return;

        FateCoinData newCoin = ownedCoins[index];
        FateCoinData oldCoin = playerUnit.currentFate;

        if (oldCoin != null)
        {
            ownedCoins[index] = oldCoin;
            Debug.Log($"🔄 สลับเหรียญ: เก็บ {oldCoin.coinName} เข้ากระเป๋า -> หยิบ {newCoin.coinName} มาใส่");
        }
        else
        {
            ownedCoins.RemoveAt(index);
            Debug.Log($"👕 สวมใส่: หยิบ {newCoin.coinName} มาใส่ (ในกระเป๋าจะหายไป)");
        }

        playerUnit.EquipFateCoin(newCoin);

        // 🔥 สำคัญ: เมื่อใส่เหรียญเสร็จ ให้สั่งอัปเดตหน้าสแต็ทด้วย เพื่อให้รูปเหรียญใหม่โชว์ขึ้นมาทันที!
        if (PlayerStatsUI.instance != null)
        {
            PlayerStatsUI.instance.UpdateStatsUI();
        }
    }
}