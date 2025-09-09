using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Dialog Data", menuName = "General Gameplay/Characters/Character Dialog Data", order = 1)]
public class CharacterDialogData : ScriptableObject
{
    public AudioClip[] speechSounds;
    [SerializeField] Expresion[] expresions;
    public Dictionary<string, Sprite> expresionHashMap;

    [System.Serializable]
    public class Expresion {
        public string key;
        public Sprite sprite;
    }

    private void OnEnable() {   
        expresionHashMap = new Dictionary<string, Sprite>();
        for (int i = 0; i < expresions.Length; i++) {
            expresionHashMap.Add(expresions[i].key, expresions[i].sprite);
        }
    }
}
    