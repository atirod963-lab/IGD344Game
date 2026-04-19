using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Scene")]
    public string targetScene;
    public string targetSpawnName; // 🔥 เปลี่ยนตรงนี้

    [Header("Transition")]
    public float transitionDuration = 1f;
    public Sprite enterImage;
    public Sprite exitImage;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        // ส่งชื่อ spawn ไปเก็บ
        Player.NextSpawnPointName = targetSpawnName;

        SceneTransitionManager.Instance.LoadScene(
            targetScene,
            transitionDuration,
            enterImage,
            exitImage
        );
    }
}