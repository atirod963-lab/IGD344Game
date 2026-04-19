using UnityEngine;
using System;

[System.Serializable]
public class Sound
{
    public string name; // ชื่อเสียงเอาไว้เรียกใช้
    public AudioClip clip; // ไฟล์เสียง
    [Range(0f, 1f)] public float volume = 1f; // ความดังของเสียงนี้
}

public class SoundManager : MonoBehaviour
{
    // ทำให้เป็น Singleton เพื่อให้เรียกใช้จากที่ไหนก็ได้ผ่าน SoundManager.Instance
    public static SoundManager Instance;

    [Header("Audio Sources")]
    public AudioSource bgmSource; // ลำโพงสำหรับเพลงพื้นหลัง
    public AudioSource sfxSource; // ลำโพงสำหรับเอฟเฟกต์

    [Header("Audio Clips (ตั้งค่าเสียงต่างๆ ตรงนี้)")]
    public Sound[] bgmSounds; // เก็บรายการเพลงพื้นหลัง
    public Sound[] sfxSounds; // เก็บรายการเสียงเอฟเฟกต์

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // ถ้ามีตัวเก่าอยู่แล้ว ให้ลบทิ้ง
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // ==========================================
    // 🎵 ระบบเล่นเพลงพื้นหลัง (BGM)
    // ==========================================
    public void PlayBGM(string name)
    {
        Sound s = Array.Find(bgmSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning($"หาเพลง BGM ชื่อ '{name}' ไม่เจอ!");
            return;
        }

        // ถ้าเพลงที่สั่งเปิด เป็นเพลงเดียวกับที่เล่นอยู่ ให้ข้ามไปเลย (เพลงจะได้ไม่เริ่มใหม่)
        if (bgmSource.clip == s.clip) return;

        bgmSource.clip = s.clip;
        bgmSource.volume = s.volume;
        bgmSource.loop = true; // เพลงพื้นหลังต้องวนลูป
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // ==========================================
    // 💥 ระบบเล่นเสียงเอฟเฟกต์ (SFX)
    // ==========================================
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.LogWarning($"หาเสียง SFX ชื่อ '{name}' ไม่เจอ!");
            return;
        }

        // ใช้ PlayOneShot เพื่อให้เสียงเล่นซ้อนกันได้ (เช่น กดปุ่มรัวๆ หรือทอยเต๋าหลายลูก)
        sfxSource.PlayOneShot(s.clip, s.volume);
    }

    // ฟังก์ชันเสริม: เผื่ออยากส่ง AudioClip เข้ามาให้เล่นตรงๆ แบบไม่ต้องตั้งค่าใน Array
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip, volume);
        }
    }

    // ==========================================
    // 🔁 ระบบเสียง SFX แบบวนลูป (เช่น เสียงเขย่าต่อเนื่อง)
    // ==========================================
    public void PlayLoopingSFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null) return;

        sfxSource.clip = s.clip;
        sfxSource.volume = s.volume;
        sfxSource.loop = true; // สั่งบังคับลูปผ่านโค้ด
        sfxSource.Play();      // ใช้ .Play() ธรรมดา มันถึงจะยอมลูป
    }

    public void StopLoopingSFX()
    {
        sfxSource.Stop(); // สั่งหยุดเสียงที่กำลังลูปอยู่
    }
}