using UnityEngine;

public abstract class BaseUnit : MonoBehaviour
{
    public string unitName;
    public float hp;
    public float atk;
    public float def;
    public int luck;

    // ระบบรับดาเมจแบบคำนวณค่า Def
    public virtual void TakeDamage(float rawDamage, bool isDefending)
    {
        float finalDamage = rawDamage;

        if (isDefending)
        {
            // ถ้าทอยแพ้ (มอนป้องกัน) ดาเมจจะถูกหักลบด้วย Def
            // สูตร: ดาเมจที่เหลือ = ดาเมจ - ค่าป้องกัน (แต่ต้องเข้าอย่างน้อย 1)
            finalDamage = Mathf.Max(1, rawDamage - def);
            Debug.Log($"{unitName} ป้องกัน! ลดดาเมจเหลือ {finalDamage}");
        }
        else
        {
            // ถ้าทอยชนะ (มอนไม่ได้ป้องกัน) โดนดาเมจเต็มๆ หรืออาจจะลดนิดเดียวตามสมดุลเกม
            Debug.Log($"{unitName} ป้องกันพลาด! รับดาเมจเต็ม {finalDamage}");
        }

        hp -= finalDamage;
        if (hp <= 0) Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{unitName} ตายแล้ว");
        Destroy(gameObject);
    }
}