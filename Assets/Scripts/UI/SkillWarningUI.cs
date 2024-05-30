using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillWarningUI : MonoBehaviour
{
    [SerializeField] float _warningDuration;
    [SerializeField] Sprite _emptySprite;
    [SerializeField] Sprite _warningSprite;
    private Image _image;

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void ShowWarning() {
        StartCoroutine(ShowWarningRoutine());
    }

    private IEnumerator ShowWarningRoutine() {
        _image.sprite = _warningSprite;
        
        yield return new WaitForSeconds(_warningDuration);
        _image.sprite = _emptySprite;
    }
}
