using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    [SerializeField] private float _maxAngle = 45f;
    [SerializeField] private GameObject _hotDogPrefab;
    private StandController _standController;

    private void Start() {
        _standController = FindObjectOfType<StandController>();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) {
            SpawnHotDog();
        }
    }

    private void SpawnHotDog () {
        if (!_standController.IsHotDogComplete()) {
            Debug.Log("Hot Dog is not completed.");
            return;
        }

        var moveDirection = GetMousePosDirClamped();
        var newHotDog = Instantiate(_hotDogPrefab, this.transform.position, Quaternion.identity).GetComponent<HotDog>();
        
        newHotDog.HotDogData.Bun = _standController.CurrentHotDogData.Bun;
        newHotDog.HotDogData.Dog = _standController.CurrentHotDogData.Dog;
        newHotDog.HotDogData.Sauce = _standController.CurrentHotDogData.Sauce;

        newHotDog.UpdateSprites();

        newHotDog.MoveDirection = moveDirection;
        
        _standController.StartAllCooldowns();
        _standController.ResetCurrentHotDogData();
    }

    private Vector3 GetMousePosDirClamped() {
        Quaternion rotation = Quaternion.Euler(0, 0, GetMousePosAngleClamped());
        Vector3 dir = rotation * Vector3.up;
        Debug.Log(dir);

        return dir;
    }

    private float GetMousePosAngleClamped() {
        float angle = Vector2.SignedAngle(Vector3.up, GetMouseWorldPos() - this.transform.position);
        float clampedAngle = Mathf.Clamp(angle, -_maxAngle, _maxAngle);

        //Debug.Log(clampedAngle);

        return clampedAngle;
    }

    private Vector3 GetMouseWorldPos() {
        Vector3 mousePos = Input.mousePosition;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mousePos);

        return mouseWorldPos;
    }
}