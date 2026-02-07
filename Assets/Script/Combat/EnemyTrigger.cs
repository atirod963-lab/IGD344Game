using UnityEngine;

public class EnemyTrigger : MonoBehaviour
{
    private BaseUnit myStats;

    void Start()
    {
        myStats = GetComponent<BaseUnit>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("⛔ เจอ Player! สั่งหยุดเดินและเริ่มสู้");

            // 1. สั่งหยุด ClickToMove2D
            ClickToMove2D playerMove = other.GetComponent<ClickToMove2D>();
            if (playerMove != null)
            {
                playerMove.StopMovementImmediately(); // สั่งรีเซ็ตค่าก่อน
                playerMove.enabled = false;           // ปิดสคริปต์เดินไปเลย
            }

            // 2. เริ่มสู้
            BattleManager.instance.StartBattle(myStats);
        }
    }
}