using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static Vector3 NextSpawnPosition;

    private Rigidbody2D rb;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            rb = GetComponent<Rigidbody2D>();

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (NextSpawnPosition != Vector3.zero)
        {
            // บังคับ z = 0 สำหรับ 2D
            Vector3 spawnPos = NextSpawnPosition;
            spawnPos.z = 0f;

            if (rb != null)
                rb.position = spawnPos;   // แนะนำสำหรับ Rigidbody2D
            else
                transform.position = spawnPos;
        }
    }
}