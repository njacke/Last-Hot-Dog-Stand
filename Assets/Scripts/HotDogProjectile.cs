using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class HotDogProjectile : MonoBehaviour
{
    [SerializeField] private float _projectileSpeed = 5f;
    public Vector3 MoveDirection { get; set; } = Vector3.zero;
    private Rigidbody2D _rb;

    private void Awake() {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate() {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.GetComponent<Enemy>()) {
            Destroy(gameObject);
        }
    }

    private void Move() {
        if(MoveDirection != Vector3.zero) {
            _rb.MovePosition(this.transform.position + _projectileSpeed * Time.fixedDeltaTime * MoveDirection);
        }
    }

    public void SetDirection() {

    }
}
