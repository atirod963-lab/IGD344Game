using UnityEngine;
using UnityEngine.UI;

public class TeleportButton : MonoBehaviour
{
    public string targetScene;
    public string targetWarpID;

    Button button;
    Image image;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();
    }

    public void Setup(string currentScene)
    {
        bool canTeleport = currentScene != targetScene;

        button.interactable = canTeleport;
        image.color = canTeleport ? Color.white : Color.gray;
    }

    public void OnClickTeleport()
    {
        TeleportUI.Instance.Close();
        TeleportManager.Instance.Teleport(targetScene, targetWarpID);
    }
}