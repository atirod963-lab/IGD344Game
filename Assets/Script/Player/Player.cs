using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;
using System.Collections;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public static Vector3 NextSpawnPosition;
    public static bool HasSpawnPosition = false;
    public static string NextSpawnTag;

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
        StartCoroutine(SetupAfterLoad());
    }

    IEnumerator SetupAfterLoad()
    {
        yield return null; // 🔥 รอ 1 frame

        if (!string.IsNullOrEmpty(NextSpawnTag))
        {
            GameObject spawn = GameObject.FindWithTag(NextSpawnTag);

            if (spawn != null)
            {
                transform.position = spawn.transform.position;
            }

            NextSpawnTag = null;
        }

        CinemachineCamera cam = FindAnyObjectByType<CinemachineCamera>();

        if (cam != null)
        {
            cam.Follow = transform;
            cam.LookAt = transform;
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