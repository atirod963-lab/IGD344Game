using UnityEngine;



public class BGMPlayer : MonoBehaviour
{
    private void Start()
    {
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BGM");

    }

}
