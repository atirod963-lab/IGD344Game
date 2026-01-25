using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenePortal : MonoBehaviour
{
    public string targetScene;
    public Transform spawnPoint;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        Player.NextSpawnPosition = spawnPoint.position;
        SceneManager.LoadScene(targetScene);
    }
}