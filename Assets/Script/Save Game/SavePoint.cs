using UnityEngine;

public class SavePoint : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        // ถ้า Player เดินมาชน
        if (other.CompareTag("Player"))
        {
            BaseUnit playerStats = other.GetComponent<BaseUnit>();
            FateInventory inventory = other.GetComponent<FateInventory>();

            if (playerStats != null)
            {
                // สั่งเซฟเกม!
                SaveSystem.Instance.SaveGame(playerStats, inventory);
            }
        }
    }
}