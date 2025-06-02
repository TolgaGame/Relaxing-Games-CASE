using UnityEngine;

public class WaterSortLocator : MonoBehaviour {
    public static WaterSortLocator _instance;
    private static bool _instanceSet;

    public static WaterSortLocator Instance {
        get {
            if (!_instanceSet) {
                _instance = FindObjectOfType<WaterSortLocator>();

                if (_instance == null) {
                    Debug.LogError("Locator is not present in the scene, singleton failed");
                    return null;
                }
                _instanceSet = true;
            }
            return _instance;
        }
    }

    [Header("Instances")]
    public WaterSortManager WaterSortManagerInstance;
    public WaterSortInputManager WaterSortGameManagerInstance;
    public BottleManager BottleManagerInstance;
    public SoundManager SoundManagerInstance;

    ///////////////////////////

    private void OnDestroy() {
        if (_instance == this) {
            _instanceSet = false;
        }
    }

}