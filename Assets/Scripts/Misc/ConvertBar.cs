using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConvertBar : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    private float _currentProgress = 0f;
    private float _maxProgress = 100f;
    private float _progressIncrement = 20f;
    private SpriteRenderer _spriteRenderer;
    public bool IsProgressCompleted { get => _currentProgress >= _maxProgress ; }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();

        _spriteRenderer.sprite = _sprites[0];

        if (_sprites.Length != 1) {
            _progressIncrement = _maxProgress / (_sprites.Length - 1);
        }
    }

    private void UpdateDisplay() {
        int spriteIndex = Mathf.FloorToInt(_currentProgress / _progressIncrement);
        if (spriteIndex < _sprites.Length) {
            _spriteRenderer.sprite = _sprites[spriteIndex];
        }
    }

    public void AddProgress() {
        _currentProgress += _progressIncrement;     
        UpdateDisplay(); 
    }
}
