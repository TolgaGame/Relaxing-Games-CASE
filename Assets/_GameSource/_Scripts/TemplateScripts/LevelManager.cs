/*
using UnityEngine;

public class LevelManager : MonoBehaviour {

    [SerializeField] NewLevelData[] LevelDatas;
    NewLevelData CurrentLevelData;

    // Spawn
    public void SpawnLevel(int index)
    {
        if (index > LevelDatas.Length - 1)
            index = GetRandomLevel();

        CurrentLevelData = LevelDatas[index];
        Instantiate(CurrentLevelData.LevelPrefab);
        PlayerPrefs.SetInt("LevelIndex", index);
    }

    int GetRandomLevel()
    {
        int index = Random.Range(0, LevelDatas.Length);

        int lastLevel = PlayerPrefs.GetInt("LevelIndex");
        if (index == lastLevel)
            return GetRandomLevel();
        else
            return index;
    }
}
*/