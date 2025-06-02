using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class BottleController : MonoBehaviour {

    #region References

    public BottleController BottleControllerInstance;
    private BottleManager BottleManager;
    public Transform BottleCoverTransform;

    #endregion

    #region Properties

    public Color topColor { get; private set; }
    public int index { get; private set; }
    private Color _transferColor;

    #endregion

    #region Serialized Fields

    [Header("Bottle Colors")]
    [SerializeField] private Color[] bottleColors;
    [SerializeField] private int[] colorsIndex;
    private Color[] normalColorsPallet = new Color[] {
        Color.clear,           // Index 0 = Empty (transparent)
        Color.red,            // Index 1 = Red
        Color.blue,           // Index 2 = Blue
        Color.yellow,         // Index 3 = Yellow
        Color.green,          // Index 4 = Green
        new Color(1f, 0.5f, 0f, 1f)  // Index 5 = Orange
    };

    [Space]
    [Header("Bottle Settings")]
    [SerializeField] private SpriteRenderer bottleMaskSR;
    [SerializeField] private float timeToRotate = 1f;
    [SerializeField] private float moveAnimationTime = 0.5f;

    [Space]
    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve ScaleAndRotationMultiplierCurve;
    [SerializeField] private AnimationCurve FillAmountCurve;
    [SerializeField] private AnimationCurve RotationSpeedMultiplier;

    [Space]
    [Header("Transform References")]
    [SerializeField] private Transform leftRotationPoint;
    [SerializeField] private Transform rightRotationPoint;
    [SerializeField] private LineRenderer lineRenderer;

    #endregion

    #region Private Variables

    private const int MAX_STACK = 3; // Maximum height of 3 units

    [Header("INTEGERS")]
    public int numberOfColorsInBottle;
    [SerializeField] private int numberOfColorsToTransfer = 0;
    private int _rotationIndex = 0;
    private int _numberOfTopColorLayers = 1;
    private int _newNumberOfColorsInOtherBottle;

    [Header("FLOATS")]
    [SerializeField] private float[] fillAmounts;
    [SerializeField] private float[] rotationValues;
    private float _directionMultiplier = 1f;

    [Header("BOOLS")]
    private bool _isFilling;
    private bool _isBeingFilled;
    private bool _stopperCommand;
    private bool _underAnimation;

    [Header("Animations Positions")]
    private Transform chosenRotationPoint;
    private Vector3 _originalPosition;
    private Vector3 _startPosition;
    private Vector3 _endPosition;

    [Header("Bottle State")]
    [SerializeField] private bool isEmptyBottle;

    [Header("Stopper Settings")]
    private bool _anim = false;
    private float _duration = 0.5f;
    private float _moveDistance = 0.5f;

    #endregion

    #region Unity Lifecycle

    private void Awake() {
        bottleColors = new Color[4];
        colorsIndex = new int[4];
        // Initialize arrays with correct values
        fillAmounts = new float[] { 0f, 0.25f, 0.5f, 0.75f, 1f };
        rotationValues = new float[] { 0f, 30f, 60f, 90f };
    }

    private void Start() {
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[numberOfColorsInBottle]);
        Debug.Log($"Bottle {index} initialized with {numberOfColorsInBottle} colors.");
        _originalPosition = transform.position;
        UpdateTopColorValues();
    }

    #endregion

    #region Color Management

    public void StartColorTransfer() {
        // Preparation before transfer
        _isFilling = true;
        BottleControllerInstance.SetIsBeingFilled(true);
        ChoseRotationPointAndDirection();

        // Calculate number of colors to transfer
        numberOfColorsToTransfer = Mathf.Min(_numberOfTopColorLayers, 4 - BottleControllerInstance.numberOfColorsInBottle);
        _newNumberOfColorsInOtherBottle = BottleControllerInstance.numberOfColorsInBottle + numberOfColorsToTransfer;

        // Transfer colors
        TransferColors();
        // Visual adjustments
        transform.GetComponent<SpriteRenderer>().sortingOrder += 2;
        bottleMaskSR.sortingOrder += 2;
        // Start animation
        StartCoroutine(MoveBottle());
    }

    private void TransferColors() {
        if (numberOfColorsInBottle <= 0) return;

        // Count the same color layers in source bottle
        int sameColorCount = 1;
        int topColorIndex = colorsIndex[numberOfColorsInBottle - 1];

        for (int i = numberOfColorsInBottle - 2; i >= 0; i--) {
            if (colorsIndex[i] == topColorIndex) {
                sameColorCount++;
            } else {
                break;
            }
        }

        // Check how many empty spaces are available in target bottle
        int availableSpace = MAX_STACK - BottleControllerInstance.numberOfColorsInBottle;
        // Determine number of layers to transfer
        numberOfColorsToTransfer = Mathf.Min(sameColorCount, availableSpace);

        if (numberOfColorsToTransfer <= 0) return;

        int startIndex = numberOfColorsInBottle - numberOfColorsToTransfer;
        int targetBottleStartIndex = BottleControllerInstance.numberOfColorsInBottle;

        // Transfer colors and indices together
        for (int i = 0; i < numberOfColorsToTransfer; i++) {
            int sourceIndex = startIndex + i;
            int targetIndex = targetBottleStartIndex + i;

            // Save the color and index before clearing source
            Color transferColor = bottleColors[sourceIndex];
            int transferIndex = colorsIndex[sourceIndex];

            // Clear source first
            bottleColors[sourceIndex] = Color.clear;
            colorsIndex[sourceIndex] = 0;

            _transferColor = transferColor;
            // Then set target
            BottleControllerInstance.SetColors(targetIndex, transferColor);
            BottleControllerInstance.colorsIndex[targetIndex] = transferIndex;
        }

        // Update visual state
        UpdateColorsOnShader();
        BottleControllerInstance.UpdateColorsOnShader();

        // Update bottle states
        if (BottleControllerInstance.IsEmptyBottle && numberOfColorsToTransfer > 0) {
            BottleControllerInstance.isEmptyBottle = false;
        }

        // Update color counts
        BottleControllerInstance.numberOfColorsInBottle += numberOfColorsToTransfer;
        _newNumberOfColorsInOtherBottle = BottleControllerInstance.numberOfColorsInBottle;

        // Make sure both bottles update their top colors
        UpdateTopColorValues();
        BottleControllerInstance.UpdateTopColorValues();
    }

    // Set all colors in a single MaterialPropertyBlock
    public void UpdateColorsOnShader() {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        bottleMaskSR.GetPropertyBlock(props);
        props.SetColor("_C1", bottleColors[0]);
        props.SetColor("_C2", bottleColors[1]);
        props.SetColor("_C3", bottleColors[2]);
        props.SetColor("_C4", bottleColors[3]);
        bottleMaskSR.SetPropertyBlock(props);
    }

    public void UpdateTopColorValues() {
        // Check if bottle is empty
        if (numberOfColorsInBottle <= 0) {
            _numberOfTopColorLayers = 0;
            topColor = new Color(0, 0, 0, 0);
            _rotationIndex = 3;
            return;
        }
        // Get top color
        topColor = bottleColors[numberOfColorsInBottle - 1];
        _numberOfTopColorLayers = 1;
        // Count layers of same color
        for (int i = numberOfColorsInBottle - 2; i >= 0; i--) {
            if (bottleColors[i].Equals(topColor))
                _numberOfTopColorLayers++;
            else
                break;
        }
        _rotationIndex = 3 - (numberOfColorsInBottle - _numberOfTopColorLayers);
    }

    #endregion

    #region Bottle State Management

    public bool FillBottleCheck(Color colorToCheck) {
        // Can always pour into empty bottle
        if (isEmptyBottle || numberOfColorsInBottle == 0)
            return true;

        // Cannot pour if bottle is full
        if (numberOfColorsInBottle >= MAX_STACK)
            return false;

        // Top color must match
        return topColor.Equals(colorToCheck);
    }

    private void FillUp(float fillAmountToAdd) {
        bottleMaskSR.material.SetFloat("_FillAmount", bottleMaskSR.material.GetFloat("_FillAmount") + fillAmountToAdd);
    }

    public void AdjustFillAmount(int value) {
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[value]);
    }

    private void ChoseRotationPointAndDirection() {
        if (transform.position.x > BottleControllerInstance.transform.position.x) {
            chosenRotationPoint = leftRotationPoint;
            _directionMultiplier = -1f;
        } else {
            chosenRotationPoint = rightRotationPoint;
            _directionMultiplier = 1f;
        }
    }

    public bool CheckIfItsDone() {
        // Completely empty bottles are not considered done
        if (numberOfColorsInBottle <= 0) {
            return false;
        }

        // Get the first non-empty color
        int firstNonEmptyColorIndex = -1;
        for (int i = 0; i < numberOfColorsInBottle; i++) {
            if (colorsIndex[i] != 0) {
                firstNonEmptyColorIndex = colorsIndex[i];
                break;
            }
        }

        // Check if all colors up to numberOfColorsInBottle are the same
        for (int i = 0; i < numberOfColorsInBottle; i++) {
            // Skip empty slots (they don't affect completion)
            if (colorsIndex[i] == 0) continue;

            // If we find a different color, the bottle is not done
            if (colorsIndex[i] != firstNonEmptyColorIndex) {
                return false;
            }
        }

        // The bottle is done if all non-empty slots have the same color
        return true;
    }

    public void LoadColorsPallet() {
        // For empty bottle state, make all colors transparent
        if (numberOfColorsInBottle == 0 || colorsIndex.All(index => index == 0)) {
            numberOfColorsInBottle = 0; // Reset to make sure
            for (int x = 0; x < bottleColors.Length; x++) {
                bottleColors[x] = Color.clear; // Completely transparent
                colorsIndex[x] = 0;
            }
        } else {
            // Normal color filling process
            for (int x = 0; x < bottleColors.Length; x++) {
                if (x < numberOfColorsInBottle && colorsIndex[x] > 0 && colorsIndex[x] < normalColorsPallet.Length) {
                    bottleColors[x] = normalColorsPallet[colorsIndex[x]];
                } else {
                    bottleColors[x] = Color.clear; // Completely transparent
                    colorsIndex[x] = 0;
                }
            }
        }

        UpdateColorsOnShader();
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[numberOfColorsInBottle]);
        UpdateTopColorValues();
    }

    public void Selected() {
        AnimateBottle(new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z), 0.25f);
    }

    public void Unselected() {
        AnimateBottle(new Vector3(transform.position.x, transform.position.y - 0.15f, transform.position.z), 0.25f);
    }

    #endregion

    #region Animation Control

    private IEnumerator RotateBottle() {
        float t = 0;
        float angleValue = 0;
        float lastAngleValue = 0;
        float rotationSpeed = timeToRotate + ((4 - numberOfColorsInBottle) * 0.1f) + ((numberOfColorsToTransfer - 1) * 0.1f);
        float targetAngle = _directionMultiplier * rotationValues[_rotationIndex];
        int safeColorCount = Mathf.Clamp(numberOfColorsInBottle, 0, 4);
        float currentFillAmount = fillAmounts[safeColorCount];

        // Line renderer settings
        if (!lineRenderer.enabled) {
            lineRenderer.startColor = lineRenderer.endColor = _transferColor; //= topColor;
            lineRenderer.SetPosition(0, chosenRotationPoint.position);
            lineRenderer.SetPosition(1, chosenRotationPoint.position - Vector3.up * 1.45f);
        }

        if (!lineRenderer.enabled)
             lineRenderer.enabled = true;

        while (t < rotationSpeed) {
            angleValue = Mathf.Lerp(0f, targetAngle, t / rotationSpeed);
            // Rotation
            transform.RotateAround(chosenRotationPoint.position, Vector3.forward, lastAngleValue - angleValue);
            // Material update
            float evaluatedAngle = ScaleAndRotationMultiplierCurve.Evaluate(angleValue);
            bottleMaskSR.material.SetFloat("_SARM", evaluatedAngle);
            // Fill amount check
            float currentEval = FillAmountCurve.Evaluate(angleValue);

            if (currentFillAmount > currentEval + 0.005f) {
                // Fill the bottle
                bottleMaskSR.material.SetFloat("_FillAmount", currentEval);
                BottleControllerInstance.FillUp(FillAmountCurve.Evaluate(lastAngleValue) - currentEval);
            }

            lastAngleValue = angleValue;
            t += Time.deltaTime * RotationSpeedMultiplier.Evaluate(angleValue);
            yield return new WaitForEndOfFrame();
        }
        //Debug.Log(numberOfColorsToTransfer);
       // Debug.Log("Number Color in bottle : " + numberOfColorsInBottle);

        // Final adjustments
        bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(targetAngle));
        bottleMaskSR.material.SetFloat("_FillAmount", FillAmountCurve.Evaluate(targetAngle));

        BottleControllerInstance.AdjustFillAmount(_newNumberOfColorsInOtherBottle);
        numberOfColorsInBottle -= numberOfColorsToTransfer;
        lineRenderer.enabled = false;
        BottleControllerInstance.UpdateTopColorValues();

        StartCoroutine(RotateBottleBack());
    }

    private IEnumerator RotateBottleBack() {
        float startAngle = _directionMultiplier * rotationValues[_rotationIndex];
        float endAngle = 0f;
        float duration = timeToRotate;
        float lastAngleValue = startAngle;

        // Animate rotation angle with DOTween
        float currentAngle = startAngle;
        Tween tween = DOTween.To(
            () => currentAngle,
            x => {
                float delta = lastAngleValue - x;
                transform.RotateAround(chosenRotationPoint.position, Vector3.forward, delta);
                bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(x));
                lastAngleValue = x;
                currentAngle = x;
            },
            endAngle,
            duration
        );
        yield return tween.WaitForCompletion();

        UpdateTopColorValues();

        float angleValue = 0f;
        transform.eulerAngles = new Vector3(0, 0, angleValue);
        bottleMaskSR.material.SetFloat("_SARM", ScaleAndRotationMultiplierCurve.Evaluate(angleValue));
        _underAnimation = false;

        // Check both bottles
        if (CheckIfItsDone()) {
            Debug.Log("Current bottle done");
            BottleManager.VerifyVictory();
            SetStopperCommand();
        }

        if (BottleControllerInstance.CheckIfItsDone()) {
            Debug.Log("Other bottle done");
            BottleManager.VerifyVictory();
            BottleControllerInstance.SetStopperCommand();
        }

        StartCoroutine(MoveBottleBack());
    }

    private IEnumerator MoveTo(Vector3 startPos, Vector3 endPos, float duration, System.Action onComplete = null) {
        transform.position = startPos;
        yield return transform.DOMove(endPos, duration).WaitForCompletion();
        onComplete?.Invoke();
    }

    private IEnumerator MoveBottle() {
        _underAnimation = true;
        _startPosition = transform.position;
        _endPosition = chosenRotationPoint == leftRotationPoint ?
            BottleControllerInstance.rightRotationPoint.position :
            BottleControllerInstance.leftRotationPoint.position;

        yield return StartCoroutine(MoveTo(_startPosition, _endPosition, moveAnimationTime,
            () => StartCoroutine(RotateBottle())));
    }

    private IEnumerator MoveBottleBack() {
        yield return StartCoroutine(MoveTo(transform.position, _originalPosition, moveAnimationTime, () => {
            transform.GetComponent<SpriteRenderer>().sortingOrder -= 2;
            bottleMaskSR.sortingOrder -= 2;
            _isFilling = false;

            BottleControllerInstance.SetIsBeingFilled(false);
           // Debug.Log("Bottle animation completed");
            if (CheckIfItsDone())
                SetStopperCommand();
        }));
    }

    public void AnimateBottle(Vector3 finalPosition, float duration) {
        StartCoroutine(MoveTo(transform.position, finalPosition, duration));
    }

    private IEnumerator MoveAnimation(Vector3 finalPosition, float duration) {
        transform.DOMove(finalPosition, duration);
        yield return new WaitForSeconds(duration);
    }

    public void AnimateBottleCover() {
        if (!_anim)  {
            _anim = true;
            // Activate and animate the bottle cover
            BottleCoverTransform.gameObject.SetActive(true);
            BottleCoverTransform.DOLocalMove(new Vector3(0f, _moveDistance, 0f), _duration)
                .SetDelay(0.25f)
                .SetEase(Ease.InOutQuad);
        }
    }

    #endregion

    #region Utility Methods

    public void setIndex(int value, BottleManager bottlesController) {
        index = value;
        BottleManager = bottlesController;
    }

    public void SetColors(int pos, Color color) {
        bottleColors[pos] = color;
    }

    public void SetIsBeingFilled(bool value) {
        _isBeingFilled = value;
    }

    public void SetColorsIndex(int[] colors, int numberOfColors) {
        numberOfColorsInBottle = Mathf.Min(numberOfColors, 4);  // Maximum 4 colors possible
        colorsIndex = new int[4];
        // Null check and array bounds check
        if (colors != null) {
            int copyLength = Mathf.Min(colors.Length, 4);
            for (int i = 0; i < copyLength; i++) {
                colorsIndex[i] = colors[i];
            }
        }
        LoadColorsPallet();
    }

    public void SetStopperCommand() {
        _stopperCommand = true;
    }

    public bool GetUnderAnimation() {
        return _underAnimation;
    }

    public bool GetIsFilling() {
        return _isFilling;
    }

    public bool GetIsBeingFilled(){
        return _isBeingFilled;
    }

    public bool IsEmptyBottle => isEmptyBottle;

    public void SetAsEmptyBottle() {
        isEmptyBottle = true;
        numberOfColorsInBottle = 0;
        for (int i = 0; i < bottleColors.Length; i++) {
            bottleColors[i] = Color.clear;
            colorsIndex[i] = 0;
        }
        UpdateColorsOnShader();
        bottleMaskSR.material.SetFloat("_FillAmount", fillAmounts[0]);
        UpdateTopColorValues();
    }

    #endregion
}