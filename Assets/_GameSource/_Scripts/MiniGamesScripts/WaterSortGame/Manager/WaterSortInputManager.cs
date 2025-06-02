using UnityEngine;

public class WaterSortInputManager : MonoBehaviour {

    #region References
    [Header("Bottle Controllers")]
    public BottleController FirstBottle;
    public BottleController SecondBottle;
    #endregion

    #region Variables
    [Header("Variables")]
    private int _moves = 0;
    private bool IsBottleTransferValid => FirstBottle != null && !FirstBottle.GetIsBeingFilled() &&
                                        !FirstBottle.GetIsFilling() && FirstBottle.numberOfColorsInBottle > 0 &&
                                        !FirstBottle.CheckIfItsDone();
    #endregion

    #region Unity Lifecycle
    private void Update() {
        if (!Input.GetMouseButtonDown(0) || WaterSortLocator.Instance.WaterSortManagerInstance.IsGamePause)
            return;

        HandleBottleSelection();
    }
    #endregion

    #region Input Handling
    private void HandleBottleSelection() {
        BottleController hitBottle = GetClickedBottle();
        if (hitBottle == null) return;

        if (FirstBottle == null) {
            HandleFirstBottleSelection(hitBottle);
        } else {
            HandleSecondBottleSelection(hitBottle);
        }
    }

    private BottleController GetClickedBottle() {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit.collider != null) {
            return hit.collider.GetComponent<BottleController>();
        }
        return null;
    }
    #endregion

    #region Bottle Selection Logic
    private void HandleFirstBottleSelection(BottleController hitBottle) {
        FirstBottle = hitBottle;
        if (!IsBottleTransferValid) {
            FirstBottle = null;
            return;
        }
        FirstBottle.Selected();
    }

    private void HandleSecondBottleSelection(BottleController hitBottle) {
        if (FirstBottle == hitBottle) {
            UnselectFirstBottle();
            return;
        }

        SecondBottle = hitBottle;
        AttemptBottleTransfer();
    }

    private void UnselectFirstBottle() {
        FirstBottle.Unselected();
        FirstBottle = null;
    }
    #endregion

    #region Transfer Logic
    private void AttemptBottleTransfer() {
        FirstBottle.BottleControllerInstance = SecondBottle;

        if (SecondBottle.GetIsFilling()) {
            SecondBottle = null;
            return;
        }

        FirstBottle.UpdateTopColorValues();
        SecondBottle.UpdateTopColorValues();

        if (SecondBottle.FillBottleCheck(FirstBottle.topColor)) {
            ExecuteBottleTransfer();
        } else {
            RetryBottleSelection(SecondBottle);
        }
    }

    private void ExecuteBottleTransfer() {
        FirstBottle.StartColorTransfer();
        FirstBottle = null;
        SecondBottle = null;
        _moves++;
    }

    private void RetryBottleSelection(BottleController newBottle) {
        FirstBottle.Unselected();
        SecondBottle = null;

        FirstBottle = newBottle;
        if (FirstBottle.GetIsBeingFilled() || FirstBottle.CheckIfItsDone() || FirstBottle.numberOfColorsInBottle == 0) {
            FirstBottle = null;
            return;
        }
        FirstBottle.Selected();
    }
    #endregion
}