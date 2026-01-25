using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

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
        // ย้ายตำแหน่งเกิด
        if (NextSpawnPosition != Vector3.zero)
        {
            Vector3 spawnPos = NextSpawnPosition;
            spawnPos.z = 0f;
            rb.position = spawnPos;
        }

        // ✅ Cinemachine 3
        CinemachineCamera cam =
            FindAnyObjectByType<CinemachineCamera>();

        if (cam != null)
        {
            cam.Follow = transform;
            cam.LookAt = transform; // 2D ใส่หรือไม่ใส่ก็ได้
        }
    }
}