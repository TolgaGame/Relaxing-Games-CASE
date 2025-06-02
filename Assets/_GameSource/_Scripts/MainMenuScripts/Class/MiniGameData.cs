using UnityEngine;

[CreateAssetMenu(fileName = "MiniGame",menuName = "Mini Game Data")]
public class MiniGameData : ScriptableObject {

    public int MiniGameIndex;
    public string MiniGameName;
    public Sprite MiniGameSprite;
}