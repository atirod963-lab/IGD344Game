using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TeleportManager : MonoBehaviour
{
    public static TeleportManager Instance;

    string targetWarpID;

    // 👈 ระยะเลื่อนจาก WarpExit (ไปทางซ้าย)
    public Vector2 spawnOffset = new Vector2(-1.5f, 0f);

    void Awake()
    {

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Teleport(string sceneName, string warpID)
    {
        targetWarpID = warpID;
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            ClickToMove2D move = player.GetComponent<ClickToMove2D>();
            if (move != null)
            {
                move.StopMovementImmediately();
            }
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneName);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        WarpPoint[] points = FindObjectsOfType<WarpPoint>();
        foreach (var point in points)
        {
            if (point.warpID == targetWarpID)
            {
                GameObject player = GameObject.FindGameObjectWithTag("Player");

                // ✅ วางผู้เล่น + offset ไปทางซ้าย
                player.transform.position =
                    (Vector2)point.transform.position + spawnOffset;

                break;
            }
        }
    }


}