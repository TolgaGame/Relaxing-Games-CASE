using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour {
    #region Fields

    [Header("Movement Settings")]
    public float MoveSpeed;
    public CharacterType characterType;
    public float WaypointReachDistance = 0.1f; // Distance threshold to consider a waypoint reached

    [Header("Animation Settings")]
    public float ScaleAnimDuration = 0.2f;
    public float ScaleDownValue = 0.8f;

    [Header("Variables")]
    private bool _isMoving = false;
    private Vector3[] _pathPoints;
    private int _currentPathIndex = 0;
    private Vector3 _originalScale;
    private CharacterController otherCharacter;

    public event Action OnCharacterCollision;

    #endregion

    #region Enums

    public enum CharacterType
    {
        Male,
        Female
    }

    #endregion

    #region Unity Lifecycle Methods

    private void Awake() {
        _originalScale = transform.localScale;
    }

    private void Start() {
        FindOtherCharacter();
    }

    private void Update() {
        if (_isMoving && _pathPoints != null && _currentPathIndex < _pathPoints.Length) {
            Vector3 targetPoint = _pathPoints[_currentPathIndex];
            float step = MoveSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPoint, step);

            // If current point is reached, move to next point
            if (Vector3.Distance(transform.position, targetPoint) < WaypointReachDistance) {
                _currentPathIndex++;
                // If all points are completed, stop movement
                if (_currentPathIndex >= _pathPoints.Length) {
                    _isMoving = false;
                    _pathPoints = null;
                }
            }
        }
    }
    #endregion

    #region Movement Methods

    public void MoveAlongPath(List<Vector2> pathPoints) {
        // Convert 2D points to 3D
        this._pathPoints = new Vector3[pathPoints.Count];
        for (int i = 0; i < pathPoints.Count; i++) {
            this._pathPoints[i] = new Vector3(pathPoints[i].x, pathPoints[i].y, transform.position.z);
        }
        _currentPathIndex = 0;

        // Play scale animation before starting movement
        PlayMoveStartAnimation(() =>
        {
            _isMoving = true;
        });
    }

    // Stops the character's movement and resets the path
    public void StopMovement() {
        _isMoving = false;
        transform.DOKill();
        _pathPoints = null;
        _currentPathIndex = 0;
    }

    #endregion

    #region Animation Methods

    private void PlayMoveStartAnimation(System.Action onComplete) {
        // Scale down animation
        transform.DOScale(_originalScale * ScaleDownValue, ScaleAnimDuration)
            .OnComplete(() => {
                // Scale up animation
                transform.DOScale(_originalScale, ScaleAnimDuration)
                    .OnComplete(() => onComplete?.Invoke());
            });
    }

    #endregion

    #region Character Interaction Methods

    private void OnTriggerEnter2D(Collider2D other) {
        if (!_isMoving) return;

        CharacterController otherCharacter = other.GetComponent<CharacterController>();
        if (otherCharacter != null && otherCharacter.characterType != this.characterType)
            OnCharacterCollision?.Invoke();
    }

    private void OnTriggerStay2D(Collider2D other) {
        // Check for continuous collision
        CharacterController otherCharacter = other.GetComponent<CharacterController>();
        if (_isMoving && otherCharacter != null && otherCharacter.characterType != this.characterType) {
            OnCharacterCollision?.Invoke();
        }
    }

    // Finds the other character of a different type in the scene
    private void FindOtherCharacter() {
        CharacterController[] characters = FindObjectsOfType<CharacterController>();
        foreach (var character in characters) {
            if (character != this && character.characterType != this.characterType) {
                otherCharacter = character;
                break;
            }
        }
    }

    #endregion
}