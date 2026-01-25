using UnityEngine;

public class DontDestroyCamera : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}