using System;
using UnityEngine;

public class SoundTable : MonoBehaviour {
    [SerializeField] AudioSource source;
    [SerializeField] SoundData[] soundTable;

    public static Action<string> PlaySound;

    private void Awake() {
        PlaySound += PlaySoundCallback;
    }

    void PlaySoundCallback(string key) {
        for (int i = 0; i < soundTable.Length; i++) {
            if(soundTable[i].key == key) {

                source.PlayOneShot(soundTable[i].clip);

                return;
            }
        }
    }

    private void OnDestroy() {
        PlaySound -= PlaySoundCallback;
    }

    [System.Serializable]
    public struct SoundData {
        public string key;
        public AudioClip clip;
    }
}
