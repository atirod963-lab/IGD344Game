using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// Top-down Y-sort: ปรับ sorting order จากตำแหน่ง Y ทุกเฟรม (หลังการเคลื่อนที่).
/// ใช้ SortingGroup ถ้ามี — ไม่เช่นนั้นใช้ SpriteRenderer ตัวแรกที่หาได้
///
/// เงื่อนไขให้ตรงกับ Tilemap (พุ่ม/ต้นไม้):
/// - Sorting Layer เดียวกับ Tilemap ที่จะแย่งหน้าหลัง (ชั้นที่สูงกว่าจะทับเสมอ ไม่สนว่า Y เท่าไร)
/// - Tilemap นั้นใช้ TilemapRenderer Mode = Individual (ถ้าเป็น Chunk จะเรียงทั้งแผงเป็นชิ้นเดียว)
/// - ใส่ <see cref="TilemapYSortEnabler"/> บน Tilemap พุ่มได้เพื่อบังคับ Individual
/// - ตั้ง Sort Origin ที่เท้า — ถ้าใช้ pivot กลางตัว จะเกิดอาการหัวโดนบังแต่ขายังโผล่หน้าพุ่ม
/// </summary>
[DisallowMultipleComponent]
public class YSorting2D : MonoBehaviour
{
    [Header("ต้องอยู่ Sorting Layer เดียวกับ Tilemap ที่จะสลับหน้าหลัง")]
    [Tooltip("จุดอ้างอิง Y (เช่น เท้า). ว่าง = ใช้ transform ของอ็อบเจ็กต์นี้")]
    [SerializeField] Transform sortOrigin;

    [Tooltip("วาดจุดอ้างอิงใน Scene เวลาเลือกอ็อบเจ็กต์ (ช่วยจัดที่เท้า)")]
    [SerializeField] bool drawSortOriginGizmo = true;

    [Tooltip("คูณ Y ก่อนปัด — ยิ่งสูงลำดับละเอียดขึ้น (มักใช้ 50–100)")]
    [SerializeField] int precision = 100;

    [Tooltip("บวกคงที่หลังคำนวณ — ใช้จัดช่วงให้ตรงกับ Tilemap / ชั้นอื่น")]
    [SerializeField] int sortingOrderOffset;

    [Tooltip("เปิด = Y ในเวิลด์สูงขึ้น วาดไว้หลัง (แบบ top-down ทั่วไป). ปิดถ้าลำดับกลับด้าน")]
    [SerializeField] bool higherWorldYIsBehind = true;

    SortingGroup sortingGroup;
    SpriteRenderer spriteRenderer;

    void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
        if (sortingGroup == null)
            sortingGroup = GetComponentInChildren<SortingGroup>(true);

        if (sortingGroup == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
                spriteRenderer = GetComponentInChildren<SpriteRenderer>(true);
        }
    }

    void LateUpdate()
    {
        Transform origin = sortOrigin != null ? sortOrigin : transform;
        float y = origin.position.y;
        float scaled = (higherWorldYIsBehind ? -y : y) * precision;
        int order = Mathf.RoundToInt(scaled) + sortingOrderOffset;

        if (sortingGroup != null)
            sortingGroup.sortingOrder = order;
        else if (spriteRenderer != null)
            spriteRenderer.sortingOrder = order;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!drawSortOriginGizmo) return;
        Transform origin = sortOrigin != null ? sortOrigin : transform;
        Gizmos.color = new Color(1f, 0.92f, 0.2f, 0.9f);
        Gizmos.DrawSphere(origin.position, 0.08f);
        Gizmos.DrawLine(origin.position + Vector3.left * 0.25f, origin.position + Vector3.right * 0.25f);
    }
#endif
}
