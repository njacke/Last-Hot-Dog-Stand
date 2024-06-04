using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private Sprite _notSelectedSprite;
    [SerializeField] private Sprite _selectedSprite;
    //[SerializeField] private Sprite _confirmedSprite;

    private Image _image;
    
    public bool IsSelected { get; private set; } = false;

    public enum ButtonAction {
        Select,
        Unselect,
        Confirm
    }

    private void Awake() {
        _image = GetComponent<Image>();
    }

    public void UpdateButtonSpriteRoutine(ButtonAction buttonAction, float delay) {
        switch (buttonAction) {
            case ButtonAction.Select:
                //yield return new WaitForSeconds(delay);
                _image.sprite = _selectedSprite;
                break;
            case ButtonAction.Unselect:
                //yield return new WaitForSeconds(delay);
                _image.sprite = _notSelectedSprite;
                break;
            case ButtonAction.Confirm:
                //yield return new WaitForSecondsRealtime(delay / 2);
                //_image.sprite = _confirmedSprite;
                //yield return new WaitForSecondsRealtime(delay / 2);
                _image.sprite = _selectedSprite;
                break;
            default:
                break;
        }     
    }    
}
