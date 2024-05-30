using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SkillSelectUI : MonoBehaviour
{
    [SerializeField] Sprite _emptySprite;
    [SerializeField] Sprite _selectedSprite;
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void SetSkillActive(bool isActive) {
        if (isActive == true) {
            _image.sprite = _selectedSprite;
        }
        else {
            _image.sprite = _emptySprite;
        }
    }
}
