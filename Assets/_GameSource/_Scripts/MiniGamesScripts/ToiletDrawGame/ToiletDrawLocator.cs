using UnityEngine;

public class ToiletDrawLocator : MonoBehaviour {
public static ToiletDrawLocator _instance;
    private static bool _instanceSet;

    public static ToiletDrawLocator Instance {
        get {
            if (!_instanceSet) {
                _instance = FindObjectOfType<ToiletDrawLocator>();

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
    public ToiletDrawGameManager ToiletDrawGameManagerInstance;
    public ToiletDrawLevelManager ToiletDrawLevelManagerInstance;
    public ToiletDrawInputManager ToiletDrawInputManagerInstance;
    public ToiletDrawUIManager ToiletDrawUIManagerInstance;
    public SoundManager SoundManagerInstance;
    public Camera MainCameraInstance;

    ///////////////////////////

    private void OnDestroy() {
        if (_instance == this) {
            _instanceSet = false;
        }
    }
}
