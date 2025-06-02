using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "ToiletDraw Data")]
public class ToiletDrawLevelData : ScriptableObject {
    [Header("Characters")]
    [Tooltip("Starting position for the male character on the level")]
    public Vector2 MaleCharacterStartPosition;
    [Tooltip("Starting position for the female character on the level")]
    public Vector2 FemaleCharacterStartPosition;

    [Header("Toilets")]
    [Tooltip("Position of the male toilet in the level")]
    public Vector2 MaleToiletPosition;
    [Tooltip("Position of the female toilet in the level")]
    public Vector2 FemaleToiletPosition;

    [Header("Camera")]
    [Tooltip("Initial camera position in the level (default is 0,0,-100)")]
    public Vector3 CameraPosition = new Vector3(0, 0, -100);
    [Tooltip("Camera orthographic size - controls how much of the level is visible")]
    public float CameraSize = 25f;
}