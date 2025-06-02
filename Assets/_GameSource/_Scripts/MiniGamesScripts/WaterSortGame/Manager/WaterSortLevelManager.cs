using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Linq;

public class WaterSortLevelManager : MonoBehaviour {

    [SerializeField]
    private BottleManager BottleManagerInstance;

    [SerializeField]
    private TextMeshProUGUI levelText;

    [SerializeField]
    [Header("Level Configurations")]
    private List<LevelData> levels = new List<LevelData>();

    private void Awake() {
        InitializeLevelDesigns();
        levelText.text = "LEVEL " + (WaterSortLocator.Instance.WaterSortManagerInstance.LevelNumber + 1).ToString();
        ConfigureLevel();
    }

    private void InitializeLevelDesigns() {
        if (BottleManagerInstance == null) {
            Debug.LogError("BottleManagerInstance is not assigned in the inspector.");
            return;
        }
    }

    private void ConfigureLevel() {
        int levelNumber = WaterSortLocator.Instance.WaterSortManagerInstance.LevelNumber;
        if (levelNumber >= 0 && levelNumber < levels.Count) {
            CreateLevel(levels[levelNumber]);
        } else {
            Debug.LogError($"Level {levelNumber} does not exist in the levels list!");
        }
    }

    private void CreateLevel(LevelData levelData) {
        BottleManagerInstance.SetBottlesAmount(levelData.TotalBottles);

        for (int i = 0; i < levelData.Bottles.Count; i++) {
            var bottleData = levelData.Bottles[i];

            if (bottleData.IsEmpty) {
                // Create empty bottle with 4 empty slots
                BottleManagerInstance.SpawnBottle(new int[] { 0, 0, 0, 0 }, 0);
            } else {
                // Normal bottle spawn
                BottleManagerInstance.SpawnBottle(bottleData.Colors,
                    Mathf.Min(bottleData.Colors.Count(x => x > 0), 3));
            }
        }
    }
}

#region Data Classes

[System.Serializable]
public class BottleData {
    [SerializeField]
    private int[] colors;
    [SerializeField]
    private bool isEmpty; // Indicates if the bottle is empty

    public int[] Colors => colors;
    public bool IsEmpty => isEmpty;

    public BottleData(int[] colors, bool isEmpty = false) {
        this.colors = colors;
        this.isEmpty = isEmpty;
    }
}

[System.Serializable]
public class LevelData {
    private const int MAX_STACK = 3;

    [SerializeField]
    private List<BottleData> bottles;

    public int TotalBottles => bottles.Count;
    public int MaxStack => MAX_STACK;
    public List<BottleData> Bottles => bottles;

    public LevelData(List<BottleData> bottles) {
        this.bottles = bottles;
    }
}

#endregion