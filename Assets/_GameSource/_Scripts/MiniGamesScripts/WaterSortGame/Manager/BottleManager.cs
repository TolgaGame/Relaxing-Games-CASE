using System.Collections.Generic;
using UnityEngine;

public class BottleManager : MonoBehaviour {

    #region References and Prefabs

    [Header("PREFABS")]
    [SerializeField] private BottleController bottlePrefab;
    private BottleController[] BottleControllerInstances;

    #endregion

    #region Variables

    [Header("VARIABLES")]
    private int _bottlesAmount = 0;
    private int _bottlesIndex = 0;

    private readonly Vector2[] rowOffsets = new Vector2[] {
        new Vector2(0f, 0f),      // 1 row
        new Vector2(0f, 0.7f),    // top row
        new Vector2(0f, -0.7f)    // bottom row
    };

    public List<BottleController> BottleControllersList;

    #endregion

    #region Unity Lifecycle

    private void Start() {

    }

    #endregion

    #region Bottle Management

    private void InstantiateBottle(int numberOfColors, int[] colors) {
        var bottle = Instantiate(bottlePrefab, ChooseBottlePosition(_bottlesIndex), Quaternion.identity);
        bottle.SetColorsIndex(colors ?? new int[4], numberOfColors);
        bottle.setIndex(_bottlesIndex, this);
        BottleControllerInstances[_bottlesIndex] = bottle;
        _bottlesIndex++;
        BottleControllersList.Add(bottle);
    }

    public void SpawnBottle(int[] colors, int numberOfColors) {
        InstantiateBottle(numberOfColors, colors);
    }

    // Spawns a new bottle with the specified colors and number of colors
    public void SetBottlesAmount(int bottles) {
        BottleControllerInstances = new BottleController[bottles];
        _bottlesAmount = bottles;
       Debug.Log("Total Bottles Amount: " + _bottlesAmount);
    }

    #endregion

    #region Victory Check

    // Check if victory conditions are met
    public void VerifyVictory() {
        Debug.Log("Verifying victory conditions...");
        if (BottleControllerInstances == null) return;

        int totalBottles = 0;
        int completedOrEmptyBottles = 0;

        for (int x = 0; x < _bottlesAmount; x++) {
            if (BottleControllerInstances[x] == null) continue;

            // Skip if bottle is under animation
            if (BottleControllerInstances[x].GetUnderAnimation())
                return;

            // Count total valid bottles (exclude null instances)
            totalBottles++;

            // A bottle is considered complete if:
            // 1. It's completely empty (numberOfColorsInBottle == 0)
            // 2. OR it has at least one color and all colors are the same (CheckIfItsDone())
            if (BottleControllerInstances[x].numberOfColorsInBottle == 0 ||
                BottleControllerInstances[x].CheckIfItsDone()) {
                completedOrEmptyBottles++;
               // Debug.Log($"Bottle {x} is complete or empty: {(BottleControllerInstances[x].numberOfColorsInBottle == 0 ? "Empty" : "Complete")}");
            }
        }

       // Debug.Log($"Total Bottles: {totalBottles}, Completed/Empty: {completedOrEmptyBottles}");

        // Victory condition: All bottles must be either empty or complete with single color
        if (totalBottles > 0 && completedOrEmptyBottles == totalBottles) {
            for (int i = 0; i < BottleControllerInstances.Length; i++) {
                BottleControllerInstances[i].AnimateBottleCover();
            }
            LoadNextLevel();
        }
    }

    // Load the next level if victory conditions are met
    private void LoadNextLevel() {
        WaterSortLocator.Instance.WaterSortManagerInstance.SaveData();
        WaterSortLocator.Instance.WaterSortManagerInstance.IsGamePause = true;
        WaterSortLocator.Instance.WaterSortManagerInstance.NextLevel();
    }

    #endregion


    #region Layout Management

    // Calculate bottle position based on index and total count
    public Vector3 ChooseBottlePosition(int bottleIndex) {
        int maxBottlesPerRow = 8;
        bool useDoubleRow = _bottlesAmount > maxBottlesPerRow;
        float spacing = GetSpacingForBottles(_bottlesAmount);
        Vector2 position;

        if (!useDoubleRow) {
            // Single row layout
            position = CalculatePositionInRow(bottleIndex, _bottlesAmount, spacing);
            return new Vector3(position.x, rowOffsets[0].y, 0);
        } else {
            // Double row layout
            int topRowCount = (_bottlesAmount + 1) / 2;
            bool isTopRow = bottleIndex < topRowCount;

            if (isTopRow) {
                position = CalculatePositionInRow(bottleIndex, topRowCount, spacing);
                return new Vector3(position.x, rowOffsets[1].y, 0);
            } else {
                position = CalculatePositionInRow(bottleIndex - topRowCount, _bottlesAmount - topRowCount, spacing);
                return new Vector3(position.x, rowOffsets[2].y, 0);
            }
        }
    }

    private Vector2 CalculatePositionInRow(int index, int totalInRow, float spacing) {
        float startX = (totalInRow - 1) * spacing / 2 * -1;
        float x = startX + (index * spacing);
        return new Vector2(x, 0);
    }

    private float GetSpacingForBottles(int totalBottles) {
        if (totalBottles <= 4) return 0.6f;
        if (totalBottles <= 6) return 0.5f;
        if (totalBottles <= 8) return 0.4f;
        return 0.4f;
    }

    #endregion

    #region State Checks

    public bool SomeBottleIsUnderAnimation() {
        if (BottleControllerInstances == null) return false;

        for (int x = 0; x < _bottlesAmount; x++) {
            if (BottleControllerInstances[x] == null) continue;
            if (BottleControllerInstances[x].GetUnderAnimation())
                return true;
        }
        return false;
    }

    #endregion
}