using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    private Dictionary<string, SoundEffect> soundEffects;
    public static AudioManager Instance { get; private set; }

    void Awake ()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        Instance = this;
        soundEffects = GetComponentsInChildren<SoundEffect>().ToDictionary((soundEffect) => soundEffect.Id.ToUpper(), (soundEffect) => soundEffect);
    }

    public void PlaySound(string key)
    {
        key = key.ToUpper();
        if (!soundEffects.ContainsKey(key))
        {
            Debug.LogErrorFormat("Failed to find key: {0}", key);
            return;
        }

        soundEffects[key].AudioSource.Play();
    }
}
