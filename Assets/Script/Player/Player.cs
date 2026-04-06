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
            // ส่งข้อมูลจากตัวเก่าไปให้ตัวใหม่
            Instance.TransferTo(this);
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

    public void TransferTo(Player newPlayer)
    {
        // ย้าย position
        newPlayer.transform.position = transform.position;

        // ย้าย Instance มาชี้ตัวใหม่
        Instance = newPlayer;
        DontDestroyOnLoad(newPlayer.gameObject);
        newPlayer.rb = newPlayer.GetComponent<Rigidbody2D>();

        // ย้าย event listener มาที่ตัวใหม่
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += newPlayer.OnSceneLoaded;

        // ลบตัวเก่าทิ้ง
        Destroy(gameObject);
    }
}