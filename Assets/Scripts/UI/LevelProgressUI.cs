using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgressUI : MonoBehaviour
{
    [SerializeField] private Sprite[] _progressSprites;

    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
        _image.sprite = _progressSprites[0];
    }

    public void UpdateSprite(int spriteIndex) {
        _image.sprite = _progressSprites[spriteIndex];
    }

    public void ResetProgressBar() {
        _image.sprite = _progressSprites[0];
    }
}
