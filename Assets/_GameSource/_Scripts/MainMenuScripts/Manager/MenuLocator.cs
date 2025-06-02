using UnityEngine;

public class MenuLocator : MonoBehaviour {
    public static MenuLocator _instance;
    private static bool _instanceSet;

    public static MenuLocator Instance {
        get {
            if (!_instanceSet) {
                _instance = FindObjectOfType<MenuLocator>();

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
    public MenuManager MenuManagerInstance;
    public SoundManager SoundManagerInstance;

    ///////////////////////////

    private void OnDestroy() {
        if (_instance == this) {
            _instanceSet = false;
        }
    }

}