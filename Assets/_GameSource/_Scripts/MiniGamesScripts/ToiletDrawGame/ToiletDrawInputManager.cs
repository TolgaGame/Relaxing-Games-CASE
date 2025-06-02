using UnityEngine;
using System.Collections.Generic;

public class ToiletDrawInputManager : MonoBehaviour {
    #region Variables and References

    [Header("Line Drawing")]
    public GameObject LinePrefabGameObject;
    public float LineWidth = 0.5f; // Increased from 0.3f to 0.5f
    public Color MaleLineColor = new Color(0, 0, 1, 1);
    public Color FemaleLineColor = new Color(1, 0, 1, 1);

    private LineRenderer _currentLineRenderer;
    private List<Vector2> _linePointsList = new List<Vector2>();
    private bool _isDrawingActive = false;

    [Header("References")]
    private Camera _mainCamera;
    private GameObject _selectedCharacterGameObject;
    private Transform _targetToiletTransform;

    private ToiletDrawGameManager _gameManager;

    #endregion

    #region Unity Lifecycle Methods

    private void Start() {
        _mainCamera = ToiletDrawLocator.Instance.MainCameraInstance;
        _gameManager = ToiletDrawLocator.Instance.ToiletDrawGameManagerInstance;
    }

    private void Update() {
        if (!_gameManager.IsGameActive) return;

        HandleInputs();
    }
    #endregion

    #region Input Handling

    private void HandleInputs()
    {
        if (Input.GetMouseButtonDown(0)) {
            StartDrawing();
        }
        else if (Input.GetMouseButton(0)) {
            ContinueDrawing();
        }
        else if (Input.GetMouseButtonUp(0)) {
            EndDrawing();
        }
    }

    private void StartDrawing() {
        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null) {
            if (hit.collider.gameObject == _gameManager.MaleCharacterGameObject) {
                _selectedCharacterGameObject = _gameManager.MaleCharacterGameObject;
                _targetToiletTransform = _gameManager.MaleToiletTransform;
                CreateNewLine(MaleLineColor);
            }
            else if (hit.collider.gameObject == _gameManager.FemaleCharacterGameObject) {
                _selectedCharacterGameObject = _gameManager.FemaleCharacterGameObject;
                _targetToiletTransform = _gameManager.FemaleToiletTransform;
                CreateNewLine(FemaleLineColor);
            }
        }
    }
    #endregion

    #region Line Drawing

    private void CreateNewLine(Color color) {
        GameObject newLine = Instantiate(LinePrefabGameObject);
        _currentLineRenderer = newLine.GetComponent<LineRenderer>();
        if (_currentLineRenderer == null) {
            Debug.LogError("LineRenderer component not found on linePrefab!");
            return;
        }
        _currentLineRenderer.startWidth = LineWidth;
        _currentLineRenderer.endWidth = LineWidth;
        _currentLineRenderer.material.color = color;
        _linePointsList.Clear();

        Vector2 characterPos = _selectedCharacterGameObject.transform.position;

        _linePointsList.Add(characterPos);
        _currentLineRenderer.positionCount = 1;
        _currentLineRenderer.SetPosition(0, characterPos);
        _isDrawingActive = true;
    }

    private void ContinueDrawing() {
        if (!_isDrawingActive) return;

        Vector2 mousePos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        if (Vector2.Distance(mousePos, _linePointsList[_linePointsList.Count - 1]) > 0.1f) {
            _linePointsList.Add(mousePos);
            _currentLineRenderer.positionCount = _linePointsList.Count;
            _currentLineRenderer.SetPosition(_linePointsList.Count - 1, mousePos);
        }
    }

    #endregion

    #region Line Completion

    private void EndDrawing() {
        if (!_isDrawingActive) return;

        _isDrawingActive = false;
        Vector2 endPoint = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (_targetToiletTransform == null) {
            CancelLine();
            return;
        }

        float distanceToTarget = Vector2.Distance(endPoint, _targetToiletTransform.position);

        if (distanceToTarget > 2f) {
            CancelLine();
        } else {
            CompleteLineDrawing();
        }
    }

    private void CancelLine() {
        if (_currentLineRenderer != null)
            Destroy(_currentLineRenderer.gameObject);

        _linePointsList.Clear();
        ResetSelection();
    }

    private void CompleteLineDrawing() {
        _linePointsList.Add(_targetToiletTransform.position);
        _currentLineRenderer.positionCount = _linePointsList.Count;
        _currentLineRenderer.SetPosition(_linePointsList.Count - 1, _targetToiletTransform.position);

        bool isMale = _selectedCharacterGameObject == _gameManager.MaleCharacterGameObject;
        _gameManager.OnLineDrawingComplete(_currentLineRenderer, new List<Vector2>(_linePointsList), isMale);
        ResetSelection();
    }

    private void ResetSelection() {
        _selectedCharacterGameObject = null;
        _targetToiletTransform = null;
        _linePointsList.Clear();
    }

    #endregion
}