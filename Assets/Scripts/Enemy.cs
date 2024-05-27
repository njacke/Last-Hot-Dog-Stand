using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _minTargetDistance = .5f;
    private Vector3 _targetPos;
    private bool _hasReachedTargetPos = false;
    private Rigidbody2D _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Start() {
        SetTargetPos();
    }

    private void Update() {
        
    }

    private void FixedUpdate() {
        if (!_hasReachedTargetPos) {
            MoveTowardsTargetPos();
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<HotDogProjectile>()) {
            Debug.Log("I was hit by a Hot Dog");
        }
    }

    private void MoveTowardsTargetPos() {
        Vector3 dir = (_targetPos - this.transform.position).normalized;

        _rb.MovePosition(this.transform.position + _moveSpeed * Time.fixedDeltaTime * dir);
        
        float distanceToTarget = Vector3.Distance(this.transform.position, _targetPos);

        if (distanceToTarget <= _minTargetDistance) {
            _hasReachedTargetPos = true;
        }
    }

    private void SetTargetPos() {
        Vector3 turretPosition = FindObjectOfType<TurretController>().transform.position;        
        _targetPos = new Vector3 (turretPosition.x, turretPosition.y + 1.5f, + turretPosition.z);
    }
}
