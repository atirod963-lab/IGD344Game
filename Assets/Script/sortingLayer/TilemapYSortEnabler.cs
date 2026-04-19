using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// แนบที่ Tilemap ที่ต้องแย่งหน้า–หลังกับตัวละคร (พุ่ม ต้นไม้ ของตกแต่งบนพื้น)
/// บังคับ <see cref="TilemapRenderer"/> เป็น Mode.Individual — ให้เรียงตามแกนโปร่งใสทีละกระเบื้อง
/// (โหมด Chunk จะวาดทั้งแผงเป็นชิ้นเดียว Custom Axis เลยช่วยไม่ได้ต่อกระเบื้อง)
/// </summary>
[RequireComponent(typeof(TilemapRenderer))]
[DisallowMultipleComponent]
public class TilemapYSortEnabler : MonoBehaviour
{
    [SerializeField] bool applyOnEnable = true;

    void OnEnable()
    {
        if (applyOnEnable)
            ApplyIndividualMode();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (applyOnEnable && gameObject.activeInHierarchy)
            ApplyIndividualMode();
    }
#endif

    /// <summary>เรียกจากโค้ดหรือปุ่มในอินสเปกเตอร์ถ้าต้องการ sync ครั้งเดียว</summary>
    [ContextMenu("Apply Individual mode (Y-sort)")]
    public void ApplyIndividualMode()
    {
        var tr = GetComponent<TilemapRenderer>();
        if (tr != null && tr.mode != TilemapRenderer.Mode.Individual)
            tr.mode = TilemapRenderer.Mode.Individual;
    }
}
