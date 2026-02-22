using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    public string warpID;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        TeleportUI.Instance.Open();
    }
}