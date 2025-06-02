using UnityEngine;

[CreateAssetMenu(fileName = "Gun Data",menuName = "Gun Data")]
public class GunData : ScriptableObject {

    public string GunName;
    public Sprite GunSprite;
    public AudioClip GunSound;
    public bool IsUnlocked;
}