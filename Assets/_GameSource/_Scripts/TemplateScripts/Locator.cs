using UnityEngine;

public class Locator : MonoBehaviour {
    public static Locator _instance;
    private static bool _instanceSet;

    public static Locator Instance {
        get {
            if (!_instanceSet) {
                _instance = FindObjectOfType<Locator>();

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
    public GameManager GameManagerInstance;
    public SoundManager SoundManagerInstance;
    public UIManager UIManagerInstance;
    public SaveSystem SaveSystemInstance;
    //public LevelManager LevelManagerInstance;

    ///////////////////////////

    private void OnDestroy() {
        if (_instance == this) {
            _instanceSet = false;
        }
    }

}