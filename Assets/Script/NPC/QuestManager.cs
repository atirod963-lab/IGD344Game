using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;
    public TextMeshProUGUI questLogText;

    void Awake()
    {
        Instance = this;
    }

    public void AddQuest(string questName)
    {
        questLogText.text += "\n- " + questName;
    }
}