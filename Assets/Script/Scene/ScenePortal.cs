using UnityEngine;

public class ScenePortal : MonoBehaviour
{
    [Header("Scene")]
    public string targetScene;
    public Transform spawnPoint;

    [Header("Transition")]
    public float transitionDuration = 1f;
    public Sprite enterImage; // รูปก่อนโหลด
    public Sprite exitImage;  // รูปหลังโหลด

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player.NextSpawnPosition = spawnPoint.position;

        SceneTransitionManager.Instance.LoadScene(
            targetScene,
            transitionDuration,
            enterImage,
            exitImage
        );
    }
}
