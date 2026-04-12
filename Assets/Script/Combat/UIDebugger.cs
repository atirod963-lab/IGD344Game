using UnityEngine;

public class UIDebugger : MonoBehaviour
{
    // โค้ดนี้จะทำงานอัตโนมัติ ทันทีที่ UI ก้อนนี้ถูก "ติ๊กปิด" หรือสั่ง SetActive(false)
    void OnDisable()
    {
        // สั่งปริ้นท์ที่มา (Stack Trace) ออกมาดูเลยว่า ใครเรียกคำสั่งปิด!
        Debug.LogWarning($"🛑 [UI พัง] '{gameObject.name}' โดนสั่งปิด! รายละเอียดด้านล่าง:\n" + StackTraceUtility.ExtractStackTrace());
    }
}