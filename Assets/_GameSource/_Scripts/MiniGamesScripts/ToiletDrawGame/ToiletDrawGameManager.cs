using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ToiletDrawGameManager : MonoBehaviour {
    #region References
    public ParticleSystem ConfettiFX;

    [Header("Characters")]
    public GameObject MaleCharacterGameObject;
    public GameObject FemaleCharacterGameObject;

    [Space]
    [Header("Toilets")]
    public Transform MaleToiletTransform;
    public Transform FemaleToiletTransform;

    #endregion

    #region Variables

    public bool IsGameActive { get; private set; } = true;
    public bool IsMaleAtTarget { get; private set; } = false;
    public bool IsFemaleAtTarget { get; private set; } = false;
    public bool HasFailed { get; private set; } = false;

    [Header("Line Renderers")]
    private LineRenderer _maleLineRenderer;
    private LineRenderer _femaleLineRenderer;
    private List<Vector2> _maleLinePointsList = new List<Vector2>();
    private List<Vector2> _femaleLinePointsList = new List<Vector2>();

    private bool _isMaleLineComplete = false;
    private bool _isFemaleLineComplete = false;
    private bool _isGameStarted = false;

    [Header("Character Controllers")]
    private CharacterController _maleController;
    private CharacterController _femaleController;

    [Header("Actions")]
    public static Action OnGameWin;
    public static Action OnGameLose;

    #endregion

    //////////////////////////////////////

    private void Start() {
        _maleController = MaleCharacterGameObject.GetComponent<CharacterController>();
        _femaleController = FemaleCharacterGameObject.GetComponent<CharacterController>();
        // Register to character collision events
        _maleController.OnCharacterCollision += OnCharacterCollision;
        _femaleController.OnCharacterCollision += OnCharacterCollision;
    }

    private void OnDestroy() {
        // Unregister from character collision events
        if (_maleController != null)
            _maleController.OnCharacterCollision -= OnCharacterCollision;
        if (_femaleController != null)
            _femaleController.OnCharacterCollision -= OnCharacterCollision;
    }

    #region Game Flow

    public void OnLineDrawingComplete(LineRenderer lineRenderer, List<Vector2> points, bool isMale) {
        if (HasFailed) return; // Don't allow new movements if failed

        if (isMale) {
            _maleLineRenderer = lineRenderer;
            _maleLinePointsList = points;
            _isMaleLineComplete = true;
        } else {
            _femaleLineRenderer = lineRenderer;
            _femaleLinePointsList = points;
            _isFemaleLineComplete = true;
        }

        if (_isMaleLineComplete && _isFemaleLineComplete) {
            StartCharacterMovements();
        }
    }

    private void Update() {
        if (!HasFailed)
            CheckCharacterPositions();
    }

    private void OnCharacterCollision() {
        if (!HasFailed)
            OnGameFailed();
    }

    private void OnGameFailed() {
        HasFailed = true;
        IsGameActive = false;
        // Stop both characters
        if (_maleController != null)
            _maleController.StopMovement();
        if (_femaleController != null)
            _femaleController.StopMovement();
        // Clean up paths
        ResetPaths();

        OnGameLose?.Invoke();
        ToiletDrawLocator.Instance.SoundManagerInstance.PlaySFX("FailSound");
        Debug.Log("Game Failed - Characters Collided!");
        MaleCharacterGameObject.transform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f).SetEase(Ease.InOutSine);
        FemaleCharacterGameObject.transform.DOLocalRotate(new Vector3(0, 0, -90), 0.5f).SetEase(Ease.InOutSine);
    }

    #endregion

    #region Character Movement

    private void StartCharacterMovements() {
        if (_maleController != null && _maleLinePointsList.Count > 0) {
            _maleController.MoveAlongPath(_maleLinePointsList);
        }
        if (_femaleController != null && _femaleLinePointsList.Count > 0) {
            _femaleController.MoveAlongPath(_femaleLinePointsList);
        }
        ResetPaths();
    }

    private void ResetPaths() {
        _isMaleLineComplete = false;
        _isFemaleLineComplete = false;

        if (_maleLineRenderer != null)
            Destroy(_maleLineRenderer.gameObject);
        if (_femaleLineRenderer != null)
            Destroy(_femaleLineRenderer.gameObject);

        _maleLinePointsList.Clear();
        _femaleLinePointsList.Clear();
    }

    #endregion

    #region Game State Checks

    private void CheckCharacterPositions() {
        if (MaleCharacterGameObject != null && MaleToiletTransform != null) {
            if (Vector2.Distance(MaleCharacterGameObject.transform.position, MaleToiletTransform.position) < 0.5f)  {
                IsMaleAtTarget = true;
            }
        }
        if (FemaleCharacterGameObject != null && FemaleToiletTransform != null) {
            if (Vector2.Distance(FemaleCharacterGameObject.transform.position, FemaleToiletTransform.position) < 0.5f) {
                IsFemaleAtTarget = true;
            }
        }

        if (IsMaleAtTarget && IsFemaleAtTarget)
            OnGameComplete();
    }

    private void OnGameComplete() {
        IsGameActive = false;
        if(_isGameStarted == false) {
            _isGameStarted = true;
            ConfettiFX.Play();
            OnGameWin?.Invoke();
            ToiletDrawLocator.Instance.SoundManagerInstance.PlaySFX("WinSound");
            Debug.Log("Game Completed - Both Characters Reached Toilets!");
            MaleToiletTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetEase(Ease.InOutSine);
            FemaleToiletTransform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.5f).SetEase(Ease.InOutSine);

            // Save level progress
            PlayerPrefs.SetInt("ToiletDrawSavedLevel", ToiletDrawLocator.Instance.ToiletDrawLevelManagerInstance.CurrentLevelIndex + 1);
            PlayerPrefs.Save();
        }
    }

    #endregion
}