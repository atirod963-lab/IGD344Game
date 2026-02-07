using UnityEngine;
[System.Serializable]
public class QuestGoal {
    public GoalType goalType;
    public int requiredAmount; // จำนวนที่ต้องการ
    public int currentAmount;  // จำนวนปัจจุบัน

    public bool IsReached() => currentAmount >= requiredAmount;

    public void IncreseAmount(int amount) {
        currentAmount = Mathf.Min(currentAmount + amount, requiredAmount);
    }
}
