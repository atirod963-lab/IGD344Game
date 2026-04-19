using UnityEngine;

public class WindParticleController : MonoBehaviour
{
    private ParticleSystem windParticle;

    void Start()
    {
        windParticle = GetComponent<ParticleSystem>();
        // สั่งให้เรียกฟังก์ชัน PlayWind ซ้ำๆ ทุก 15
        //  วินาที
        InvokeRepeating("PlayWind", 0f, 15f); 
    }

    void PlayWind()
    {
        windParticle.Play();
    }
}
