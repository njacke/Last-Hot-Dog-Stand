using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class TurretController : MonoBehaviour
{
    [SerializeField] private float _maxAimAngle = 30f;
    [SerializeField] private GameObject _hotDogPrefab;
    [SerializeField] private Sprite[] _turretSprites;
    [SerializeField] private Transform _projectileHolder;
    
    private AimDirection _currentAimDir = AimDirection.Middle;
    private SpriteRenderer _spriteRenderer;


    private enum AimDirection {
        Middle,
        Left,
        Right
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start() {
        UpdateSprite();
    }

    private void Update() {
        if (!GameManager.Instance.PlayerControlsLocked) {
            PlayerInput();
        }
    }

    private void PlayerInput() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            SpawnHotDog();
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow)) {
            ChangeAimLeft();
        }

        if (Input.GetKeyDown(KeyCode.RightArrow)) {
            ChangeAimRight();
        }
    }

    private void ChangeAimLeft() {
        if (_currentAimDir == AimDirection.Right) {
            _currentAimDir = AimDirection.Middle;            
        }
        else if (_currentAimDir == AimDirection.Middle) {
            _currentAimDir = AimDirection.Left;
        }

        UpdateSprite();
    }

    private void ChangeAimRight() {
        if (_currentAimDir == AimDirection.Left) {
            _currentAimDir = AimDirection.Middle;            
        }
        else if (_currentAimDir == AimDirection.Middle) {
            _currentAimDir = AimDirection.Right;
        }
        
        UpdateSprite();
    }

    private void UpdateSprite() {
        switch (_currentAimDir) {
            case AimDirection.Middle:
                _spriteRenderer.sprite = _turretSprites[0];
                break;
            case AimDirection.Left:
                _spriteRenderer.sprite = _turretSprites[1];
                break;
            case AimDirection.Right:
                _spriteRenderer.sprite = _turretSprites[2];
                break;
            default:
                break;
        }
    }

    private void SpawnHotDog () {
        if (!GameManager.Instance.StandController.IsHotDogComplete()) {
            Debug.Log("Hot Dog is not completed.");
            return;
        }

        var moveDirection = GetMoveDirFromAim();
        var newHotDog = Instantiate(_hotDogPrefab, _projectileHolder.position, Quaternion.identity).GetComponent<HotDog>();
        
        newHotDog.HotDogData.Bun = GameManager.Instance.StandController.CurrentHotDogData.Bun;
        newHotDog.HotDogData.Dog = GameManager.Instance.StandController.CurrentHotDogData.Dog;
        newHotDog.HotDogData.Sauce = GameManager.Instance.StandController.CurrentHotDogData.Sauce;

        newHotDog.UpdateSprites();

        newHotDog.MoveDirection = moveDirection;
        
        GameManager.Instance.StandController.StartAllCooldowns();
        GameManager.Instance.StandController.ResetCurrentHotDogData();
    }

    private Vector3 GetMoveDirFromAim() {
        var angle = 0f;

        switch (_currentAimDir) {
            case AimDirection.Left:
                angle = -_maxAimAngle;
                break;
            case AimDirection.Middle:
                angle = 0f;
                break;
            case AimDirection.Right:
                angle = _maxAimAngle;
                break;
        }

        var rotation = Quaternion.Euler(0, 0, - angle);
        Vector3 dir = (rotation * transform.up).normalized;

        return dir;
    }

    private Vector3 GetMousePosDirClamped() {
        Quaternion rotation = Quaternion.Euler(0, 0, GetMousePosAngleClamped());
        Vector3 dir = rotation * Vector3.up;
        Debug.Log(dir);

        return dir;
    }

    private float GetMousePosAngleClamped() {
        float angle = Vector2.SignedAngle(Vector3.up, GetMouseWorldPos() - this.transform.position);
        float clampedAngle = Mathf.Clamp(angle, -_maxAimAngle, _maxAimAngle);

        //Debug.Log(clampedAngle);

        return clampedAngle;
    }

    private Vector3 GetMouseWorldPos() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        return mouseWorldPos;
    }

    public void ResetTurret() {
        _currentAimDir = AimDirection.Middle;
        UpdateSprite();
    }
}